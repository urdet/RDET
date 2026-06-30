import { useEffect, useMemo, useState } from 'react';
import { Check, Link2, Send, X } from 'lucide-react';
import {
  acceptAgencyLink,
  acceptAgencyTransferRule,
  cancelInterAgencyTransfer,
  createAgencyLink,
  createAgencyTransferRule,
  listAgencyAccounts,
  listAgencyLinks,
  listAgencyTransferRules,
  listInterAgencyTransfers,
  rejectAgencyLink,
  rejectAgencyTransferRule,
} from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { DataTable } from '../../shared/ui/DataTable';
import { Panel } from '../../shared/ui/Panel';
import { Account, Agency, AgencyLink, AgencyTransferRule, CurrentUser, InterAgencyTransfer } from '../../types';
import { money } from '../../utils/format';

type Props = {
  accounts: Account[];
  agencies: Agency[];
  currentUser: CurrentUser | null;
};

export function InterAgencyTransfersPage({ accounts, agencies, currentUser }: Props) {
  const [links, setLinks] = useState<AgencyLink[]>([]);
  const [rules, setRules] = useState<AgencyTransferRule[]>([]);
  const [transfers, setTransfers] = useState<InterAgencyTransfer[]>([]);
  const [remoteAccounts, setRemoteAccounts] = useState<Account[]>([]);
  const [targetAgencyId, setTargetAgencyId] = useState('');
  const [linkId, setLinkId] = useState('');
  const [sourceAccountId, setSourceAccountId] = useState('');
  const [destinationAccountId, setDestinationAccountId] = useState('');
  const [ruleName, setRuleName] = useState('');
  const [ruleDescription, setRuleDescription] = useState('');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  const activeAgencyId = currentUser?.company_id ?? null;
  const activeLinks = links.filter((link) => link.status === 'active');
  const selectedLink = activeLinks.find((link) => String(link.id) === linkId) ?? activeLinks[0] ?? null;
  const remoteAgencyId = selectedLink && activeAgencyId
    ? (selectedLink.agency_a_id === activeAgencyId ? selectedLink.agency_b_id : selectedLink.agency_a_id)
    : null;
  const localAccounts = accounts.filter((account) => activeAgencyId && account.company_ids.includes(activeAgencyId));
  const otherAgencies = agencies.filter((agency) => agency.id !== activeAgencyId);
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
    if (link.status !== 'pending') return link.status;
    return linkTargetAgencyId(link) === activeAgencyId ? 'Pending approval' : 'Pending acceptance';
  }

  async function refresh() {
    const [linkRows, ruleRows, transferRows] = await Promise.all([
      listAgencyLinks(),
      listAgencyTransferRules(),
      listInterAgencyTransfers(),
    ]);
    setLinks(linkRows);
    setRules(ruleRows);
    setTransfers(transferRows);
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
        setError(err instanceof Error ? err.message : 'Comptes destination introuvables.');
      });
  }, [remoteAgencyId]);

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
      setError('Choisir une agence.');
      return;
    }
    await run(() => createAgencyLink(Number(targetAgencyId)));
    setTargetAgencyId('');
  }

  async function submitRule() {
    if (!selectedLink || !activeAgencyId || !remoteAgencyId || !sourceAccountId || !destinationAccountId || !ruleName.trim()) {
      setError('Completer la regle.');
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

  return (
    <div className="inter-agency-page">
      <div className="inter-agency-grid">
        <Panel title="Agency Links" icon={Link2}>
          <div className="inline-form">
            <select value={targetAgencyId} onChange={(event) => setTargetAgencyId(event.target.value)}>
              <option value="">Agence</option>
              {otherAgencies.map((agency) => <option key={agency.id} value={agency.id}>{agency.name}</option>)}
            </select>
            <button disabled={saving} onClick={submitLink}>Request</button>
          </div>
          <div className="compact-list">
            {links.map((link) => (
              <div className="compact-row" key={link.id}>
                <span>{link.agency_a_name} / {link.agency_b_name}</span>
                <b>{linkStatusLabel(link)}</b>
                {link.status === 'pending' && linkTargetAgencyId(link) === activeAgencyId && (
                  <>
                    <CircleButton title="Accept" icon={Check} onClick={() => run(() => acceptAgencyLink(link.id))} />
                    <CircleButton title="Reject" icon={X} onClick={() => run(() => rejectAgencyLink(link.id))} />
                  </>
                )}
              </div>
            ))}
          </div>
        </Panel>

        <Panel title="Transfer Rules" icon={Send}>
          <div className="rule-form">
            <select value={linkId} onChange={(event) => setLinkId(event.target.value)}>
              <option value="">Lien</option>
              {linkChoices.map((choice) => <option key={choice.id} value={choice.id}>{choice.label}</option>)}
            </select>
            <select value={sourceAccountId} onChange={(event) => setSourceAccountId(event.target.value)}>
              <option value="">Compte source</option>
              {localAccounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
            </select>
            <select value={destinationAccountId} onChange={(event) => setDestinationAccountId(event.target.value)}>
              <option value="">Compte destination</option>
              {remoteAccounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
            </select>
            <input value={ruleName} onChange={(event) => setRuleName(event.target.value)} placeholder="Nom regle" />
            <input value={ruleDescription} onChange={(event) => setRuleDescription(event.target.value)} placeholder="Description" />
            <button disabled={saving} onClick={submitRule}>Create</button>
          </div>
          <div className="compact-list">
            {rules.map((rule) => (
              <div className="compact-row" key={rule.id}>
                <span>{rule.name} - {rule.source_account_name} {'->'} {rule.destination_agency_name}/{rule.destination_account_name}</span>
                <b>{rule.status}</b>
                {rule.status === 'pending' && rule.destination_agency_id === activeAgencyId && (
                  <>
                    <CircleButton title="Accept" icon={Check} onClick={() => run(() => acceptAgencyTransferRule(rule.id))} />
                    <CircleButton title="Reject" icon={X} onClick={() => run(() => rejectAgencyTransferRule(rule.id))} />
                  </>
                )}
              </div>
            ))}
          </div>
        </Panel>
      </div>

      {error && <div className="transfer-error">{error}</div>}

      <DataTable
        title="Inter-agency Transfers"
        headers={['Date', 'From', 'To', 'Amount', 'Status', 'Note', 'Action']}
        rows={transfers.map((item) => [
          new Date(item.created_at).toLocaleDateString(),
          `${item.source_agency_name ?? ''} / ${item.source_account_name ?? ''}`,
          `${item.destination_agency_name ?? ''} / ${item.destination_account_name ?? ''}`,
          money(item.amount),
          item.status,
          item.note ?? '',
          item.status === 'pending_receiver' && item.destination_agency_id === activeAgencyId
            ? <button className="table-icon-button danger" disabled={saving} onClick={() => run(() => cancelInterAgencyTransfer(item.id))}>Cancel</button>
            : '',
        ])}
      />
    </div>
  );
}
