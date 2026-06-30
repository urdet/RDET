import { useEffect, useMemo, useState } from 'react';
import { Bar, BarChart, CartesianGrid, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import { CalendarDays, Download, FileText, Percent, Plus, ReceiptText, TrendingUp } from 'lucide-react';
import { api } from '../../api';
import { CircleButton } from '../../shared/ui/CircleButton';
import { DataTable } from '../../shared/ui/DataTable';
import { Panel } from '../../shared/ui/Panel';
import { Dashboard } from '../../types';
import { money } from '../../utils/format';
import { Metric } from '../dashboard/Metric';

type ReportRow = {
  id: number;
  service: string;
  direction: 'IN' | 'OUT';
  amount: string;
  fee: string;
  description: string | null;
  occurred_at: string;
};

type ReportBucket = {
  service?: string;
  date?: string;
  IN: string;
  OUT: string;
  fees: string;
  count: number;
};

type ReportData = {
  kpis: {
    total_in: string;
    total_out: string;
    fees: string;
    net: string;
    count: number;
    average: string;
  };
  by_service: ReportBucket[];
  by_day: ReportBucket[];
  rows: ReportRow[];
};

function todayValue() {
  return new Date().toISOString().slice(0, 10);
}

function monthStartValue() {
  const now = new Date();
  return new Date(now.getFullYear(), now.getMonth(), 1).toISOString().slice(0, 10);
}

export function ReportsPage({ dashboard }: { dashboard: Dashboard | null }) {
  const [fromDate, setFromDate] = useState(monthStartValue());
  const [toDate, setToDate] = useState(todayValue());
  const [report, setReport] = useState<ReportData | null>(null);
  const [error, setError] = useState('');

  useEffect(() => {
    setError('');
    api<ReportData>(`/reports?from_date=${fromDate}&to_date=${toDate}`)
      .then(setReport)
      .catch((err) => setError(err instanceof Error ? err.message : 'Report loading failed.'));
  }, [fromDate, toDate]);

  const dayData = useMemo(() => report?.by_day.map((item) => ({
    date: item.date,
    IN: Number(item.IN || 0),
    OUT: Number(item.OUT || 0),
    fees: Number(item.fees || 0),
  })) ?? [], [report]);

  const serviceData = useMemo(() => report?.by_service.map((item) => ({
    service: item.service,
    volume: Number(item.IN || 0) + Number(item.OUT || 0),
    fees: Number(item.fees || 0),
  })).sort((a, b) => b.volume - a.volume).slice(0, 8) ?? [], [report]);

  const topService = serviceData[0]?.service ?? 'None';
  const kpis = report?.kpis;

  return (
    <div className="reports-page">
      <div className="report-toolbar">
        <label className="form-field">
          From
          <input type="date" value={fromDate} onChange={(event) => setFromDate(event.target.value)} />
        </label>
        <label className="form-field">
          To
          <input type="date" value={toDate} onChange={(event) => setToDate(event.target.value)} />
        </label>
        <CircleButton title="Export PDF" icon={Download} onClick={() => window.print()} />
      </div>

      {error && <div className="transaction-feedback error">{error}</div>}

      <div className="grid gap-4 lg:grid-cols-4">
        <Metric label="IN" value={money(kpis?.total_in ?? dashboard?.service_in ?? 0)} icon={Plus} />
        <Metric label="OUT" value={money(kpis?.total_out ?? dashboard?.service_out ?? 0)} icon={ReceiptText} />
        <Metric label="Fees" value={money(kpis?.fees ?? dashboard?.fees ?? 0)} icon={Percent} />
        <Metric label="Net" value={money(kpis?.net ?? 0)} icon={TrendingUp} />
      </div>

      <div className="grid gap-4 lg:grid-cols-3">
        <Panel title="Daily movement" icon={CalendarDays} className="lg:col-span-2">
          <div className="report-chart">
            <ResponsiveContainer width="100%" height={280}>
              <LineChart data={dayData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis />
                <Tooltip formatter={(value) => money(Number(value))} />
                <Line type="monotone" dataKey="IN" stroke="#047857" strokeWidth={3} />
                <Line type="monotone" dataKey="OUT" stroke="#dc2626" strokeWidth={3} />
                <Line type="monotone" dataKey="fees" stroke="#2563eb" strokeWidth={2} />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </Panel>

        <Panel title="High value" icon={FileText}>
          <div className="report-insights">
            <div><span>Operations</span><strong>{kpis?.count ?? 0}</strong></div>
            <div><span>Average ticket</span><strong>{money(kpis?.average ?? 0)}</strong></div>
            <div><span>Top service</span><strong>{topService}</strong></div>
            <div><span>Total balance</span><strong>{money(dashboard?.total_balance ?? 0)}</strong></div>
          </div>
        </Panel>
      </div>

      <Panel title="Services volume" icon={FileText}>
        <div className="report-chart">
          <ResponsiveContainer width="100%" height={280}>
            <BarChart data={serviceData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="service" />
              <YAxis />
              <Tooltip formatter={(value) => money(Number(value))} />
              <Bar dataKey="volume" fill="#4f46e5" />
              <Bar dataKey="fees" fill="#0f766e" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </Panel>

      <Panel title="Transactions" icon={FileText}>
        <DataTable
          headers={['No', 'Type', 'Service', 'Amount', 'Fees', 'Date', 'Description']}
          rows={(report?.rows ?? []).map((row) => [
            row.id,
            row.direction,
            row.service,
            money(row.amount),
            money(row.fee),
            new Date(row.occurred_at).toLocaleString(),
            row.description ?? '',
          ])}
        />
      </Panel>
    </div>
  );
}
