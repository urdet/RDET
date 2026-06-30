import { Save } from 'lucide-react';

type CircleButtonProps = {
  title: string;
  icon: typeof Save;
  type?: 'button' | 'submit';
  onClick?: () => void;
};

export function CircleButton({ title, icon: Icon, type = 'button', onClick }: CircleButtonProps) {
  return (
    <button type={type} className="circle-action" title={title} aria-label={title} onClick={onClick}>
      <Icon className="h-5 w-5" />
    </button>
  );
}
