import { ArrowDownCircle, ArrowUpCircle, Plus, RefreshCw, Receipt } from 'lucide-react';
import { FormEvent, useEffect, useMemo, useState } from 'react';
import { api } from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { AgencyLedgerEntry } from '../../types';
import { todayInputValue } from '../../utils/format';

const expenseCategories = ['Rent', 'Internet', 'Salary', 'Transport', 'Office', 'Repair', 'Tax', 'Other'];
const incomeCategories = ['Commission', 'Bonus', 'Service income', 'Other'];

function money(value: number) {
  return value.toFixed(2);
}

export function ExpensesPage() {
  const [rows, setRows] = useState<AgencyLedgerEntry[]>([]);
  const [kind, setKind] = useState<'expense' | 'income'>('expense');
  const [category, setCategory] = useState('Rent');
  const [amount, setAmount] = useState('');
  const [description, setDescription] = useState('');
  const [date, setDate] = useState(todayInputValue());
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  const categories = kind === 'expense' ? expenseCategories : incomeCategories;
  const todayRows = rows.filter((row) => row.occurred_at.slice(0, 10) === todayInputValue());
  const totals = useMemo(() => {
    return rows.reduce((acc, row) => {
      const value = Number(row.amount || 0);
      return row.kind === 'income'
        ? { ...acc, income: acc.income + value }
        : { ...acc, expense: acc.expense + value };
    }, { income: 0, expense: 0 });
  }, [rows]);
  const todayTotal = todayRows.reduce((sum, row) => sum + (row.kind === 'income' ? Number(row.amount) : -Number(row.amount)), 0);

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

  return (
    <div className="expenses-page">
      <section className="expenses-entry">
        <div className="expenses-head">
          <div>
            <div className="workflow-eyebrow"><Receipt className="h-4 w-4" /> Agency ledger</div>
            <h2>Expenses & income</h2>
          </div>
          <CircleButton title="Refresh" icon={RefreshCw} onClick={load} />
        </div>

        <div className="expense-kpis">
          <div><span>Income</span><strong>{money(totals.income)}</strong></div>
          <div><span>Expenses</span><strong>{money(totals.expense)}</strong></div>
          <div><span>Net</span><strong className={totals.income - totals.expense >= 0 ? 'positive' : 'negative'}>{money(totals.income - totals.expense)}</strong></div>
          <div><span>Today net</span><strong className={todayTotal >= 0 ? 'positive' : 'negative'}>{money(todayTotal)}</strong></div>
        </div>

        <form className="expense-form" onSubmit={submit}>
          <div className="expense-kind-toggle">
            <button type="button" className={kind === 'expense' ? 'active expense' : ''} onClick={() => setKind('expense')}><ArrowDownCircle className="h-4 w-4" /> Expense</button>
            <button type="button" className={kind === 'income' ? 'active income' : ''} onClick={() => setKind('income')}><ArrowUpCircle className="h-4 w-4" /> Income</button>
          </div>
          <label className="form-field">
            Category
            <select value={category} onChange={(event) => setCategory(event.target.value)}>
              {categories.map((item) => <option key={item} value={item}>{item}</option>)}
            </select>
          </label>
          <label className="form-field">
            Amount
            <input value={amount} inputMode="decimal" onChange={(event) => setAmount(event.target.value)} />
          </label>
          <label className="form-field">
            Date
            <input type="date" value={date} onChange={(event) => setDate(event.target.value)} />
          </label>
          <label className="form-field expense-note">
            Note
            <input value={description} onChange={(event) => setDescription(event.target.value)} />
          </label>
          <button className="expense-submit" disabled={saving || !amount}><Plus className="h-4 w-4" /> Save</button>
        </form>
        {error && <div className="transaction-feedback error">{error}</div>}
      </section>

      <section className="expenses-history">
        <div className="palette-title">Recent entries</div>
        {rows.map((row) => (
          <div className={`expense-row ${row.kind}`} key={row.id}>
            <div className="expense-row-icon">{row.kind === 'income' ? <ArrowUpCircle className="h-4 w-4" /> : <ArrowDownCircle className="h-4 w-4" />}</div>
            <div>
              <strong>{row.category}</strong>
              <span>{row.description || row.occurred_at.slice(0, 10)}</span>
            </div>
            <em>{row.kind === 'income' ? '+' : '-'}{row.amount}</em>
          </div>
        ))}
        {!rows.length && <div className="transaction-empty-import">No entries yet.</div>}
      </section>
    </div>
  );
}
