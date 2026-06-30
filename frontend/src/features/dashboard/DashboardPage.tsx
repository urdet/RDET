import { Banknote, BarChart3, ClipboardList, Plus, ReceiptText } from 'lucide-react';
import { useMemo } from 'react';
import { Bar, BarChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import { Panel } from '../../shared/ui/Panel';
import { Dashboard } from '../../types';
import { money } from '../../utils/format';
import { Metric } from './Metric';

export function DashboardPage({ dashboard }: { dashboard: Dashboard | null }) {
  const data = useMemo(() => dashboard?.accounts.map((account) => ({ name: account.name, balance: Number(account.balance) })) ?? [], [dashboard]);
  return (
    <div className="grid gap-4 xl:grid-cols-[360px_1fr]">
      <div className="grid gap-3">
        <Metric label="Total balance" value={money(dashboard?.total_balance ?? 0)} icon={Banknote} />
        <Metric label="Service IN" value={money(dashboard?.service_in ?? 0)} icon={Plus} />
        <Metric label="Service OUT" value={money(dashboard?.service_out ?? 0)} icon={ReceiptText} />
        <Metric label="Unpaid" value={money(dashboard?.unpaid_total ?? 0)} icon={ClipboardList} />
      </div>
      <Panel title="Service chart" icon={BarChart3} className="min-h-[420px]">
        <div className="h-[360px]">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={data}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" tick={{ fontSize: 11 }} />
              <YAxis tick={{ fontSize: 11 }} />
              <Tooltip />
              <Bar dataKey="balance" fill="#4f46e5" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </Panel>
    </div>
  );
}
