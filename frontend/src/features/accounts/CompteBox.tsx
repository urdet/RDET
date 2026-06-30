import { ArrowLeftRight, ClipboardList, ExternalLink, EyeOff, Landmark, MoreVertical, Receipt, RotateCw, WalletCards } from 'lucide-react';
import { useState } from 'react';
import { Account } from '../../types';
import { money } from '../../utils/format';
import { AccountActionSlot, AccountButtonWidget } from './accountCardConfig';

type CompteBoxProps = {
  account: Account;
  texts?: string[];
  buttons?: AccountButtonWidget[];
  showAction?: boolean;
  onAction?: (action: AccountActionSlot) => void;
  onOpen?: () => void;
  onDetails?: () => void;
};

const actionIcons = {
  hidden: EyeOff,
  versement: WalletCards,
  transfer: ArrowLeftRight,
  cash: Landmark,
  unpaid: Receipt,
  transactions: ClipboardList,
  refresh: RotateCw,
};

function ActionButton({ button, onAction }: { button: AccountButtonWidget; onAction?: (action: AccountActionSlot) => void }) {
  const action = button.action;
  if (action === 'hidden') return <span />;
  const Icon = actionIcons[action as keyof typeof actionIcons] ?? ExternalLink;
  return (
    <button className="compte-action-button" title={button.label || action} aria-label={button.label || action} onClick={(event) => { event.stopPropagation(); onAction?.(action); }}>
      <Icon className="h-4 w-4" />
    </button>
  );
}

export function CompteBox({ account, texts = [], buttons = [], showAction = true, onAction, onOpen, onDetails }: CompteBoxProps) {
  const visibleButtons = buttons.filter((button) => button.visible && button.action !== 'hidden').sort((a, b) => a.position - b.position).slice(0, 2);
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <div className={`compte-box ${onOpen ? 'clickable' : ''}`} onClick={onOpen}>
      <div className="compte-border">
        {onDetails && (
          <div className="compte-menu-wrap">
            <button
              className="compte-menu-button"
              title="Options"
              aria-label="Options"
              onClick={(event) => {
                event.stopPropagation();
                setMenuOpen((value) => !value);
              }}
            >
              <MoreVertical className="h-4 w-4" />
            </button>
            {menuOpen && (
              <div className="compte-menu">
                <button
                  type="button"
                  onClick={(event) => {
                    event.stopPropagation();
                    setMenuOpen(false);
                    onDetails();
                  }}
                >
                  Details
                </button>
              </div>
            )}
          </div>
        )}
        <div className="compte-name">{account.name}</div>
        <div className="compte-solde">{money(account.balance)}</div>
        {showAction && (
          <div className="compte-actions-row">
            {visibleButtons[0] ? <ActionButton button={visibleButtons[0]} onAction={onAction} /> : <span className="compte-action-placeholder" />}
            <div className="compte-dynamic-texts">
              {texts.map((text, index) => <span key={`${text}-${index}`}>{text}</span>)}
            </div>
            {visibleButtons[1] ? <ActionButton button={visibleButtons[1]} onAction={onAction} /> : <span className="compte-action-placeholder" />}
          </div>
        )}
        {!showAction && (
          <div className="compte-dynamic-texts standalone">
            {texts.map((text, index) => <span key={`${text}-${index}`}>{text}</span>)}
          </div>
        )}
      </div>
    </div>
  );
}
