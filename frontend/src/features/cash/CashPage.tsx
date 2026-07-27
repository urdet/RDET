import { Calculator, ClipboardList, History, Minus, Plus, Save, Trash2, X } from 'lucide-react';
import { useEffect, useMemo, useState } from 'react';
import { api, getAppSettings } from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { Panel } from '../../shared/ui/Panel';
import { Account, AccountContributionEntry, AppSettings, TransferContribution } from '../../types';
import { money, todayInputValue } from '../../utils/format';

type CashPageProps = {
  accounts: Account[];
  onSaved: () => void;
};

type ContributorCard = {
  name: string;
  total: number;
  entries: Array<AccountContributionEntry & { contribution: TransferContribution; signedAmount: number }>;
};

type CashAdjustmentEntry = { id: number; counter_name: string; direction: '+' | '-'; amount: string; note: string | null; created_at: string };
type CashAdjustments = { counters: Array<{ name: string; total: string }>; entries: CashAdjustmentEntry[] };

export function CashPage({ accounts, onSaved }: CashPageProps) {
  const denominations = ['10000', '1000', '200', '100', '50', '20', '10', '5', '2', '1', '0.5'];
  const [activeTab, setActiveTab] = useState<'cash' | 'unpaid'>('cash');
  const [counts, setCounts] = useState<Record<string, string>>({});
  const [settings, setSettings] = useState<AppSettings>({ cashAccountId: '', unpaidAccountId: '' });
  const [contributionEntries, setContributionEntries] = useState<AccountContributionEntry[]>([]);
  const [unpaidAmount, setUnpaidAmount] = useState('');
  const [creatingPerson, setCreatingPerson] = useState(false);
  const [newPersonName, setNewPersonName] = useState('');
  const [newPersonAmount, setNewPersonAmount] = useState('');
  const [movementPerson, setMovementPerson] = useState<ContributorCard | null>(null);
  const [movementDirection, setMovementDirection] = useState<'+' | '-'>('+');
  const [historyPerson, setHistoryPerson] = useState<ContributorCard | null>(null);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [cashAdjustments, setCashAdjustments] = useState<CashAdjustments>({ counters: [], entries: [] });
  const [adjustmentCounter, setAdjustmentCounter] = useState('');
  const [adjustmentDirection, setAdjustmentDirection] = useState<'+' | '-'>('+');
  const [adjustmentAmount, setAdjustmentAmount] = useState('');
  const [adjustmentNote, setAdjustmentNote] = useState('');
  const [adjustmentOpen, setAdjustmentOpen] = useState(false);
  const [adjustmentHistory, setAdjustmentHistory] = useState('');
  const denominationTotal = denominations.reduce((sum, item) => sum + Number(item) * Number(counts[item] || 0), 0);
  const adjustmentTotal = cashAdjustments.counters.reduce((sum, counter) => sum + Number(counter.total || 0), 0);
  const total = denominationTotal + adjustmentTotal;
  const cashAccount = accounts.find((account) => String(account.id) === settings.cashAccountId);
  const unpaidAccount = accounts.find((account) => String(account.id) === settings.unpaidAccountId);
  const contributorCards = useMemo(() => {
    const cards = new Map<string, ContributorCard>();
    contributionEntries.forEach((entry) => {
      entry.contributions.forEach((contribution) => {
        const direction = contribution.direction ?? entry.direction;
        const signedAmount = (direction === 'versement' ? 1 : -1) * Number(contribution.amount || entry.amount || 0);
        const name = (contribution.name || 'Sans nom').trim();
        const key = name.toLowerCase();
        const card = cards.get(key) ?? { name, total: 0, entries: [] };
        card.total += Number.isFinite(signedAmount) ? signedAmount : 0;
        card.entries.push({ ...entry, contribution: { ...contribution, direction }, signedAmount });
        cards.set(key, card);
      });
    });
    return Array.from(cards.values()).sort((left, right) => left.name.localeCompare(right.name));
  }, [contributionEntries]);

  useEffect(() => {
    getAppSettings<Partial<AppSettings>>()
      .then((value) => setSettings({ cashAccountId: String(value.cashAccountId ?? ''), unpaidAccountId: String(value.unpaidAccountId ?? '') }))
      .catch(() => undefined);
  }, []);

  useEffect(() => {
    loadTodayCashCounts();
    loadCashAdjustments();
  }, []);

  async function loadTodayCashCounts() {
    const today = todayInputValue();
    const value = await api<{ counts: Record<string, number> }>(`/cash-counts/${today}`).catch(() => ({ counts: {} }));
    setCounts(Object.fromEntries(Object.entries(value.counts ?? {}).map(([key, count]) => [key, String(count)])));
  }

  async function loadCashAdjustments() {
    const value = await api<CashAdjustments>('/cash-adjustments').catch(() => ({ counters: [{ name: 'Caisse 2', total: '0' }], entries: [] }));
    setCashAdjustments(value);
  }

  function openCashAdjustment(name: string, direction: '+' | '-') {
    setAdjustmentCounter(name);
    setAdjustmentDirection(direction);
    setAdjustmentAmount('');
    setAdjustmentNote('');
    setError('');
    setAdjustmentOpen(true);
  }

  async function saveCashAdjustment() {
    if (!adjustmentCounter.trim() || Number(adjustmentAmount) <= 0) {
      setError('Saisir un nom et un montant valide.');
      return;
    }
    try {
      await api('/cash-adjustments', {
        method: 'POST',
        body: JSON.stringify({ counter_name: adjustmentCounter.trim(), direction: adjustmentDirection, amount: adjustmentAmount, note: adjustmentNote }),
      });
      setAdjustmentOpen(false);
      await loadCashAdjustments();
      await onSaved();
      setMessage(`${adjustmentCounter} mise à jour.`);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Opération impossible.');
    }
  }

  async function deleteCashAdjustment(name: string) {
    if (!window.confirm(`Supprimer ${name} et tout son historique ?`)) return;
    setError('');
    try {
      await api(`/cash-adjustments/${encodeURIComponent(name)}`, { method: 'DELETE' });
      await loadCashAdjustments();
      await onSaved();
      setMessage(`${name} supprimée.`);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Suppression impossible.');
    }
  }

  useEffect(() => {
    if (activeTab !== 'unpaid' || !settings.unpaidAccountId) return;
    refreshUnpaidContributors();
  }, [activeTab, settings.unpaidAccountId]);

  function refreshUnpaidContributors() {
    if (!settings.unpaidAccountId) {
      setContributionEntries([]);
      return Promise.resolve();
    }
    return api<AccountContributionEntry[]>(`/accounts/${settings.unpaidAccountId}/contributions`)
      .then(setContributionEntries)
      .catch(() => setContributionEntries([]));
  }

  async function submit(event: React.FormEvent) {
    event.preventDefault();
    setError('');
    setMessage('');
    const cleanCounts = Object.fromEntries(Object.entries(counts).filter(([, value]) => value !== '' && Number(value) > 0));
    try {
      await api('/cash-counts', { method: 'POST', body: JSON.stringify({ counted_on: todayInputValue(), counts: cleanCounts }) });
      await loadTodayCashCounts();
      await onSaved();
      setMessage(cashAccount ? `Caisse sauvegardee et solde applique a ${cashAccount.name}.` : 'Caisse sauvegardee.');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Sauvegarde impossible.');
    }
  }

  async function saveUnpaid() {
    if (!movementPerson || Number(unpaidAmount) <= 0) {
      setError('Saisir un montant valide.');
      return;
    }
    setError('');
    setMessage('');
    try {
      await api('/unpaid-movements', {
        method: 'POST',
        body: JSON.stringify({
          person_name: movementPerson.name,
          direction: movementDirection,
          amount: unpaidAmount,
          description: movementDirection === '+' ? 'Add' : 'Retrieve',
        }),
      });
      setUnpaidAmount('');
      setMovementPerson(null);
      await refreshUnpaidContributors();
      await onSaved();
      setMessage('Non paye mis a jour.');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Operation impossible.');
    }
  }

  async function createUnpaidPerson() {
    if (!newPersonName.trim()) {
      setError('Saisir le nom.');
      return;
    }
    if (Number(newPersonAmount) <= 0) {
      setError('Saisir un montant valide.');
      return;
    }
    setError('');
    setMessage('');
    try {
      await api('/unpaid-movements', {
        method: 'POST',
        body: JSON.stringify({
          person_name: newPersonName.trim(),
          direction: '+',
          amount: newPersonAmount,
          description: 'Add',
        }),
      });
      setCreatingPerson(false);
      setNewPersonName('');
      setNewPersonAmount('');
      await refreshUnpaidContributors();
      await onSaved();
      setMessage('Personne ajoutee.');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Creation impossible.');
    }
  }

  function openMovement(item: ContributorCard, direction: '+' | '-') {
    setError('');
    setMessage('');
    setMovementPerson(item);
    setMovementDirection(direction);
    setUnpaidAmount('');
  }

  function openHistory(item: ContributorCard) {
    setHistoryPerson(item);
  }

  return (
    <Panel title="Caisse et non payée" icon={Calculator}>
      <div className="cash-tabs">
        <button className={activeTab === 'cash' ? 'active' : ''} type="button" onClick={() => setActiveTab('cash')}>
          <Calculator className="h-4 w-4" />
          <span>Détail</span>
        </button>
        <button className={activeTab === 'unpaid' ? 'active' : ''} type="button" onClick={() => setActiveTab('unpaid')}>
          <ClipboardList className="h-4 w-4" />
          <span>Non payée</span>
        </button>
      </div>

      {activeTab === 'cash' && (
        <form onSubmit={submit}>
          <div className="cash-total-row">
            <div>
              <div className="cash-total">Total: {money(total)}</div>
              <div className="formula-help">Compte cible: {cashAccount?.name ?? 'Aucun compte configure'}</div>
            </div>
            <CircleButton title="Save" icon={Save} type="submit" />
          </div>
          {(message || error) && <div className={`transaction-feedback ${error ? 'error' : 'success'}`}>{error || message}</div>}
          <div className="denomination-grid">
            {denominations.map((item) => (
              <label key={item} className="denomination-field">
                <span>{item}</span>
                <input value={counts[item] ?? ''} inputMode="numeric" onChange={(event) => setCounts((prev) => ({ ...prev, [item]: event.target.value }))} />
              </label>
            ))}
          </div>
          <section className="cash-adjustments-section">
            <div className="config-section-header">
              <h3>Ajustements</h3>
              <button className="settings-small-action" type="button" onClick={() => openCashAdjustment(`Caisse ${cashAdjustments.counters.length + 2}`, '+')}><Plus className="h-4 w-4" /> Ajouter</button>
            </div>
            <div className="cash-adjustment-grid">
              {cashAdjustments.counters.map((counter) => (
                <article className="cash-adjustment-card" key={counter.name}>
                  <button className="cash-adjustment-main" type="button" onClick={() => setAdjustmentHistory(counter.name)}>
                    <strong>{counter.name}</strong>
                    <b>{money(counter.total)}</b>
                  </button>
                  <button className="mini-action add" title="Ajouter" type="button" onClick={() => openCashAdjustment(counter.name, '+')}><Plus className="h-4 w-4" /></button>
                  <button className="mini-action out" title="Retirer" type="button" onClick={() => openCashAdjustment(counter.name, '-')}><Minus className="h-4 w-4" /></button>
                  <CircleButton title="Historique" icon={History} onClick={() => setAdjustmentHistory(counter.name)} />
                  <button className="mini-action delete" title="Supprimer" type="button" onClick={() => deleteCashAdjustment(counter.name)}><Trash2 className="h-4 w-4" /></button>
                </article>
              ))}
            </div>
          </section>
        </form>
      )}

      {activeTab === 'unpaid' && (
        <>
          <div className="unpaid-list-toolbar">
            <div className="formula-help">Compte cible: {unpaidAccount?.name ?? 'Aucun compte configure'}</div>
            <div className="unpaid-toolbar-actions">
              <CircleButton title="Ajouter personne" icon={Plus} onClick={() => { setError(''); setCreatingPerson(true); }} />
            </div>
          </div>
          {(message || error) && <div className={`transaction-feedback ${error ? 'error' : 'success'}`}>{error || message}</div>}
          <div className="contribution-card-grid unpaid-contributor-grid">
            {contributorCards.map((item) => (
              <article className="unpaid-detail-card contribution-person-card" key={item.name}>
                <button className="unpaid-person-main" type="button" onClick={() => openHistory(item)}>
                  <strong>{item.name}</strong>
                  <span>{unpaidAccount?.name ?? 'Compte cible'} - {item.entries.length} mouvements</span>
                </button>
                <b className={item.total >= 0 ? '' : 'negative'}>{item.total >= 0 ? '+' : '-'}{money(Math.abs(item.total))}</b>
                <button className="mini-action add" title="Add" type="button" onClick={() => openMovement(item, '+')}><Plus className="h-4 w-4" /></button>
                <button className="mini-action out" title="Retrieve" type="button" onClick={() => openMovement(item, '-')}><Minus className="h-4 w-4" /></button>
                <CircleButton title="History" icon={History} onClick={() => openHistory(item)} />
              </article>
            ))}
            {!contributorCards.length && <div className="empty-service-state">Aucun contributeur pour le compte cible.</div>}
          </div>
        </>
      )}

      {creatingPerson && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label="Ajouter personne non paye">
            <Panel title="Ajouter personne" icon={Plus}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div>
                    <div className="transfer-source">Nouvelle personne</div>
                    <div className="transfer-balance">Compte cible: {unpaidAccount?.name ?? 'Aucun compte configure'}</div>
                  </div>
                  <button className="circle-action" title="Fermer" onClick={() => setCreatingPerson(false)}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <label className="form-field">
                  Personne
                  <input value={newPersonName} onChange={(event) => setNewPersonName(event.target.value)} placeholder="Nom" />
                </label>
                <label className="form-field">
                  Montant initial
                  <input value={newPersonAmount} inputMode="decimal" onChange={(event) => setNewPersonAmount(event.target.value)} placeholder="0.00" />
                </label>
                {error && <div className="transfer-error">{error}</div>}
                <div className="modal-actions">
                  <button className="modal-cancel" onClick={() => setCreatingPerson(false)}>Annuler</button>
                  <button className="transfer-submit" onClick={createUnpaidPerson}>Ajouter</button>
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      {movementPerson && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label={movementDirection === '+' ? 'Ajouter non paye' : 'Recuperer non paye'}>
            <Panel title={movementDirection === '+' ? 'Ajouter montant' : 'Recuperer montant'} icon={movementDirection === '+' ? Plus : Minus}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div className="history-summary-line">
                    <div className="transfer-source">{movementPerson.name}</div>
                    <div className="transfer-balance">Total: {movementPerson.total >= 0 ? '+' : '-'}{money(Math.abs(movementPerson.total))}</div>
                  </div>
                  <button className="circle-action" title="Fermer" onClick={() => setMovementPerson(null)}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <label className="form-field">
                  Montant
                  <input value={unpaidAmount} inputMode="decimal" onChange={(event) => setUnpaidAmount(event.target.value)} placeholder="0.00" />
                </label>
                {error && <div className="transfer-error">{error}</div>}
                <div className="modal-actions">
                  <button className="modal-cancel" onClick={() => setMovementPerson(null)}>Annuler</button>
                  <button className="transfer-submit" onClick={saveUnpaid}>{movementDirection === '+' ? 'Ajouter' : 'Recuperer'}</button>
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      {historyPerson && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label="Historique non paye">
            <Panel title={`Historique ${historyPerson.name}`} icon={History}>
              <div className="transfer-panel">
                <div className="transfer-panel-header">
                  <div className="history-summary-line">
                    <div className="transfer-source">{historyPerson.name}</div>
                    <div className="transfer-balance">Total: {historyPerson.total >= 0 ? '+' : '-'}{money(Math.abs(historyPerson.total))}</div>
                  </div>
                  <button className="circle-action" title="Fermer" onClick={() => setHistoryPerson(null)}>
                    <X className="h-4 w-4" />
                  </button>
                </div>
                <div className="unpaid-history-list">
                  {historyPerson.entries.map((row) => (
                    <div className={`unpaid-history-row ${row.contribution.direction === 'versement' ? 'add' : 'out'}`} key={`${historyPerson.name}-${row.id}-${row.occurred_at}`}>
                      <span>{new Date(row.occurred_at).toLocaleString()}</span>
                      <small>{row.description ?? (row.contribution.direction === 'versement' ? 'Add' : 'Retrieve')}</small>
                      <strong>{row.signedAmount >= 0 ? '+' : '-'}{money(Math.abs(row.signedAmount))}</strong>
                    </div>
                  ))}
                  {!historyPerson.entries.length && <div className="empty-service-state">Aucun historique.</div>}
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      {adjustmentOpen && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label="Mouvement caisse">
            <Panel title={`${adjustmentDirection === '+' ? 'Ajouter à' : 'Retirer de'} ${adjustmentCounter}`} icon={adjustmentDirection === '+' ? Plus : Minus}>
              <div className="transfer-panel">
                <div className="transfer-panel-header modal-close-only">
                  <button className="circle-action" title="Fermer" onClick={() => setAdjustmentOpen(false)}><X className="h-4 w-4" /></button>
                </div>
                <label className="form-field">Nom<input value={adjustmentCounter} onChange={(event) => setAdjustmentCounter(event.target.value)} /></label>
                <label className="form-field">Montant<input value={adjustmentAmount} inputMode="decimal" onChange={(event) => setAdjustmentAmount(event.target.value)} placeholder="0.00" /></label>
                <label className="form-field">Note<input value={adjustmentNote} onChange={(event) => setAdjustmentNote(event.target.value)} /></label>
                {error && <div className="transfer-error">{error}</div>}
                <div className="modal-actions">
                  <button className="modal-cancel" type="button" onClick={() => setAdjustmentOpen(false)}>Annuler</button>
                  <button className="transfer-submit" type="button" onClick={saveCashAdjustment}>Valider</button>
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}

      {adjustmentHistory && (
        <div className="account-modal-backdrop" role="presentation">
          <div className="account-modal" role="dialog" aria-modal="true" aria-label={`Historique ${adjustmentHistory}`}>
            <Panel title={`Historique ${adjustmentHistory}`} icon={History}>
              <div className="transfer-panel">
                <div className="transfer-panel-header modal-close-only">
                  <button className="circle-action" title="Fermer" onClick={() => setAdjustmentHistory('')}><X className="h-4 w-4" /></button>
                </div>
                <div className="unpaid-history-list">
                  {cashAdjustments.entries.filter((row) => row.counter_name === adjustmentHistory).map((row) => (
                    <div className={`unpaid-history-row ${row.direction === '+' ? 'add' : 'out'}`} key={row.id}>
                      <span>{new Date(row.created_at).toLocaleString()}</span>
                      <small>{row.note || 'Mouvement'}</small>
                      <strong>{row.direction}{money(row.amount)}</strong>
                    </div>
                  ))}
                  {!cashAdjustments.entries.some((row) => row.counter_name === adjustmentHistory) && <div className="empty-service-state">Aucun historique.</div>}
                </div>
              </div>
            </Panel>
          </div>
        </div>
      )}
    </Panel>
  );
}
