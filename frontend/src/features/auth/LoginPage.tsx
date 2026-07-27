import { ArrowRight, AtSign, Building2, CheckCircle2, Eye, EyeOff, LockKeyhole, Moon, Sun, User, UserPlus } from 'lucide-react';
import { FormEvent, ReactNode, useState } from 'react';
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
  const [showPassword, setShowPassword] = useState(false);
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
        <div className={`auth-form-card ${mode === 'register' ? 'is-register' : ''}`}>
          <div className="auth-tabs" role="tablist" aria-label="Authentication mode">
            <button type="button" className={mode === 'login' ? 'active' : ''} onClick={() => switchMode('login')}>Sign in</button>
            <button type="button" className={mode === 'register' ? 'active' : ''} onClick={() => switchMode('register')}>Create agency</button>
          </div>

          <div className="auth-form-heading">
            <div className="auth-icon">{mode === 'login' ? <LockKeyhole className="h-5 w-5" /> : <UserPlus className="h-5 w-5" />}</div>
            <div>
              <h2>{mode === 'login' ? 'Welcome back' : 'Start your agency'}</h2>
              <p>{mode === 'login' ? 'Access your AgencyOS workspace.' : 'Set up your workspace and administrator account.'}</p>
            </div>
          </div>

          <form onSubmit={submit} className="auth-form">
            {mode === 'register' && (
              <>
                <div className="auth-form-section">
                  <span>Agency information</span>
                  <small>Your workspace name can be changed later.</small>
                </div>
                <AuthInput icon={<Building2 />} label="Agency name" value={agencyName} onChange={setAgencyName} placeholder="e.g. Atlas Services" autoComplete="organization" required />
                <div className="auth-form-section">
                  <span>Administrator</span>
                  <small>This will be the first admin account.</small>
                </div>
                <div className="auth-form-grid">
                  <AuthInput icon={<User />} label="First name" value={firstName} onChange={setFirstName} placeholder="Sara" autoComplete="given-name" required />
                  <AuthInput icon={<User />} label="Last name" value={lastName} onChange={setLastName} placeholder="Amrani" autoComplete="family-name" required />
                </div>
              </>
            )}
            <AuthInput icon={<AtSign />} label="Username" value={username} onChange={setUsername} placeholder="admin" autoComplete="username" required />
            <AuthInput
              icon={<LockKeyhole />}
              label="Password"
              value={password}
              onChange={setPassword}
              type={showPassword ? 'text' : 'password'}
              placeholder="Enter your password"
              autoComplete={mode === 'login' ? 'current-password' : 'new-password'}
              minLength={6}
              required
              action={(
                <button type="button" className="auth-password-toggle" onClick={() => setShowPassword((current) => !current)} aria-label={showPassword ? 'Hide password' : 'Show password'}>
                  {showPassword ? <EyeOff /> : <Eye />}
                </button>
              )}
            />
            {error && <div className="auth-error">{error}</div>}
            <button className="auth-submit" disabled={saving}>
              <span>{saving ? 'Please wait...' : mode === 'login' ? 'Sign in' : 'Create agency'}</span>
              <ArrowRight className="h-4 w-4" />
            </button>
            {mode === 'register' && <p className="auth-form-note">You can invite more users after the agency is created.</p>}
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
  autoComplete,
  minLength,
  icon,
  action,
  required,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  type?: string;
  placeholder?: string;
  autoComplete?: string;
  minLength?: number;
  icon?: ReactNode;
  action?: ReactNode;
  required?: boolean;
}) {
  return (
    <label className="auth-field">
      <span>{label}</span>
      <div className="auth-input-wrap">
        {icon && <span className="auth-input-icon">{icon}</span>}
        <input type={type} value={value} onChange={(event) => onChange(event.target.value)} placeholder={placeholder} autoComplete={autoComplete} minLength={minLength} required={required} />
        {action}
      </div>
    </label>
  );
}
