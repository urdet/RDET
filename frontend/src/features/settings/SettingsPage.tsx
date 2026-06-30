import { Plus, Save, Settings2, Trash2 } from 'lucide-react';
import { useEffect, useState } from 'react';
import { getAppSettings, saveAppSettings } from '../../api';
import { screens } from '../../navigation';
import { emptySectionPermission, permissionActionLabels, permissionActions } from '../../permissions';
import { CircleButton } from '../../shared/ui/CircleButton';
import { Panel } from '../../shared/ui/Panel';
import { Account, AppSettings, ManualImportRule, PermissionAction, ScreenId, Service, UserRole } from '../../types';

const defaultSettings: AppSettings = {
  cashAccountId: '',
  unpaidAccountId: '',
  aiProvider: 'openai',
  importMode: 'ai',
  openaiApiKey: '',
  openaiModel: 'gpt-4.1-mini',
  geminiApiKey: '',
  geminiModel: 'gemini-2.5-flash',
  openaiImportPrompt: 'Map spreadsheet rows into service transactions. Detect IN/OUT from labels when possible. Use only positive transaction amounts. Ignore totals, balances, headings, and repeated footer rows.',
  manualImportRules: [],
  rolePermissions: {},
};

const permissionRoles: Array<Exclude<UserRole, 'Admin'>> = ['Chef', 'User'];
const permissionScreens = screens.filter((screen) => screen.id !== 'settings');

function permissionValue(settings: AppSettings, role: Exclude<UserRole, 'Admin'>, screen: ScreenId, action: PermissionAction) {
  return Boolean(settings.rolePermissions?.[role]?.[screen]?.[action]);
}

function nextPermission(settings: AppSettings, role: Exclude<UserRole, 'Admin'>, screen: ScreenId, action: PermissionAction, value: boolean): AppSettings {
  const currentRole = settings.rolePermissions?.[role] ?? {};
  const currentSection = { ...emptySectionPermission(), ...(currentRole[screen] ?? {}) };
  return {
    ...settings,
    rolePermissions: {
      ...(settings.rolePermissions ?? {}),
      [role]: {
        ...currentRole,
        [screen]: { ...currentSection, [action]: value },
      },
    },
  };
}

const matchTypeLabels: Array<{ value: ManualImportRule['matchType']; label: string }> = [
  { value: 'starts_with', label: 'Starts with' },
  { value: 'equals', label: 'Equals' },
  { value: 'contains', label: 'Contains' },
  { value: 'ends_with', label: 'Ends with' },
  { value: 'regex', label: 'Regex' },
];

function cleanManualRules(value: unknown): ManualImportRule[] {
  if (!Array.isArray(value)) return [];
  return value.map((rule, index) => {
    const item = rule as Partial<ManualImportRule>;
    return {
      id: String(item.id || `rule-${index}-${Date.now()}`),
      label: String(item.label ?? ''),
      enabled: item.enabled !== false,
      matchType: matchTypeLabels.some((option) => option.value === item.matchType) ? item.matchType as ManualImportRule['matchType'] : 'contains',
      pattern: String(item.pattern ?? ''),
      serviceId: String(item.serviceId ?? ''),
      direction: item.direction === 'IN' || item.direction === 'OUT' ? item.direction : '',
      caseSensitive: Boolean(item.caseSensitive),
    };
  });
}

export function SettingsPage({ accounts, services }: { accounts: Account[]; services: Service[] }) {
  const [settings, setSettings] = useState<AppSettings>(defaultSettings);
  const [saved, setSaved] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    getAppSettings<Partial<AppSettings>>()
      .then((value) => setSettings({
        ...defaultSettings,
        ...value,
        cashAccountId: String(value.cashAccountId ?? ''),
        unpaidAccountId: String(value.unpaidAccountId ?? ''),
        aiProvider: value.aiProvider === 'google_gemini' ? 'google_gemini' : 'openai',
        importMode: value.importMode === 'manual' ? 'manual' : 'ai',
        openaiApiKey: String(value.openaiApiKey ?? ''),
        openaiModel: String(value.openaiModel ?? defaultSettings.openaiModel),
        geminiApiKey: String(value.geminiApiKey ?? ''),
        geminiModel: String(value.geminiModel ?? defaultSettings.geminiModel),
        openaiImportPrompt: String(value.openaiImportPrompt ?? defaultSettings.openaiImportPrompt),
        manualImportRules: cleanManualRules(value.manualImportRules),
        rolePermissions: value.rolePermissions ?? {},
      }))
      .catch((err) => setError(err instanceof Error ? err.message : 'Chargement impossible.'));
  }, []);

  async function persist() {
    setError('');
    setSaved(false);
    try {
      const next = {
        ...settings,
        cashAccountId: String(settings.cashAccountId || ''),
        unpaidAccountId: String(settings.unpaidAccountId || ''),
        aiProvider: settings.aiProvider === 'google_gemini' ? 'google_gemini' : 'openai',
        importMode: settings.importMode === 'manual' ? 'manual' : 'ai',
        openaiApiKey: String(settings.openaiApiKey || ''),
        openaiModel: String(settings.openaiModel || defaultSettings.openaiModel),
        geminiApiKey: String(settings.geminiApiKey || ''),
        geminiModel: String(settings.geminiModel || defaultSettings.geminiModel),
        openaiImportPrompt: String(settings.openaiImportPrompt || ''),
        manualImportRules: cleanManualRules(settings.manualImportRules),
      };
      await saveAppSettings(next);
      setSettings(next);
      setSaved(true);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Sauvegarde impossible.');
    }
  }

  function addManualRule() {
    setSettings((current) => ({
      ...current,
      manualImportRules: [
        ...(current.manualImportRules ?? []),
        {
          id: `rule-${Date.now()}`,
          label: '',
          enabled: true,
          matchType: 'contains',
          pattern: '',
          serviceId: services[0]?.id ? String(services[0].id) : '',
          direction: '',
          caseSensitive: false,
        },
      ],
    }));
    setSaved(false);
  }

  function updateManualRule(ruleId: string, patch: Partial<ManualImportRule>) {
    setSettings((current) => ({
      ...current,
      manualImportRules: (current.manualImportRules ?? []).map((rule) => rule.id === ruleId ? { ...rule, ...patch } : rule),
    }));
    setSaved(false);
  }

  function deleteManualRule(ruleId: string) {
    setSettings((current) => ({
      ...current,
      manualImportRules: (current.manualImportRules ?? []).filter((rule) => rule.id !== ruleId),
    }));
    setSaved(false);
  }

  return (
    <Panel title="Settings" icon={Settings2}>
      <div className="settings-layout">
        <div className="settings-copy">
          <h2>Parametres generaux</h2>
          <p>Choisir le compte qui recoit le total calcule dans la section caisse.</p>
        </div>
        <div className="settings-grid">
          <label className="form-field settings-wide">
            Compte caisse
            <select value={settings.cashAccountId} onChange={(event) => { setSettings((current) => ({ ...current, cashAccountId: event.target.value })); setSaved(false); }}>
              <option value="">Aucun compte</option>
              {accounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
            </select>
          </label>
          <label className="form-field settings-wide">
            Compte non paye
            <select value={settings.unpaidAccountId} onChange={(event) => { setSettings((current) => ({ ...current, unpaidAccountId: event.target.value })); setSaved(false); }}>
              <option value="">Aucun compte</option>
              {accounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
            </select>
          </label>
        </div>
        <section className="config-section">
          <div className="config-section-header">
            <h3>Excel import</h3>
          </div>
          <div className="settings-grid">
            <div className="form-field settings-wide">
              Default import mode
              <div className="provider-toggle">
                <button type="button" className={(settings.importMode ?? 'ai') === 'ai' ? 'active' : ''} onClick={() => { setSettings((current) => ({ ...current, importMode: 'ai' })); setSaved(false); }}>AI scan</button>
                <button type="button" className={settings.importMode === 'manual' ? 'active' : ''} onClick={() => { setSettings((current) => ({ ...current, importMode: 'manual' })); setSaved(false); }}>Manual rules</button>
              </div>
            </div>
            <div className="form-field settings-wide">
              AI provider
              <div className="provider-toggle">
                <button type="button" className={(settings.aiProvider ?? 'openai') === 'openai' ? 'active' : ''} onClick={() => { setSettings((current) => ({ ...current, aiProvider: 'openai' })); setSaved(false); }}>OpenAI</button>
                <button type="button" className={settings.aiProvider === 'google_gemini' ? 'active' : ''} onClick={() => { setSettings((current) => ({ ...current, aiProvider: 'google_gemini' })); setSaved(false); }}>Google Gemini</button>
              </div>
            </div>
            {(settings.aiProvider ?? 'openai') === 'openai' ? (
              <>
                <label className="form-field">
                  OpenAI model
                  <input value={settings.openaiModel ?? ''} placeholder={defaultSettings.openaiModel} onChange={(event) => { setSettings((current) => ({ ...current, openaiModel: event.target.value })); setSaved(false); }} />
                </label>
                <label className="form-field settings-wide">
                  OpenAI API key
                  <input type="password" value={settings.openaiApiKey ?? ''} placeholder="Use .env key when empty" onChange={(event) => { setSettings((current) => ({ ...current, openaiApiKey: event.target.value })); setSaved(false); }} />
                </label>
              </>
            ) : (
              <>
                <label className="form-field">
                  Gemini model
                  <input value={settings.geminiModel ?? ''} placeholder={defaultSettings.geminiModel} onChange={(event) => { setSettings((current) => ({ ...current, geminiModel: event.target.value })); setSaved(false); }} />
                </label>
                <label className="form-field settings-wide">
                  Gemini API key
                  <input type="password" value={settings.geminiApiKey ?? ''} placeholder="Use .env key when empty" onChange={(event) => { setSettings((current) => ({ ...current, geminiApiKey: event.target.value })); setSaved(false); }} />
                </label>
              </>
            )}
          </div>
          <label className="form-field settings-wide">
            Prompt configuration
            <textarea
              className="settings-textarea"
              value={settings.openaiImportPrompt ?? ''}
              onChange={(event) => { setSettings((current) => ({ ...current, openaiImportPrompt: event.target.value })); setSaved(false); }}
            />
          </label>
          <div className="manual-import-config">
            <div className="config-section-header">
              <h3>Manual service detection</h3>
              <button type="button" className="settings-small-action" onClick={addManualRule}><Plus className="h-4 w-4" /> Add rule</button>
            </div>
            <div className="manual-import-rules">
              {(settings.manualImportRules ?? []).map((rule) => (
                <div className="manual-import-rule" key={rule.id}>
                  <label className="manual-rule-enabled" title="Enabled">
                    <input type="checkbox" checked={rule.enabled} onChange={(event) => updateManualRule(rule.id, { enabled: event.target.checked })} />
                  </label>
                  <label className="form-field">
                    Rule name
                    <input value={rule.label} placeholder="Example: Dmane cash withdrawals" onChange={(event) => updateManualRule(rule.id, { label: event.target.value })} />
                  </label>
                  <label className="form-field">
                    Match condition
                    <select value={rule.matchType} onChange={(event) => updateManualRule(rule.id, { matchType: event.target.value as ManualImportRule['matchType'] })}>
                      {matchTypeLabels.map((option) => <option key={option.value} value={option.value}>{option.label}</option>)}
                    </select>
                  </label>
                  <label className="form-field">
                    Description text
                    <input value={rule.pattern} placeholder="Text to find, for example: CASH OUT or ddd" onChange={(event) => updateManualRule(rule.id, { pattern: event.target.value })} />
                  </label>
                  <label className="form-field">
                    Detected service
                    <select value={rule.serviceId} onChange={(event) => updateManualRule(rule.id, { serviceId: event.target.value })}>
                      <option value="">Choose the service for matched rows</option>
                      {services.map((service) => <option key={service.id} value={service.id}>{service.name}</option>)}
                    </select>
                  </label>
                  <label className="form-field">
                    Transaction type
                    <select value={rule.direction ?? ''} onChange={(event) => updateManualRule(rule.id, { direction: event.target.value as ManualImportRule['direction'] })}>
                      <option value="">Auto from file or service</option>
                      <option value="IN">IN</option>
                      <option value="OUT">OUT</option>
                    </select>
                  </label>
                  <label className="manual-rule-case" title="Case sensitive: distinguish uppercase and lowercase text">
                    <input type="checkbox" checked={Boolean(rule.caseSensitive)} onChange={(event) => updateManualRule(rule.id, { caseSensitive: event.target.checked })} />
                    Aa
                  </label>
                  <button type="button" className="manual-rule-delete" title="Delete rule" onClick={() => deleteManualRule(rule.id)}><Trash2 className="h-4 w-4" /></button>
                </div>
              ))}
              {!(settings.manualImportRules ?? []).length && <div className="empty-service-state">No manual import rules yet.</div>}
            </div>
          </div>
        </section>
        <section className="config-section">
          <div className="config-section-header">
            <h3>Validities permissions</h3>
          </div>
          <div className="permission-matrix">
            {permissionRoles.map((role) => (
              <div className="permission-role-block" key={role}>
                <h4>{role}</h4>
                <div className="permission-grid">
                  <div className="permission-grid-head">Section</div>
                  {permissionActions.map((action) => <div className="permission-grid-head" key={action}>{permissionActionLabels[action]}</div>)}
                  {permissionScreens.map((screen) => (
                    <div className="permission-row" key={`${role}-${screen.id}`}>
                      <div className="permission-section">{screen.label}</div>
                      {permissionActions.map((action) => (
                        <label className="permission-check" key={action}>
                          <input
                            type="checkbox"
                            checked={permissionValue(settings, role, screen.id, action)}
                            onChange={(event) => { setSettings((current) => nextPermission(current, role, screen.id, action, event.target.checked)); setSaved(false); }}
                          />
                        </label>
                      ))}
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>
        </section>
        {error && <div className="transaction-feedback error">{error}</div>}
        <div className="settings-actions">
          {saved && <span className="settings-saved">Settings saved</span>}
          <CircleButton title="Save settings" icon={Save} onClick={persist} />
        </div>
      </div>
    </Panel>
  );
}
