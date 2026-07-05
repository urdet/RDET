import { useEffect, useMemo, useState } from 'react';
import { Check, Link2, Send, X } from 'lucide-react';
import {
  acceptAgencyLink,
  acceptAgencyTransferRule,
  acceptInterAgencySettlement,
  cancelInterAgencyTransfer,
  createAgencyLink,
  createAgencyTransferRule,
  createInterAgencySettlement,
  listAgencyAccounts,
  listAgencyLinks,
  listAgencyTransferRules,
  listInterAgencySettlements,
  listInterAgencyTransfers,
  rejectAgencyLink,
  rejectAgencyTransferRule,
} from '../../api';
import { Language, tr, trStatus } from '../../i18n';
import { CircleButton } from '../../shared/ui/CircleButton';
import { DataTable } from '../../shared/ui/DataTable';
import { Panel } from '../../shared/ui/Panel';
import { Account, Agency, AgencyLink, AgencyTransferRule, CurrentUser, InterAgencySettlement, InterAgencyTransfer } from '../../types';
import { money } from '../../utils/format';

type Props = {
  accounts: Account[];
  agencies: Agency[];
  currentUser: CurrentUser | null;
  language: Language;
};

type ReturnDebtOption = {
  accountId: number;
  accountName: string;
  transfer: InterAgencyTransfer;
  transferIds: number[];
  totalRemaining: number;
};

export function InterAgencyTransfersPage({ accounts, agencies, currentUser, language }: Props) {
  const [links, setLinks] = useState<AgencyLink[]>([]);
  const [rules, setRules] = useState<AgencyTransferRule[]>([]);
  const [transfers, setTransfers] = useState<InterAgencyTransfer[]>([]);
  const [settlements, setSettlements] = useState<InterAgencySettlement[]>([]);
  const [remoteAccounts, setRemoteAccounts] = useState<Account[]>([]);
  const [returnReceiverAccounts, setReturnReceiverAccounts] = useState<Account[]>([]);
  const [targetAgencyId, setTargetAgencyId] = useState('');
  const [linkId, setLinkId] = useState('');
  const [sourceAccountId, setSourceAccountId] = useState('');
  const [destinationAccountId, setDestinationAccountId] = useState('');
  const [ruleName, setRuleName] = useState('');
  const [ruleDescription, setRuleDescription] = useState('');
  const [returnTransferId, setReturnTransferId] = useState('');
  const [returnPayerAccountId, setReturnPayerAccountId] = useState('');
  const [returnReceiverAccountId, setReturnReceiverAccountId] = useState('');
  const [returnAmount, setReturnAmount] = useState('');
  const [returnNote, setReturnNote] = useState('');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  const t = (key: string) => tr(key, language);
  const status = (value: string | null | undefined) => trStatus(value, language);
  const activeAgencyId = currentUser?.company_id ?? null;
  const activeLinks = links.filter((link) => link.status === 'active');
  const selectedLink = activeLinks.find((link) => String(link.id) === linkId) ?? activeLinks[0] ?? null;
  const remoteAgencyId = selectedLink && activeAgencyId
    ? (selectedLink.agency_a_id === activeAgencyId ? selectedLink.agency_b_id : selectedLink.agency_a_id)
    : null;
  const localAccounts = accounts.filter((account) => activeAgencyId && account.company_ids.includes(activeAgencyId));
  const otherAgencies = agencies.filter((agency) => agency.id !== activeAgencyId);
  const returnableTransfers = transfers.filter((item) => item.status === 'accepted' && item.destination_agency_id === activeAgencyId);
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
  const linkChoices = useMemo(() => activeLinks.map((link) => ({
    id: link.id,
    label: link.agency_a_id === activeAgencyId ? link.agency_b_name : link.agency_a_name,
  })), [activeAgencyId, activeLinks]);

  function linkTargetAgencyId(link: AgencyLink) {
    if (link.target_agency_id) return link.target_agency_id;
    if (link.requested_agency_id === link.agency_a_id) return link.agency_b_id;
    if (link.requested_agency_id === link.agency_b_id) return link.agency_a_id;
    return null;
  }

  function linkStatusLabel(link: AgencyLink) {
    if (link.status !== 'pending') return status(link.status);
    return linkTargetAgencyId(link) === activeAgencyId ? t('pendingYourApproval') : t('pendingAgency');
  }

  async function refresh() {
    const [linkRows, ruleRows, transferRows, settlementRows] = await Promise.all([
      listAgencyLinks(),
      listAgencyTransferRules(),
      listInterAgencyTransfers(),
      listInterAgencySettlements(),
    ]);
    setLinks(linkRows);
    setRules(ruleRows);
    setTransfers(transferRows);
    setSettlements(settlementRows);
    if (!linkId && linkRows.find((item) => item.status === 'active')) {
      setLinkId(String(linkRows.find((item) => item.status === 'active')?.id ?? ''));
    }
  }

  useEffect(() => {
    refresh().catch((err) => setError(err instanceof Error ? err.message : 'Chargement impossible.'));
  }, [activeAgencyId]);

  useEffect(() => {
    setDestinationAccountId('');
    if (!remoteAgencyId) {
      setRemoteAccounts([]);
      return;
    }
    listAgencyAccounts(remoteAgencyId)
      .then(setRemoteAccounts)
      .catch((err) => {
        setRemoteAccounts([]);
        setError(err instanceof Error ? err.message : t('loadAgencyAccountsError'));
      });
  }, [remoteAgencyId, language]);

  useEffect(() => {
    if (!selectedReturnTransfer) {
      setReturnReceiverAccounts([]);
      return;
    }
    setReturnTransferId(String(selectedReturnTransfer.id));
    setReturnAmount('');
    listAgencyAccounts(selectedReturnTransfer.source_agency_id)
      .then((rows) => {
        setReturnReceiverAccounts(rows);
        setReturnReceiverAccountId(String(selectedReturnTransfer.source_account_id || rows[0]?.id || ''));
      })
      .catch(() => setReturnReceiverAccounts([]));
    setReturnPayerAccountId((current) => current || String(localAccounts[0]?.id ?? ''));
  }, [selectedReturnTransfer?.id]);

  async function run(action: () => Promise<unknown>) {
    setSaving(true);
    setError('');
    try {
      await action();
      await refresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Action impossible.');
    } finally {
      setSaving(false);
    }
  }

  async function submitLink() {
    if (!targetAgencyId) {
      setError(t('chooseAgencyError'));
      return;
    }
    await run(() => createAgencyLink(Number(targetAgencyId)));
    setTargetAgencyId('');
  }

  async function submitRule() {
    if (!selectedLink || !activeAgencyId || !remoteAgencyId || !sourceAccountId || !destinationAccountId || !ruleName.trim()) {
      setError(t('completeRuleError'));
      return;
    }
    await run(() => createAgencyTransferRule({
      agency_link_id: selectedLink.id,
      source_agency_id: activeAgencyId,
      source_account_id: Number(sourceAccountId),
      destination_agency_id: remoteAgencyId,
      destination_account_id: Number(destinationAccountId),
      name: ruleName.trim(),
      description: ruleDescription || undefined,
    }));
    setRuleName('');
    setRuleDescription('');
  }

  async function submitSettlement() {
    if (!selectedReturnTransfer || !returnPayerAccountId || !returnReceiverAccountId || Number(returnAmount) <= 0) {
      setError(t('completeReturnError'));
      return;
    }
    await run(() => createInterAgencySettlement({
      inter_agency_transfer_id: selectedReturnTransfer.id,
      payer_account_id: Number(returnPayerAccountId),
      receiver_account_id: Number(returnReceiverAccountId),
      amount: returnAmount,
      note: returnNote || undefined,
    }));
    setReturnAmount('');
    setReturnNote('');
  }

  return (
    <div className="inter-agency-page">
      <div className="inter-agency-grid">
        <Panel title={t('agencyLinks')} icon={Link2}>
          <div className="inline-form">
            <select value={targetAgencyId} onChange={(event) => setTargetAgencyId(event.target.value)}>
              <option value="">{t('selectAgency')}</option>
              {otherAgencies.map((agency) => <option key={agency.id} value={agency.id}>{agency.name}</option>)}
            </select>
            <button disabled={saving} onClick={submitLink}>{t('requestLink')}</button>
          </div>
          <div className="compact-list">
            {links.map((link) => (
              <div className="compact-row" key={link.id}>
                <span>{link.agency_a_name} / {link.agency_b_name}</span>
                <b>{linkStatusLabel(link)}</b>
                {link.status === 'pending' && linkTargetAgencyId(link) === activeAgencyId && (
                  <>
                    <CircleButton title={t('accept')} icon={Check} onClick={() => run(() => acceptAgencyLink(link.id))} />
                    <CircleButton title={t('reject')} icon={X} onClick={() => run(() => rejectAgencyLink(link.id))} />
                  </>
                )}
              </div>
            ))}
          </div>
        </Panel>

        <Panel title={t('transferRules')} icon={Send}>
          <div className="rule-form">
            <select value={linkId} onChange={(event) => setLinkId(event.target.value)}>
              <option value="">{t('link')}</option>
              {linkChoices.map((choice) => <option key={choice.id} value={choice.id}>{choice.label}</option>)}
            </select>
            <select value={sourceAccountId} onChange={(event) => setSourceAccountId(event.target.value)}>
              <option value="">{t('sourceAccount')}</option>
              {localAccounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
            </select>
            <select value={destinationAccountId} onChange={(event) => setDestinationAccountId(event.target.value)}>
              <option value="">{t('targetAccount')}</option>
              {remoteAccounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
            </select>
            <input value={ruleName} onChange={(event) => setRuleName(event.target.value)} placeholder={t('ruleName')} />
            <input value={ruleDescription} onChange={(event) => setRuleDescription(event.target.value)} placeholder={t('note')} />
            <button disabled={saving} onClick={submitRule}>{t('create')}</button>
          </div>
          <div className="compact-list">
            {rules.map((rule) => (
              <div className="compact-row" key={rule.id}>
                <span>{rule.name} - {rule.source_account_name} ← {rule.destination_agency_name}/{rule.destination_account_name}</span>
                <b>{status(rule.status)}</b>
                {rule.status === 'pending' && rule.destination_agency_id === activeAgencyId && (
                  <>
                    <CircleButton title={t('accept')} icon={Check} onClick={() => run(() => acceptAgencyTransferRule(rule.id))} />
                    <CircleButton title={t('reject')} icon={X} onClick={() => run(() => rejectAgencyTransferRule(rule.id))} />
                  </>
                )}
              </div>
            ))}
          </div>
        </Panel>
      </div>

      {error && <div className="transfer-error">{error}</div>}

      <Panel title={t('requestReturn')} icon={Send}>
        <div className="rule-form settlement-form">
          <select value={returnTransferId} onChange={(event) => setReturnTransferId(event.target.value)}>
            <option value="">{t('debtOriginalAccount')}</option>
            {returnDebtOptions.map((item) => (
              <option key={item.accountId} value={item.transfer.id}>
                {item.accountName} - {t('remainingAmount')}: {money(item.totalRemaining)}
              </option>
            ))}
          </select>
          <select value={returnPayerAccountId} onChange={(event) => setReturnPayerAccountId(event.target.value)}>
            <option value="">{t('fromAccount')}</option>
            {localAccounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
          </select>
          <select value={returnReceiverAccountId} onChange={(event) => setReturnReceiverAccountId(event.target.value)}>
            <option value="">{t('targetAccount')}</option>
            {returnReceiverAccounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
          </select>
          <input value={returnAmount} onChange={(event) => setReturnAmount(event.target.value)} placeholder={t('amount')} />
          <input value={returnNote} onChange={(event) => setReturnNote(event.target.value)} placeholder={t('note')} />
          <button disabled={saving || !returnDebtOptions.length} onClick={submitSettlement}>{t('requestReturn')}</button>
        </div>
        {selectedReturnDebt && (
          <div className="settlement-simple-summary">
            <span>{t('remainingAmount')}: <b>{money(selectedReturnDebt.totalRemaining)}</b></span>
          </div>
        )}
        {!returnDebtOptions.length && <div className="empty-service-state">{t('noAcceptedDebt')}</div>}
      </Panel>

      <DataTable
        title={t('interAgencyTransfers')}
        headers={[t('date'), t('sourceAgencyAccount'), t('targetAgencyAccount'), t('amount'), t('status'), t('note'), t('action')]}
        rows={transfers.map((item) => [
          new Date(item.created_at).toLocaleDateString(),
          `${item.source_agency_name ?? ''} / ${item.source_account_name ?? ''}`,
          `${item.destination_agency_name ?? ''} / ${item.destination_account_name ?? ''}`,
          money(item.amount),
          status(item.status),
          item.note ?? '',
          item.status === 'pending_receiver' && item.destination_agency_id === activeAgencyId
            ? <button className="table-icon-button danger" disabled={saving} onClick={() => run(() => cancelInterAgencyTransfer(item.id))}>{t('cancel')}</button>
            : '',
        ])}
      />
      <DataTable
        title={t('returnHistory')}
        headers={[t('date'), t('debtAccount'), t('fromAccount'), t('targetAccount'), t('amount'), t('status'), t('action')]}
        rows={settlements.map((item) => [
          new Date(item.created_at).toLocaleDateString(),
          item.debt_account_name ?? '',
          `${item.payer_agency_name ?? ''} / ${item.payer_account_name ?? ''}`,
          `${item.receiver_agency_name ?? ''} / ${item.receiver_account_name ?? ''}`,
          money(item.amount),
          status(item.status),
          item.status === 'pending' && item.receiver_agency_id === activeAgencyId
            ? <button className="table-icon-button" disabled={saving} onClick={() => run(() => acceptInterAgencySettlement(item.id))}>{t('accept')}</button>
            : '',
        ])}
      />
    </div>
  );
}
