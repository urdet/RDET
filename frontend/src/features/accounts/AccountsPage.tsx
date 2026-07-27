import { useEffect, useMemo, useState } from 'react';
import { ArrowDownToLine, ArrowRightLeft, Banknote, ChevronDown, History, Landmark, Minus, Plus, RefreshCw, Save, Send, Settings2, TrendingDown, TrendingUp, X, type LucideIcon } from 'lucide-react';
import { acceptInterAgencySettlement, acceptInterAgencyTransfer, api, cancelInterAgencyTransfer, createAccount, createInterAgencySettlement, createInterAgencyTransfer, getAccountContributions, getAccountsScreenSettings, getAppSettings, listAgencyAccounts, listAgencyTransferRules, listInterAgencySettlements, listInterAgencyTransfers, removeAccount, saveAccountsScreenSettings, updateAccountBalance } from '../../api';
import { Language, tr } from '../../i18n';
import { can } from '../../permissions';
import { CircleButton } from '../../shared/ui/CircleButton';
import { Panel } from '../../shared/ui/Panel';
import { Account, AccountContributionEntry, AccountMovementEntry, AgencyTransferRule, AppSettings, CurrentUser, Dashboard, InterAgencySettlement, InterAgencyTransfer, ScreenId, TransferContribution } from '../../types';
import { arAccountName, arText } from '../../utils/arabic';
import { money } from '../../utils/format';
import { actionTargets, AccountActionSlot, AccountButtonWidget, AccountCardConfig, AccountCardConfigMap, getAccountCardConfig, loadAccountCardConfigs, normalizeAccountCardConfig, renderTextWidget, saveAccountCardConfigs } from './accountCardConfig';
import { CompteBox } from './CompteBox';

type AccountsPageProps = {
  accounts: Account[];
  dashboard: Dashboard | null;
  currentUser: CurrentUser | null;
  language: Language;
  onRefresh: () => void;
  onNavigate: (screen: ScreenId) => void;
};

type ActiveAction = {
  kind: AccountActionSlot;
  account: Account;
  config: AccountCardConfig;
} | null;

type ContributorCard = {
  name: string;
  total: number;
  entries: Array<AccountContributionEntry & { contribution: TransferContribution; signedAmount: number }>;
};

type ReturnDebtOption = {
  accountId: number;
  accountName: string;
  transfer: InterAgencyTransfer;
  transferIds: number[];
  totalRemaining: number;
};

type AccountKpiId = 'cashBalance' | 'cashReal' | 'unpaid' | 'calculatedCash' | 'totalDebit' | 'totalCredit' | 'todayDeposits';
type AccountKpiConfig = { id: AccountKpiId; label: string; formula: string; visible: boolean };

const defaultAccountKpis: AccountKpiConfig[] = [
  { id: 'cashBalance', label: 'Balance caisse', formula: 'Caisse réelle + Factures non payées - Caisse calculée', visible: true },
  { id: 'cashReal', label: 'Caisse réelle', formula: 'Caisse réelle', visible: true },
  { id: 'unpaid', label: 'Factures non payées', formula: 'Factures non payées', visible: true },
  { id: 'calculatedCash', label: 'Caisse calculée', formula: 'Caisse calculée', visible: true },
  { id: 'totalDebit', label: 'Total débit', formula: 'Total débit', visible: false },
  { id: 'totalCredit', label: 'Total crédit', formula: 'Total crédit', visible: false },
  { id: 'todayDeposits', label: 'Versements du jour', formula: 'Versements du jour', visible: false },
];

function validKpisOrDefault(value: unknown): AccountKpiConfig[] {
  if (!Array.isArray(value)) return defaultAccountKpis;
  const legacyIds = value.filter((item): item is AccountKpiId => typeof item === 'string' && defaultAccountKpis.some((option) => option.id === item));
  if (legacyIds.length) return defaultAccountKpis.map((item) => ({ ...item, visible: legacyIds.includes(item.id) }));
  const saved = new Map(value.filter((item): item is Partial<AccountKpiConfig> & { id: AccountKpiId } => Boolean(item && typeof item === 'object' && defaultAccountKpis.some((option) => option.id === (item as AccountKpiConfig).id))).map((item) => [item.id, item]));
  return defaultAccountKpis.map((fallback) => {
    const item = saved.get(fallback.id);
    return { id: fallback.id, label: typeof item?.label === 'string' ? item.label : fallback.label, formula: typeof item?.formula === 'string' ? item.formula : fallback.formula, visible: typeof item?.visible === 'boolean' ? item.visible : fallback.visible };
  });
}

function evaluateKpiFormula(formula: string, accounts: Account[], dashboard: Dashboard | null): number | null {
  const normalize = (value: string) => value.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase().replace(/\s+/g, ' ').trim();
  const variables: Record<string, number> = {
    'caisse reelle': Number(dashboard?.cash_real ?? 0), 'factures non payees': Number(dashboard?.unpaid_total ?? 0),
    'non paye': Number(dashboard?.unpaid_total ?? 0), 'caisse calculee': Number(dashboard?.total_balance ?? 0),
    'solde total': Number(dashboard?.total_balance ?? 0), 'total debit': Number(dashboard?.total_debit ?? 0),
    'total credit': Number(dashboard?.total_credit ?? 0), 'versements du jour': Number(dashboard?.service_in ?? 0),
    'retraits du jour': Number(dashboard?.service_out ?? 0), frais: Number(dashboard?.fees ?? 0),
  };
  accounts.forEach((account) => { variables[normalize(account.name)] = Number(account.balance ?? 0); });
  let expression = normalize(formula).replace(/\{([^}]+)\}/g, '$1');
  Object.entries(variables).sort(([a], [b]) => b.length - a.length).forEach(([name, value]) => {
    expression = expression.replace(new RegExp(name.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g'), `(${value})`);
  });
  if (!/^[\d+\-*/().\s]+$/.test(expression)) return null;
  try { const result = Function(`"use strict"; return (${expression});`)(); return Number.isFinite(result) ? result : null; } catch { return null; }
}

const accountOrderStorageKey = 'rdet_accounts_order';

export function AccountsPage({ accounts, dashboard, currentUser, language, onRefresh, onNavigate }: AccountsPageProps) {
  const t = (key: string) => tr(key, language);
  const [configs, setConfigs] = useState<AccountCardConfigMap>(() => loadAccountCardConfigs());
  const [appSettings, setAppSettings] = useState<Partial<AppSettings>>({});
  const [accountOrder, setAccountOrder] = useState<string[]>(() => {
    try {
      const value = localStorage.getItem(accountOrderStorageKey);
      return value ? JSON.parse(value) : [];
    } catch {
      return [];
    }
  });
  const [draggingAccountId, setDraggingAccountId] = useState('');
  const [orderDirty, setOrderDirty] = useState(false);
  const [activeAction, setActiveAction] = useState<ActiveAction>(null);
  const [movementType, setMovementType] = useState<'versement' | 'retrait'>('versement');
  const [sourceAccountId, setSourceAccountId] = useState('');
  const [targetAccountId, setTargetAccountId] = useState('');
  const [amount, setAmount] = useState('');
  const [description, setDescription] = useState('');
  const [personName, setPersonName] = useState('');
  const [contributorName, setContributorName] = useState('');
  const [contributorPickerOpen, setContributorPickerOpen] = useState(false);
  const [creatingAccount, setCreatingAccount] = useState(false);
  const [newAccountName, setNewAccountName] = useState('');
  const [newAccountBalance, setNewAccountBalance] = useState('0');
  const [newAccountSide, setNewAccountSide] = useState<'debit' | 'credit'>('debit');
  const [editingBalanceAccount, setEditingBalanceAccount] = useState<Account | null>(null);
  const [manualBalance, setManualBalance] = useState('');
  const [manualAccountSide, setManualAccountSide] = useState<'debit' | 'credit'>('debit');
  const [detailsAccount, setDetailsAccount] = useState<Account | null>(null);
  const [detailsRows, setDetailsRows] = useState<AccountMovementEntry[]>([]);
  const [contributionAccountId, setContributionAccountId] = useState(accounts[0] ? String(accounts[0].id) : '');
  const [contributionEntries, setContributionEntries] = useState<AccountContributionEntry[]>([]);
  const [popupContributionEntries, setPopupContributionEntries] = useState<AccountContributionEntry[]>([]);
  const [historyContributorName, setHistoryContributorName] = useState('');
  const [interAgencyRules, setInterAgencyRules] = useState<AgencyTransferRule[]>([]);
  const [incomingTransfers, setIncomingTransfers] = useState<InterAgencyTransfer[]>([]);
  const [acceptedTransfers, setAcceptedTransfers] = useState<InterAgencyTransfer[]>([]);
  const [settlements, setSettlements] = useState<InterAgencySettlement[]>([]);
  const [interAgencyOpen, setInterAgencyOpen] = useState(false);
  const [interAgencyMode, setInterAgencyMode] = useState<'send' | 'return'>('send');
  const [interAgencyRuleId, setInterAgencyRuleId] = useState('');
  const [interAgencyAmount, setInterAgencyAmount] = useState('');
  const [interAgencyNote, setInterAgencyNote] = useState('');
  const [returnTransferId, setReturnTransferId] = useState('');
  const [returnPayerAccountId, setReturnPayerAccountId] = useState('');
  const [returnReceiverAccountId, setReturnReceiverAccountId] = useState('');
  const [returnReceiverAccounts, setReturnReceiverAccounts] = useState<Account[]>([]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [kpiConfigs, setKpiConfigs] = useState<AccountKpiConfig[]>(defaultAccountKpis);
  const [kpiSettingsOpen, setKpiSettingsOpen] = useState(false);

  const orderedAccounts = useMemo(() => {
    const orderIndex = new Map(accountOrder.map((accountId, index) => [accountId, index]));
    return [...accounts].sort((left, right) => {
      const leftIndex = orderIndex.get(String(left.id));
      const rightIndex = orderIndex.get(String(right.id));
      if (leftIndex !== undefined && rightIndex !== undefined) return leftIndex - rightIndex;
      if (leftIndex !== undefined) return -1;
      if (rightIndex !== undefined) return 1;
      return accounts.indexOf(left) - accounts.indexOf(right);
    });
  }, [accounts, accountOrder]);

  const visibleAccounts = accounts.filter((account) => account.visible).length;
  const positiveAccounts = accounts.filter((account) => Number(account.balance) >= 0).length;
  const transferTargets = accounts.filter((account) => String(account.id) !== sourceAccountId);
  const selectedContributionAccount = accounts.find((account) => String(account.id) === contributionAccountId) ?? accounts[0];
  const canCreateAccount = can(currentUser, appSettings, 'accounts', 'create');
  const canConfigureCards = can(currentUser, appSettings, 'account-settings', 'configure');
  const canConfigureKpis = currentUser?.role === 'Admin';
  const canChangeBalance = can(currentUser, appSettings, 'accounts', 'changeBalance');
  const canOpenDetails = can(currentUser, appSettings, 'accounts', 'open');
  const canDeleteAccount = can(currentUser, appSettings, 'accounts', 'delete');
  const canUseAccountActions = can(currentUser, appSettings, 'accounts', 'accountAction');
  const canUseTransfer = can(currentUser, appSettings, 'accounts', 'transfer');
  const canUseMovement = can(currentUser, appSettings, 'accounts', 'movement');
  const activeInterAgencyRules = interAgencyRules.filter((rule) => rule.status === 'active' && rule.active && rule.source_agency_id === currentUser?.company_id);
  const returnableTransfers = acceptedTransfers.filter((item) => item.status === 'accepted' && item.destination_agency_id === currentUser?.company_id);
  const pendingReturnSettlements = settlements.filter((item) => item.status === 'pending' && item.receiver_agency_id === currentUser?.company_id);
  const returnDebtOptions = useMemo(() => {
    const options = new Map<number, ReturnDebtOption>();
    returnableTransfers.forEach((transfer) => {
      const accountId = transfer.source_account_id;
      const current = options.get(accountId);
      const remaining = Number(transfer.remaining_amount ?? transfer.amount ?? 0);
      if (current) {
        current.transferIds.push(transfer.id);
        current.totalRemaining += Number.isFinite(remaining) ? remaining : 0;
        return;
      }
      options.set(accountId, {
        accountId,
        accountName: transfer.source_account_name ?? t('account'),
        transfer,
        transferIds: [transfer.id],
        totalRemaining: Number.isFinite(remaining) ? remaining : 0,
      });
    });
    return Array.from(options.values()).sort((left, right) => left.accountName.localeCompare(right.accountName));
  }, [returnableTransfers, language]);
  const selectedReturnDebt = returnDebtOptions.find((item) => item.transferIds.includes(Number(returnTransferId))) ?? returnDebtOptions[0] ?? null;
  const selectedReturnTransfer = selectedReturnDebt?.transfer ?? null;

  const contributorCards = useMemo(() => {
    const cards = new Map<string, ContributorCard>();
    contributionEntries.forEach((entry) => {
      entry.contributions.forEach((contribution) => {
        const direction = contribution.direction ?? entry.direction;
        const signedAmount = (direction === 'versement' ? 1 : -1) * Number(contribution.amount || entry.amount || 0);
        const name = (contribution.name || 'Sans nom').trim();
        const key = name.toLowerCase();
        const card = cards.get(key) ?? { name, total: 0, entries: [] };
        card.total += Number.isFinite(signedAmount) ? signedAmount : 0;
        card.entries.push({ ...entry, contribution: { ...contribution, direction }, signedAmount });
        cards.set(key, card);
      });
    });
    return Array.from(cards.values()).sort((left, right) => left.name.localeCompare(right.name));
  }, [contributionEntries]);

  useEffect(() => {
    getAppSettings<Partial<AppSettings>>().then(setAppSettings).catch(() => setAppSettings({}));
    getAccountsScreenSettings<AccountCardConfigMap>()
      .then((serverConfigs) => {
        const savedOrder = (serverConfigs as unknown as { __accountOrder?: unknown })?.__accountOrder;
        const savedKpis = (serverConfigs as unknown as { __kpis?: unknown })?.__kpis;
        if (Array.isArray(savedKpis)) setKpiConfigs(validKpisOrDefault(savedKpis));
        if (Array.isArray(savedOrder)) {
          const cleanOrder = savedOrder.map(String);
          setAccountOrder(cleanOrder);
          localStorage.setItem(accountOrderStorageKey, JSON.stringify(cleanOrder));
        }
        const accountConfigs = Object.fromEntries(Object.entries(serverConfigs ?? {}).filter(([key]) => key !== '__accountOrder' && key !== '__kpis')) as AccountCardConfigMap;
        if (accountConfigs && Object.keys(accountConfigs).length) {
          const legacy = accountConfigs as unknown as { buttons?: unknown; popups?: unknown };
          const normalized = legacy.buttons || legacy.popups
            ? Object.fromEntries(accounts.map((account) => [String(account.id), normalizeAccountCardConfig({ ...getAccountCardConfig(configs, account.id), buttons: legacy.buttons as AccountButtonWidget[] | undefined, popups: legacy.popups as AccountCardConfig['popups'] | undefined })]))
            : Object.fromEntries(Object.entries(accountConfigs).map(([accountId, accountConfig]) => [accountId, normalizeAccountCardConfig(accountConfig)]));
          setConfigs(normalized);
          saveAccountCardConfigs(normalized);
          if (legacy.buttons || legacy.popups) saveAccountsScreenSettings({ ...normalized, __accountOrder: savedOrder ?? accountOrder, __kpis: validKpisOrDefault(savedKpis) } as unknown as AccountCardConfigMap).catch(() => undefined);
        } else {
          saveAccountsScreenSettings({ ...configs, __accountOrder: accountOrder, __kpis: validKpisOrDefault(savedKpis) } as unknown as AccountCardConfigMap).catch(() => undefined);
        }
      })
      .catch(() => undefined);
  }, []);

  useEffect(() => {
    if (!accounts.length) return;
    setAccountOrder((current) => {
      const known = new Set(accounts.map((account) => String(account.id)));
      const next = [...current.filter((accountId) => known.has(accountId)), ...accounts.map((account) => String(account.id)).filter((accountId) => !current.includes(accountId))];
      localStorage.setItem(accountOrderStorageKey, JSON.stringify(next));
      return next;
    });
  }, [accounts]);

  useEffect(() => {
    if (!contributionAccountId && accounts[0]) {
      setContributionAccountId(String(accounts[0].id));
    }
  }, [accounts, contributionAccountId]);

  useEffect(() => {
    if (!contributionAccountId) return;
    getAccountContributions(Number(contributionAccountId))
      .then((rows) => {
        setContributionEntries(rows);
        setHistoryContributorName('');
      })
      .catch(() => setContributionEntries([]));
  }, [contributionAccountId]);

  async function refreshInterAgency() {
    const [rules, transfers, settlementRows] = await Promise.all([
      listAgencyTransferRules().catch(() => []),
      listInterAgencyTransfers().catch(() => []),
      listInterAgencySettlements().catch(() => []),
    ]);
    setInterAgencyRules(rules);
    setIncomingTransfers(transfers.filter((item) => item.status === 'pending_receiver' && item.destination_agency_id === currentUser?.company_id));
    setAcceptedTransfers(transfers.filter((item) => item.status === 'accepted'));
    setSettlements(settlementRows);
  }

  useEffect(() => {
    if (!currentUser?.company_id) return;
    refreshInterAgency().catch(() => undefined);
  }, [currentUser?.company_id]);

  useEffect(() => {
    if (!selectedReturnTransfer) {
      setReturnReceiverAccounts([]);
      setReturnReceiverAccountId('');
      return;
    }
    setReturnTransferId(String(selectedReturnTransfer.id));
    setReturnReceiverAccountId(String(selectedReturnTransfer.source_account_id));
    setReturnPayerAccountId((current) => current || String(accounts[0]?.id ?? ''));
    listAgencyAccounts(selectedReturnTransfer.source_agency_id)
      .then((rows) => {
        setReturnReceiverAccounts(rows);
        if (!rows.some((account) => account.id === selectedReturnTransfer.source_account_id)) {
          setReturnReceiverAccountId(String(rows[0]?.id ?? selectedReturnTransfer.source_account_id));
        }
      })
      .catch(() => setReturnReceiverAccounts([]));
  }, [selectedReturnTransfer?.id, accounts.length]);

  const historyContributor = contributorCards.find((card) => card.name === historyContributorName) ?? null;
  const popupContributorNames = useMemo(() => {
    const names = new Map<string, string>();
    popupContributionEntries.forEach((entry) => {
      entry.contributions.forEach((contribution) => {
        const name = (contribution.name || '').trim();
        if (!name) return;
        const key = name.toLowerCase();
        if (!names.has(key)) names.set(key, name);
      });
    });
    return Array.from(names.values()).sort((left, right) => left.localeCompare(right));
  }, [popupContributionEntries]);
  const filteredContributorNames = useMemo(() => {
    const normalized = contributorName.trim().toLowerCase();
    if (!normalized) return popupContributorNames;
    return popupContributorNames.filter((name) => name.toLowerCase().includes(normalized));
  }, [contributorName, popupContributorNames]);

  useEffect(() => {
    if (activeAction?.kind !== 'versement' || !targetAccountId) {
      setPopupContributionEntries([]);
      return;
    }
    getAccountContributions(Number(targetAccountId))
      .then(setPopupContributionEntries)
      .catch(() => setPopupContributionEntries([]));
  }, [activeAction?.kind, targetAccountId]);

  function openAction(account: Account, kind: AccountActionSlot, accountConfig: AccountCardConfig) {
    if (!canUseAccountActions || (kind === 'transfer' && !canUseTransfer) || (kind === 'versement' && !canUseMovement)) {
      setError(t('noPermission'));
      return;
    }
    const target = actionTargets[kind];
    if (target) {
      onNavigate(target);
      return;
    }
    if (kind === 'refresh') {
      onRefresh();
      return;
    }
    if (kind === 'hidden') return;
    const popups = accountConfig.popups;
    setError('');
    setAmount(kind === 'transfer' && popups.transfer.applyFixedAmount ? popups.transfer.fixedAmount : kind === 'versement' && popups.movement.applyFixedAmount ? popups.movement.fixedAmount : '');
    setDescription(kind === 'transfer' && popups.transfer.applyFixedDescription ? popups.transfer.fixedDescription : kind === 'versement' && popups.movement.applyFixedDescription ? popups.movement.fixedDescription : '');
    setPersonName('');
    setContributorName('');
    setMovementType(popups.movement.applyFixedType ? popups.movement.fixedType : popups.movement.defaultType);
    setSourceAccountId(kind === 'transfer' ? (popups.transfer.applyFixedFromAccount ? popups.transfer.fixedFromAccountId : String(account.id)) : '');
    setTargetAccountId(kind === 'transfer'
      ? (popups.transfer.applyFixedToAccount ? popups.transfer.fixedToAccountId : '')
      : (popups.movement.applyFixedAccount ? popups.movement.fixedAccountId : String(account.id)));
    setActiveAction({ account, kind, config: accountConfig });
  }

  function openContributorMovement(card: ContributorCard, type: 'versement' | 'retrait') {
    if (!selectedContributionAccount) return;
    const accountConfig = getAccountCardConfig(configs, selectedContributionAccount.id);
    setError('');
    setAmount('');
    setDescription('');
    setPersonName('');
    setContributorName(card.name);
    setMovementType(type);
    setSourceAccountId('');
    setTargetAccountId(String(selectedContributionAccount.id));
    setActiveAction({ account: selectedContributionAccount, kind: 'versement', config: accountConfig });
  }

  function canonicalContributorName(value: string) {
    const clean = value.trim();
    if (!clean) return '';
    return popupContributorNames.find((name) => name.toLowerCase() === clean.toLowerCase()) ?? clean;
  }

  function closePopup() {
    if (saving) return;
    setActiveAction(null);
    setError('');
  }

  async function openAccountDetails(account: Account) {
    setError('');
    setDetailsAccount(account);
    setDetailsRows([]);
    const rows = await api<AccountMovementEntry[]>(`/accounts/${account.id}/movements`).catch(() => []);
    setDetailsRows(rows);
  }

  async function deleteAccount(account: Account) {
    const confirmed = window.confirm(`Supprimer le compte « ${arAccountName(account.name)} » de cette agence ? Son historique financier sera conservé.`);
    if (!confirmed) return;
    setError('');
    try {
      await removeAccount(account.id);
      const nextOrder = accountOrder.filter((accountId) => accountId !== String(account.id));
      const nextConfigs = Object.fromEntries(Object.entries(configs).filter(([accountId]) => accountId !== String(account.id))) as AccountCardConfigMap;
      setAccountOrder(nextOrder);
      setConfigs(nextConfigs);
      localStorage.setItem(accountOrderStorageKey, JSON.stringify(nextOrder));
      saveAccountCardConfigs(nextConfigs);
      await saveAccountsScreenSettings({ ...nextConfigs, __accountOrder: nextOrder, __kpis: kpiConfigs } as unknown as AccountCardConfigMap);
      await onRefresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Suppression du compte impossible.');
    }
  }

  function reorderAccounts(sourceId: string, targetId: string) {
    if (!sourceId || sourceId === targetId) return;
    const baseOrder = accountOrder.length ? accountOrder : accounts.map((account) => String(account.id));
    const next = [...baseOrder];
    const sourceIndex = next.indexOf(sourceId);
    const targetIndex = next.indexOf(targetId);
    if (sourceIndex < 0 || targetIndex < 0) return;
    next.splice(sourceIndex, 1);
    next.splice(targetIndex, 0, sourceId);
    setAccountOrder(next);
    localStorage.setItem(accountOrderStorageKey, JSON.stringify(next));
    setOrderDirty(true);
  }

  async function saveAccountOrder() {
    const cleanOrder = accountOrder.length ? accountOrder : accounts.map((account) => String(account.id));
    setSaving(true);
    setError('');
    try {
      await saveAccountsScreenSettings({ ...configs, __accountOrder: cleanOrder, __kpis: kpiConfigs } as unknown as AccountCardConfigMap);
      localStorage.setItem(accountOrderStorageKey, JSON.stringify(cleanOrder));
      setOrderDirty(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Sauvegarde ordre impossible.');
    } finally {
      setSaving(false);
    }
  }

  async function saveKpiConfigs(next: AccountKpiConfig[]) {
    if (!canConfigureKpis || !next.some((item) => item.visible)) return;
    setKpiConfigs(next);
    await saveAccountsScreenSettings({ ...configs, __accountOrder: accountOrder, __kpis: next } as unknown as AccountCardConfigMap).catch(() => undefined);
  }

  async function submitAccountAction() {
    if (!activeAction || Number(amount) <= 0) {
      setError('Saisir un montant valide.');
      return;
    }

    if (activeAction.kind === 'versement' && !targetAccountId) {
      setError('Choisir le compte.');
      return;
    }

    if (activeAction.kind === 'transfer' && (!sourceAccountId || !targetAccountId || sourceAccountId === targetAccountId)) {
      setError('Choisir deux comptes differents.');
      return;
    }

    setSaving(true);
    setError('');
    try {
      if (activeAction.kind === 'versement') {
        await api('/transfers', {
          method: 'POST',
          body: JSON.stringify({
            from_account_id: movementType === 'retrait' ? Number(targetAccountId) : undefined,
            to_account_id: movementType === 'versement' ? Number(targetAccountId) : undefined,
            amount,
            description: description || `${movementType === 'versement' ? 'Versement' : 'Retrait'} compte`,
            context_account_id: activeAction.account.id,
            contributions: canonicalContributorName(contributorName)
              ? [{ name: canonicalContributorName(contributorName), amount, direction: movementType }]
              : undefined,
          }),
        });
      }

      if (activeAction.kind === 'transfer') {
        await api('/transfers', {
          method: 'POST',
          body: JSON.stringify({
            from_account_id: Number(sourceAccountId),
            to_account_id: Number(targetAccountId),
            amount,
            description: description || 'Transfert entre comptes',
            context_account_id: activeAction.account.id,
          }),
        });
      }

      if (activeAction.kind === 'unpaid') {
        await api('/unpaid-items', {
          method: 'POST',
          body: JSON.stringify({
            person_name: personName || activeAction.account.name,
            amount,
            description: description || `${t('unpaid')} ${activeAction.account.name}`,
          }),
        });
      }

      setActiveAction(null);
      await onRefresh();
      if (contributionAccountId) {
        getAccountContributions(Number(contributionAccountId)).then(setContributionEntries).catch(() => undefined);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Action impossible.');
    } finally {
      setSaving(false);
    }
  }

  async function submitInterAgencyTransfer() {
    if (!interAgencyRuleId || Number(interAgencyAmount) <= 0) {
      setError(t('completeSendError'));
      return;
    }
    setSaving(true);
    setError('');
    try {
      await createInterAgencyTransfer({
        transfer_rule_id: Number(interAgencyRuleId),
        amount: interAgencyAmount,
        note: interAgencyNote || undefined,
      });
      setInterAgencyOpen(false);
      setInterAgencyAmount('');
      setInterAgencyNote('');
      await refreshInterAgency();
    } catch (err) {
      setError(err instanceof Error ? err.message : t('sendTransferFailed'));
    } finally {
      setSaving(false);
    }
  }

  async function submitInterAgencyReturn() {
    if (!selectedReturnTransfer || !returnPayerAccountId || !returnReceiverAccountId || Number(interAgencyAmount) <= 0) {
      setError(t('completeReturnError'));
      return;
    }
    setSaving(true);
    setError('');
    try {
      await createInterAgencySettlement({
        inter_agency_transfer_id: selectedReturnTransfer.id,
        payer_account_id: Number(returnPayerAccountId),
        receiver_account_id: Number(returnReceiverAccountId),
        amount: interAgencyAmount,
        note: interAgencyNote || undefined,
      });
      setInterAgencyOpen(false);
      setInterAgencyAmount('');
      setInterAgencyNote('');
      await refreshInterAgency();
    } catch (err) {
      setError(err instanceof Error ? err.message : t('returnRequestFailed'));
    } finally {
      setSaving(false);
    }
  }

  async function decideIncomingTransfer(transferId: number, decision: 'accept' | 'cancel') {
    setSaving(true);
    setError('');
    try {
      if (decision === 'accept') {
        await acceptInterAgencyTransfer(transferId);
      } else {
        await cancelInterAgencyTransfer(transferId);
      }
      await refreshInterAgency();
      await onRefresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : t('decisionFailed'));
    } finally {
      setSaving(false);
    }
  }

  async function acceptPendingReturn(settlementId: number) {
    setSaving(true);
    setError('');
    try {
      await acceptInterAgencySettlement(settlementId);
      await refreshInterAgency();
      await onRefresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : t('decisionFailed'));
    } finally {
      setSaving(false);
    }
  }

  async function submitNewAccount() {
    if (!newAccountName.trim()) {
      setError('Saisir le nom du compte.');
      return;
    }
    if (Number.isNaN(Number(newAccountBalance))) {
      setError('Saisir un solde valide.');
      return;
    }
    setSaving(true);
    setError('');
    try {
      await createAccount({ name: newAccountName.trim(), balance: newAccountBalance || '0', visible: true, normal_balance_side: newAccountSide });
      setCreatingAccount(false);
      setNewAccountName('');
      setNewAccountBalance('0');
      setNewAccountSide('debit');
      await onRefresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Creation du compte impossible.');
    } finally {
      setSaving(false);
    }
  }

  async function submitManualBalance() {
    if (!editingBalanceAccount) return;
    if (Number.isNaN(Number(manualBalance))) {
      setError('Saisir un solde valide.');
      return;
    }
    setSaving(true);
    setError('');
    try {
      await updateAccountBalance(editingBalanceAccount.id, manualBalance || '0', manualAccountSide);
      setEditingBalanceAccount(null);
      await onRefresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Modification du solde impossible.');
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="space-y-4">
      <div className="accounts-kpi-section">
        <div className="accounts-kpi-grid">
          {kpiConfigs.filter((item) => item.visible).map((item) => {
            const icon = item.id === 'unpaid' ? Banknote : item.id === 'totalDebit' || item.id === 'todayDeposits' ? TrendingUp : item.id === 'totalCredit' ? TrendingDown : Landmark;
            const value = evaluateKpiFormula(item.formula, accounts, dashboard);
            return <KpiCard key={item.id} icon={icon} label={item.label} value={value === null ? '#FORMULA' : money(value)} />;
          })}
        </div>
        {canConfigureKpis && (
          <div className="kpi-config-wrap">
            <button type="button" className="kpi-config-button" onClick={() => setKpiSettingsOpen((open) => !open)} aria-expanded={kpiSettingsOpen}>
              <Settings2 className="h-4 w-4" /> Configurer les KPI
            </button>
            {kpiSettingsOpen && (
              <div className="kpi-config-panel">
                <strong>Indicateurs et formules</strong>
                {kpiConfigs.map((item) => (
                  <label key={item.id} className="kpi-formula-row">
                    <input type="checkbox" checked={item.visible} onChange={() => saveKpiConfigs(kpiConfigs.map((current) => current.id === item.id ? { ...current, visible: !current.visible } : current))} />
                    <input value={item.label} aria-label="Nom du KPI" onChange={(event) => setKpiConfigs(kpiConfigs.map((current) => current.id === item.id ? { ...current, label: event.target.value } : current))} onBlur={() => saveKpiConfigs(kpiConfigs)} />
                    <input value={item.formula} aria-label={`Formule ${item.label}`} onChange={(event) => setKpiConfigs(kpiConfigs.map((current) => current.id === item.id ? { ...current, formula: event.target.value } : current))} onBlur={() => saveKpiConfigs(kpiConfigs)} />
                  </label>
                ))}
                <small>
                  Variables générales : Caisse réelle, Factures non payées, Caisse calculée, Solde total, Total débit, Total crédit, Versements du jour, Retraits du jour, Frais.
                  {' '}Comptes : {accounts.length ? accounts.map((account) => account.name).join(', ') : 'Aucun compte'}.
                  {' '}Opérateurs : + − * / ( ).
                </small>
              </div>
            )}
          </div>
        )}
      </div>

      <div className="accounts-toolbar">
        <div className="account-toolbar-actions">
          <div className="account-count">{orderedAccounts.length} {t('accountsCount')}, {visibleAccounts} {t('visible')}, {positiveAccounts} {t('positive')}</div>
          {canUseTransfer && <CircleButton title={t('interAgencyTransfers')} icon={Send} onClick={() => { setError(''); setInterAgencyMode(activeInterAgencyRules.length ? 'send' : 'return'); setInterAgencyOpen(true); setInterAgencyRuleId(activeInterAgencyRules[0] ? String(activeInterAgencyRules[0].id) : ''); setReturnTransferId(returnableTransfers[0] ? String(returnableTransfers[0].id) : ''); }} />}
          {canCreateAccount && <CircleButton title={t('createAccount')} icon={Plus} onClick={() => { setError(''); setCreatingAccount(true); }} />}
          {orderDirty && <CircleButton title={t('saveOrder')} icon={Save} onClick={saveAccountOrder} />}
          {canConfigureCards && <CircleButton title={t('cardSettings')} icon={Settings2} onClick={() => onNavigate('account-settings')} />}
          <CircleButton title={tr('refresh', language)} icon={RefreshCw} onClick={onRefresh} />
        </div>
      </div>

      {incomingTransfers.length > 0 && (
        <section className="incoming-transfer-strip">
          <div className="config-section-header">
            <div>
              <h3>{t('incomingTransfers')}</h3>
            </div>
            <CircleButton title={t('history')} icon={History} onClick={() => onNavigate('inter-agency-transfers')} />
          </div>
          <div className="incoming-transfer-grid">
            {incomingTransfers.map((item) => (
              <article className="incoming-transfer-card" key={item.id}>
                <div>
                  <strong>{item.source_agency_name ?? 'Agence'}</strong>
                  <span>{arAccountName(item.source_account_name ?? t('account'))} ← {arAccountName(item.destination_account_name ?? t('account'))}</span>
                  {item.note && <small>{item.note}</small>}
                </div>
                <b>{money(item.amount)}</b>
                <div className="incoming-transfer-actions">
                  <button disabled={saving} onClick={() => decideIncomingTransfer(item.id, 'accept')}>{t('accept')}</button>
                  <button disabled={saving} onClick={() => decideIncomingTransfer(item.id, 'cancel')}>{t('reject')}</button>
                </div>
              </article>
            ))}
          </div>
        </section>
      )}

      {pendingReturnSettlements.length > 0 && (
        <section className="incoming-transfer-strip">
          <div className="config-section-header">
            <div>
              <h3>{t('pendingReturnRequests')}</h3>
            </div>
            <CircleButton title={t('history')} icon={History} onClick={() => onNavigate('inter-agency-transfers')} />
          </div>
          <div className="incoming-transfer-grid">
            {pendingReturnSettlements.map((item) => (
              <article className="incoming-transfer-card" key={item.id}>
                <div>
                  <strong>{item.payer_agency_name ?? t('interAgencyReturn')}</strong>
                  <span>{arAccountName(item.payer_account_name ?? t('account'))} → {arAccountName(item.receiver_account_name ?? t('account'))}</span>
                  <small>{t('debtAccount')}: {arAccountName(item.debt_account_name ?? t('account'))}</small>
                  {item.note && <small>{item.note}</small>}
                </div>
                <b>{money(item.amount)}</b>
                <div className="incoming-transfer-actions">
                  <button disabled={saving} onClick={() => acceptPendingReturn(item.id)}>{t('accept')}</button>
                </div>
              </article>
            ))}
          </div>
        </section>
      )}

      <div className="flow-board">
        {orderedAccounts.map((account) => {
          const config = getAccountCardConfig(configs, account.id);
          return (
            <div
              key={account.id}
              className={`account-drag-wrapper ${draggingAccountId === String(account.id) ? 'dragging' : ''}`}
              draggable
              onDragEnd={() => setDraggingAccountId('')}
              onDragOver={(event) => event.preventDefault()}
              onDragStart={(event) => {
                setDraggingAccountId(String(account.id));
                event.dataTransfer.effectAllowed = 'move';
                event.dataTransfer.setData('text/plain', String(account.id));
              }}
              onDrop={(event) => {
                event.preventDefault();
                const sourceId = event.dataTransfer.getData('text/plain') || draggingAccountId;
                reorderAccounts(sourceId, String(account.id));
                setDraggingAccountId('');
              }}
            >
              <CompteBox
                account={account}
                texts={config.texts.filter((item) => item.visible).sort((a, b) => a.position - b.position).map((item) => renderTextWidget(item, account, dashboard))}
                buttons={config.buttons}
                onOpen={canChangeBalance ? () => {
                  setError('');
                  setEditingBalanceAccount(account);
                  setManualBalance(String(account.balance ?? '0'));
                  setManualAccountSide(account.normal_balance_side ?? 'debit');
                } : undefined}
                onAction={(kind) => openAction(account, kind, config)}
                onDetails={canOpenDetails ? () => openAccountDetails(account) : undefined}
                onDelete={canDeleteAccount ? () => deleteAccount(account) : undefined}
              />
            </div>
          );
        })}
      </div>

      {detailsAccount && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label={t('accountDetails')}>
            <Panel title={arAccountName(detailsAccount.name)} icon={History}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div className="account-details-total">
                    <span>{t('totalBalance')}</span>
                    <strong>{money(detailsAccount.balance)}</strong>
                  </div>
                  <button className="circle-action" title={t('close')} onClick={() => setDetailsAccount(null)}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <div className="account-movement-header">
                  <span>{t('date')}</span>
                  <span>{t('note')}</span>
                  <strong>{t('debit')}</strong>
                  <strong>{t('credit')}</strong>
                  <strong>{t('action')}</strong>
                </div>
                <div className="account-movement-list">
                  {detailsRows.map((row) => (
                    <div className={`account-movement-row ${row.direction}`} key={row.id}>
                      <span>{new Date(row.occurred_at).toLocaleDateString()}</span>
                      <small>
                        {row.description ?? (row.direction === 'in' ? `${t('fromAccount')} ${arAccountName(row.from_account_name) || '-'}` : `${t('targetAccount')} ${arAccountName(row.to_account_name) || '-'}`)}
                      </small>
                      <b>{Number(row.debit || 0) ? money(row.debit) : '-'}</b>
                      <b>{Number(row.credit || 0) ? money(row.credit) : '-'}</b>
                      <strong>{Number(row.balance_effect || 0) >= 0 ? '+' : '-'}{money(Math.abs(Number(row.balance_effect || 0)))}</strong>
                    </div>
                  ))}
                  {!detailsRows.length && <div className="empty-service-state">{t('accountMovementsEmpty')}</div>}
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      <section className="contribution-section">
        <div className="config-section-header">
          <div>
            <h3>{t('details')} {t('contributor')}</h3>
          </div>
          <label className="form-field contribution-filter">
            {t('account')}
            <select value={contributionAccountId} onChange={(event) => setContributionAccountId(event.target.value)}>
              {accounts.map((account) => <option key={account.id} value={account.id}>{arAccountName(account.name)}</option>)}
            </select>
          </label>
        </div>
        <div className="contribution-card-grid">
          {contributorCards.map((card) => (
            <article className="unpaid-detail-card contribution-person-card" key={card.name}>
              <button className="unpaid-person-main" type="button" onClick={() => setHistoryContributorName(card.name)}>
                <strong>{card.name}</strong>
                <span>{arAccountName(selectedContributionAccount?.name) || t('account')} - {card.entries.length}</span>
              </button>
              <b className={card.total >= 0 ? '' : 'negative'}>{card.total >= 0 ? '+' : '-'}{money(Math.abs(card.total))}</b>
              {canUseMovement && <button className="mini-action add" title={tr('interAgencySend', language)} type="button" onClick={() => openContributorMovement(card, 'versement')}><Plus className="h-4 w-4" /></button>}
              {canUseMovement && <button className="mini-action out" title={tr('interAgencyReturn', language)} type="button" onClick={() => openContributorMovement(card, 'retrait')}><Minus className="h-4 w-4" /></button>}
              <CircleButton title={t('history')} icon={History} onClick={() => setHistoryContributorName(card.name)} />
            </article>
          ))}
          {!contributorCards.length && <div className="empty-service-state">{t('empty')}</div>}
        </div>
      </section>

      {historyContributor && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label={t('history')}>
            <Panel title={`${t('history')} ${historyContributor.name}`} icon={ChevronDown}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div className="account-details-total">
                    <span>{t('totalBalance')}</span>
                    <strong>{historyContributor.total >= 0 ? '+' : '-'}{money(Math.abs(historyContributor.total))}</strong>
                  </div>
                  <button className="circle-action" title={t('close')} onClick={() => setHistoryContributorName('')}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <div className="contribution-person-list">
                  {historyContributor.entries.map((entry) => (
                    <div className={`contribution-person ${entry.contribution.direction}`} key={`${historyContributor.name}-${entry.id}-${entry.occurred_at}`}>
                      <span>{new Date(entry.occurred_at).toLocaleDateString()}</span>
                      <small>{entry.description ?? (entry.contribution.direction === 'versement' ? tr('interAgencySend', language) : tr('interAgencyReturn', language))}</small>
                      <strong>{entry.signedAmount >= 0 ? '+' : '-'}{money(Math.abs(entry.signedAmount))}</strong>
                    </div>
                  ))}
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      {editingBalanceAccount && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label={t('editBalance')}>
            <Panel title={t('editBalance')} icon={Banknote}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div className="transfer-source">{arAccountName(editingBalanceAccount.name)}</div>
                  <button className="circle-action" title={t('close')} onClick={() => !saving && setEditingBalanceAccount(null)}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <label className="form-field">
                  {t('newBalance')}
                  <input value={manualBalance} inputMode="decimal" onChange={(event) => setManualBalance(event.target.value)} placeholder="0.00" />
                </label>
                <label className="form-field">
                  {t('accountBase')}
                  <select value={manualAccountSide} onChange={(event) => setManualAccountSide(event.target.value as 'debit' | 'credit')}>
                    <option value="debit">{t('baseDebit')}</option>
                    <option value="credit">{t('baseCredit')}</option>
                  </select>
                </label>
                {error && <div className="transfer-error">{error}</div>}
                <div className="modal-actions">
                  <button className="modal-cancel" disabled={saving} onClick={() => setEditingBalanceAccount(null)}>{t('cancel')}</button>
                  <button className="transfer-submit" disabled={saving} onClick={submitManualBalance}>{saving ? t('saveProgress') : t('saveBalance')}</button>
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      {creatingAccount && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label={t('createAccount')}>
            <Panel title={t('createAccount')} icon={Plus}>
              <div className="transfer-panel">
                <div className="transfer-panel-header modal-close-only">
                  <button className="circle-action" title={t('close')} onClick={() => !saving && setCreatingAccount(false)}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <div className="settings-grid">
                  <label className="form-field settings-wide">
                    {t('accountName')}
                    <input value={newAccountName} onChange={(event) => setNewAccountName(event.target.value)} placeholder={t('exampleAccount')} />
                  </label>
                  <label className="form-field settings-wide">
                    {t('initialBalance')}
                    <input value={newAccountBalance} inputMode="decimal" onChange={(event) => setNewAccountBalance(event.target.value)} placeholder="0.00" />
                  </label>
                  <label className="form-field settings-wide">
                    {t('accountBase')}
                    <select value={newAccountSide} onChange={(event) => setNewAccountSide(event.target.value as 'debit' | 'credit')}>
                      <option value="debit">{t('baseDebit')}</option>
                      <option value="credit">{t('baseCredit')}</option>
                    </select>
                  </label>
                </div>
                {error && <div className="transfer-error">{error}</div>}
                <div className="modal-actions">
                  <button className="modal-cancel" disabled={saving} onClick={() => setCreatingAccount(false)}>{t('cancel')}</button>
                  <button className="transfer-submit" disabled={saving} onClick={submitNewAccount}>{saving ? t('createProgress') : t('createAccount')}</button>
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      {interAgencyOpen && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label={t('interAgencyTransfers')}>
            <Panel title={t('interAgencyTransfers')} icon={Send}>
              <div className="transfer-panel">
                <div className="transfer-panel-header modal-close-only">
                  <button className="circle-action" title={t('close')} onClick={() => !saving && setInterAgencyOpen(false)}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <div className="operation-toggle settings-wide">
                  <button type="button" className={interAgencyMode === 'send' ? 'active' : ''} onClick={() => setInterAgencyMode('send')}>{t('interAgencySend')}</button>
                  <button type="button" className={interAgencyMode === 'return' ? 'active' : ''} onClick={() => setInterAgencyMode('return')}>{t('interAgencyReturn')}</button>
                </div>
                {interAgencyMode === 'send' && (
                  <label className="form-field">
                    {t('transferRules')}
                    <select value={interAgencyRuleId} onChange={(event) => setInterAgencyRuleId(event.target.value)}>
                      <option value="">{t('selectRule')}</option>
                      {activeInterAgencyRules.map((rule) => (
                        <option key={rule.id} value={rule.id}>
                          {rule.name} - {arAccountName(rule.source_account_name)} → {rule.destination_agency_name}/{arAccountName(rule.destination_account_name)}
                        </option>
                      ))}
                    </select>
                  </label>
                )}
                {interAgencyMode === 'return' && (
                  <>
                    <label className="form-field">
                      {t('debtOriginalAccount')}
                      <select value={returnTransferId} onChange={(event) => setReturnTransferId(event.target.value)}>
                        <option value="">{t('selectDebt')}</option>
                        {returnDebtOptions.map((item) => (
                          <option key={item.accountId} value={item.transfer.id}>
                            {arAccountName(item.accountName)} - {t('remainingAmount')}: {money(item.totalRemaining)}
                          </option>
                        ))}
                      </select>
                    </label>
                    <label className="form-field">
                      {t('fromAccount')}
                      <select value={returnPayerAccountId} onChange={(event) => setReturnPayerAccountId(event.target.value)}>
                        <option value="">{t('selectAccount')}</option>
                        {accounts.map((account) => <option key={account.id} value={account.id}>{arAccountName(account.name)}</option>)}
                      </select>
                    </label>
                    <label className="form-field">
                      {t('targetAccount')}
                      <select value={returnReceiverAccountId} onChange={(event) => setReturnReceiverAccountId(event.target.value)}>
                        <option value="">{t('selectAccount')}</option>
                        {returnReceiverAccounts.map((account) => <option key={account.id} value={account.id}>{arAccountName(account.name)}</option>)}
                      </select>
                    </label>
                    {selectedReturnDebt && (
                      <div className="settlement-simple-summary">
                        <span>{t('remainingAmount')}: <b>{money(selectedReturnDebt.totalRemaining)}</b></span>
                      </div>
                    )}
                  </>
                )}
                <label className="form-field">
                  {t('amount')}
                  <input value={interAgencyAmount} inputMode="decimal" onChange={(event) => setInterAgencyAmount(event.target.value)} placeholder="0.00" />
                </label>
                <label className="form-field">
                  {t('note')}
                  <input value={interAgencyNote} onChange={(event) => setInterAgencyNote(event.target.value)} placeholder={t('note')} />
                </label>
                {interAgencyMode === 'send' && !activeInterAgencyRules.length && <div className="empty-service-state">{t('noAcceptedRule')}</div>}
                {interAgencyMode === 'return' && !returnDebtOptions.length && <div className="empty-service-state">{t('noAcceptedDebt')}</div>}
                {error && <div className="transfer-error">{error}</div>}
                <div className="modal-actions">
                  <button className="modal-cancel" disabled={saving} onClick={() => setInterAgencyOpen(false)}>{t('cancel')}</button>
                  <button
                    className="transfer-submit"
                    disabled={saving || (interAgencyMode === 'send' ? !activeInterAgencyRules.length : !returnDebtOptions.length)}
                    onClick={interAgencyMode === 'send' ? submitInterAgencyTransfer : submitInterAgencyReturn}
                  >
                    {saving ? t('saveProgress') : interAgencyMode === 'send' ? t('interAgencySend') : t('requestReturn')}
                  </button>
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      {activeAction && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label={arText(modalTitle(activeAction.kind, activeAction.config.popups))}>
            <Panel title={arText(modalTitle(activeAction.kind, activeAction.config.popups))} icon={activeAction.kind === 'versement' ? ArrowDownToLine : ArrowRightLeft}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div className="history-summary-line">
                    <div className="transfer-source">{arAccountName(activeAction.account.name)}</div>
                    <div className="transfer-balance">{money(activeAction.account.balance)}</div>
                  </div>
                  <button className="circle-action" title={t('close')} onClick={closePopup}>
                    <X className="h-4 w-4" />
                  </button>
                </div>

                {activeAction.kind === 'versement' && (
                  <div className="transfer-grid single-operation">
                    {!activeAction.config.popups.movement.applyFixedType && (
                      <label className="form-field settings-wide">
                        {t('operation')}
                        <div className="operation-toggle">
                          <button type="button" className={movementType === 'versement' ? 'active' : ''} onClick={() => setMovementType('versement')}>{arText(activeAction.config.popups.movement.versementLabel)}</button>
                          <button type="button" className={movementType === 'retrait' ? 'active' : ''} onClick={() => setMovementType('retrait')}>{arText(activeAction.config.popups.movement.retraitLabel)}</button>
                        </div>
                      </label>
                    )}
                    {!activeAction.config.popups.movement.applyFixedAccount && (
                      <label className="form-field">
                        {arText(activeAction.config.popups.movement.accountLabel)}
                        <select value={targetAccountId} onChange={(event) => setTargetAccountId(event.target.value)}>
                          <option value="">{t('select')}</option>
                          {accounts.map((account) => (
                            <option key={account.id} value={account.id}>{arAccountName(account.name)}</option>
                          ))}
                        </select>
                      </label>
                    )}
                    <label className="form-field">
                      {arText(activeAction.config.popups.movement.amountLabel)}
                      <input value={amount} disabled={activeAction.config.popups.movement.applyFixedAmount} inputMode="decimal" onChange={(event) => setAmount(event.target.value)} placeholder="0.00" />
                    </label>
                    {activeAction.config.popups.movement.showDescription && !activeAction.config.popups.movement.applyFixedDescription && (
                      <label className="form-field transfer-description">
                        {arText(activeAction.config.popups.movement.descriptionLabel)}
                        <input value={description} onChange={(event) => setDescription(event.target.value)} placeholder={t('note')} />
                      </label>
                    )}
                    {activeAction.config.popups.movement.showContributors && (
                      <label className="form-field transfer-description contributor-picker-field">
                        {arText(activeAction.config.popups.movement.contributorsLabel)}
                        <input
                          value={contributorName}
                          onBlur={() => window.setTimeout(() => setContributorPickerOpen(false), 120)}
                          onChange={(event) => {
                            setContributorName(event.target.value);
                            setContributorPickerOpen(true);
                          }}
                          onFocus={() => setContributorPickerOpen(true)}
                          placeholder={t('contributorName')}
                        />
                        {contributorPickerOpen && Boolean(filteredContributorNames.length) && (
                          <div className="contributor-picker-list">
                            {filteredContributorNames.map((name) => (
                              <button key={name} type="button" onMouseDown={(event) => event.preventDefault()} onClick={() => { setContributorName(name); setContributorPickerOpen(false); }}>
                                {name}
                              </button>
                            ))}
                          </div>
                        )}
                      </label>
                    )}
                  </div>
                )}

                {activeAction.kind === 'transfer' && (
                  <div className="transfer-grid">
                    <label className="form-field">
                      {arText(activeAction.config.popups.transfer.fromLabel)}
                      <select value={sourceAccountId} disabled={activeAction.config.popups.transfer.applyFixedFromAccount} onChange={(event) => setSourceAccountId(event.target.value)}>
                        <option value="">{t('select')}</option>
                        {accounts.map((account) => (
                          <option key={account.id} value={account.id}>{arAccountName(account.name)}</option>
                        ))}
                      </select>
                    </label>
                    <label className="form-field">
                      {arText(activeAction.config.popups.transfer.toLabel)}
                      <select value={targetAccountId} disabled={activeAction.config.popups.transfer.applyFixedToAccount} onChange={(event) => setTargetAccountId(event.target.value)}>
                        <option value="">{t('select')}</option>
                        {transferTargets.map((account) => (
                          <option key={account.id} value={account.id}>{arAccountName(account.name)}</option>
                        ))}
                      </select>
                    </label>
                    <label className="form-field">
                      {arText(activeAction.config.popups.transfer.amountLabel)}
                      <input value={amount} disabled={activeAction.config.popups.transfer.applyFixedAmount} inputMode="decimal" onChange={(event) => setAmount(event.target.value)} placeholder="0.00" />
                    </label>
                  </div>
                )}

                {activeAction.kind === 'unpaid' && (
                  <div className="transfer-grid">
                    <label className="form-field">
                      {t('person')}
                      <input value={personName} onChange={(event) => setPersonName(event.target.value)} placeholder={t('customerName')} />
                    </label>
                    <label className="form-field">
                      {t('amount')}
                      <input value={amount} inputMode="decimal" onChange={(event) => setAmount(event.target.value)} placeholder="0.00" />
                    </label>
                    <label className="form-field transfer-description">
                      {t('note')}
                      <input value={description} onChange={(event) => setDescription(event.target.value)} placeholder={t('note')} />
                    </label>
                  </div>
                )}

                {activeAction.kind === 'transfer' && activeAction.config.popups.transfer.showDescription && (
                  <label className="form-field">
                    {arText(activeAction.config.popups.transfer.descriptionLabel)}
                    <input value={description} disabled={activeAction.config.popups.transfer.applyFixedDescription} onChange={(event) => setDescription(event.target.value)} placeholder={t('note')} />
                  </label>
                )}

                {error && <div className="transfer-error">{error}</div>}
                <div className="modal-actions">
                  <button className="modal-cancel" disabled={saving} onClick={closePopup}>{arText(activeAction.kind === 'transfer' ? activeAction.config.popups.transfer.cancelLabel : activeAction.config.popups.movement.cancelLabel)}</button>
                  <button className="transfer-submit" disabled={saving} onClick={submitAccountAction}>
                    {saving ? t('saveProgress') : arText(activeAction.kind === 'transfer' ? activeAction.config.popups.transfer.validateLabel : activeAction.config.popups.movement.validateLabel)}
                  </button>
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}
    </div>
  );
}

function KpiCard({ icon: Icon, label, value }: { icon: LucideIcon; label: string; value: string }) {
  return (
    <div className="account-kpi-card">
      <div className="account-kpi-icon"><Icon className="h-5 w-5" /></div>
      <div>
        <div className="account-kpi-label">{label}</div>
        <div className="account-kpi-value">{value}</div>
      </div>
    </div>
  );
}

function modalTitle(kind: AccountActionSlot, popups?: AccountCardConfig['popups']) {
  if (kind === 'versement') return popups?.movement.title ?? 'إيداع في الحساب';
  if (kind === 'transfer') return popups?.transfer.title ?? 'تحويل بين الحسابات';
  if (kind === 'unpaid') return 'إضافة غير مدفوع';
  return 'عملية الحساب';
}
