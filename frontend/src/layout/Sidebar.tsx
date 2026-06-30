import { LogOut, Search, Settings2, UserCircle } from 'lucide-react';
import { useEffect, useMemo, useState } from 'react';
import { getAppSettings } from '../api';
import { appName, Language, screenLabels, shellText } from '../i18n';
import { screens } from '../navigation';
import { can } from '../permissions';
import { BrandLogo } from '../shared/ui/BrandLogo';
import { AppSettings, CurrentUser, ScreenId } from '../types';

type SidebarProps = {
  activeScreen: ScreenId;
  currentUser: CurrentUser | null;
  language: Language;
  onLogout: () => void;
  onNavigate: (screen: ScreenId) => void;
};

export function Sidebar({ activeScreen, currentUser, language, onLogout, onNavigate }: SidebarProps) {
  const [query, setQuery] = useState('');
  const [profileOpen, setProfileOpen] = useState(false);
  const [appSettings, setAppSettings] = useState<Partial<AppSettings>>({});
  const t = shellText[language];

  useEffect(() => {
    getAppSettings<Partial<AppSettings>>().then(setAppSettings).catch(() => setAppSettings({}));
  }, [currentUser?.company_id]);

  function canView(screen: ScreenId) {
    return can(currentUser, appSettings, screen, 'view');
  }

  const visibleScreens = useMemo(() => {
    const normalized = query.trim().toLowerCase();
    const allowed = screens.filter((item) => canView(item.id));
    if (!normalized) return allowed;
    return allowed.filter((item) => screenLabels[language][item.id].toLowerCase().includes(normalized));
  }, [appSettings, currentUser, language, query]);
  const initials = currentUser ? `${currentUser.first_name[0] ?? ''}${currentUser.last_name[0] ?? ''}`.toUpperCase() : 'AO';

  return (
    <aside className="notion-sidebar">
      <div className="sidebar-profile-wrap">
        <button className="sidebar-profile-block" type="button" onClick={() => setProfileOpen((open) => !open)}>
          <div className="sidebar-profile" title="Profile">
            {currentUser?.image_url ? <img src={currentUser.image_url} alt="" /> : currentUser ? <span>{initials}</span> : <BrandLogo />}
          </div>
          <div className="sidebar-profile-text">
            <strong>{currentUser ? `${currentUser.first_name} ${currentUser.last_name}` : appName}</strong>
            <span>{currentUser?.company?.name ?? t.workspace}</span>
          </div>
        </button>
        {profileOpen && (
          <div className="sidebar-profile-menu">
            <button
              type="button"
              onClick={() => {
                setProfileOpen(false);
                onNavigate('profile');
              }}
            >
              <UserCircle className="h-4 w-4" />
              <span>{screenLabels[language].profile}</span>
            </button>
            <button type="button" onClick={onLogout}>
              <LogOut className="h-4 w-4" />
              <span>{t.logout}</span>
            </button>
          </div>
        )}
      </div>
      <label className="sidebar-search">
        <Search className="h-4 w-4" />
        <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder={t.search} />
      </label>
      <nav className="sidebar-nav">
        {visibleScreens.map((item) => {
          const Icon = item.icon;
          const active = activeScreen === item.id;
          const label = screenLabels[language][item.id];
          return (
            <button
              key={item.id}
              className={`sidebar-link ${active ? 'active' : ''}`}
              title={label}
              onClick={() => onNavigate(item.id)}
            >
              <Icon className="h-5 w-5" />
              <span>{label}</span>
            </button>
          );
        })}
      </nav>
      {currentUser?.role === 'Admin' && (
        <button className={`sidebar-link mb-3 ${activeScreen === 'settings' ? 'active' : ''}`} title={screenLabels[language].settings} onClick={() => onNavigate('settings')}>
          <Settings2 className="h-5 w-5" />
          <span>{screenLabels[language].settings}</span>
        </button>
      )}
    </aside>
  );
}
