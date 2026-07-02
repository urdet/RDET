import { Account, AccountContributionEntry, Agency, AgencyLink, AgencyTransferRule, CurrentUser, InterAgencySettlement, InterAgencyTransfer, InterAgencyTransferStatus, ManagedUser, UserPermissionMap, UserRole } from './types';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8000';

export async function api<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem('rdet_token');
  const response = await fetch(`${API_URL}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers ?? {}),
    },
  });
  if (!response.ok) {
    const body = await response.json().catch(() => ({}));
    if (response.status === 401) {
      localStorage.removeItem('rdet_token');
      window.dispatchEvent(new Event('rdet:unauthorized'));
    }
    throw new Error(body.detail ?? `Request failed: ${response.status}`);
  }
  return response.json();
}

export async function login(username: string, password: string) {
  return api<{ access_token: string }>('/auth/login', {
    method: 'POST',
    body: JSON.stringify({ username, password }),
  });
}

export async function register(payload: {
  agency_name: string;
  first_name: string;
  last_name: string;
  username: string;
  password: string;
}) {
  return api<{ access_token: string }>('/auth/register', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export async function createAgency(name: string) {
  return api<Agency>('/agencies', {
    method: 'POST',
    body: JSON.stringify({ name }),
  });
}

export async function switchAgency(agencyId: number) {
  return api<CurrentUser>(`/me/agency/${agencyId}`, { method: 'POST' });
}

export async function createAccount(payload: { name: string; balance: string; visible: boolean; normal_balance_side?: 'debit' | 'credit' }) {
  return api<Account>('/accounts', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export async function updateAccountBalance(accountId: number, balance: string, normalBalanceSide?: 'debit' | 'credit') {
  return api<Account>(`/accounts/${accountId}/balance`, {
    method: 'PATCH',
    body: JSON.stringify({ balance, normal_balance_side: normalBalanceSide }),
  });
}

export async function listAgencyAccounts(agencyId: number) {
  return api<Account[]>(`/agencies/${agencyId}/accounts`);
}

export async function listAgencyLinks() {
  return api<AgencyLink[]>('/agency-links');
}

export async function createAgencyLink(agencyId: number) {
  return api<AgencyLink>('/agency-links', {
    method: 'POST',
    body: JSON.stringify({ agency_id: agencyId }),
  });
}

export async function acceptAgencyLink(linkId: number) {
  return api<AgencyLink>(`/agency-links/${linkId}/accept`, { method: 'POST' });
}

export async function rejectAgencyLink(linkId: number) {
  return api<AgencyLink>(`/agency-links/${linkId}/reject`, { method: 'POST' });
}

export async function listAgencyTransferRules() {
  return api<AgencyTransferRule[]>('/agency-transfer-rules');
}

export async function createAgencyTransferRule(payload: {
  agency_link_id: number;
  source_agency_id: number;
  source_account_id: number;
  destination_agency_id: number;
  destination_account_id: number;
  name: string;
  description?: string;
}) {
  return api<AgencyTransferRule>('/agency-transfer-rules', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export async function acceptAgencyTransferRule(ruleId: number) {
  return api<AgencyTransferRule>(`/agency-transfer-rules/${ruleId}/accept`, { method: 'POST' });
}

export async function rejectAgencyTransferRule(ruleId: number) {
  return api<AgencyTransferRule>(`/agency-transfer-rules/${ruleId}/reject`, { method: 'POST' });
}

export async function listInterAgencyTransfers(status?: InterAgencyTransferStatus) {
  return api<InterAgencyTransfer[]>(`/inter-agency-transfers${status ? `?status=${status}` : ''}`);
}

export async function createInterAgencyTransfer(payload: { transfer_rule_id: number; amount: string; note?: string }) {
  return api<InterAgencyTransfer>('/inter-agency-transfers', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export async function acceptInterAgencyTransfer(transferId: number) {
  return api<InterAgencyTransfer>(`/inter-agency-transfers/${transferId}/accept`, { method: 'POST' });
}

export async function cancelInterAgencyTransfer(transferId: number) {
  return api<InterAgencyTransfer>(`/inter-agency-transfers/${transferId}/cancel`, { method: 'POST' });
}

export async function listInterAgencySettlements() {
  return api<InterAgencySettlement[]>('/inter-agency-settlements');
}

export async function createInterAgencySettlement(payload: {
  inter_agency_transfer_id: number;
  payer_account_id: number;
  receiver_account_id: number;
  amount: string;
  note?: string;
}) {
  return api<InterAgencySettlement>('/inter-agency-settlements', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export async function acceptInterAgencySettlement(settlementId: number) {
  return api<InterAgencySettlement>(`/inter-agency-settlements/${settlementId}/accept`, { method: 'POST' });
}

export async function getAccountContributions(accountId: number) {
  return api<AccountContributionEntry[]>(`/accounts/${accountId}/contributions`);
}

export async function getAccountsScreenSettings<T>() {
  return api<T>('/settings/accounts-screen');
}

export async function saveAccountsScreenSettings<T>(config: T) {
  return api<T>('/settings/accounts-screen', {
    method: 'PATCH',
    body: JSON.stringify({ config }),
  });
}

export async function getAccountActionSettings<T>() {
  return api<T>('/settings/account-actions');
}

export async function saveAccountActionSettings<T>(config: T) {
  return api<T>('/settings/account-actions', {
    method: 'PATCH',
    body: JSON.stringify({ config }),
  });
}

export async function getAppSettings<T>() {
  return api<T>('/settings/app');
}

export async function saveAppSettings<T>(config: T) {
  return api<T>('/settings/app', {
    method: 'PATCH',
    body: JSON.stringify({ config }),
  });
}

export async function updateProfile(payload: {
  first_name: string;
  last_name: string;
  email: string | null;
  phone: string | null;
  image_url: string | null;
}) {
  return api<CurrentUser>('/me/profile', {
    method: 'PATCH',
    body: JSON.stringify(payload),
  });
}

export async function updateCredentials(payload: {
  username: string;
  current_password?: string;
  new_password?: string;
}) {
  return api<CurrentUser>('/me/credentials', {
    method: 'PATCH',
    body: JSON.stringify(payload),
  });
}

export async function listUsers() {
  return api<ManagedUser[]>('/users');
}

export async function createUser(payload: {
  first_name: string;
  last_name: string;
  username: string;
  password: string;
  role: UserRole;
  permissions?: UserPermissionMap | null;
  active: boolean;
}) {
  return api<ManagedUser>('/users', {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export async function updateUser(userId: number, payload: {
  first_name: string;
  last_name: string;
  username: string;
  password?: string;
  role: UserRole;
  permissions?: UserPermissionMap | null;
  active: boolean;
}) {
  return api<ManagedUser>(`/users/${userId}`, {
    method: 'PATCH',
    body: JSON.stringify(payload),
  });
}
