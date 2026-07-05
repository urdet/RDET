import { Check, RefreshCw, Save, Upload, X } from 'lucide-react';
import { api } from '../../api';
import { CompteBox } from '../accounts/CompteBox';
import { CircleButton } from '../../shared/ui/CircleButton';
import { Account, Direction, OperationRow, Service } from '../../types';
import { todayInputValue } from '../../utils/format';
import { createClientId } from '../../utils/id';
import { OperationGrid } from './OperationGrid';
import { useEffect, useState } from 'react';

type TransactionWorkspaceProps = {
  service: Service;
  accounts: Account[];
  onBack: () => void;
  onSaved: () => void;
};

function newRow(): OperationRow {
  return { clientId: createClientId(), amount: '', fee: '', status: 'draft' };
}

function supportsDirection(service: Service, direction: Direction) {
  const type = (service.transaction_type ?? service.switch_type ?? 'IN & OUT').toUpperCase();
  return type === 'IN & OUT' || type === '[IN][OUT]' || type === '' || type.includes(direction);
}

function serviceMovementAccounts(accounts: Account[]) {
  const cash = accounts.find((account) => ['caisse calculee', 'caisse calculée', 'caise calcule', 'caise calculee'].includes(account.name.toLowerCase()));
  const fundex = accounts.find((account) => account.name.toLowerCase() === 'fundex');
  return [cash, fundex].filter(Boolean) as Account[];
}

function configuredMovementAccounts(service: Service, accounts: Account[]) {
  const ids = new Set<number>();
  const routingConfig = service.routing_config ?? {};
  (['IN', 'OUT'] as Direction[]).forEach((direction) => {
    const route = routingConfig[direction];
    if (route?.from_account_id) ids.add(route.from_account_id);
    if (route?.to_account_id) ids.add(route.to_account_id);
  });
  return accounts.filter((account) => ids.has(account.id));
}

export function TransactionWorkspace({ service, accounts, onBack, onSaved }: TransactionWorkspaceProps) {
  const [date] = useState(todayInputValue());
  const [autoSave, setAutoSave] = useState(false);
  const [inRows, setInRows] = useState<OperationRow[]>([newRow()]);
  const [outRows, setOutRows] = useState<OperationRow[]>([newRow()]);
  const [saving, setSaving] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const hasFees = Boolean(service.switch_type || service.transaction_type);
  const configuredAccounts = configuredMovementAccounts(service, accounts);
  const relatedAccounts = configuredAccounts.length
    ? configuredAccounts
    : service.primary_account_id && service.secondary_account_id
    ? accounts.filter((account) => account.id === service.primary_account_id || account.id === service.secondary_account_id)
    : serviceMovementAccounts(accounts);

  useEffect(() => {
    let active = true;
    async function loadServiceTransactions() {
      setMessage('');
      setError('');
      try {
        const rows = await api<Array<{ id: number; direction: Direction; amount: string; fee: string; occurred_at: string }>>(
          `/service-transactions?service_id=${service.id}`,
        );
        if (!active) return;
        const inItems: OperationRow[] = [];
        const outItems: OperationRow[] = [];
        rows.forEach((row) => {
          const item = {
            clientId: createClientId(),
            id: row.id,
            amount: String(row.amount),
            fee: String(row.fee ?? ''),
            status: 'saved' as const,
          };
          row.direction === 'IN' ? inItems.push(item) : outItems.push(item);
        });
        setInRows(inItems.length ? [...inItems, newRow()] : [newRow()]);
        setOutRows(outItems.length ? [...outItems, newRow()] : [newRow()]);
      } catch (err) {
        if (active) setError(err instanceof Error ? err.message : 'Operations non chargees.');
      }
    }
    loadServiceTransactions();
    return () => {
      active = false;
    };
  }, [service.id, date, reloadKey]);

  function updateRows(direction: Direction, updater: (rows: OperationRow[]) => OperationRow[]) {
    direction === 'IN' ? setInRows(updater) : setOutRows(updater);
  }

  function addRow(direction: Direction) {
    updateRows(direction, (rows) => {
      const last = rows[rows.length - 1];
      return last?.amount === '' ? rows : [...rows, newRow()];
    });
  }

  function changeRow(direction: Direction, clientId: string, patch: Partial<OperationRow>) {
    updateRows(direction, (rows) => rows.map((row) => row.clientId === clientId ? { ...row, ...patch } : row));
  }

  async function deleteRow(direction: Direction, clientId: string) {
    const rows = direction === 'IN' ? inRows : outRows;
    const row = rows.find((item) => item.clientId === clientId);
    if (!row || row.status === 'saving') return;
    setMessage('');
    setError('');
    try {
      if (row.id) {
        changeRow(direction, clientId, { status: 'saving' });
        await api(`/service-transactions/${row.id}`, { method: 'DELETE' });
        await onSaved();
      }
      updateRows(direction, (items) => {
        const next = items.filter((item) => item.clientId !== clientId);
        return next.length ? next : [newRow()];
      });
      setMessage('Operation supprimee.');
    } catch (err) {
      changeRow(direction, clientId, { status: 'error' });
      setError(err instanceof Error ? err.message : 'Operation non supprimee.');
    }
  }

  async function saveRow(direction: Direction, clientId: string, refresh = true) {
    const rows = direction === 'IN' ? inRows : outRows;
    const row = rows.find((item) => item.clientId === clientId);
    if (!row?.amount || row.status === 'saving') return false;

    setMessage('');
    setError('');
    changeRow(direction, clientId, { status: 'saving' });
    try {
      const saved = row.id
        ? await api<{ id: number }>(`/service-transactions/${row.id}`, {
            method: 'PATCH',
            body: JSON.stringify({ amount: row.amount, fee: direction === 'IN' ? row.fee || '0' : '0' }),
          })
        : await api<{ id: number }>('/service-transactions', {
            method: 'POST',
            body: JSON.stringify({
              service_id: service.id,
              direction,
              amount: row.amount,
              fee: direction === 'IN' ? row.fee || '0' : '0',
              occurred_at: `${date}T00:00:00`,
            }),
          });
      changeRow(direction, clientId, { id: saved.id, status: 'saved' });
      setMessage('Operation enregistree.');
      if (refresh) await onSaved();
      return true;
    } catch (err) {
      changeRow(direction, clientId, { status: 'error' });
      setError(err instanceof Error ? err.message : 'Operation non enregistree.');
      return false;
    }
  }

  async function saveRows(direction: Direction, rows: OperationRow[]) {
    for (const row of rows) {
      await saveRow(direction, row.clientId, false);
    }
  }

  function ensureBlankRows() {
    setInRows((rows) => rows.some((row) => row.amount === '') ? rows : [...rows, newRow()]);
    setOutRows((rows) => rows.some((row) => row.amount === '') ? rows : [...rows, newRow()]);
  }

  async function saveAll() {
    setSaving(true);
    setMessage('');
    setError('');
    try {
      await saveRows('IN', inRows);
      await saveRows('OUT', outRows);
      ensureBlankRows();
      await onSaved();
      setMessage('Toutes les operations visibles sont synchronisees.');
    } finally {
      setSaving(false);
    }
  }

  function refreshWorkspace() {
    setReloadKey((value) => value + 1);
    onSaved();
  }

  return (
    <div className="transaction-workspace">
      <div className="transaction-header">
        <button className="circle-action" title="Back to services" onClick={onBack}>
          <Upload className="h-5 w-5" />
        </button>
        <div>
          <h2>{service.name}</h2>
          <div className="service-relation">{service.transaction_type ?? 'IN & OUT'}</div>
        </div>
        <div className="service-picture">
          {service.image_url ? <img src={service.image_url} alt="" /> : service.name.slice(0, 2).toUpperCase()}
        </div>
      </div>

      <div className="transaction-body">
        <section className="transaction-entry-panel">
          <div className="transaction-tools">
            <label className="toggle-line">
              <input type="checkbox" checked={autoSave} onChange={(event) => setAutoSave(event.target.checked)} />
              <span>{autoSave ? 'Auto Save' : 'Manuel Save'}</span>
            </label>
            <CircleButton title="Refresh comptes" icon={RefreshCw} onClick={refreshWorkspace} />
            {!autoSave && <CircleButton title={saving ? 'Saving' : 'Save'} icon={Save} onClick={saveAll} />}
          </div>
          {(message || error) && <div className={`transaction-feedback ${error ? 'error' : 'success'}`}>{error || message}</div>}

          <div className="operation-grid-layout">
            {supportsDirection(service, 'IN') && (
              <OperationGrid
                direction="IN"
                rows={inRows}
                hasFees={hasFees}
                autoSave={autoSave}
                onAdd={() => addRow('IN')}
                onChange={(id, patch) => changeRow('IN', id, patch)}
                onDelete={(id) => deleteRow('IN', id)}
                onCommit={(id) => saveRow('IN', id)}
              />
            )}
            {supportsDirection(service, 'OUT') && (
              <OperationGrid
                direction="OUT"
                rows={outRows}
                hasFees={false}
                autoSave={autoSave}
                onAdd={() => addRow('OUT')}
                onChange={(id, patch) => changeRow('OUT', id, patch)}
                onDelete={(id) => deleteRow('OUT', id)}
                onCommit={(id) => saveRow('OUT', id)}
              />
            )}
          </div>
        </section>

        <aside className="related-comptes">
          <div className="related-title">lie au comptes:</div>
          {relatedAccounts.map((account) => <CompteBox key={account.id} account={account} showAction={false} />)}
          <div className="related-actions">
            <CircleButton title="Check" icon={Check} />
            <CircleButton title="Close" icon={X} onClick={onBack} />
          </div>
        </aside>
      </div>
    </div>
  );
}
  function rowTotal(direction: Direction, row: OperationRow) {
    const amount = Number(row.amount || 0);
    const fee = direction === 'IN' ? Number(row.fee || 0) : 0;
    return (amount + fee).toFixed(2);
  }
