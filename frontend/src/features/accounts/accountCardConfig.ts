import { Account, Dashboard, ScreenId, UserRole } from '../../types';

export type AccountActionSlot = 'hidden' | 'versement' | 'transfer' | 'cash' | 'unpaid' | 'transactions' | 'refresh' | ScreenId;

export type AccountTextWidget = {
  id: string;
  visible: boolean;
  label: string;
  formula: string;
  position: number;
};

export type AccountButtonWidget = {
  id: string;
  visible: boolean;
  label: string;
  action: AccountActionSlot;
  position: number;
};

export type AccountContributorConfig = {
  id: string;
  name: string;
  percentage: string;
  visible: boolean;
};

export type AccountCardConfig = {
  texts: AccountTextWidget[];
  buttons: AccountButtonWidget[];
  popups: AccountsPopupConfig;
  visibility: AccountVisibilityConfig;
};

export type AccountCardConfigMap = Record<string, AccountCardConfig>;

export type AccountVisibilityConfig = {
  hiddenRoles: Array<Exclude<UserRole, 'Admin'>>;
  hiddenUserIds: string[];
};

export type AccountsScreenConfig = {
  buttons: AccountButtonWidget[];
  popups: AccountsPopupConfig;
};

export type MovementPopupConfig = {
  title: string;
  versementLabel: string;
  retraitLabel: string;
  defaultType: 'versement' | 'retrait';
  applyFixedType: boolean;
  fixedType: 'versement' | 'retrait';
  accountLabel: string;
  applyFixedAccount: boolean;
  fixedAccountId: string;
  amountLabel: string;
  applyFixedAmount: boolean;
  fixedAmount: string;
  descriptionLabel: string;
  showDescription: boolean;
  applyFixedDescription: boolean;
  fixedDescription: string;
  showContributors: boolean;
  contributorsLabel: string;
  contributors: AccountContributorConfig[];
  validateLabel: string;
  cancelLabel: string;
};

export type TransferPopupConfig = {
  title: string;
  fromLabel: string;
  applyFixedFromAccount: boolean;
  fixedFromAccountId: string;
  toLabel: string;
  applyFixedToAccount: boolean;
  fixedToAccountId: string;
  amountLabel: string;
  applyFixedAmount: boolean;
  fixedAmount: string;
  descriptionLabel: string;
  showDescription: boolean;
  applyFixedDescription: boolean;
  fixedDescription: string;
  validateLabel: string;
  cancelLabel: string;
};

export type AccountsPopupConfig = {
  movement: MovementPopupConfig;
  transfer: TransferPopupConfig;
};

export const defaultAccountsPopupConfig: AccountsPopupConfig = {
  movement: {
    title: 'إيداع / سحب',
    versementLabel: 'إيداع',
    retraitLabel: 'سحب',
    defaultType: 'versement',
    applyFixedType: false,
    fixedType: 'versement',
    accountLabel: 'الحساب',
    applyFixedAccount: false,
    fixedAccountId: '',
    amountLabel: 'المبلغ',
    applyFixedAmount: false,
    fixedAmount: '',
    descriptionLabel: 'ملاحظة',
    showDescription: true,
    applyFixedDescription: false,
    fixedDescription: '',
    showContributors: false,
    contributorsLabel: 'المساهم',
    contributors: [],
    validateLabel: 'تأكيد',
    cancelLabel: 'إلغاء',
  },
  transfer: {
    title: 'تحويل بين الحسابات',
    fromLabel: 'من حساب',
    applyFixedFromAccount: false,
    fixedFromAccountId: '',
    toLabel: 'إلى حساب',
    applyFixedToAccount: false,
    fixedToAccountId: '',
    amountLabel: 'المبلغ',
    applyFixedAmount: false,
    fixedAmount: '',
    descriptionLabel: 'ملاحظة',
    showDescription: true,
    applyFixedDescription: false,
    fixedDescription: '',
    validateLabel: 'تأكيد',
    cancelLabel: 'إلغاء',
  },
};

export const fixedCompteButtons: AccountButtonWidget[] = [
  { id: 'fixed-versement', visible: true, label: 'إيداع / سحب', action: 'versement', position: 1 },
  { id: 'fixed-transfer', visible: true, label: 'تحويل', action: 'transfer', position: 2 },
];

export const defaultAccountCardConfig: AccountCardConfig = {
  texts: [
    { id: 'text-1', visible: true, label: 'Balance', formula: '{Caisse calculée - Caisse réel - Non payé}', position: 1 },
    { id: 'text-2', visible: true, label: 'Maj', formula: '{Date maj}', position: 2 },
  ],
  buttons: fixedCompteButtons,
  popups: defaultAccountsPopupConfig,
  visibility: {
    hiddenRoles: [],
    hiddenUserIds: [],
  },
};

export const defaultAccountsScreenConfig: AccountsScreenConfig = {
  buttons: fixedCompteButtons,
  popups: defaultAccountsPopupConfig,
};

export const actionSlotOptions: Array<{ value: AccountActionSlot; label: string }> = [
  { value: 'hidden', label: 'Masquer' },
  { value: 'versement', label: 'Versement' },
  { value: 'transfer', label: 'Transfert' },
  { value: 'cash', label: 'Aller caisse' },
  { value: 'unpaid', label: 'Non paye' },
  { value: 'transactions', label: 'Transactions' },
  { value: 'refresh', label: 'Actualiser' },
  { value: 'accounts', label: 'Aller Soldes' },
  { value: 'account-settings', label: 'Aller Reglage soldes' },
  { value: 'services', label: 'Aller Services' },
  { value: 'reports', label: 'Aller Rapports' },
  { value: 'register', label: 'Aller Registre' },
  { value: 'users', label: 'Aller Utilisateurs' },
  { value: 'home', label: 'Aller Dashboard' },
];

export const formulaMetrics = [
  'Solde compte',
  'Ancien solde',
  'Last change date',
  'Date maj',
  'Caisse calculée',
  'Caisse reel',
  'Caisse réel',
  'Non payé',
  'Credit',
  'Crédit',
  'Debit',
  'Débit',
  'Total ventes',
  'Total achats',
  'Frais',
  'Versements jour',
  'Retraits jour',
];

export const actionTargets: Partial<Record<AccountActionSlot, ScreenId>> = {
  cash: 'cash',
  transactions: 'transactions',
  home: 'home',
  accounts: 'accounts',
  'account-settings': 'account-settings',
  services: 'services',
  reports: 'reports',
  register: 'register',
  users: 'users',
  profile: 'profile',
  'account-workflows': 'account-workflows',
  'transaction-workflows': 'transaction-workflows',
};

export function availableFormulaMetrics(accounts: Account[]) {
  return [...formulaMetrics, ...accounts.map((account) => account.name)];
}

const storageKey = 'rdet_account_card_configs';
const screenStorageKey = 'rdet_accounts_screen_config';

function uid(prefix: string) {
  return `${prefix}-${Math.random().toString(36).slice(2, 9)}`;
}

export function newTextWidget(position: number): AccountTextWidget {
  return { id: uid('text'), visible: true, label: `Text${position}`, formula: '{Solde compte}', position };
}

export function newButtonWidget(position: number): AccountButtonWidget {
  return { id: uid('button'), visible: true, label: `Button${position}`, action: 'versement', position };
}

export function newContributor(position: number): AccountContributorConfig {
  return { id: uid('contributor'), name: `Contributeur ${position}`, percentage: '', visible: true };
}

export function normalizeAccountsScreenConfig(config: Partial<AccountsScreenConfig> | null | undefined): AccountsScreenConfig {
  return {
    buttons: config?.buttons?.length ? config.buttons : defaultAccountsScreenConfig.buttons,
    popups: {
      movement: { ...defaultAccountsScreenConfig.popups.movement, ...(config?.popups?.movement ?? {}) },
      transfer: { ...defaultAccountsScreenConfig.popups.transfer, ...(config?.popups?.transfer ?? {}) },
    },
  };
}

export function loadAccountsScreenConfig(): AccountsScreenConfig {
  const raw = localStorage.getItem(screenStorageKey);
  if (!raw) return defaultAccountsScreenConfig;
  try {
    return normalizeAccountsScreenConfig(JSON.parse(raw));
  } catch {
    return defaultAccountsScreenConfig;
  }
}

export function saveAccountsScreenConfig(config: AccountsScreenConfig) {
  localStorage.setItem(screenStorageKey, JSON.stringify(normalizeAccountsScreenConfig(config)));
}

export function resetAccountsScreenConfig() {
  localStorage.removeItem(screenStorageKey);
}

export function normalizeAccountCardConfig(config: Partial<AccountCardConfig> | null | undefined): AccountCardConfig {
  const legacy = config as Partial<AccountCardConfig> & {
    leftInfo?: string;
    rightInfo?: string;
    leftAction?: AccountActionSlot;
    rightAction?: AccountActionSlot;
    staticNote?: string;
  } | null | undefined;

  if (legacy && ('leftInfo' in legacy || 'rightInfo' in legacy || 'leftAction' in legacy || 'rightAction' in legacy)) {
    return {
      texts: [
        { id: 'text-1', visible: true, label: 'Info 1', formula: legacy.staticNote || '{Solde compte}', position: 1 },
        { id: 'text-2', visible: true, label: 'Info 2', formula: '{Date maj}', position: 2 },
      ],
      buttons: loadAccountsScreenConfig().buttons,
      popups: loadAccountsScreenConfig().popups,
      visibility: defaultAccountCardConfig.visibility,
    };
  }

  const fallbackScreen = loadAccountsScreenConfig();
  return {
    texts: config?.texts?.length ? config.texts : defaultAccountCardConfig.texts,
    buttons: config?.buttons?.length ? config.buttons : fallbackScreen.buttons,
    popups: {
      movement: { ...defaultAccountsPopupConfig.movement, ...(config?.popups?.movement ?? fallbackScreen.popups.movement) },
      transfer: { ...defaultAccountsPopupConfig.transfer, ...(config?.popups?.transfer ?? fallbackScreen.popups.transfer) },
    },
    visibility: {
      hiddenRoles: Array.isArray(config?.visibility?.hiddenRoles)
        ? config.visibility.hiddenRoles.filter((role): role is Exclude<UserRole, 'Admin'> => role === 'Chef' || role === 'User')
        : [],
      hiddenUserIds: Array.isArray(config?.visibility?.hiddenUserIds) ? config.visibility.hiddenUserIds.map(String) : [],
    },
  };
}

export function loadAccountCardConfigs(): AccountCardConfigMap {
  const raw = localStorage.getItem(storageKey) ?? localStorage.getItem('rdet_account_card_config');
  if (!raw) return {};
  try {
    const parsed = JSON.parse(raw);
    if ('texts' in parsed || 'leftInfo' in parsed) return { default: normalizeAccountCardConfig(parsed) };
    return Object.fromEntries(Object.entries(parsed).map(([accountId, config]) => [accountId, normalizeAccountCardConfig(config as Partial<AccountCardConfig>)]));
  } catch {
    return {};
  }
}

export function saveAccountCardConfigs(configs: AccountCardConfigMap) {
  localStorage.setItem(storageKey, JSON.stringify(configs));
}

export function getAccountCardConfig(configs: AccountCardConfigMap, accountId: number): AccountCardConfig {
  return normalizeAccountCardConfig(configs[String(accountId)] ?? configs.default);
}

export function saveAccountCardConfig(accountId: number, config: AccountCardConfig) {
  const configs = loadAccountCardConfigs();
  configs[String(accountId)] = normalizeAccountCardConfig(config);
  saveAccountCardConfigs(configs);
}

export function resetAccountCardConfig(accountId: number) {
  const configs = loadAccountCardConfigs();
  delete configs[String(accountId)];
  saveAccountCardConfigs(configs);
}

function normalizeName(value: string) {
  return value.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase().replace(/\s+/g, ' ').trim();
}

function metricContext(account: Account, dashboard: Dashboard | null): Record<string, number | string> {
  const context: Record<string, number | string> = {
    'solde compte': Number(account.balance || 0),
    'ancien solde': Number(account.previous_balance ?? account.balance ?? 0),
    'last change date': new Date(account.updated_at).toLocaleDateString(),
    'caisse calculee': Number(dashboard?.total_balance || 0),
    'caisse reel': Number(dashboard?.cash_real || 0),
    'non paye': Number(dashboard?.unpaid_total || 0),
    credit: Number(dashboard?.credit || dashboard?.service_in || 0),
    debit: Number(dashboard?.debit || dashboard?.service_out || 0),
    'total ventes': Number(dashboard?.total_sales || 0),
    'total achats': Number(dashboard?.total_purchases || 0),
    frais: Number(dashboard?.fees || 0),
    'versements jour': Number(dashboard?.service_in || 0),
    'retraits jour': Number(dashboard?.service_out || 0),
    'date maj': new Date(account.updated_at).toLocaleDateString(),
  };
  dashboard?.accounts.forEach((item) => {
    context[normalizeName(item.name)] = Number(item.balance || 0);
  });
  return context;
}

export function renderTextWidget(widget: AccountTextWidget, account: Account, dashboard: Dashboard | null) {
  const result = evaluateTemplate(widget.formula, account, dashboard);
  return widget.label ? `${widget.label} : ${result}` : result;
}

export function evaluateTemplate(template: string, account: Account, dashboard: Dashboard | null) {
  return template.replace(/\{([^}]+)\}/g, (_, expression: string) => evaluateExpression(expression, account, dashboard));
}

function evaluateExpression(expression: string, account: Account, dashboard: Dashboard | null) {
  const context = metricContext(account, dashboard);
  const normalizedExpression = normalizeName(expression);
  const directValue = context[normalizedExpression];
  if (typeof directValue === 'string') return directValue;
  if (typeof directValue === 'number') return formatFormulaValue(directValue);

  let arithmetic = normalizedExpression;
  Object.entries(context)
    .filter(([, value]) => typeof value === 'number')
    .sort(([left], [right]) => right.length - left.length)
    .forEach(([name, value]) => {
      arithmetic = arithmetic.replace(new RegExp(`\\b${escapeRegExp(name)}\\b`, 'g'), String(value));
    });

  if (!/^[\d+\-*/().\s]+$/.test(arithmetic)) return '#FORMULA';

  try {
    const value = Function(`"use strict"; return (${arithmetic});`)();
    return Number.isFinite(value) ? formatFormulaValue(value) : '#FORMULA';
  } catch {
    return '#FORMULA';
  }
}

function escapeRegExp(value: string) {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

function formatFormulaValue(value: number) {
  return value.toLocaleString('fr-MA', { maximumFractionDigits: 2 });
}
