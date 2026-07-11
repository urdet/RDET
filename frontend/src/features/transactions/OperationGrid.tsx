import { Trash2 } from 'lucide-react';
import type { KeyboardEvent } from 'react';
import { Direction, OperationRow } from '../../types';

type OperationGridProps = {
  direction: Direction;
  rows: OperationRow[];
  hasFees: boolean;
  autoSave: boolean;
  onAdd: () => void;
  onChange: (clientId: string, patch: Partial<OperationRow>) => void;
  onDelete: (clientId: string) => void;
  onCommit: (clientId: string) => void;
};

const statusLabel: Record<OperationRow['status'], string> = {
  draft: 'Brouillon',
  saving: 'Save...',
  saved: 'Valide',
  error: 'Erreur',
};

export function OperationGrid({ direction, rows, hasFees, autoSave, onAdd, onChange, onDelete, onCommit }: OperationGridProps) {
  const total = rows.reduce((sum, row) => sum + Number(row.amount || 0) + (hasFees ? Number(row.fee || 0) : 0), 0);

  function commitOnEnter(event: KeyboardEvent<HTMLInputElement>, clientId: string) {
    if (event.key !== 'Enter') return;
    event.preventDefault();
    onCommit(clientId);
  }

  return (
    <div className="operation-block">
      <div className="operation-label">{direction}s Operations</div>
      <table className="operation-grid">
        <thead>
          <tr>
            <th className="hidden-col">ID</th>
            <th>{direction}s</th>
            {hasFees && <th>Frais</th>}
            <th className="status-col">Etat</th>
            <th className="delete-col"></th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row, index) => {
            const locked = row.status === 'saving';
            return (
            <tr key={row.clientId}>
              <td className="hidden-col">{row.id ?? ''}</td>
              <td>
                <input
                  value={row.amount}
                  inputMode="decimal"
                  disabled={locked}
                  onChange={(event) => onChange(row.clientId, { amount: event.target.value, status: 'draft' })}
                  onFocus={() => index === rows.length - 1 && onAdd()}
                  onBlur={() => autoSave && onCommit(row.clientId)}
                  onKeyDown={(event) => commitOnEnter(event, row.clientId)}
                />
              </td>
              {hasFees && (
                <td>
                  <input
                    value={row.fee}
                    inputMode="decimal"
                    disabled={locked}
                    onChange={(event) => onChange(row.clientId, { fee: event.target.value, status: 'draft' })}
                    onBlur={() => autoSave && onCommit(row.clientId)}
                    onKeyDown={(event) => commitOnEnter(event, row.clientId)}
                  />
                </td>
              )}
              <td className="status-col">
                <button className={`row-status ${row.status}`} title={statusLabel[row.status]} onClick={() => onCommit(row.clientId)} disabled={locked || !row.amount}>
                  {row.import_batch_id ? 'Excel' : statusLabel[row.status]}
                </button>
              </td>
              <td>
                <button className="grid-delete" title="Delete" onClick={() => onDelete(row.clientId)} disabled={row.status === 'saving'}>
                  <Trash2 className="h-4 w-4" />
                </button>
              </td>
            </tr>
            );
          })}
        </tbody>
      </table>
      <div className="operation-total">{total.toFixed(2)}</div>
    </div>
  );
}
