import { Plus, Save, Search, ShieldCheck, Users } from 'lucide-react';
import { useEffect, useMemo, useState } from 'react';
import { createUser, getAppSettings, listUsers, updateUser } from '../../api';
import { screens } from '../../navigation';
import { emptySectionPermission, permissionActionLabels, permissionActions, rolePermissions } from '../../permissions';
import { CircleButton } from '../../shared/ui/CircleButton';
import { Panel } from '../../shared/ui/Panel';
import { AppSettings, ManagedUser, PermissionAction, ScreenId, UserPermissionMap, UserRole } from '../../types';

const roles: UserRole[] = ['Admin', 'Chef', 'User'];

const emptyForm = {
  id: 0,
  first_name: '',
  last_name: '',
  username: '',
  password: '',
  role: 'User' as UserRole,
  active: true,
  permissions: {} as UserPermissionMap,
};

export function UsersPage() {
  const [users, setUsers] = useState<ManagedUser[]>([]);
  const [query, setQuery] = useState('');
  const [form, setForm] = useState(emptyForm);
  const [appSettings, setAppSettings] = useState<Partial<AppSettings>>({});
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const filteredUsers = useMemo(() => {
    const normalized = query.trim().toLowerCase();
    if (!normalized) return users;
    return users.filter((user) => `${user.first_name} ${user.last_name} ${user.username} ${user.role}`.toLowerCase().includes(normalized));
  }, [query, users]);

  useEffect(() => {
    refresh();
    getAppSettings<Partial<AppSettings>>().then(setAppSettings).catch(() => setAppSettings({}));
  }, []);

  function defaultPermissionsForRole(role: UserRole): UserPermissionMap {
    if (role === 'Admin') return {};
    return JSON.parse(JSON.stringify(rolePermissions(appSettings, role)));
  }

  function refresh() {
    return listUsers().then(setUsers).catch((err) => setError(err instanceof Error ? err.message : 'Chargement impossible.'));
  }

  function selectUser(user: ManagedUser) {
    setForm({
      id: user.id,
      first_name: user.first_name,
      last_name: user.last_name,
      username: user.username,
      password: '',
      role: user.role,
      active: user.active,
      permissions: user.permissions ?? defaultPermissionsForRole(user.role),
    });
    setMessage('');
    setError('');
  }

  async function save() {
    if (!form.first_name.trim() || !form.last_name.trim() || !form.username.trim()) {
      setError('Remplir nom, prenom et username.');
      return;
    }
    if (!form.id && form.password.length < 6) {
      setError('Mot de passe requis pour un nouvel utilisateur.');
      return;
    }
    setError('');
    setMessage('');
    try {
      if (form.id) {
        await updateUser(form.id, {
          first_name: form.first_name,
          last_name: form.last_name,
          username: form.username,
          role: form.role,
          permissions: form.role === 'Admin' ? null : form.permissions,
          active: form.active,
          ...(form.password ? { password: form.password } : {}),
        });
      } else {
        await createUser({
          first_name: form.first_name,
          last_name: form.last_name,
          username: form.username,
          password: form.password,
          role: form.role,
          permissions: form.role === 'Admin' ? null : form.permissions,
          active: form.active,
        });
      }
      await refresh();
      setForm(emptyForm);
      setMessage('Utilisateur sauvegarde.');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Sauvegarde impossible.');
    }
  }

  function setRole(role: UserRole) {
    setForm((current) => ({ ...current, role, permissions: defaultPermissionsForRole(role) }));
  }

  function permissionValue(screen: ScreenId, action: PermissionAction) {
    return Boolean(form.permissions?.[screen]?.[action]);
  }

  function setPermission(screen: ScreenId, action: PermissionAction, value: boolean) {
    setForm((current) => ({
      ...current,
      permissions: {
        ...(current.permissions ?? {}),
        [screen]: {
          ...emptySectionPermission(),
          ...(current.permissions?.[screen] ?? {}),
          [action]: value,
        },
      },
    }));
  }

  return (
    <Panel title="Utilisateurs" icon={Users}>
      <div className="users-layout">
        <aside className="users-list">
          <label className="account-search compact">
            <Search className="h-4 w-4" />
            <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Utilisateur..." />
          </label>
          <div className="account-settings-items">
            {filteredUsers.map((user) => (
              <button key={user.id} className={`account-settings-item ${form.id === user.id ? 'active' : ''}`} onClick={() => selectUser(user)}>
                <span>{user.first_name} {user.last_name}</span>
                <small>{user.role}</small>
              </button>
            ))}
          </div>
        </aside>

        <section className="users-editor">
          <div className="config-section-header">
            <h3>{form.id ? 'Modifier utilisateur' : 'Nouvel utilisateur'}</h3>
            <CircleButton title="New user" icon={Plus} onClick={() => setForm(emptyForm)} />
          </div>
          <div className="settings-grid">
            <label className="form-field">
              First name
              <input value={form.first_name} onChange={(event) => setForm((current) => ({ ...current, first_name: event.target.value }))} />
            </label>
            <label className="form-field">
              Last name
              <input value={form.last_name} onChange={(event) => setForm((current) => ({ ...current, last_name: event.target.value }))} />
            </label>
            <label className="form-field">
              Username
              <input value={form.username} onChange={(event) => setForm((current) => ({ ...current, username: event.target.value }))} />
            </label>
            <label className="form-field">
              Password
              <input type="password" value={form.password} onChange={(event) => setForm((current) => ({ ...current, password: event.target.value }))} placeholder={form.id ? 'Leave blank to keep' : ''} />
            </label>
            <label className="form-field">
              Validity
              <select value={form.role} onChange={(event) => setRole(event.target.value as UserRole)}>
                {roles.map((role) => <option key={role} value={role}>{role}</option>)}
              </select>
            </label>
            <label className="toggle-line user-active-toggle">
              <input type="checkbox" checked={form.active} onChange={(event) => setForm((current) => ({ ...current, active: event.target.checked }))} />
              <span>Active</span>
            </label>
          </div>
          <div className="role-hint"><ShieldCheck className="h-4 w-4" /> Admin is unrestricted. Other users start from Settings defaults, then this page can override each action.</div>
          {form.role !== 'Admin' && (
            <section className="config-section">
              <div className="config-section-header">
                <h3>User permissions</h3>
                <button className="settings-small-action" type="button" onClick={() => setForm((current) => ({ ...current, permissions: defaultPermissionsForRole(current.role) }))}>
                  Load role defaults
                </button>
              </div>
              <div className="permission-grid user-permission-grid">
                <div className="permission-grid-head">Section</div>
                {permissionActions.map((action) => <div className="permission-grid-head" key={action}>{permissionActionLabels[action]}</div>)}
                {screens.filter((screen) => screen.id !== 'settings').map((screen) => (
                  <div className="permission-row" key={screen.id}>
                    <div className="permission-section">{screen.label}</div>
                    {permissionActions.map((action) => (
                      <label className="permission-check" key={action}>
                        <input
                          type="checkbox"
                          checked={permissionValue(screen.id, action)}
                          onChange={(event) => setPermission(screen.id, action, event.target.checked)}
                        />
                      </label>
                    ))}
                  </div>
                ))}
              </div>
            </section>
          )}
          {(message || error) && <div className={`transaction-feedback ${error ? 'error' : 'success'}`}>{error || message}</div>}
          <div className="settings-actions">
            <CircleButton title="Save user" icon={Save} onClick={save} />
          </div>
        </section>
      </div>
    </Panel>
  );
}
