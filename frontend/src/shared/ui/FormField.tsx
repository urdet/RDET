import { Account } from '../../types';

export function Input({ label, value, onChange, type = 'text' }: { label: string; value: string; onChange: (value: string) => void; type?: string }) {
  return <label className="form-field">{label}<input type={type} value={value} onChange={(event) => onChange(event.target.value)} /></label>;
}

export function AccountSelect({ label, value, onChange, accounts }: { label: string; value: string; onChange: (value: string) => void; accounts: Account[] }) {
  return (
    <label className="form-field">
      {label}
      <select value={value} onChange={(event) => onChange(event.target.value)}>
        <option value="">Select...</option>
        {accounts.map((account) => <option key={account.id} value={account.id}>{account.name}</option>)}
      </select>
    </label>
  );
}
