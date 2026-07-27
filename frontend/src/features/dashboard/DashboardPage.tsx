import { ArrowDownRight, ArrowUpRight, Banknote, CircleDollarSign, ClipboardList, Landmark, Scale, WalletCards } from 'lucide-react';
import { useMemo } from 'react';
import { Bar, BarChart, CartesianGrid, Cell, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import { Dashboard } from '../../types';
import { money } from '../../utils/format';

function ratio(value: number, total: number) {
  if (!total) return 0;
  return Math.min(100, Math.max(0, Math.abs(value / total) * 100));
}

export function DashboardPage({ dashboard }: { dashboard: Dashboard | null }) {
  const values = useMemo(() => {
    const accounts = dashboard?.accounts ?? [];
    const totalIn = Number(dashboard?.service_in ?? 0);
    const totalOut = Number(dashboard?.service_out ?? 0);
    const fees = Number(dashboard?.fees ?? 0);
    const totalBalance = Number(dashboard?.total_balance ?? 0);
    const cashReal = Number(dashboard?.cash_real ?? 0);
    const unpaid = Number(dashboard?.unpaid_total ?? 0);
    const netFlow = totalIn - totalOut + fees;
    const sortedAccounts = [...accounts].sort((left, right) => Math.abs(Number(right.balance)) - Math.abs(Number(left.balance)));
    return {
      accounts,
      sortedAccounts,
      totalIn,
      totalOut,
      fees,
      totalBalance,
      cashReal,
      unpaid,
      netFlow,
      positiveAccounts: accounts.filter((account) => Number(account.balance) >= 0).length,
      negativeAccounts: accounts.filter((account) => Number(account.balance) < 0).length,
    };
  }, [dashboard]);

  const flowData = [
    { name: 'Entrées', value: values.totalIn, color: '#16a34a' },
    { name: 'Sorties', value: values.totalOut, color: '#dc2626' },
    { name: 'Frais', value: values.fees, color: '#2563eb' },
  ];
  const accountChart = values.sortedAccounts.slice(0, 8).map((account) => ({ name: account.name, balance: Number(account.balance) }));
  const kpis = [
    { label: 'Solde total', value: values.totalBalance, detail: `${values.accounts.length} comptes`, icon: Banknote, tone: 'neutral' },
    { label: 'Caisse réelle', value: values.cashReal, detail: `${ratio(values.cashReal, values.totalBalance).toFixed(0)}% du solde total`, icon: WalletCards, tone: 'blue' },
    { label: 'Entrées du jour', value: values.totalIn, detail: 'Mouvements entrants', icon: ArrowUpRight, tone: 'positive' },
    { label: 'Sorties du jour', value: values.totalOut, detail: 'Mouvements sortants', icon: ArrowDownRight, tone: 'negative' },
    { label: 'Non payé', value: values.unpaid, detail: 'Montant à récupérer', icon: ClipboardList, tone: 'warning' },
    { label: 'Flux net', value: values.netFlow, detail: values.netFlow >= 0 ? 'Journée positive' : 'Journée négative', icon: Scale, tone: values.netFlow >= 0 ? 'positive' : 'negative' },
  ];

  return (
    <div className="dashboard-overview">
      <header className="dashboard-heading">
        <div>
          <span className="dashboard-eyebrow">Vue opérationnelle</span>
          <h1>Tableau de bord</h1>
          <p>Soldes, activité quotidienne et points à surveiller.</p>
        </div>
        <div className="dashboard-status">
          <span className="live-dot" />
          Données en direct
        </div>
      </header>

      <section className="dashboard-kpi-grid">
        {kpis.map(({ label, value, detail, icon: Icon, tone }) => (
          <article className={`dashboard-kpi ${tone}`} key={label}>
            <div className="dashboard-kpi-top">
              <span>{label}</span>
              <div className="dashboard-kpi-icon"><Icon className="h-4 w-4" /></div>
            </div>
            <strong>{money(value)}</strong>
            <small>{detail}</small>
          </article>
        ))}
      </section>

      <section className="dashboard-main-grid">
        <article className="dashboard-card dashboard-flow-card">
          <div className="dashboard-card-heading">
            <div><span>Activité</span><h2>Entrées et sorties</h2></div>
            <b className={values.netFlow >= 0 ? 'positive' : 'negative'}>{values.netFlow >= 0 ? '+' : ''}{money(values.netFlow)}</b>
          </div>
          <div className="dashboard-chart">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={flowData} margin={{ top: 10, right: 10, left: 4, bottom: 0 }}>
                <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="var(--line)" />
                <XAxis dataKey="name" axisLine={false} tickLine={false} tick={{ fill: 'var(--muted)', fontSize: 12 }} />
                <YAxis axisLine={false} tickLine={false} tick={{ fill: 'var(--muted)', fontSize: 11 }} width={58} />
                <Tooltip formatter={(value) => money(Number(value))} cursor={{ fill: 'var(--hover)' }} />
                <Bar dataKey="value" radius={[6, 6, 0, 0]} maxBarSize={70}>
                  {flowData.map((item) => <Cell key={item.name} fill={item.color} />)}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </article>

        <article className="dashboard-card dashboard-health-card">
          <div className="dashboard-card-heading">
            <div><span>Trésorerie</span><h2>Indicateurs de santé</h2></div>
            <CircleDollarSign className="h-5 w-5" />
          </div>
          <div className="health-metric">
            <div><span>Caisse réelle</span><strong>{money(values.cashReal)}</strong></div>
            <div className="health-track"><i style={{ width: `${ratio(values.cashReal, values.totalBalance)}%` }} /></div>
          </div>
          <div className="health-metric warning">
            <div><span>Non payé</span><strong>{money(values.unpaid)}</strong></div>
            <div className="health-track"><i style={{ width: `${ratio(values.unpaid, values.totalBalance)}%` }} /></div>
          </div>
          <div className="dashboard-health-summary">
            <div><span>Comptes positifs</span><strong>{values.positiveAccounts}</strong></div>
            <div><span>Comptes négatifs</span><strong>{values.negativeAccounts}</strong></div>
            <div><span>Écart caisse / impayés</span><strong className={values.cashReal - values.unpaid >= 0 ? 'positive' : 'negative'}>{money(values.cashReal - values.unpaid)}</strong></div>
          </div>
        </article>
      </section>

      <section className="dashboard-main-grid dashboard-accounts-grid">
        <article className="dashboard-card">
          <div className="dashboard-card-heading">
            <div><span>Répartition</span><h2>Principaux soldes</h2></div>
            <Landmark className="h-5 w-5" />
          </div>
          <div className="dashboard-account-chart">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart layout="vertical" data={accountChart} margin={{ top: 0, right: 20, left: 10, bottom: 0 }}>
                <CartesianGrid strokeDasharray="3 3" horizontal={false} stroke="var(--line)" />
                <XAxis type="number" axisLine={false} tickLine={false} tick={{ fill: 'var(--muted)', fontSize: 10 }} />
                <YAxis type="category" dataKey="name" width={110} axisLine={false} tickLine={false} tick={{ fill: 'var(--muted)', fontSize: 11 }} />
                <Tooltip formatter={(value) => money(Number(value))} cursor={{ fill: 'var(--hover)' }} />
                <Bar dataKey="balance" radius={[0, 5, 5, 0]} maxBarSize={22}>
                  {accountChart.map((item) => <Cell key={item.name} fill={item.balance >= 0 ? '#4f46e5' : '#dc2626'} />)}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </div>
        </article>

        <article className="dashboard-card dashboard-account-table-card">
          <div className="dashboard-card-heading">
            <div><span>Comptes</span><h2>Situation actuelle</h2></div>
            <span className="dashboard-table-count">{values.accounts.length}</span>
          </div>
          <div className="dashboard-table-wrap">
            <table className="dashboard-account-table">
              <thead><tr><th>Compte</th><th>Solde</th><th>Hier</th><th>Évolution</th></tr></thead>
              <tbody>
                {values.sortedAccounts.slice(0, 10).map((account) => {
                  const balance = Number(account.balance);
                  const yesterday = Number(account.yesterday_balance ?? account.balance);
                  const change = balance - yesterday;
                  return (
                    <tr key={account.id}>
                      <th>{account.name}</th>
                      <td className={balance < 0 ? 'negative' : ''}>{money(balance)}</td>
                      <td>{money(yesterday)}</td>
                      <td className={change >= 0 ? 'positive' : 'negative'}>{change >= 0 ? '+' : ''}{money(change)}</td>
                    </tr>
                  );
                })}
                {!values.accounts.length && <tr><td colSpan={4}>Aucun compte disponible.</td></tr>}
              </tbody>
            </table>
          </div>
        </article>
      </section>
    </div>
  );
}
