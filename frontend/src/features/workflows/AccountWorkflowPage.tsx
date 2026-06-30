import { ArrowDownToLine, ArrowUpFromLine, Link2, Plus, RefreshCw, Save, Trash2 } from 'lucide-react';
import { useEffect, useMemo, useState } from 'react';
import { getAccountActionSettings, saveAccountActionSettings } from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { Account, AccountActionEvent, AccountActionRule, AccountActionSettings } from '../../types';

const defaultSettings: AccountActionSettings = { rules: [] };

function uid() {
  return `rule-${Math.random().toString(36).slice(2, 9)}`;
}

function newRule(accounts: Account[]): AccountActionRule {
  return {
    id: uid(),
    name: 'Nouvelle action',
    enabled: true,
    accountId: accounts[0] ? String(accounts[0].id) : '',
    event: 'money_in',
    linkedAccountIds: accounts[1] ? [String(accounts[1].id)] : [],
    effect: 'add',
  };
}

function normalizeSettings(settings: Partial<AccountActionSettings> | null | undefined): AccountActionSettings {
  return {
    rules: Array.isArray(settings?.rules)
      ? settings.rules.map((rule) => ({
          id: rule.id || uid(),
          name: rule.name || 'Action compte',
          enabled: rule.enabled !== false,
          accountId: String(rule.accountId || ''),
          event: rule.event === 'money_out' ? 'money_out' : 'money_in',
          linkedAccountIds: Array.isArray(rule.linkedAccountIds) ? rule.linkedAccountIds.map(String) : [],
          effect: rule.effect === 'subtract' ? 'subtract' : 'add',
        }))
      : [],
  };
}

export function AccountWorkflowPage({ accounts }: { accounts: Account[] }) {
  const [settings, setSettings] = useState<AccountActionSettings>(defaultSettings);
  const [selectedId, setSelectedId] = useState('');
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const selectedRule = settings.rules.find((rule) => rule.id === selectedId) ?? settings.rules[0];

  const accountName = useMemo(() => new Map(accounts.map((account) => [String(account.id), account.name])), [accounts]);

  useEffect(() => {
    getAccountActionSettings<AccountActionSettings>()
      .then((serverSettings) => {
        const normalized = normalizeSettings(serverSettings);
        setSettings(normalized);
        setSelectedId(normalized.rules[0]?.id ?? '');
      })
      .catch((err) => setError(err instanceof Error ? err.message : 'Chargement impossible.'));
  }, []);

  function updateRule(ruleId: string, patch: Partial<AccountActionRule>) {
    setSettings((current) => ({
      rules: current.rules.map((rule) => rule.id === ruleId ? { ...rule, ...patch } : rule),
    }));
    setMessage('');
  }

  function addRule() {
    const rule = newRule(accounts);
    setSettings((current) => ({ rules: [...current.rules, rule] }));
    setSelectedId(rule.id);
    setMessage('');
  }

  function deleteRule(ruleId: string) {
    const rules = settings.rules.filter((rule) => rule.id !== ruleId);
    setSettings({ rules });
    setSelectedId(rules[0]?.id ?? '');
    setMessage('');
  }

  function toggleLinked(rule: AccountActionRule, accountId: string) {
    const linkedAccountIds = rule.linkedAccountIds.includes(accountId)
      ? rule.linkedAccountIds.filter((id) => id !== accountId)
      : [...rule.linkedAccountIds, accountId];
    updateRule(rule.id, { linkedAccountIds });
  }

  async function persist() {
    setSaving(true);
    setError('');
    setMessage('');
    try {
      const normalized = normalizeSettings(settings);
      await saveAccountActionSettings(normalized);
      setSettings(normalized);
      setMessage('Actions sauvegardees.');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Sauvegarde impossible.');
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="account-actions-page">
      <div className="workflow-page-header">
        <div>
          <div className="workflow-eyebrow"><Link2 className="h-4 w-4" /> Account actions</div>
          <h2>Actions des comptes</h2>
          <p>Configure les evenements executes sur chaque transaction. Quand l'argent entre ou sort d'un compte, la meme somme peut etre ajoutee ou retiree aux comptes lies.</p>
        </div>
        <div className="settings-actions">
          {message && <span className="settings-saved">{message}</span>}
          <CircleButton title="Nouvelle action" icon={Plus} onClick={addRule} />
          <CircleButton title={saving ? 'Saving' : 'Save'} icon={Save} onClick={persist} />
        </div>
      </div>

      {error && <div className="transaction-feedback error">{error}</div>}

      <div className="account-actions-layout">
        <aside className="account-actions-list">
          <div className="palette-title">Actions</div>
          {settings.rules.map((rule) => (
            <button key={rule.id} className={`account-action-item ${selectedRule?.id === rule.id ? 'active' : ''}`} onClick={() => setSelectedId(rule.id)}>
              <span>{rule.name}</span>
              <small>{accountName.get(rule.accountId) ?? 'Compte'} · {rule.event === 'money_in' ? 'Entree' : 'Sortie'}</small>
            </button>
          ))}
          {!settings.rules.length && <div className="empty-service-state">Aucune action configuree.</div>}
        </aside>

        <section className="account-action-editor">
          {selectedRule ? (
            <>
              <div className="account-action-editor-header">
                <div>
                  <h3>{selectedRule.name}</h3>
                  <p>{selectedRule.enabled ? 'Active sur les prochaines transactions.' : 'Desactivee.'}</p>
                </div>
                <CircleButton title="Supprimer" icon={Trash2} onClick={() => deleteRule(selectedRule.id)} />
              </div>

              <div className="settings-grid">
                <label className="form-field settings-wide">
                  Nom
                  <input value={selectedRule.name} onChange={(event) => updateRule(selectedRule.id, { name: event.target.value })} />
                </label>
                <label className="toggle-line account-action-toggle">
                  <input type="checkbox" checked={selectedRule.enabled} onChange={(event) => updateRule(selectedRule.id, { enabled: event.target.checked })} />
                  <span>Action active</span>
                </label>
                <label className="form-field">
                  Compte declencheur
                  <select value={selectedRule.accountId} onChange={(event) => updateRule(selectedRule.id, { accountId: event.target.value })}>
                    <option value="">Selectionner</option>
                    {accounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
                  </select>
                </label>
                <label className="form-field">
                  Evenement
                  <select value={selectedRule.event} onChange={(event) => updateRule(selectedRule.id, { event: event.target.value as AccountActionEvent })}>
                    <option value="money_in">Argent entre</option>
                    <option value="money_out">Argent sort</option>
                  </select>
                </label>
                <label className="form-field">
                  Effet sur comptes lies
                  <select value={selectedRule.effect} onChange={(event) => updateRule(selectedRule.id, { effect: event.target.value as AccountActionRule['effect'] })}>
                    <option value="add">Ajouter le montant</option>
                    <option value="subtract">Retirer le montant</option>
                  </select>
                </label>
              </div>

              <div className="account-action-linked">
                <div className="config-section-header">
                  <h3>Comptes lies</h3>
                  <span>{selectedRule.linkedAccountIds.length} selectionnes</span>
                </div>
                <div className="linked-account-grid">
                  {accounts.filter((account) => String(account.id) !== selectedRule.accountId).map((account) => (
                    <label key={account.id} className={`linked-account-tile ${selectedRule.linkedAccountIds.includes(String(account.id)) ? 'selected' : ''}`}>
                      <input type="checkbox" checked={selectedRule.linkedAccountIds.includes(String(account.id))} onChange={() => toggleLinked(selectedRule, String(account.id))} />
                      <span>{account.name}</span>
                    </label>
                  ))}
                </div>
              </div>
            </>
          ) : (
            <div className="empty-service-state">Cree une action pour commencer.</div>
          )}
        </section>

        <aside className="account-action-preview">
          <div className="palette-title">Execution</div>
          {selectedRule ? (
            <div className="action-preview-flow">
              <div className="action-preview-step">
                {selectedRule.event === 'money_in' ? <ArrowDownToLine className="h-4 w-4" /> : <ArrowUpFromLine className="h-4 w-4" />}
                <span>{selectedRule.event === 'money_in' ? 'Money comes' : 'Money goes'}</span>
              </div>
              <div className="action-preview-account">{accountName.get(selectedRule.accountId) ?? 'Compte declencheur'}</div>
              <div className="action-preview-step muted">
                <RefreshCw className="h-4 w-4" />
                <span>{selectedRule.effect === 'add' ? 'Add amount to' : 'Subtract amount from'}</span>
              </div>
              {selectedRule.linkedAccountIds.map((accountId) => (
                <div key={accountId} className="action-preview-account linked">{accountName.get(accountId) ?? accountId}</div>
              ))}
            </div>
          ) : (
            <div className="empty-service-state">Selectionne une action.</div>
          )}
        </aside>
      </div>
    </div>
  );
}
