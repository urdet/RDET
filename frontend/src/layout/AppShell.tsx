import { PropsWithChildren } from 'react';
import { Bell, Languages, Moon, MoreHorizontal, Plus, RefreshCw, Sun } from 'lucide-react';
import { appName, languageNames, Language, screenLabels, shellText, ThemeMode } from '../i18n';
import { screens } from '../navigation';
import { Agency, CurrentUser, ScreenId } from '../types';
import { Sidebar } from './Sidebar';

type AppShellProps = PropsWithChildren<{
  activeScreen: ScreenId;
  error: string;
  language: Language;
  theme: ThemeMode;
  currentUser: CurrentUser | null;
  agencies: Agency[];
  onRefresh: () => void;
  onLogout: () => void;
  onNavigate: (screen: ScreenId) => void;
  onAgencyChange: (agencyId: number) => void;
  onAgencyCreate: () => void;
  onLanguageChange: (language: Language) => void;
  onThemeChange: (theme: ThemeMode) => void;
}>;

export function AppShell({
  activeScreen,
  error,
  language,
  theme,
  currentUser,
  agencies,
  onRefresh,
  onLogout,
  onNavigate,
  onAgencyChange,
  onAgencyCreate,
  onLanguageChange,
  onThemeChange,
  children,
}: AppShellProps) {
  const active = screens.find((item) => item.id === activeScreen) ?? screens[0];
  const ActiveIcon = active.icon;
  const t = shellText[language];
  const activeLabel = screenLabels[language][active.id];

  return (
    <div className="notion-shell">
      <Sidebar activeScreen={activeScreen} currentUser={currentUser} language={language} onLogout={onLogout} onNavigate={onNavigate} />

      <main className="notion-workspace">
        <header className="notion-topbar">
          <div className="topbar-title" aria-label="Breadcrumb">
            <span>{t.workspace}</span>
            <span>/</span>
            <span>{currentUser?.company?.name ?? appName}</span>
            <span>/</span>
            <strong>{activeLabel}</strong>
          </div>
          <div className="topbar-actions">
            <label className="topbar-select agency-switcher" title={t.workspace} aria-label={t.workspace}>
              <select value={currentUser?.company_id ?? ''} onChange={(event) => onAgencyChange(Number(event.target.value))}>
                {agencies.map((agency) => <option key={agency.id} value={agency.id}>{agency.name}</option>)}
              </select>
            </label>
            <button className="circle-action" title="Create agency" aria-label="Create agency" onClick={onAgencyCreate}>
              <Plus className="h-4 w-4" />
            </button>
            <button className="circle-action" title={t.refresh} aria-label={t.refresh} onClick={onRefresh}>
              <RefreshCw className="h-4 w-4" />
            </button>
            <button
              className="circle-action"
              title={theme === 'dark' ? t.themeLight : t.themeDark}
              aria-label={theme === 'dark' ? t.themeLight : t.themeDark}
              onClick={() => onThemeChange(theme === 'dark' ? 'light' : 'dark')}
            >
              {theme === 'dark' ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
            </button>
            <label className="topbar-select" title={t.language} aria-label={t.language}>
              <Languages className="h-4 w-4" />
              <select value={language} onChange={(event) => onLanguageChange(event.target.value as Language)}>
                {Object.entries(languageNames).map(([value, label]) => <option key={value} value={value}>{label}</option>)}
              </select>
            </label>
            <button className="circle-action" title={t.notifications} aria-label={t.notifications}>
              <Bell className="h-4 w-4" />
            </button>
            <button className="circle-action" title={t.more} aria-label={t.more}>
              <MoreHorizontal className="h-4 w-4" />
            </button>
          </div>
        </header>

        <section className="notion-hero compact">
          <div className="hero-copy">
            <div className="hero-eyebrow">
              <ActiveIcon className="h-4 w-4" />
              <span>{appName}</span>
            </div>
            <div className="hero-title">{activeLabel}</div>
            <div className="hero-description">
              {currentUser ? `${currentUser.first_name} ${currentUser.last_name}` : t.user} · {currentUser?.company?.name ?? t.allSpaces}
            </div>
          </div>
        </section>

        {error && <div className="mx-5 mt-4 rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

        <div className="page-canvas">{children}</div>
      </main>
    </div>
  );
}
