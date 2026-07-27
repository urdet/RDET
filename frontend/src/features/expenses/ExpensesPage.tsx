import {
  ArrowDownCircle,
  ArrowUpCircle,
  CalendarDays,
  Lock,
  Plus,
  RefreshCw,
  Receipt,
  Trash2,
  TrendingDown,
  TrendingUp,
  WalletCards,
} from 'lucide-react';
import { FormEvent, useEffect, useMemo, useState } from 'react';
import { api } from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { AgencyLedgerEntry } from '../../types';
import { money, todayInputValue } from '../../utils/format';

const expenseCategories = ['Rent', 'Internet', 'Salary', 'Transport', 'Office', 'Repair', 'Tax', 'Other'];
const incomeCategories = ['Commission', 'Bonus', 'Service income', 'Other'];

export function ExpensesPage() {
  const [rows, setRows] = useState<AgencyLedgerEntry[]>([]);
  const [kind, setKind] = useState<'expense' | 'income'>('expense');
  const [category, setCategory] = useState('Rent');
  const [amount, setAmount] = useState('');
  const [description, setDescription] = useState('');
  const [date, setDate] = useState(todayInputValue());
  const [saving, setSaving] = useState(false);
  const [actionId, setActionId] = useState<number | null>(null);
  const [error, setError] = useState('');

  const categories = kind === 'expense' ? expenseCategories : incomeCategories;
  const totals = useMemo(() => rows.reduce((acc, row) => {
    const value = Number(row.amount || 0);
    return row.kind === 'income'
      ? { ...acc, income: acc.income + value }
      : { ...acc, expense: acc.expense + value };
  }, { income: 0, expense: 0 }), [rows]);
  const todayTotal = useMemo(() => rows
    .filter((row) => row.occurred_at.slice(0, 10) === todayInputValue())
    .reduce((sum, row) => sum + (row.kind === 'income' ? Number(row.amount) : -Number(row.amount)), 0), [rows]);

  async function load() {
    setRows(await api<AgencyLedgerEntry[]>('/agency-ledger'));
  }

  useEffect(() => {
    load().catch((err) => setError(err instanceof Error ? err.message : 'Unable to load.'));
  }, []);

  useEffect(() => {
    setCategory((kind === 'expense' ? expenseCategories : incomeCategories)[0]);
  }, [kind]);

  async function submit(event: FormEvent) {
    event.preventDefault();
    if (!amount) return;
    setSaving(true);
    setError('');
    try {
      await api<AgencyLedgerEntry>('/agency-ledger', {
        method: 'POST',
        body: JSON.stringify({
          kind,
          category,
          amount,
          description: description || null,
          occurred_at: `${date}T00:00:00`,
        }),
      });
      setAmount('');
      setDescription('');
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Save failed.');
    } finally {
      setSaving(false);
    }
  }

  async function lockEntry(row: AgencyLedgerEntry) {
    if (!window.confirm('Lock this transaction? A locked transaction cannot be deleted.')) return;
    setActionId(row.id);
    setError('');
    try {
      await api(`/agency-ledger/${row.id}/lock`, { method: 'POST' });
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to lock transaction.');
    } finally {
      setActionId(null);
    }
  }

  async function deleteEntry(row: AgencyLedgerEntry) {
    if (!window.confirm(`Delete ${row.category} — ${money(Number(row.amount))}?`)) return;
    setActionId(row.id);
    setError('');
    try {
      await api(`/agency-ledger/${row.id}`, { method: 'DELETE' });
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to delete transaction.');
    } finally {
      setActionId(null);
    }
  }

  return (
    <div className="expenses-page expenses-redesign">
      <section className="expenses-entry">
        <div className="expenses-head">
          <div>
            <div className="workflow-eyebrow"><WalletCards className="h-4 w-4" /> Agency ledger</div>
            <h2>Expenses & income</h2>
            <p>Record and review every agency cash movement.</p>
          </div>
          <CircleButton title="Refresh" icon={RefreshCw} onClick={load} />
        </div>

        <div className="expense-kpis">
          <div className="income"><span><TrendingUp /> Total income</span><strong>{money(totals.income)}</strong></div>
          <div className="expense"><span><TrendingDown /> Total expenses</span><strong>{money(totals.expense)}</strong></div>
          <div className="net"><span><WalletCards /> Net balance</span><strong className={totals.income - totals.expense >= 0 ? 'positive' : 'negative'}>{money(totals.income - totals.expense)}</strong></div>
          <div className="today"><span><CalendarDays /> Today</span><strong className={todayTotal >= 0 ? 'positive' : 'negative'}>{money(todayTotal)}</strong></div>
        </div>

        <div className="expense-composer">
          <div className="expense-composer-title">
            <div><Receipt /><span>New transaction</span></div>
            <small>Complete the fields, then save.</small>
          </div>
          <form className="expense-form" onSubmit={submit}>
            <div className="expense-kind-toggle">
              <button type="button" className={kind === 'expense' ? 'active expense' : ''} onClick={() => setKind('expense')}><ArrowDownCircle /> Expense</button>
              <button type="button" className={kind === 'income' ? 'active income' : ''} onClick={() => setKind('income')}><ArrowUpCircle /> Income</button>
            </div>
            <label className="form-field">
              Category
              <select value={category} onChange={(event) => setCategory(event.target.value)}>
                {categories.map((item) => <option key={item} value={item}>{item}</option>)}
              </select>
            </label>
            <label className="form-field">
              Amount
              <input value={amount} inputMode="decimal" placeholder="0.00" onChange={(event) => setAmount(event.target.value)} />
            </label>
            <label className="form-field">
              Date
              <input type="date" value={date} onChange={(event) => setDate(event.target.value)} />
            </label>
            <label className="form-field expense-note">
              Note <small>Optional</small>
              <input value={description} placeholder="Short description" onChange={(event) => setDescription(event.target.value)} />
            </label>
            <button className="expense-submit" disabled={saving || !amount}><Plus /> {saving ? 'Saving…' : 'Save transaction'}</button>
          </form>
        </div>
        {error && <div className="transaction-feedback error">{error}</div>}
      </section>

      <section className="expenses-history">
        <div className="expenses-history-head">
          <div>
            <span>Transaction history</span>
            <small>{rows.length} {rows.length === 1 ? 'entry' : 'entries'}</small>
          </div>
          <Receipt />
        </div>
        <div className="expense-list">
          {rows.map((row) => (
            <article className={`expense-row ${row.kind} ${row.locked ? 'locked' : ''}`} key={row.id}>
              <div className="expense-row-icon">{row.kind === 'income' ? <ArrowUpCircle /> : <ArrowDownCircle />}</div>
              <div className="expense-row-main">
                <div className="expense-row-title">
                  <strong>{row.category}</strong>
                  {row.locked && <span className="expense-lock-badge"><Lock /> Locked</span>}
                </div>
                {row.description && <p>{row.description}</p>}
                <span className="expense-row-date"><CalendarDays /> {row.occurred_at.slice(0, 10)}</span>
              </div>
              <em>{row.kind === 'income' ? '+' : '-'}{money(Number(row.amount))}</em>
              <div className="expense-row-actions">
                {!row.locked && (
                  <>
                    <button type="button" title="Lock transaction" aria-label="Lock transaction" disabled={actionId === row.id} onClick={() => lockEntry(row)}><Lock /></button>
                    <button type="button" className="danger" title="Delete transaction" aria-label="Delete transaction" disabled={actionId === row.id} onClick={() => deleteEntry(row)}><Trash2 /></button>
                  </>
                )}
              </div>
            </article>
          ))}
        </div>
        {!rows.length && <div className="transaction-empty-import">No transactions yet.</div>}
      </section>
    </div>
  );
}
