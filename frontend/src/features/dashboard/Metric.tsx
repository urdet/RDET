import { Banknote } from 'lucide-react';

export function Metric({ label, value, icon: Icon }: { label: string; value: string; icon: typeof Banknote }) {
  return (
    <div className="metric-box">
      <div className="flex items-center justify-between">
        <span>{label}</span>
        <span className="metric-status"><Icon className="h-3.5 w-3.5" /></span>
      </div>
      <div className="metric-value">{value}</div>
      <div className="metric-detail">Balance · Mise a jour instantanee</div>
    </div>
  );
}
