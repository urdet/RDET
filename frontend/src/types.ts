export type ScreenId =
  | 'home'
  | 'accounts'
  | 'account-settings'
  | 'services'
  | 'transactions'
  | 'inter-agency-transfers'
  | 'expenses'
  | 'cash'
  | 'register'
  | 'users'
  | 'reports'
  | 'profile'
  | 'settings'
  | 'account-workflows'
  | 'transaction-workflows';

export type Agency = {
  id: number;
  name: string;
};

export type Account = {
  id: number;
  name: string;
  balance: string;
  previous_balance: string | null;
  debit_total: string;
  credit_total: string;
  normal_balance_side: 'debit' | 'credit';
  visible: boolean;
  company_ids: number[];
  legacy_id: number | null;
  updated_at: string;
};

export type AgencyLinkStatus = 'pending' | 'active' | 'rejected' | 'disabled';

export type AgencyTransferRuleStatus = 'pending' | 'active' | 'rejected' | 'disabled';

export type InterAgencyTransferStatus = 'pending_receiver' | 'accepted' | 'cancelled' | 'rejected';

export type AgencyLink = {
  id: number;
  agency_a_id: number;
  agency_b_id: number;
  agency_a_name: string | null;
  agency_b_name: string | null;
  status: AgencyLinkStatus;
  requested_agency_id: number | null;
  target_agency_id: number | null;
  requested_by_user_id: number | null;
  accepted_by_user_id: number | null;
  created_at: string;
  accepted_at: string | null;
  disabled_at: string | null;
};

export type AgencyTransferRule = {
  id: number;
  agency_link_id: number;
  source_agency_id: number;
  source_account_id: number;
  destination_agency_id: number;
  destination_account_id: number;
  source_agency_name: string | null;
  source_account_name: string | null;
  destination_agency_name: string | null;
  destination_account_name: string | null;
  name: string;
  description: string | null;
  status: AgencyTransferRuleStatus;
  active: boolean;
  created_by_user_id: number | null;
  accepted_by_user_id: number | null;
  created_at: string;
  accepted_at: string | null;
};

export type InterAgencyTransfer = {
  id: number;
  transfer_rule_id: number;
  source_agency_id: number;
  source_account_id: number;
  destination_agency_id: number;
  destination_account_id: number;
  source_agency_name: string | null;
  source_account_name: string | null;
  destination_agency_name: string | null;
  destination_account_name: string | null;
  rule_name: string | null;
  amount: string;
  note: string | null;
  status: InterAgencyTransferStatus;
  created_by_user_id: number | null;
  receiver_decision_by_user_id: number | null;
  created_at: string;
  decided_at: string | null;
  settled_amount: string;
  remaining_amount: string;
};

export type InterAgencySettlement = {
  id: number;
  inter_agency_transfer_id: number;
  payer_agency_id: number;
  payer_account_id: number;
  payer_agency_name: string | null;
  payer_account_name: string | null;
  receiver_agency_id: number;
  receiver_account_id: number;
  receiver_agency_name: string | null;
  receiver_account_name: string | null;
  debt_account_id: number;
  debt_account_name: string | null;
  amount: string;
  note: string | null;
  status: 'pending' | 'accepted' | 'rejected';
  account_transfer_id: number | null;
  created_by_user_id: number | null;
  accepted_by_user_id: number | null;
  created_at: string;
  accepted_at: string | null;
};

export type Dashboard = {
  total_balance: string;
  total_debit: string;
  total_credit: string;
  service_in: string;
  service_out: string;
  fees: string;
  unpaid_total: string;
  cash_real: string;
  credit: string;
  debit: string;
  total_sales: string;
  total_purchases: string;
  accounts: Account[];
};

export type CurrentUser = {
  id: number;
  company_id: number | null;
  username: string;
  first_name: string;
  last_name: string;
  email: string | null;
  phone: string | null;
  image_url: string | null;
  role: 'Admin' | 'Chef' | 'User';
  permissions: UserPermissionMap | null;
  company: Agency | null;
};

export type UserRole = 'Admin' | 'Chef' | 'User';

export type ManagedUser = {
  id: number;
  company_id: number | null;
  username: string;
  first_name: string;
  last_name: string;
  email: string | null;
  phone: string | null;
  image_url: string | null;
  role: UserRole;
  permissions: UserPermissionMap | null;
  active: boolean;
  company: Agency | null;
};

export type Service = {
  id: number;
  company_id: number | null;
  name: string;
  image_url: string | null;
  transaction_type: string | null;
  switch_type: string | null;
  primary_account_id: number | null;
  secondary_account_id: number | null;
  routing_config: ServiceRoutingConfig | null;
  active: boolean;
};

export type Direction = 'IN' | 'OUT';

export type ServiceRouteConfig = {
  from_account_id?: number;
  to_account_id?: number;
};

export type ServiceRoutingConfig = Partial<Record<Direction, ServiceRouteConfig>>;

export type OperationRow = {
  clientId: string;
  id?: number;
  amount: string;
  fee: string;
  status: 'draft' | 'saving' | 'saved' | 'error';
};

export type WorkflowNodeKind = 'trigger' | 'account' | 'operation' | 'condition' | 'fee' | 'audit';

export type WorkflowNode = {
  id: string;
  kind: WorkflowNodeKind;
  title: string;
  subtitle: string;
  x: number;
  y: number;
};

export type WorkflowEdge = {
  from: string;
  to: string;
};

export type AccountActionEvent = 'money_in' | 'money_out';

export type AccountActionEffect = 'add' | 'subtract';

export type AccountActionRule = {
  id: string;
  name: string;
  enabled: boolean;
  accountId: string;
  event: AccountActionEvent;
  linkedAccountIds: string[];
  effect: AccountActionEffect;
};

export type AccountActionSettings = {
  rules: AccountActionRule[];
};

export type TransferContribution = {
  account_id?: number;
  agency_id?: number;
  name: string;
  amount: string;
  direction: 'versement' | 'retrait';
};

export type AccountContributionEntry = {
  id: number;
  account_id: number;
  amount: string;
  direction: 'versement' | 'retrait';
  description: string | null;
  occurred_at: string;
  contributions: TransferContribution[];
};

export type AccountMovementEntry = {
  id: number;
  account_id: number;
  amount: string;
  direction: 'in' | 'out';
  side: 'debit' | 'credit';
  debit: string;
  credit: string;
  balance_effect: string;
  balance_after: string;
  description: string | null;
  occurred_at: string;
  from_account_id: number | null;
  to_account_id: number | null;
  from_account_name: string | null;
  to_account_name: string | null;
  contributions: TransferContribution[];
};

export type AgencyLedgerEntry = {
  id: number;
  kind: 'expense' | 'income';
  category: string;
  amount: string;
  description: string | null;
  occurred_at: string;
  created_by: number | null;
};

export type AppSettings = {
  cashAccountId: string;
  unpaidAccountId: string;
  aiProvider?: 'openai' | 'google_gemini';
  importMode?: 'ai' | 'manual';
  openaiApiKey?: string;
  openaiModel?: string;
  geminiApiKey?: string;
  geminiModel?: string;
  openaiImportPrompt?: string;
  manualImportRules?: ManualImportRule[];
  rolePermissions?: RolePermissions;
};

export type AppConfigImportReport = {
  accounts_created: number;
  accounts_updated: number;
  services_created: number;
  services_updated: number;
  settings_imported: number;
  links_created: number;
  rules_created: number;
  rules_updated: number;
  skipped: string[];
};

export type ManualImportMatchType = 'starts_with' | 'equals' | 'contains' | 'ends_with' | 'regex';

export type ManualImportRule = {
  id: string;
  label: string;
  enabled: boolean;
  matchType: ManualImportMatchType;
  pattern: string;
  serviceId: string;
  direction?: '' | Direction;
  caseSensitive?: boolean;
};

export type PermissionAction =
  | 'view'
  | 'create'
  | 'edit'
  | 'delete'
  | 'save'
  | 'open'
  | 'changeBalance'
  | 'accountAction'
  | 'transfer'
  | 'movement'
  | 'import'
  | 'export'
  | 'configure';

export type SectionPermission = Partial<Record<PermissionAction, boolean>>;

export type UserPermissionMap = Partial<Record<ScreenId, SectionPermission>>;

export type RolePermissions = Partial<Record<Exclude<UserRole, 'Admin'>, UserPermissionMap>>;

export type UnpaidItem = {
  id: number;
  person_name: string;
  amount: string;
  description: string | null;
  settled: boolean;
};

export type UnpaidMovement = {
  id: number;
  person_name: string;
  direction: '+' | '-';
  amount: string;
  description: string | null;
  occurred_at: string;
};
