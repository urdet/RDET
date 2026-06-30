import { Database } from 'lucide-react';

type ShortcutButtonProps = {
  tone: 'red' | 'yellow' | 'green' | 'cyan';
  label: string;
  icon: typeof Database;
  onClick: () => void;
};

export function ShortcutButton({ tone, label, icon: Icon, onClick }: ShortcutButtonProps) {
  return (
    <button className={`shortcut-button ${tone}`} onClick={onClick}>
      <Icon className="h-7 w-7" />
      <span>{label}</span>
    </button>
  );
}
