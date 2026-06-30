import { ScreenId } from './types';

export type Language = 'en' | 'fr' | 'ar';
export type ThemeMode = 'light' | 'dark';

export const appName = 'AgencyOS';

export const languageNames: Record<Language, string> = {
  en: 'English',
  fr: 'Francais',
  ar: 'العربية',
};

export const shellText = {
  en: {
    workspace: 'Agency',
    search: 'Search',
    logout: 'Logout',
    refresh: 'Refresh',
    notifications: 'Notifications',
    more: 'More',
    allSpaces: 'All spaces',
    user: 'User',
    themeLight: 'Light theme',
    themeDark: 'Dark theme',
    language: 'Language',
  },
  fr: {
    workspace: 'Agence',
    search: 'Rechercher',
    logout: 'Deconnexion',
    refresh: 'Actualiser',
    notifications: 'Notifications',
    more: 'Plus',
    allSpaces: 'Tous les espaces',
    user: 'Utilisateur',
    themeLight: 'Theme clair',
    themeDark: 'Theme sombre',
    language: 'Langue',
  },
  ar: {
    workspace: 'الوكالة',
    search: 'بحث',
    logout: 'تسجيل الخروج',
    refresh: 'تحديث',
    notifications: 'الإشعارات',
    more: 'المزيد',
    allSpaces: 'كل المساحات',
    user: 'المستخدم',
    themeLight: 'الوضع الفاتح',
    themeDark: 'الوضع الداكن',
    language: 'اللغة',
  },
} satisfies Record<Language, Record<string, string>>;

export const screenLabels: Record<Language, Record<ScreenId, string>> = {
  en: {
    accounts: 'Balances',
    'account-settings': 'Balance settings',
    transactions: 'Transactions',
    'inter-agency-transfers': 'Inter-agency transfers',
    expenses: 'Expenses',
    reports: 'Reports',
    profile: 'Profile',
    settings: 'Settings',
    register: 'Register',
    home: 'Dashboard',
    services: 'Services',
    'account-workflows': 'Account actions',
    'transaction-workflows': 'Transaction workflows',
    cash: 'Cash',
    users: 'Users',
  },
  fr: {
    accounts: 'Soldes',
    'account-settings': 'Reglages soldes',
    transactions: 'Transactions',
    'inter-agency-transfers': 'Transferts inter-agences',
    expenses: 'Depenses',
    reports: 'Rapports',
    profile: 'Profil',
    settings: 'Parametres',
    register: 'Registre',
    home: 'Tableau de bord',
    services: 'Services',
    'account-workflows': 'Actions comptes',
    'transaction-workflows': 'Workflows transactions',
    cash: 'Caisse',
    users: 'Utilisateurs',
  },
  ar: {
    accounts: 'الأرصدة',
    'account-settings': 'إعدادات الأرصدة',
    transactions: 'المعاملات',
    'inter-agency-transfers': 'تحويلات بين الوكالات',
    expenses: 'المصاريف',
    reports: 'التقارير',
    profile: 'الملف الشخصي',
    settings: 'الإعدادات',
    register: 'السجل',
    home: 'لوحة التحكم',
    services: 'الخدمات',
    'account-workflows': 'إجراءات الحسابات',
    'transaction-workflows': 'مسارات المعاملات',
    cash: 'الصندوق',
    users: 'المستخدمون',
  },
};

export function isLanguage(value: string | null): value is Language {
  return value === 'en' || value === 'fr' || value === 'ar';
}

export function isThemeMode(value: string | null): value is ThemeMode {
  return value === 'light' || value === 'dark';
}
