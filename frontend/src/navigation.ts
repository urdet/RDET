import {
  BarChart3,
  BookOpen,
  BriefcaseBusiness,
  Calculator,
  Database,
  FileText,
  GitFork,
  Link2,
  Receipt,
  ReceiptText,
  Send,
  Settings2,
  Users,
} from 'lucide-react';
import { ScreenId } from './types';

export const screens: Array<{ id: ScreenId; label: string; icon: typeof BarChart3 }> = [
  { id: 'accounts', label: 'Soldes', icon: Database },
  { id: 'account-settings', label: 'Solde settings', icon: Settings2 },
  { id: 'transactions', label: 'Transactions', icon: ReceiptText },
  { id: 'inter-agency-transfers', label: 'Inter-agency', icon: Send },
  { id: 'expenses', label: 'Expenses', icon: Receipt },
  { id: 'reports', label: 'Reports', icon: FileText },
  { id: 'register', label: 'Register', icon: BookOpen },
  { id: 'home', label: 'Home', icon: BarChart3 },
  { id: 'services', label: 'Services', icon: BriefcaseBusiness },
  { id: 'account-workflows', label: 'Compte actions', icon: Link2 },
  { id: 'transaction-workflows', label: 'Transaction workflows', icon: GitFork },
  { id: 'cash', label: 'Caisse et non payée', icon: Calculator },
  { id: 'users', label: 'Users', icon: Users },
];
