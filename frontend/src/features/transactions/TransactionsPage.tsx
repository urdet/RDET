import { useEffect, useState } from 'react';
import { FileSpreadsheet, Loader2, Play, Save, X } from 'lucide-react';
import { api, getAppSettings } from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { Account, AppSettings, Direction, Service } from '../../types';
import { TransactionWorkspace } from './TransactionWorkspace';

type TransactionsPageProps = {
  services: Service[];
  accounts: Account[];
  onSaved: () => void;
};

type ImportRow = {
  kind?: 'service' | 'charge' | 'unknown';
  service_id?: number | null;
  service: string;
  direction: Direction;
  amount: string;
  fee: string;
  solde?: string;
  occurred_at?: string | null;
  description: string | null;
  status?: 'draft' | 'saving' | 'saved' | 'error';
  error_message?: string;
  source_row_number?: number;
  id?: number;
};

function serviceType(service: Service) {
  return service.transaction_type ?? service.switch_type ?? 'IN & OUT';
}

function fullRowKey(row: ImportRow) {
  return [
    row.kind ?? 'service',
    row.service_id ?? '',
    row.service.trim().toLowerCase(),
    row.direction,
    row.amount.trim(),
    (row.fee ?? '').trim(),
    (row.solde ?? '').trim(),
    row.occurred_at ?? '',
    (row.description ?? '').trim().toLowerCase(),
  ].join('|');
}

function importRowClass(row: ImportRow) {
  return [
    row.status === 'error' ? 'error' : '',
    row.kind === 'unknown' ? 'unknown' : '',
    row.kind === 'charge' ? 'charge' : '',
  ].filter(Boolean).join(' ');
}

export function TransactionsPage({ services, accounts, onSaved }: TransactionsPageProps) {
  const [manualService, setManualService] = useState<Service | null>(null);
  const [importing, setImporting] = useState(false);
  const [saving, setSaving] = useState(false);
  const [rows, setRows] = useState<ImportRow[]>([]);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [importMode, setImportMode] = useState<'ai' | 'manual'>('ai');

  useEffect(() => {
    getAppSettings<Partial<AppSettings>>()
      .then((value) => setImportMode(value.importMode === 'manual' ? 'manual' : 'ai'))
      .catch(() => undefined);
  }, []);

  const totals = rows.reduce((acc, row) => {
    const amount = Number(row.amount || 0);
    if (row.kind === 'charge') return { ...acc, charges: acc.charges + amount };
    return { ...acc, [row.direction]: acc[row.direction] + amount };
  }, { IN: 0, OUT: 0, charges: 0 });

  if (manualService) {
    return <TransactionWorkspace service={manualService} accounts={accounts} onBack={() => setManualService(null)} onSaved={onSaved} />;
  }

  async function importExcel(file: File | null) {
    if (!file) return;
    setImporting(true);
    setRows([]);
    setMessage('');
    setError('');
    try {
      const formData = new FormData();
      formData.append('file', file);
      formData.append('mode', importMode);
      const token = localStorage.getItem('rdet_token');
      const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:8000';
      const response = await fetch(`${apiUrl}/service-transactions/import-ai`, {
        method: 'POST',
        headers: token ? { Authorization: `Bearer ${token}` } : {},
        body: formData,
      });
      if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.detail ?? `Import failed: ${response.status}`);
      }
      const body = await response.json() as { rows: ImportRow[]; mode?: string };
      setRows(body.rows.map((row) => ({ ...row, status: row.status ?? 'draft' })));
      setMessage(`${body.rows.length} rows ready to review with ${body.mode === 'manual' ? 'manual rules' : 'AI scan'}.`);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Import failed.');
    } finally {
      setImporting(false);
    }
  }

  function updateRow(index: number, patch: Partial<ImportRow>) {
    setRows((current) => current.map((row, rowIndex) => rowIndex === index ? { ...row, ...patch, status: 'draft', error_message: '' } : row));
  }

  async function saveImport() {
    setSaving(true);
    setMessage('');
    setError('');
    try {
      const nextRows = [...rows];
      const seen = new Map<string, number>();
      let hasDuplicate = false;
      for (let index = 0; index < nextRows.length; index += 1) {
        const row = nextRows[index];
        if (!row.amount || row.status === 'saved') continue;
        const key = fullRowKey(row);
        const firstIndex = seen.get(key);
        if (firstIndex !== undefined) {
          hasDuplicate = true;
          nextRows[index] = { ...row, status: 'error', error_message: `Duplicate line: matches row ${firstIndex + 1}` };
        } else {
          seen.set(key, index);
        }
      }
      if (hasDuplicate) {
        setRows(nextRows);
        setError('Duplicate lines found. Check the highlighted rows.');
        return;
      }
      for (let index = 0; index < nextRows.length; index += 1) {
        const row = nextRows[index];
        if (!row.amount || row.status === 'saved' || row.status === 'error') continue;
        nextRows[index] = { ...row, status: 'saving' };
        setRows([...nextRows]);
        try {
          if (row.kind === 'charge') {
            throw new Error(`Charge indicated only: ${row.description || row.service || 'charge'}. It was not saved as a service transaction.`);
          }
          if (row.kind === 'unknown' || !row.service_id) {
            throw new Error(`Service not configured: ${row.service || 'unknown'}. Row listed only, not saved.`);
          }
          const saved = await api<{ id: number }>('/service-transactions', {
            method: 'POST',
            body: JSON.stringify({
              service_id: row.service_id,
              direction: row.direction,
              amount: row.amount,
              fee: row.fee || '0',
              occurred_at: row.occurred_at || null,
              description: row.description,
            }),
          });
          nextRows[index] = { ...row, id: saved.id, status: 'saved', error_message: '' };
        } catch (err) {
          nextRows[index] = { ...row, status: 'error', error_message: err instanceof Error ? err.message : 'Save failed' };
        }
        setRows([...nextRows]);
      }
      if (nextRows.some((row) => row.status === 'error')) {
        setError('Some rows were not saved. Check the red rows.');
        return;
      }
      setRows([]);
      await onSaved();
      setMessage('Import saved.');
    } finally {
      setSaving(false);
    }
  }

  function cancelImport() {
    setRows([]);
    setMessage('');
    setError('');
  }

  return (
    <div className="transactions-home">
      <section className="transactions-import-main">
        <div className="transactions-import-header">
          <div>
            <div className="workflow-eyebrow"><FileSpreadsheet className="h-4 w-4" /> Excel import</div>
            <h2>Transactions</h2>
            <p>Upload Excel or CSV, review the scanned table, then save the rows.</p>
          </div>
          <div className="transaction-import-actions">
            <CircleButton title={saving ? 'Saving' : 'Save'} icon={Save} onClick={saveImport} />
            <CircleButton title="Cancel" icon={X} onClick={cancelImport} />
          </div>
        </div>

        <div className="transaction-import-controls">
          <label className="import-dropzone">
            {importing ? <Loader2 className="h-5 w-5 animate-spin" /> : <FileSpreadsheet className="h-5 w-5" />}
            <span>{importing ? 'Scanning file...' : 'Upload Excel / CSV'}</span>
            <input type="file" accept=".xlsx,.xls,.csv" disabled={importing} onChange={(event) => importExcel(event.target.files?.[0] ?? null)} />
          </label>
          <label className="form-field import-mode-field">
            Scan mode
            <select value={importMode} onChange={(event) => setImportMode(event.target.value as 'ai' | 'manual')} disabled={importing}>
              <option value="ai">AI scan</option>
              <option value="manual">Manual rules</option>
            </select>
          </label>
        </div>

        {(message || error) && <div className={`transaction-feedback ${error ? 'error' : 'success'}`}>{error || message}</div>}

        {rows.length ? (
          <>
            <div className="import-summary-row">
              <span>Rows: {rows.length}</span>
              <span>IN: {totals.IN.toFixed(2)}</span>
              <span>OUT: {totals.OUT.toFixed(2)}</span>
              <span>Charges: {totals.charges.toFixed(2)}</span>
            </div>
            <div className="transaction-import-table">
              <table>
                <thead>
                  <tr>
                    <th>Service</th>
                    <th>Kind</th>
                    <th>Type</th>
                    <th>Amount</th>
                    <th>Fee</th>
                    <th>Solde</th>
                    <th>Date/time</th>
                    <th>Description</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {rows.map((row, index) => (
                    <tr key={index} className={importRowClass(row)}>
                      <td>{row.service}</td>
                      <td>{row.kind ?? 'service'}</td>
                      <td>
                        <select value={row.direction} onChange={(event) => updateRow(index, { direction: event.target.value as Direction })}>
                          <option value="IN">IN</option>
                          <option value="OUT">OUT</option>
                        </select>
                      </td>
                      <td><input value={row.amount} inputMode="decimal" onChange={(event) => updateRow(index, { amount: event.target.value })} /></td>
                      <td><input value={row.fee ?? ''} inputMode="decimal" onChange={(event) => updateRow(index, { fee: event.target.value })} /></td>
                      <td><input value={row.solde ?? ''} inputMode="decimal" onChange={(event) => updateRow(index, { solde: event.target.value })} /></td>
                      <td><input value={row.occurred_at ?? ''} onChange={(event) => updateRow(index, { occurred_at: event.target.value })} /></td>
                      <td><input value={row.description ?? ''} onChange={(event) => updateRow(index, { description: event.target.value })} /></td>
                      <td title={row.error_message}>{row.error_message || row.status || 'draft'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </>
        ) : (
          <div className="transaction-empty-import">
            <FileSpreadsheet className="h-10 w-10" />
            <strong>No import loaded</strong>
            <span>Upload an Excel/CSV file.</span>
          </div>
        )}
      </section>

      <aside className="transactions-service-shortcuts">
        <div className="palette-title">Manual transaction</div>
        {services.map((service) => (
          <button key={service.id} className="transaction-service-shortcut" onClick={() => setManualService(service)}>
            {service.image_url ? <img src={service.image_url} alt="" /> : <span>{service.name.slice(0, 2).toUpperCase()}</span>}
            <strong>{service.name}</strong>
            <small>{serviceType(service)}</small>
            <Play className="h-4 w-4" />
          </button>
        ))}
        {!services.length && <div className="empty-service-state">No services yet</div>}
      </aside>
    </div>
  );
}
