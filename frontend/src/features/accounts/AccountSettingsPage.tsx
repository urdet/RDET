import { useEffect, useMemo, useState } from 'react';
import { Plus, RotateCcw, Save, Search, Settings2, Trash2 } from 'lucide-react';
import { getAccountsScreenSettings, listUsers, saveAccountsScreenSettings } from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { Panel } from '../../shared/ui/Panel';
import { Account, Dashboard, ManagedUser, UserRole } from '../../types';
import { money } from '../../utils/format';
import {
  AccountCardConfig,
  AccountTextWidget,
  actionSlotOptions,
  availableFormulaMetrics,
  defaultAccountCardConfig,
  getAccountCardConfig,
  getButtonPopupConfig,
  loadAccountCardConfigs,
  newButtonWidget,
  newTextWidget,
  renderTextWidget,
  resetAccountCardConfig,
  saveAccountCardConfig,
  saveAccountCardConfigs,
  AccountButtonWidget,
  AccountCardConfigMap,
  normalizeAccountCardConfig,
} from './accountCardConfig';
import { CompteBox } from './CompteBox';

type AccountSettingsPageProps = {
  accounts: Account[];
  dashboard: Dashboard | null;
};

const visibilityRoles: Array<Exclude<UserRole, 'Admin'>> = ['Chef', 'User'];

export function AccountSettingsPage({ accounts, dashboard }: AccountSettingsPageProps) {
  const [configs, setConfigs] = useState(() => loadAccountCardConfigs());
  const [users, setUsers] = useState<ManagedUser[]>([]);
  const [selectedAccountId, setSelectedAccountId] = useState(accounts[0]?.id ?? 0);
  const [query, setQuery] = useState('');
  const [saved, setSaved] = useState(false);
  const [screenSaved, setScreenSaved] = useState(false);
  const [selectedPopupButtonId, setSelectedPopupButtonId] = useState('');

  const selectedAccount = accounts.find((account) => account.id === selectedAccountId) ?? accounts[0];
  const config = selectedAccount ? getAccountCardConfig(configs, selectedAccount.id) : defaultAccountCardConfig;
  const screenConfig = { buttons: config.buttons, popups: config.popups };
  const selectedPopupButton = screenConfig.buttons.find((button) => button.id === selectedPopupButtonId) ?? screenConfig.buttons[0];
  const selectedButtonPopups = selectedPopupButton ? getButtonPopupConfig(selectedPopupButton, config.popups) : config.popups;

  const filteredAccounts = useMemo(() => {
    const normalized = query.trim().toLowerCase();
    if (!normalized) return accounts;
    return accounts.filter((account) => account.name.toLowerCase().includes(normalized) || String(account.legacy_id ?? account.id).includes(normalized));
  }, [accounts, query]);

  function update(nextConfig: AccountCardConfig) {
    if (!selectedAccount) return;
    setConfigs((prev) => {
      const nextConfigs = { ...prev, [selectedAccount.id]: nextConfig };
      saveAccountCardConfigs(nextConfigs);
      saveAccountsScreenSettings(nextConfigs)
        .then(() => setScreenSaved(true))
        .catch(() => setScreenSaved(false));
      return nextConfigs;
    });
    setSaved(false);
  }

  function updateText(id: string, patch: Partial<AccountTextWidget>) {
    update({ ...config, texts: config.texts.map((item) => item.id === id ? { ...item, ...patch } : item) });
  }

  function updateButton(id: string, patch: Partial<AccountButtonWidget>) {
    update({ ...config, buttons: config.buttons.map((item) => item.id === id ? { ...item, ...patch } : item) });
  }

  function updateScreenConfig(updater: (prev: typeof screenConfig) => typeof screenConfig) {
    const next = updater(screenConfig);
    update({ ...config, buttons: next.buttons, popups: next.popups });
    setScreenSaved(false);
  }

  function updateMovementPopup(patch: Partial<typeof screenConfig.popups.movement>) {
    if (!selectedPopupButton) return;
    updateButton(selectedPopupButton.id, { popupConfig: { ...selectedButtonPopups, movement: { ...selectedButtonPopups.movement, ...patch } } });
  }

  function updateTransferPopup(patch: Partial<typeof screenConfig.popups.transfer>) {
    if (!selectedPopupButton) return;
    updateButton(selectedPopupButton.id, { popupConfig: { ...selectedButtonPopups, transfer: { ...selectedButtonPopups.transfer, ...patch } } });
  }

  function toggleHiddenRole(role: Exclude<UserRole, 'Admin'>, hidden: boolean) {
    const hiddenRoles = new Set(config.visibility.hiddenRoles);
    if (hidden) hiddenRoles.add(role);
    else hiddenRoles.delete(role);
    update({ ...config, visibility: { ...config.visibility, hiddenRoles: Array.from(hiddenRoles) } });
  }

  function toggleHiddenUser(userId: number, hidden: boolean) {
    const hiddenUserIds = new Set(config.visibility.hiddenUserIds);
    if (hidden) hiddenUserIds.add(String(userId));
    else hiddenUserIds.delete(String(userId));
    update({ ...config, visibility: { ...config.visibility, hiddenUserIds: Array.from(hiddenUserIds) } });
  }

  async function persist() {
    if (!selectedAccount) return;
    saveAccountCardConfig(selectedAccount.id, config);
    await saveAccountsScreenSettings({ ...configs, [selectedAccount.id]: config });
    setSaved(true);
  }

  function reset() {
    if (!selectedAccount) return;
    resetAccountCardConfig(selectedAccount.id);
    setConfigs(loadAccountCardConfigs());
    setSaved(true);
  }

  async function persistScreenConfig() {
    if (!selectedAccount) return;
    saveAccountCardConfig(selectedAccount.id, { ...config, buttons: screenConfig.buttons, popups: screenConfig.popups });
    await saveAccountsScreenSettings({ ...configs, [selectedAccount.id]: { ...config, buttons: screenConfig.buttons, popups: screenConfig.popups } });
    setScreenSaved(true);
  }

  function resetScreenConfig() {
    update({ ...config, buttons: defaultAccountCardConfig.buttons, popups: defaultAccountCardConfig.popups });
    setScreenSaved(true);
  }

  useEffect(() => {
    listUsers().then(setUsers).catch(() => setUsers([]));
    getAccountsScreenSettings<AccountCardConfigMap>()
      .then((serverConfigs) => {
        if (serverConfigs && Object.keys(serverConfigs).length) {
          const legacy = serverConfigs as unknown as { buttons?: unknown; popups?: unknown };
          const normalized = legacy.buttons || legacy.popups
            ? Object.fromEntries(accounts.map((account) => [String(account.id), normalizeAccountCardConfig({ ...getAccountCardConfig(configs, account.id), buttons: legacy.buttons as AccountButtonWidget[] | undefined, popups: legacy.popups as AccountCardConfig['popups'] | undefined })]))
            : Object.fromEntries(Object.entries(serverConfigs).map(([accountId, accountConfig]) => [accountId, normalizeAccountCardConfig(accountConfig)]));
          setConfigs(normalized);
          saveAccountCardConfigs(normalized);
          if (legacy.buttons || legacy.popups) saveAccountsScreenSettings(normalized).catch(() => undefined);
        } else {
          saveAccountsScreenSettings(configs).catch(() => undefined);
        }
      })
      .catch(() => undefined);
  }, []);

  if (!selectedAccount) {
    return <Panel title="Configuration des cartes comptes" icon={Settings2}>Aucun compte disponible.</Panel>;
  }

  const previewTexts = config.texts.filter((item) => item.visible).sort((a, b) => a.position - b.position).map((item) => renderTextWidget(item, selectedAccount, dashboard));

  return (
    <Panel title="Configuration dynamique des comptes" icon={Settings2}>
      <div className="account-settings-layout">
        <aside className="account-settings-list">
          <label className="account-search compact">
            <Search className="h-4 w-4" />
            <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Compte..." />
          </label>
          <div className="account-settings-items">
            {filteredAccounts.map((account) => (
              <button
                key={account.id}
                className={`account-settings-item ${account.id === selectedAccount.id ? 'active' : ''}`}
                onClick={() => {
                  setSelectedAccountId(account.id);
                  setSelectedPopupButtonId('');
                  setSaved(false);
                }}
              >
                <span>{account.name}</span>
                <small>{money(account.balance)}</small>
              </button>
            ))}
          </div>
        </aside>

        <div className="settings-layout">
          <div className="settings-copy">
            <h2>{selectedAccount.name}</h2>
            <p>Use variables between braces, for example {'{Ancien solde}'} or {'{Bank}'}. Each account name is available as a variable automatically.</p>
          </div>

          <div className="settings-preview">
            <CompteBox account={selectedAccount} texts={previewTexts} buttons={screenConfig.buttons} popups={screenConfig.popups} />
          </div>

          <section className="config-section">
            <div className="config-section-header">
              <h3>Account visibility</h3>
            </div>
            <div className="formula-help">Admin always sees every account. Selected roles or users will not see this account, its balance, or its history.</div>
            <div className="visibility-settings">
              <div className="visibility-box">
                <strong>Hide from roles</strong>
                <div className="visibility-options">
                  {visibilityRoles.map((role) => (
                    <label className="toggle-line" key={role}>
                      <input type="checkbox" checked={config.visibility.hiddenRoles.includes(role)} onChange={(event) => toggleHiddenRole(role, event.target.checked)} />
                      <span>{role}</span>
                    </label>
                  ))}
                </div>
              </div>
              <div className="visibility-box">
                <strong>Hide from users</strong>
                <div className="visibility-user-list">
                  {users.filter((item) => item.role !== 'Admin').map((item) => (
                    <label className="visibility-user-row" key={item.id}>
                      <input type="checkbox" checked={config.visibility.hiddenUserIds.includes(String(item.id))} onChange={(event) => toggleHiddenUser(item.id, event.target.checked)} />
                      <span>{item.first_name} {item.last_name}</span>
                      <small>{item.username} - {item.role}</small>
                    </label>
                  ))}
                  {!users.filter((item) => item.role !== 'Admin').length && <div className="empty-service-state">No non-admin users found.</div>}
                </div>
              </div>
            </div>
          </section>

          <section className="config-section">
            <div className="config-section-header">
              <h3>Text fields</h3>
              <CircleButton title="Add text" icon={Plus} onClick={() => update({ ...config, texts: [...config.texts, newTextWidget(config.texts.length + 1)] })} />
            </div>
            <div className="formula-help">Variables: {availableFormulaMetrics(accounts).join(', ')}</div>
            <div className="config-list">
              {config.texts.sort((a, b) => a.position - b.position).map((text) => (
                <div className="config-row" key={text.id}>
                  <label className="toggle-line">
                    <input type="checkbox" checked={text.visible} onChange={(event) => updateText(text.id, { visible: event.target.checked })} />
                    <span>Show</span>
                  </label>
                  <label className="form-field">
                    Label
                    <input value={text.label} onChange={(event) => updateText(text.id, { label: event.target.value })} />
                  </label>
                  <label className="form-field config-formula">
                    Formula / text
                    <input value={text.formula} onChange={(event) => updateText(text.id, { formula: event.target.value })} />
                  </label>
                  <label className="form-field config-position">
                    Pos
                    <input value={text.position} inputMode="numeric" onChange={(event) => updateText(text.id, { position: Number(event.target.value || 0) })} />
                  </label>
                  <button className="grid-delete" title="Delete" onClick={() => update({ ...config, texts: config.texts.filter((item) => item.id !== text.id) })}>
                    <Trash2 className="h-4 w-4" />
                  </button>
                </div>
              ))}
            </div>
          </section>

          <section className="config-section">
            <div className="config-section-header">
              <h3>Popup buttons</h3>
              <CircleButton title="Add button" icon={Plus} onClick={() => {
                updateScreenConfig((prev) => ({ ...prev, buttons: [...prev.buttons, newButtonWidget(prev.buttons.length + 1)] }));
              }} />
            </div>
            <div className="formula-help">These buttons and actions are saved only for the selected compte.</div>
            <div className="config-list">
              {[...screenConfig.buttons].sort((a, b) => a.position - b.position).map((button) => (
                <div className={`config-row button-config-row ${selectedPopupButton?.id === button.id ? 'active' : ''}`} key={button.id}>
                  <label className="toggle-line">
                    <input type="checkbox" checked={button.visible} onChange={(event) => updateButton(button.id, { visible: event.target.checked })} />
                    <span>Show</span>
                  </label>
                  <label className="form-field">
                    Label
                    <input value={button.label} onChange={(event) => updateButton(button.id, { label: event.target.value })} />
                  </label>
                  <label className="form-field">
                    Popup / action
                    <select value={button.action} onChange={(event) => { updateButton(button.id, { action: event.target.value as AccountButtonWidget['action'] }); setSelectedPopupButtonId(button.id); }}>
                      {actionSlotOptions.map((option) => <option key={option.value} value={option.value}>{option.label}</option>)}
                    </select>
                  </label>
                  <label className="form-field config-position">
                    Pos
                    <input value={button.position} inputMode="numeric" onChange={(event) => updateButton(button.id, { position: Number(event.target.value || 0) })} />
                  </label>
                  {(button.action === 'versement' || button.action === 'transfer') && (
                    <button type="button" className="button-settings-trigger" onClick={() => setSelectedPopupButtonId(button.id)}>
                      Configure
                    </button>
                  )}
                  <button className="grid-delete" title="Delete" onClick={() => {
                    updateScreenConfig((prev) => ({ ...prev, buttons: prev.buttons.filter((item) => item.id !== button.id) }));
                  }}>
                    <Trash2 className="h-4 w-4" />
                  </button>
                </div>
              ))}
            </div>
            <div className="settings-actions">
              {screenSaved && <span className="settings-saved">Popup config saved</span>}
              <CircleButton title="Reset popups" icon={RotateCcw} onClick={resetScreenConfig} />
              <CircleButton title="Save popups" icon={Save} onClick={persistScreenConfig} />
            </div>
          </section>

          {selectedPopupButton && selectedPopupButton.action === 'versement' && (
          <section className="config-section">
            <div className="config-section-header">
              <h3>{selectedPopupButton.label} — Versement / Retrait popup</h3>
            </div>
            <div className="settings-grid">
              <label className="form-field">
                Popup title
                <input value={selectedButtonPopups.movement.title} onChange={(event) => updateMovementPopup({ title: event.target.value })} />
              </label>
              <label className="form-field">
                Default operation
                <select value={selectedButtonPopups.movement.defaultType} onChange={(event) => updateMovementPopup({ defaultType: event.target.value as 'versement' | 'retrait' })}>
                  <option value="versement">Versement</option>
                  <option value="retrait">Retrait</option>
                </select>
              </label>
              <label className="toggle-line settings-wide">
                <input type="checkbox" checked={selectedButtonPopups.movement.applyFixedType} onChange={(event) => updateMovementPopup({ applyFixedType: event.target.checked })} />
                <span>Apply fixed operation type</span>
              </label>
              <label className="form-field">
                Fixed operation type
                <select value={selectedButtonPopups.movement.fixedType} onChange={(event) => updateMovementPopup({ fixedType: event.target.value as 'versement' | 'retrait' })}>
                  <option value="versement">Versement</option>
                  <option value="retrait">Retrait</option>
                </select>
              </label>
              <label className="form-field">
                Versement toggle label
                <input value={selectedButtonPopups.movement.versementLabel} onChange={(event) => updateMovementPopup({ versementLabel: event.target.value })} />
              </label>
              <label className="form-field">
                Retrait toggle label
                <input value={selectedButtonPopups.movement.retraitLabel} onChange={(event) => updateMovementPopup({ retraitLabel: event.target.value })} />
              </label>
              <label className="form-field">
                Account label
                <input value={selectedButtonPopups.movement.accountLabel} onChange={(event) => updateMovementPopup({ accountLabel: event.target.value })} />
              </label>
              <label className="toggle-line settings-wide">
                <input type="checkbox" checked={selectedButtonPopups.movement.applyFixedAccount} onChange={(event) => updateMovementPopup({ applyFixedAccount: event.target.checked })} />
                <span>Apply fixed compte</span>
              </label>
              <label className="form-field">
                Fixed compte
                <select value={selectedButtonPopups.movement.fixedAccountId} onChange={(event) => updateMovementPopup({ fixedAccountId: event.target.value })}>
                  <option value="">Selectionner</option>
                  {accounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
                </select>
              </label>
              <label className="form-field">
                Amount label
                <input value={selectedButtonPopups.movement.amountLabel} onChange={(event) => updateMovementPopup({ amountLabel: event.target.value })} />
              </label>
              <label className="toggle-line settings-wide">
                <input type="checkbox" checked={selectedButtonPopups.movement.applyFixedAmount} onChange={(event) => updateMovementPopup({ applyFixedAmount: event.target.checked })} />
                <span>Apply fixed amount</span>
              </label>
              <label className="form-field">
                Fixed amount
                <input value={selectedButtonPopups.movement.fixedAmount} inputMode="decimal" onChange={(event) => updateMovementPopup({ fixedAmount: event.target.value })} />
              </label>
              <label className="form-field">
                Validate label
                <input value={selectedButtonPopups.movement.validateLabel} onChange={(event) => updateMovementPopup({ validateLabel: event.target.value })} />
              </label>
              <label className="form-field">
                Cancel label
                <input value={selectedButtonPopups.movement.cancelLabel} onChange={(event) => updateMovementPopup({ cancelLabel: event.target.value })} />
              </label>
              <label className="form-field settings-wide">
                Description label
                <input value={selectedButtonPopups.movement.descriptionLabel} onChange={(event) => updateMovementPopup({ descriptionLabel: event.target.value })} />
              </label>
              <label className="toggle-line settings-wide">
                <input type="checkbox" checked={selectedButtonPopups.movement.showDescription} onChange={(event) => updateMovementPopup({ showDescription: event.target.checked })} />
                <span>Show description field</span>
              </label>
              <label className="toggle-line settings-wide">
                <input type="checkbox" checked={selectedButtonPopups.movement.applyFixedDescription} onChange={(event) => updateMovementPopup({ applyFixedDescription: event.target.checked })} />
                <span>Apply fixed description</span>
              </label>
              <label className="form-field settings-wide">
                Fixed description
                <input value={selectedButtonPopups.movement.fixedDescription} onChange={(event) => updateMovementPopup({ fixedDescription: event.target.value })} />
              </label>
              <label className="toggle-line settings-wide">
                <input type="checkbox" checked={selectedButtonPopups.movement.showContributors} onChange={(event) => updateMovementPopup({ showContributors: event.target.checked })} />
                <span>Show contributor name in versement/retrait</span>
              </label>
              <label className="form-field settings-wide">
                Contributor label
                <input value={selectedButtonPopups.movement.contributorsLabel} onChange={(event) => updateMovementPopup({ contributorsLabel: event.target.value })} />
              </label>
            </div>
          </section>
          )}

          {selectedPopupButton && selectedPopupButton.action === 'transfer' && (
          <section className="config-section">
            <div className="config-section-header">
              <h3>{selectedPopupButton.label} — Transfert popup</h3>
            </div>
            <div className="settings-grid">
              <label className="form-field">
                Popup title
                <input value={selectedButtonPopups.transfer.title} onChange={(event) => updateTransferPopup({ title: event.target.value })} />
              </label>
              <label className="form-field">
                From label
                <input value={selectedButtonPopups.transfer.fromLabel} onChange={(event) => updateTransferPopup({ fromLabel: event.target.value })} />
              </label>
              <label className="toggle-line settings-wide">
                <input type="checkbox" checked={selectedButtonPopups.transfer.applyFixedFromAccount} onChange={(event) => updateTransferPopup({ applyFixedFromAccount: event.target.checked })} />
                <span>Apply fixed from compte</span>
              </label>
              <label className="form-field">
                Fixed from compte
                <select value={selectedButtonPopups.transfer.fixedFromAccountId} onChange={(event) => updateTransferPopup({ fixedFromAccountId: event.target.value })}>
                  <option value="">Selectionner</option>
                  {accounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
                </select>
              </label>
              <label className="form-field">
                To label
                <input value={selectedButtonPopups.transfer.toLabel} onChange={(event) => updateTransferPopup({ toLabel: event.target.value })} />
              </label>
              <label className="toggle-line settings-wide">
                <input type="checkbox" checked={selectedButtonPopups.transfer.applyFixedToAccount} onChange={(event) => updateTransferPopup({ applyFixedToAccount: event.target.checked })} />
                <span>Apply fixed to compte</span>
              </label>
              <label className="form-field">
                Fixed to compte
                <select value={selectedButtonPopups.transfer.fixedToAccountId} onChange={(event) => updateTransferPopup({ fixedToAccountId: event.target.value })}>
                  <option value="">Selectionner</option>
                  {accounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
                </select>
              </label>
              <label className="form-field">
                Amount source
                <select value={selectedButtonPopups.transfer.amountMode} onChange={(event) => updateTransferPopup({ amountMode: event.target.value as 'manual' | 'completedTransfer' })}>
                  <option value="manual">Manual amount</option>
                  <option value="completedTransfer">Completed transfers list</option>
                </select>
              </label>
              {selectedButtonPopups.transfer.amountMode === 'completedTransfer' && (
                <label className="form-field">
                  Reference source (account C)
                  <select value={selectedButtonPopups.transfer.completedTransferFromAccountId} onChange={(event) => updateTransferPopup({ completedTransferFromAccountId: event.target.value })}>
                    <option value="">Selectionner</option>
                    {accounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
                  </select>
                </label>
              )}
              <label className="form-field">
                Amount label
                <input value={selectedButtonPopups.transfer.amountLabel} onChange={(event) => updateTransferPopup({ amountLabel: event.target.value })} />
              </label>
              {selectedButtonPopups.transfer.amountMode === 'manual' && (
                <>
                  <label className="toggle-line settings-wide">
                    <input type="checkbox" checked={selectedButtonPopups.transfer.applyFixedAmount} onChange={(event) => updateTransferPopup({ applyFixedAmount: event.target.checked })} />
                    <span>Apply fixed amount</span>
                  </label>
                  <label className="form-field">
                    Fixed amount
                    <input value={selectedButtonPopups.transfer.fixedAmount} inputMode="decimal" onChange={(event) => updateTransferPopup({ fixedAmount: event.target.value })} />
                  </label>
                </>
              )}
              <label className="form-field">
                Validate label
                <input value={selectedButtonPopups.transfer.validateLabel} onChange={(event) => updateTransferPopup({ validateLabel: event.target.value })} />
              </label>
              <label className="form-field">
                Cancel label
                <input value={selectedButtonPopups.transfer.cancelLabel} onChange={(event) => updateTransferPopup({ cancelLabel: event.target.value })} />
              </label>
              <label className="form-field settings-wide">
                Description label
                <input value={selectedButtonPopups.transfer.descriptionLabel} onChange={(event) => updateTransferPopup({ descriptionLabel: event.target.value })} />
              </label>
              <label className="toggle-line settings-wide">
                <input type="checkbox" checked={selectedButtonPopups.transfer.showDescription} onChange={(event) => updateTransferPopup({ showDescription: event.target.checked })} />
                <span>Show description field</span>
              </label>
              <label className="toggle-line settings-wide">
                <input type="checkbox" checked={selectedButtonPopups.transfer.applyFixedDescription} onChange={(event) => updateTransferPopup({ applyFixedDescription: event.target.checked })} />
                <span>Apply fixed description</span>
              </label>
              <label className="form-field settings-wide">
                Fixed description
                <input value={selectedButtonPopups.transfer.fixedDescription} onChange={(event) => updateTransferPopup({ fixedDescription: event.target.value })} />
              </label>
            </div>
          </section>
          )}

          <div className="settings-actions">
            {saved && <span className="settings-saved">Configuration du compte sauvegardee</span>}
            <CircleButton title="Reset compte" icon={RotateCcw} onClick={reset} />
            <CircleButton title="Save compte" icon={Save} onClick={persist} />
          </div>
        </div>
      </div>
    </Panel>
  );
}
