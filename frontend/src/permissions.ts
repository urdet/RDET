import { AppSettings, CurrentUser, PermissionAction, ScreenId, SectionPermission, UserPermissionMap, UserRole } from './types';

export const permissionActions: PermissionAction[] = [
  'view',
  'create',
  'edit',
  'delete',
  'save',
  'open',
  'changeBalance',
  'accountAction',
  'transfer',
  'movement',
  'import',
  'export',
  'configure',
];

export const permissionActionLabels: Record<PermissionAction, string> = {
  view: 'View',
  create: 'Create',
  edit: 'Edit',
  delete: 'Delete',
  save: 'Save',
  open: 'Open details',
  changeBalance: 'Change solde',
  accountAction: 'Compte action',
  transfer: 'Transfer',
  movement: 'Movement',
  import: 'Import',
  export: 'Export',
  configure: 'Configure',
};

export function emptySectionPermission(): SectionPermission {
  return Object.fromEntries(permissionActions.map((action) => [action, false])) as SectionPermission;
}

export function rolePermissions(settings: Partial<AppSettings>, role: Exclude<UserRole, 'Admin'>): UserPermissionMap {
  return settings.rolePermissions?.[role] ?? {};
}

export function effectivePermissions(user: CurrentUser | null, settings: Partial<AppSettings>): UserPermissionMap {
  if (!user || user.role === 'Admin') return {};
  return {
    ...rolePermissions(settings, user.role),
    ...(user.permissions ?? {}),
  };
}

export function can(user: CurrentUser | null, settings: Partial<AppSettings>, screen: ScreenId, action: PermissionAction) {
  if (!user || user.role === 'Admin') return true;
  if (screen === 'settings') return false;
  return Boolean(effectivePermissions(user, settings)[screen]?.[action]);
}
