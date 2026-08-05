import { ArrowLeftRight, ArrowRight, ClipboardList, ExternalLink, EyeOff, Landmark, MoreVertical, Receipt, RotateCw, Trash2, WalletCards } from 'lucide-react';
import { useState } from 'react';
import { tr } from '../../i18n';
import { Account } from '../../types';
import { arAccountName, arActionLabel } from '../../utils/arabic';
import { accountBalance } from '../../utils/format';
import { AccountActionSlot, AccountButtonWidget, AccountsPopupConfig, getButtonPopupConfig } from './accountCardConfig';

type CompteBoxProps = {
  account: Account;
  texts?: string[];
  buttons?: AccountButtonWidget[];
  popups?: AccountsPopupConfig;
  showAction?: boolean;
  onAction?: (action: AccountActionSlot, button: AccountButtonWidget) => void;
  onOpen?: () => void;
  onDetails?: () => void;
  onDelete?: () => void;
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

function moneyDirection(button: AccountButtonWidget, account: Account, popups?: AccountsPopupConfig): 'in' | 'out' | null {
  const buttonPopups = popups ? getButtonPopupConfig(button, popups) : button.popupConfig;
  if (button.action === 'versement') {
    const type = buttonPopups?.movement.applyFixedType ? buttonPopups.movement.fixedType : buttonPopups?.movement.defaultType;
    return type === 'retrait' ? 'out' : 'in';
  }
  if (button.action === 'transfer') {
    const accountId = String(account.id);
    const fromAccountId = buttonPopups?.transfer.applyFixedFromAccount ? buttonPopups.transfer.fixedFromAccountId : accountId;
    const toAccountId = buttonPopups?.transfer.applyFixedToAccount ? buttonPopups.transfer.fixedToAccountId : '';
    if (toAccountId === accountId && fromAccountId !== accountId) return 'in';
    if (fromAccountId === accountId) return 'out';
  }
  return null;
}

function ActionButton({ button, account, popups, onAction }: { button: AccountButtonWidget; account: Account; popups?: AccountsPopupConfig; onAction?: (action: AccountActionSlot) => void }) {
  const action = button.action;
  if (action === 'hidden') return <span />;
  const direction = moneyDirection(button, account, popups);
  const Icon = direction ? ArrowRight : actionIcons[action as keyof typeof actionIcons] ?? ExternalLink;
  return (
    <button className={`compte-action-button${direction ? ` money-${direction}` : ''}`} title={arActionLabel(button.label || action)} aria-label={arActionLabel(button.label || action)} onClick={(event) => { event.stopPropagation(); onAction?.(action, button); }}>
      <Icon className="h-4 w-4" />
    </button>
  );
}
export function CompteBox({ account, texts = [], buttons = [], popups, showAction = true, onAction, onOpen, onDetails, onDelete }: CompteBoxProps) {
  const visibleButtons = buttons.filter((button) => button.visible && button.action !== 'hidden').sort((a, b) => a.position - b.position).slice(0, 2);
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <div className={`compte-box ${onOpen ? 'clickable' : ''}`} onClick={onOpen}>
      <div className="compte-border">
        {(onDetails || onDelete) && (
          <div className="compte-menu-wrap">
            <button
              className="compte-menu-button"
              title={tr('options')}
              aria-label={tr('options')}
              onClick={(event) => {
                event.stopPropagation();
                setMenuOpen((value) => !value);
              }}
            >
              <MoreVertical className="h-4 w-4" />
            </button>
            {menuOpen && (
              <div className="compte-menu">
                {onDetails && <button
                  type="button"
                  onClick={(event) => {
                    event.stopPropagation();
                    setMenuOpen(false);
                    onDetails();
                  }}
                >
                  {tr('details')}
                </button>}
                {onDelete && <button
                  type="button"
                  className="danger"
                  onClick={(event) => {
                    event.stopPropagation();
                    setMenuOpen(false);
                    onDelete();
                  }}
                >
                  <Trash2 className="h-4 w-4" /> Supprimer
                </button>}
              </div>
            )}
          </div>
        )}
        <div className="compte-name">{arAccountName(account.name)}</div>
        <div className="compte-solde">{accountBalance(account.balance)}</div>
        {showAction && (
          <div className="compte-actions-row">
            {visibleButtons[0] ? <ActionButton button={visibleButtons[0]} account={account} popups={popups} onAction={onAction} /> : <span className="compte-action-placeholder" />}
            <div className="compte-dynamic-texts">
              {texts.map((text, index) => <span key={`${text}-${index}`}>{text}</span>)}
            </div>
            {visibleButtons[1] ? <ActionButton button={visibleButtons[1]} account={account} popups={popups} onAction={onAction} /> : <span className="compte-action-placeholder" />}
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
