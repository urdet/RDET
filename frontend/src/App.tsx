import { useEffect, useState } from 'react';
import { api, createAgency, switchAgency } from './api';
import { LoginPage } from './features/auth/LoginPage';
import { AccountSettingsPage } from './features/accounts/AccountSettingsPage';
import { AccountsPage } from './features/accounts/AccountsPage';
import { CashPage } from './features/cash/CashPage';
import { DashboardPage } from './features/dashboard/DashboardPage';
import { ExpensesPage } from './features/expenses/ExpensesPage';
import { RegisterPage } from './features/register/RegisterPage';
import { ReportsPage } from './features/reports/ReportsPage';
import { InterAgencyTransfersPage } from './features/interAgency/InterAgencyTransfersPage';
import { ServicesPage } from './features/services/ServicesPage';
import { SettingsPage } from './features/settings/SettingsPage';
import { TransactionsPage } from './features/transactions/TransactionsPage';
import { UsersPage } from './features/users/UsersPage';
import { ProfilePage } from './features/profile/ProfilePage';
import { AccountWorkflowPage } from './features/workflows/AccountWorkflowPage';
import { TransactionWorkflowPage } from './features/workflows/TransactionWorkflowPage';
import { isLanguage, isThemeMode, Language, ThemeMode, tr } from './i18n';
import { AppShell } from './layout/AppShell';
import { Account, Agency, CurrentUser, Dashboard, ScreenId, Service } from './types';
import { installStaticUiTranslator, translateStaticUi } from './utils/domUiTranslations';

export function App() {
  const [token, setToken] = useState(localStorage.getItem('rdet_token'));
  const [theme, setTheme] = useState<ThemeMode>(() => {
    const saved = localStorage.getItem('agencyos_theme');
    return isThemeMode(saved) ? saved : 'light';
  });
  const [language, setLanguage] = useState<Language>(() => {
    const saved = localStorage.getItem('agencyos_language');
    return isLanguage(saved) ? saved : 'ar';
  });
  const [screen, setScreen] = useState<ScreenId>('accounts');
  const [dashboard, setDashboard] = useState<Dashboard | null>(null);
  const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);
  const [agencies, setAgencies] = useState<Agency[]>([]);
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [services, setServices] = useState<Service[]>([]);
  const [error, setError] = useState('');

  async function refresh() {
    if (!localStorage.getItem('rdet_token')) return;
    const [me, agencyRows, summary, accountRows, serviceRows] = await Promise.all([
      api<CurrentUser>('/me'),
      api<Agency[]>('/agencies'),
      api<Dashboard>('/dashboard'),
      api<Account[]>('/accounts'),
      api<Service[]>('/services'),
    ]);
    setCurrentUser(me);
    setAgencies(agencyRows);
    setDashboard(summary);
    setAccounts(accountRows);
    setServices(serviceRows);
  }

  useEffect(() => {
    refresh().catch((err) => setError(err.message));
  }, [token]);

  useEffect(() => {
    function handleUnauthorized() {
      setToken(null);
      setCurrentUser(null);
      setError('Session expired. Please login again.');
    }
    window.addEventListener('rdet:unauthorized', handleUnauthorized);
    return () => window.removeEventListener('rdet:unauthorized', handleUnauthorized);
  }, []);

  useEffect(() => {
    localStorage.setItem('agencyos_theme', theme);
    document.documentElement.dataset.theme = theme;
  }, [theme]);

  useEffect(() => {
    localStorage.setItem('agencyos_language', language);
    document.documentElement.lang = language;
    document.documentElement.dir = language === 'ar' ? 'rtl' : 'ltr';
    window.setTimeout(() => translateStaticUi(), 0);
  }, [language]);

  useEffect(() => installStaticUiTranslator(), []);

  async function changeAgency(agencyId: number) {
    if (!agencyId) return;
    await switchAgency(agencyId);
    await refresh();
  }

  async function addAgency() {
    const name = window.prompt(tr('agencyNamePrompt', language));
    if (!name?.trim()) return;
    try {
      await createAgency(name.trim());
      await refresh();
      setError('');
    } catch (err) {
      setError(err instanceof Error ? err.message : tr('createAccountFailed', language));
    }
  }

  if (!token) {
    return (
      <LoginPage
        theme={theme}
        onThemeChange={setTheme}
        onLogin={(nextToken) => { localStorage.setItem('rdet_token', nextToken); setToken(nextToken); }}
      />
    );
  }

  return (
    <AppShell
      activeScreen={screen}
      error={error}
      language={language}
      theme={theme}
      currentUser={currentUser}
      agencies={agencies}
      onRefresh={refresh}
      onLogout={() => { localStorage.removeItem('rdet_token'); setToken(null); }}
      onNavigate={setScreen}
      onAgencyChange={changeAgency}
      onAgencyCreate={addAgency}
      onLanguageChange={setLanguage}
      onThemeChange={setTheme}
    >
      {screen === 'home' && <DashboardPage dashboard={dashboard} />}
      {screen === 'accounts' && <AccountsPage accounts={accounts} dashboard={dashboard} currentUser={currentUser} language={language} onRefresh={refresh} onNavigate={setScreen} />}
      {screen === 'account-settings' && <AccountSettingsPage accounts={accounts} dashboard={dashboard} />}
      {screen === 'services' && <ServicesPage services={services} accounts={accounts} onSaved={refresh} />}
      {screen === 'transactions' && <TransactionsPage services={services} accounts={accounts} onSaved={refresh} />}
      {screen === 'inter-agency-transfers' && <InterAgencyTransfersPage accounts={accounts} agencies={agencies} currentUser={currentUser} language={language} />}
      {screen === 'expenses' && <ExpensesPage />}
      {screen === 'cash' && <CashPage accounts={accounts} onSaved={refresh} />}
      {screen === 'register' && <RegisterPage />}
      {screen === 'users' && <UsersPage />}
      {screen === 'reports' && <ReportsPage dashboard={dashboard} />}
      {screen === 'settings' && <SettingsPage accounts={accounts} services={services} />}
      {screen === 'profile' && <ProfilePage currentUser={currentUser} onSaved={refresh} />}
      {screen === 'account-workflows' && <AccountWorkflowPage accounts={accounts} />}
      {screen === 'transaction-workflows' && <TransactionWorkflowPage accounts={accounts} services={services} onSaved={refresh} />}
    </AppShell>
  );
}
