import { Account, Service, WorkflowEdge, WorkflowNode } from '../../types';

export function accountWorkflowPreset(accounts: Account[]): { nodes: WorkflowNode[]; edges: WorkflowEdge[] } {
  const main = accounts[0];
  return {
    nodes: [
      { id: 'trigger', kind: 'trigger', title: 'Versement / Retrait', subtitle: 'Manual operation from compte panel', x: 80, y: 120 },
      { id: 'account', kind: 'account', title: main?.name ?? 'Compte', subtitle: 'Selected account', x: 360, y: 120 },
      { id: 'direction', kind: 'condition', title: 'Direction', subtitle: 'Versement adds, retrait subtracts', x: 640, y: 80 },
      { id: 'balance', kind: 'operation', title: 'Update solde', subtitle: 'Apply movement atomically', x: 640, y: 220 },
      { id: 'audit', kind: 'audit', title: 'TrackClass', subtitle: 'Create audit line', x: 920, y: 160 },
    ],
    edges: [
      { from: 'trigger', to: 'account' },
      { from: 'account', to: 'direction' },
      { from: 'direction', to: 'balance' },
      { from: 'balance', to: 'audit' },
    ],
  };
}

export function transactionWorkflowPreset(accounts: Account[], services: Service[]): { nodes: WorkflowNode[]; edges: WorkflowEdge[] } {
  const service = services[0];
  const cash = accounts.find((account) => ['caisse calculee', 'caisse calculée', 'caise calcule', 'caise calculee'].includes(account.name.toLowerCase()));
  const fundex = accounts.find((account) => account.name.toLowerCase() === 'fundex');

  return {
    nodes: [
      { id: 'service', kind: 'trigger', title: service?.name ?? 'Service', subtitle: 'Selected service button', x: 80, y: 150 },
      { id: 'type', kind: 'condition', title: service?.transaction_type ?? 'IN / OUT', subtitle: 'Allowed service direction', x: 340, y: 150 },
      { id: 'from', kind: 'account', title: cash?.name ?? 'Caisse Calculee', subtitle: 'IN source / OUT target', x: 610, y: 80 },
      { id: 'to', kind: 'account', title: fundex?.name ?? 'Fundex', subtitle: 'IN target / OUT source', x: 610, y: 240 },
      { id: 'fee', kind: 'fee', title: 'Frais', subtitle: 'Amount-range fee rule', x: 875, y: 80 },
      { id: 'transaction', kind: 'operation', title: 'Save transaction', subtitle: 'transactionsservices + balance updates', x: 875, y: 240 },
      { id: 'audit', kind: 'audit', title: 'Audit', subtitle: 'Track user/service line', x: 1140, y: 170 },
    ],
    edges: [
      { from: 'service', to: 'type' },
      { from: 'type', to: 'from' },
      { from: 'type', to: 'to' },
      { from: 'from', to: 'transaction' },
      { from: 'to', to: 'transaction' },
      { from: 'fee', to: 'transaction' },
      { from: 'transaction', to: 'audit' },
    ],
  };
}
