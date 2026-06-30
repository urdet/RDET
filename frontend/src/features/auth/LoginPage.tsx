import { ArrowRight, Building2, CheckCircle2, LockKeyhole, Moon, Sun, UserPlus } from 'lucide-react';
import { FormEvent, useState } from 'react';
import { login, register } from '../../api';
import { appName, ThemeMode } from '../../i18n';
import { BrandLogo } from '../../shared/ui/BrandLogo';

type LoginPageProps = {
  theme: ThemeMode;
  onThemeChange: (theme: ThemeMode) => void;
  onLogin: (token: string) => void;
};

type AuthMode = 'login' | 'register';

export function LoginPage({ theme, onThemeChange, onLogin }: LoginPageProps) {
  const [mode, setMode] = useState<AuthMode>('login');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [agencyName, setAgencyName] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setError('');
    setSaving(true);
    try {
      const data = mode === 'login'
        ? await login(username, password)
        : await register({
          agency_name: agencyName,
          first_name: firstName,
          last_name: lastName,
          username,
          password,
        });
      onLogin(data.access_token);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Authentication failed');
    } finally {
      setSaving(false);
    }
  }

  function switchMode(nextMode: AuthMode) {
    setMode(nextMode);
    setError('');
    if (nextMode === 'register' && username === 'admin') {
      setUsername('');
      setPassword('');
    }
    if (nextMode === 'login' && !username) {
      setUsername('admin');
      setPassword('admin123');
    }
  }

  return (
    <main className="auth-screen">
      <button
        className="auth-theme-toggle"
        type="button"
        title={theme === 'dark' ? 'Light theme' : 'Dark theme'}
        aria-label={theme === 'dark' ? 'Light theme' : 'Dark theme'}
        onClick={() => onThemeChange(theme === 'dark' ? 'light' : 'dark')}
      >
        {theme === 'dark' ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
      </button>
      <section className="auth-brand-panel">
        <div className="auth-brand-top">
          <BrandLogo variant="full" />
        </div>
        <div className="auth-copy">
          <div className="hero-eyebrow">
            <Building2 className="h-4 w-4" />
            <span>Agency command center</span>
          </div>
          <h1>{appName}</h1>
          <p>Run balances, services, transactions, cash controls, reports, and users from one clean agency operating system.</p>
        </div>
        <div className="auth-points">
          <span><CheckCircle2 className="h-4 w-4" /> Multi-user agency access</span>
          <span><CheckCircle2 className="h-4 w-4" /> Admin account created automatically</span>
          <span><CheckCircle2 className="h-4 w-4" /> Fast dashboard workflow</span>
        </div>
      </section>

      <section className="auth-form-panel">
        <div className="auth-form-card">
          <div className="auth-tabs" role="tablist" aria-label="Authentication mode">
            <button type="button" className={mode === 'login' ? 'active' : ''} onClick={() => switchMode('login')}>Sign in</button>
            <button type="button" className={mode === 'register' ? 'active' : ''} onClick={() => switchMode('register')}>Create agency</button>
          </div>

          <div className="auth-form-heading">
            <div className="auth-icon">{mode === 'login' ? <LockKeyhole className="h-5 w-5" /> : <UserPlus className="h-5 w-5" />}</div>
            <div>
              <h2>{mode === 'login' ? 'Welcome back' : 'Start your agency'}</h2>
              <p>{mode === 'login' ? 'Access your AgencyOS workspace.' : 'Create the agency and first admin user.'}</p>
            </div>
          </div>

          <form onSubmit={submit} className="auth-form">
            {mode === 'register' && (
              <>
                <AuthInput label="Agency name" value={agencyName} onChange={setAgencyName} placeholder="Atlas Services" required />
                <div className="auth-form-grid">
                  <AuthInput label="First name" value={firstName} onChange={setFirstName} placeholder="Sara" required />
                  <AuthInput label="Last name" value={lastName} onChange={setLastName} placeholder="Amrani" required />
                </div>
              </>
            )}
            <AuthInput label="Username" value={username} onChange={setUsername} placeholder="admin" required />
            <AuthInput label="Password" value={password} onChange={setPassword} type="password" placeholder="••••••••" required />
            {error && <div className="auth-error">{error}</div>}
            <button className="auth-submit" disabled={saving}>
              <span>{saving ? 'Please wait...' : mode === 'login' ? 'Sign in' : 'Create agency'}</span>
              <ArrowRight className="h-4 w-4" />
            </button>
          </form>
        </div>
      </section>
    </main>
  );
}

function AuthInput({
  label,
  value,
  onChange,
  type = 'text',
  placeholder,
  required,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  type?: string;
  placeholder?: string;
  required?: boolean;
}) {
  return (
    <label className="auth-field">
      <span>{label}</span>
      <input type={type} value={value} onChange={(event) => onChange(event.target.value)} placeholder={placeholder} required={required} />
    </label>
  );
}
