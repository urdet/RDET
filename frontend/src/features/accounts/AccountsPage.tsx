import { useEffect, useMemo, useState } from 'react';
import { ArrowDownToLine, ArrowRightLeft, Banknote, ChevronDown, History, Landmark, Minus, Plus, RefreshCw, Save, Search, Send, Settings2, TrendingDown, TrendingUp, X, type LucideIcon } from 'lucide-react';
import { acceptInterAgencyTransfer, api, cancelInterAgencyTransfer, createAccount, createInterAgencyTransfer, getAccountContributions, getAccountsScreenSettings, getAppSettings, listAgencyTransferRules, listInterAgencyTransfers, saveAccountsScreenSettings, updateAccountBalance } from '../../api';
import { can } from '../../permissions';
import { CircleButton } from '../../shared/ui/CircleButton';
import { Panel } from '../../shared/ui/Panel';
import { Account, AccountContributionEntry, AccountMovementEntry, AgencyTransferRule, AppSettings, CurrentUser, Dashboard, InterAgencyTransfer, ScreenId, TransferContribution } from '../../types';
import { money } from '../../utils/format';
import { actionTargets, AccountActionSlot, AccountButtonWidget, AccountCardConfig, AccountCardConfigMap, getAccountCardConfig, loadAccountCardConfigs, normalizeAccountCardConfig, renderTextWidget, saveAccountCardConfigs } from './accountCardConfig';
import { CompteBox } from './CompteBox';

type AccountsPageProps = {
  accounts: Account[];
  dashboard: Dashboard | null;
  currentUser: CurrentUser | null;
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

const accountOrderStorageKey = 'rdet_accounts_order';

export function AccountsPage({ accounts, dashboard, currentUser, onRefresh, onNavigate }: AccountsPageProps) {
  const [query, setQuery] = useState('');
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
  const [editingBalanceAccount, setEditingBalanceAccount] = useState<Account | null>(null);
  const [manualBalance, setManualBalance] = useState('');
  const [detailsAccount, setDetailsAccount] = useState<Account | null>(null);
  const [detailsRows, setDetailsRows] = useState<AccountMovementEntry[]>([]);
  const [contributionAccountId, setContributionAccountId] = useState(accounts[0] ? String(accounts[0].id) : '');
  const [contributionEntries, setContributionEntries] = useState<AccountContributionEntry[]>([]);
  const [popupContributionEntries, setPopupContributionEntries] = useState<AccountContributionEntry[]>([]);
  const [historyContributorName, setHistoryContributorName] = useState('');
  const [interAgencyRules, setInterAgencyRules] = useState<AgencyTransferRule[]>([]);
  const [incomingTransfers, setIncomingTransfers] = useState<InterAgencyTransfer[]>([]);
  const [interAgencyOpen, setInterAgencyOpen] = useState(false);
  const [interAgencyRuleId, setInterAgencyRuleId] = useState('');
  const [interAgencyAmount, setInterAgencyAmount] = useState('');
  const [interAgencyNote, setInterAgencyNote] = useState('');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

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

  const filteredAccounts = useMemo(() => {
    const normalized = query.trim().toLowerCase();
    if (!normalized) return orderedAccounts;
    return orderedAccounts.filter((account) => account.name.toLowerCase().includes(normalized) || String(account.legacy_id ?? account.id).includes(normalized));
  }, [orderedAccounts, query]);

  const visibleAccounts = accounts.filter((account) => account.visible).length;
  const positiveAccounts = accounts.filter((account) => Number(account.balance) >= 0).length;
  const transferTargets = accounts.filter((account) => String(account.id) !== sourceAccountId);
  const selectedContributionAccount = accounts.find((account) => String(account.id) === contributionAccountId) ?? accounts[0];
  const canCreateAccount = can(currentUser, appSettings, 'accounts', 'create');
  const canConfigureCards = can(currentUser, appSettings, 'account-settings', 'configure');
  const canChangeBalance = can(currentUser, appSettings, 'accounts', 'changeBalance');
  const canOpenDetails = can(currentUser, appSettings, 'accounts', 'open');
  const canUseAccountActions = can(currentUser, appSettings, 'accounts', 'accountAction');
  const canUseTransfer = can(currentUser, appSettings, 'accounts', 'transfer');
  const canUseMovement = can(currentUser, appSettings, 'accounts', 'movement');
  const activeInterAgencyRules = interAgencyRules.filter((rule) => rule.status === 'active' && rule.active && rule.source_agency_id === currentUser?.company_id);

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
        if (Array.isArray(savedOrder)) {
          const cleanOrder = savedOrder.map(String);
          setAccountOrder(cleanOrder);
          localStorage.setItem(accountOrderStorageKey, JSON.stringify(cleanOrder));
        }
        const accountConfigs = Object.fromEntries(Object.entries(serverConfigs ?? {}).filter(([key]) => key !== '__accountOrder')) as AccountCardConfigMap;
        if (accountConfigs && Object.keys(accountConfigs).length) {
          const legacy = accountConfigs as unknown as { buttons?: unknown; popups?: unknown };
          const normalized = legacy.buttons || legacy.popups
            ? Object.fromEntries(accounts.map((account) => [String(account.id), normalizeAccountCardConfig({ ...getAccountCardConfig(configs, account.id), buttons: legacy.buttons as AccountButtonWidget[] | undefined, popups: legacy.popups as AccountCardConfig['popups'] | undefined })]))
            : Object.fromEntries(Object.entries(accountConfigs).map(([accountId, accountConfig]) => [accountId, normalizeAccountCardConfig(accountConfig)]));
          setConfigs(normalized);
          saveAccountCardConfigs(normalized);
          if (legacy.buttons || legacy.popups) saveAccountsScreenSettings({ ...normalized, __accountOrder: savedOrder ?? accountOrder } as unknown as AccountCardConfigMap).catch(() => undefined);
        } else {
          saveAccountsScreenSettings({ ...configs, __accountOrder: accountOrder } as unknown as AccountCardConfigMap).catch(() => undefined);
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
    const [rules, transfers] = await Promise.all([
      listAgencyTransferRules().catch(() => []),
      listInterAgencyTransfers('pending_receiver').catch(() => []),
    ]);
    setInterAgencyRules(rules);
    setIncomingTransfers(transfers.filter((item) => item.destination_agency_id === currentUser?.company_id));
  }

  useEffect(() => {
    if (!currentUser?.company_id) return;
    refreshInterAgency().catch(() => undefined);
  }, [currentUser?.company_id]);

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
      setError('Action non autorisee pour cet utilisateur.');
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
      await saveAccountsScreenSettings({ ...configs, __accountOrder: cleanOrder } as unknown as AccountCardConfigMap);
      localStorage.setItem(accountOrderStorageKey, JSON.stringify(cleanOrder));
      setOrderDirty(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Sauvegarde ordre impossible.');
    } finally {
      setSaving(false);
    }
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
            description: description || `Non paye lie a ${activeAction.account.name}`,
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
      setError('Choisir une regle et un montant valide.');
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
      setError(err instanceof Error ? err.message : 'Transfert inter-agence impossible.');
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
      setError(err instanceof Error ? err.message : 'Decision impossible.');
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
      await createAccount({ name: newAccountName.trim(), balance: newAccountBalance || '0', visible: true });
      setCreatingAccount(false);
      setNewAccountName('');
      setNewAccountBalance('0');
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
      await updateAccountBalance(editingBalanceAccount.id, manualBalance || '0');
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
      <div className="accounts-kpi-grid">
        <KpiCard icon={Landmark} label="Solde total" value={money(dashboard?.total_balance ?? 0)} />
        <KpiCard icon={TrendingUp} label="Versements jour" value={money(dashboard?.service_in ?? 0)} />
        <KpiCard icon={TrendingDown} label="Retraits jour" value={money(dashboard?.service_out ?? 0)} />
        <KpiCard icon={Banknote} label="Non paye" value={money(dashboard?.unpaid_total ?? 0)} />
      </div>

      <div className="accounts-toolbar">
        <label className="account-search">
          <Search className="h-4 w-4" />
          <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Rechercher un compte..." />
        </label>
        <div className="account-toolbar-actions">
          <div className="account-count">{filteredAccounts.length} comptes, {visibleAccounts} visibles, {positiveAccounts} positifs</div>
          {canUseTransfer && <CircleButton title="Transfert inter-agence" icon={Send} onClick={() => { setError(''); setInterAgencyOpen(true); setInterAgencyRuleId(activeInterAgencyRules[0] ? String(activeInterAgencyRules[0].id) : ''); }} />}
          {canCreateAccount && <CircleButton title="Ajouter compte" icon={Plus} onClick={() => { setError(''); setCreatingAccount(true); }} />}
          {orderDirty && <CircleButton title="Enregistrer ordre" icon={Save} onClick={saveAccountOrder} />}
          {canConfigureCards && <CircleButton title="Configurer cartes" icon={Settings2} onClick={() => onNavigate('account-settings')} />}
          <CircleButton title="Actualiser" icon={RefreshCw} onClick={onRefresh} />
        </div>
      </div>

      {incomingTransfers.length > 0 && (
        <section className="incoming-transfer-strip">
          <div className="config-section-header">
            <div>
              <h3>Incoming Transfers</h3>
            </div>
            <CircleButton title="Historique" icon={History} onClick={() => onNavigate('inter-agency-transfers')} />
          </div>
          <div className="incoming-transfer-grid">
            {incomingTransfers.map((item) => (
              <article className="incoming-transfer-card" key={item.id}>
                <div>
                  <strong>{item.source_agency_name ?? 'Agence'}</strong>
                  <span>{item.source_account_name ?? 'Compte'} {'->'} {item.destination_account_name ?? 'Compte'}</span>
                  {item.note && <small>{item.note}</small>}
                </div>
                <b>{money(item.amount)}</b>
                <div className="incoming-transfer-actions">
                  <button disabled={saving} onClick={() => decideIncomingTransfer(item.id, 'accept')}>Accept</button>
                  <button disabled={saving} onClick={() => decideIncomingTransfer(item.id, 'cancel')}>Cancel</button>
                </div>
              </article>
            ))}
          </div>
        </section>
      )}

      <div className="flow-board">
        {filteredAccounts.map((account) => {
          const config = getAccountCardConfig(configs, account.id);
          return (
            <div
              key={account.id}
              className={`account-drag-wrapper ${draggingAccountId === String(account.id) ? 'dragging' : ''}`}
              draggable={!query.trim()}
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
                } : undefined}
                onAction={(kind) => openAction(account, kind, config)}
                onDetails={canOpenDetails ? () => openAccountDetails(account) : undefined}
              />
            </div>
          );
        })}
      </div>

      {detailsAccount && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label="Details compte">
            <Panel title={`Details ${detailsAccount.name}`} icon={History}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div className="history-summary-line">
                    <div className="transfer-source">{detailsAccount.name}</div>
                    <div className="transfer-balance">Solde: {money(detailsAccount.balance)}</div>
                  </div>
                  <button className="circle-action" title="Fermer" onClick={() => setDetailsAccount(null)}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <div className="account-movement-list">
                  {detailsRows.map((row) => (
                    <div className={`account-movement-row ${row.direction}`} key={row.id}>
                      <span>{new Date(row.occurred_at).toLocaleDateString()}</span>
                      <small>
                        {row.description ?? (row.direction === 'in' ? `Depuis ${row.from_account_name ?? 'externe'}` : `Vers ${row.to_account_name ?? 'externe'}`)}
                      </small>
                      <strong>{row.direction === 'in' ? '+' : '-'}{money(row.amount)}</strong>
                    </div>
                  ))}
                  {!detailsRows.length && <div className="empty-service-state">Aucun mouvement.</div>}
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      <section className="contribution-section">
        <div className="config-section-header">
          <div>
            <h3>Details des contributions</h3>
            <div className="formula-help">Filtrer par compte pour voir l'historique des contributeurs: versement en vert, retrait en rouge.</div>
          </div>
          <label className="form-field contribution-filter">
            Compte
            <select value={contributionAccountId} onChange={(event) => setContributionAccountId(event.target.value)}>
              {accounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
            </select>
          </label>
        </div>
        <div className="contribution-card-grid">
          {contributorCards.map((card) => (
            <article className="unpaid-detail-card contribution-person-card" key={card.name}>
              <button className="unpaid-person-main" type="button" onClick={() => setHistoryContributorName(card.name)}>
                <strong>{card.name}</strong>
                <span>{selectedContributionAccount?.name ?? 'Compte'} - {card.entries.length} mouvements</span>
              </button>
              <b className={card.total >= 0 ? '' : 'negative'}>{card.total >= 0 ? '+' : '-'}{money(Math.abs(card.total))}</b>
              {canUseMovement && <button className="mini-action add" title="Versement" type="button" onClick={() => openContributorMovement(card, 'versement')}><Plus className="h-4 w-4" /></button>}
              {canUseMovement && <button className="mini-action out" title="Retrait" type="button" onClick={() => openContributorMovement(card, 'retrait')}><Minus className="h-4 w-4" /></button>}
              <CircleButton title="History" icon={History} onClick={() => setHistoryContributorName(card.name)} />
            </article>
          ))}
          {!contributorCards.length && <div className="empty-service-state">Aucune contribution pour ce compte.</div>}
        </div>
      </section>

      {historyContributor && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label="Historique contributeur">
            <Panel title={`Historique ${historyContributor.name}`} icon={ChevronDown}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div className="history-summary-line">
                    <div className="transfer-source">{historyContributor.name}</div>
                    <div className="transfer-balance">Total: {historyContributor.total >= 0 ? '+' : '-'}{money(Math.abs(historyContributor.total))}</div>
                  </div>
                  <button className="circle-action" title="Fermer" onClick={() => setHistoryContributorName('')}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <div className="contribution-person-list">
                  {historyContributor.entries.map((entry) => (
                    <div className={`contribution-person ${entry.contribution.direction}`} key={`${historyContributor.name}-${entry.id}-${entry.occurred_at}`}>
                      <span>{new Date(entry.occurred_at).toLocaleDateString()}</span>
                      <small>{entry.description ?? (entry.contribution.direction === 'versement' ? 'Versement' : 'Retrait')}</small>
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
          <div className="account-modal" role="dialog" aria-modal="true" aria-label="Modifier solde">
            <Panel title="Modifier solde" icon={Banknote}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div className="history-summary-line">
                    <div className="transfer-source">{editingBalanceAccount.name}</div>
                    <div className="transfer-balance">Ancien solde: {money(editingBalanceAccount.previous_balance ?? editingBalanceAccount.balance)}</div>
                  </div>
                  <button className="circle-action" title="Fermer" onClick={() => !saving && setEditingBalanceAccount(null)}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <label className="form-field">
                  Nouveau solde
                  <input value={manualBalance} inputMode="decimal" onChange={(event) => setManualBalance(event.target.value)} placeholder="0.00" />
                </label>
                {error && <div className="transfer-error">{error}</div>}
                <div className="modal-actions">
                  <button className="modal-cancel" disabled={saving} onClick={() => setEditingBalanceAccount(null)}>Annuler</button>
                  <button className="transfer-submit" disabled={saving} onClick={submitManualBalance}>{saving ? 'Enregistrement...' : 'Enregistrer solde'}</button>
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      {creatingAccount && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label="Ajouter compte">
            <Panel title="Ajouter compte" icon={Plus}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div>
                    <div className="transfer-source">Nouveau compte</div>
                    <div className="transfer-balance">Agence active</div>
                  </div>
                  <button className="circle-action" title="Fermer" onClick={() => !saving && setCreatingAccount(false)}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <div className="settings-grid">
                  <label className="form-field settings-wide">
                    Nom du compte
                    <input value={newAccountName} onChange={(event) => setNewAccountName(event.target.value)} placeholder="Ex: Cash principal" />
                  </label>
                  <label className="form-field settings-wide">
                    Solde initial
                    <input value={newAccountBalance} inputMode="decimal" onChange={(event) => setNewAccountBalance(event.target.value)} placeholder="0.00" />
                  </label>
                </div>
                {error && <div className="transfer-error">{error}</div>}
                <div className="modal-actions">
                  <button className="modal-cancel" disabled={saving} onClick={() => setCreatingAccount(false)}>Annuler</button>
                  <button className="transfer-submit" disabled={saving} onClick={submitNewAccount}>{saving ? 'Creation...' : 'Creer compte'}</button>
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      {interAgencyOpen && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label="Transfert inter-agence">
            <Panel title="Transfert inter-agence" icon={Send}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div>
                    <div className="transfer-source">Demande de transfert</div>
                    <div className="transfer-balance">Validation par l'agence destinataire</div>
                  </div>
                  <button className="circle-action" title="Fermer" onClick={() => !saving && setInterAgencyOpen(false)}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <label className="form-field">
                  Regle
                  <select value={interAgencyRuleId} onChange={(event) => setInterAgencyRuleId(event.target.value)}>
                    <option value="">Selectionner</option>
                    {activeInterAgencyRules.map((rule) => (
                      <option key={rule.id} value={rule.id}>
                        {rule.name} - {rule.source_account_name} vers {rule.destination_agency_name}/{rule.destination_account_name}
                      </option>
                    ))}
                  </select>
                </label>
                <label className="form-field">
                  Montant
                  <input value={interAgencyAmount} inputMode="decimal" onChange={(event) => setInterAgencyAmount(event.target.value)} placeholder="0.00" />
                </label>
                <label className="form-field">
                  Note
                  <input value={interAgencyNote} onChange={(event) => setInterAgencyNote(event.target.value)} placeholder="Note" />
                </label>
                {!activeInterAgencyRules.length && <div className="empty-service-state">Aucune regle active. Creez et faites accepter une regle dans Inter-agency.</div>}
                {error && <div className="transfer-error">{error}</div>}
                <div className="modal-actions">
                  <button className="modal-cancel" disabled={saving} onClick={() => setInterAgencyOpen(false)}>Annuler</button>
                  <button className="transfer-submit" disabled={saving || !activeInterAgencyRules.length} onClick={submitInterAgencyTransfer}>
                    {saving ? 'Envoi...' : 'Envoyer'}
                  </button>
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      {activeAction && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label={modalTitle(activeAction.kind, activeAction.config.popups)}>
            <Panel title={modalTitle(activeAction.kind, activeAction.config.popups)} icon={activeAction.kind === 'versement' ? ArrowDownToLine : ArrowRightLeft}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div className="history-summary-line">
                    <div className="transfer-source">{activeAction.account.name}</div>
                    <div className="transfer-balance">{money(activeAction.account.balance)}</div>
                  </div>
                  <button className="circle-action" title="Fermer" onClick={closePopup}>
                    <X className="h-4 w-4" />
                  </button>
                </div>

                {activeAction.kind === 'versement' && (
                  <div className="transfer-grid single-operation">
                    {!activeAction.config.popups.movement.applyFixedType && (
                      <label className="form-field settings-wide">
                        Operation
                        <div className="operation-toggle">
                          <button type="button" className={movementType === 'versement' ? 'active' : ''} onClick={() => setMovementType('versement')}>{activeAction.config.popups.movement.versementLabel}</button>
                          <button type="button" className={movementType === 'retrait' ? 'active' : ''} onClick={() => setMovementType('retrait')}>{activeAction.config.popups.movement.retraitLabel}</button>
                        </div>
                      </label>
                    )}
                    {!activeAction.config.popups.movement.applyFixedAccount && (
                      <label className="form-field">
                        {activeAction.config.popups.movement.accountLabel}
                        <select value={targetAccountId} onChange={(event) => setTargetAccountId(event.target.value)}>
                          <option value="">Selectionner</option>
                          {accounts.map((account) => (
                            <option key={account.id} value={account.id}>{account.name}</option>
                          ))}
                        </select>
                      </label>
                    )}
                    <label className="form-field">
                      {activeAction.config.popups.movement.amountLabel}
                      <input value={amount} disabled={activeAction.config.popups.movement.applyFixedAmount} inputMode="decimal" onChange={(event) => setAmount(event.target.value)} placeholder="0.00" />
                    </label>
                    {activeAction.config.popups.movement.showDescription && !activeAction.config.popups.movement.applyFixedDescription && (
                      <label className="form-field transfer-description">
                        {activeAction.config.popups.movement.descriptionLabel}
                        <input value={description} onChange={(event) => setDescription(event.target.value)} placeholder="Note" />
                      </label>
                    )}
                    {activeAction.config.popups.movement.showContributors && (
                      <label className="form-field transfer-description contributor-picker-field">
                        {activeAction.config.popups.movement.contributorsLabel}
                        <input
                          value={contributorName}
                          onBlur={() => window.setTimeout(() => setContributorPickerOpen(false), 120)}
                          onChange={(event) => {
                            setContributorName(event.target.value);
                            setContributorPickerOpen(true);
                          }}
                          onFocus={() => setContributorPickerOpen(true)}
                          placeholder="Nom contributeur"
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
                      {activeAction.config.popups.transfer.fromLabel}
                      <select value={sourceAccountId} disabled={activeAction.config.popups.transfer.applyFixedFromAccount} onChange={(event) => setSourceAccountId(event.target.value)}>
                        <option value="">Selectionner</option>
                        {accounts.map((account) => (
                          <option key={account.id} value={account.id}>{account.name}</option>
                        ))}
                      </select>
                    </label>
                    <label className="form-field">
                      {activeAction.config.popups.transfer.toLabel}
                      <select value={targetAccountId} disabled={activeAction.config.popups.transfer.applyFixedToAccount} onChange={(event) => setTargetAccountId(event.target.value)}>
                        <option value="">Selectionner</option>
                        {transferTargets.map((account) => (
                          <option key={account.id} value={account.id}>{account.name}</option>
                        ))}
                      </select>
                    </label>
                    <label className="form-field">
                      {activeAction.config.popups.transfer.amountLabel}
                      <input value={amount} disabled={activeAction.config.popups.transfer.applyFixedAmount} inputMode="decimal" onChange={(event) => setAmount(event.target.value)} placeholder="0.00" />
                    </label>
                  </div>
                )}

                {activeAction.kind === 'unpaid' && (
                  <div className="transfer-grid">
                    <label className="form-field">
                      Personne
                      <input value={personName} onChange={(event) => setPersonName(event.target.value)} placeholder="Nom client" />
                    </label>
                    <label className="form-field">
                      Montant
                      <input value={amount} inputMode="decimal" onChange={(event) => setAmount(event.target.value)} placeholder="0.00" />
                    </label>
                    <label className="form-field transfer-description">
                      Description
                      <input value={description} onChange={(event) => setDescription(event.target.value)} placeholder="Note" />
                    </label>
                  </div>
                )}

                {activeAction.kind === 'transfer' && activeAction.config.popups.transfer.showDescription && (
                  <label className="form-field">
                    {activeAction.config.popups.transfer.descriptionLabel}
                    <input value={description} disabled={activeAction.config.popups.transfer.applyFixedDescription} onChange={(event) => setDescription(event.target.value)} placeholder="Note" />
                  </label>
                )}

                {error && <div className="transfer-error">{error}</div>}
                <div className="modal-actions">
                  <button className="modal-cancel" disabled={saving} onClick={closePopup}>{activeAction.kind === 'transfer' ? activeAction.config.popups.transfer.cancelLabel : activeAction.config.popups.movement.cancelLabel}</button>
                  <button className="transfer-submit" disabled={saving} onClick={submitAccountAction}>
                    {saving ? 'Enregistrement...' : activeAction.kind === 'transfer' ? activeAction.config.popups.transfer.validateLabel : activeAction.config.popups.movement.validateLabel}
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
  if (kind === 'versement') return popups?.movement.title ?? 'Versement au compte';
  if (kind === 'transfer') return popups?.transfer.title ?? 'Transfert entre comptes';
  if (kind === 'unpaid') return 'Ajouter non paye';
  return 'Action compte';
}
