import { PropsWithChildren } from 'react';
import { Save } from 'lucide-react';

type PanelProps = PropsWithChildren<{
  title?: string;
  icon?: typeof Save;
  className?: string;
}>;

export function Panel({ title, icon: Icon, className = '', children }: PanelProps) {
  return (
    <section className={`notion-card ${className}`}>
      {title && (
        <div className="panel-title">
          {Icon && <Icon className="h-5 w-5" />}
          <span>{title}</span>
        </div>
      )}
      {children}
    </section>
  );
}
