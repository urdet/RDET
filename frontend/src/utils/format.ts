export function money(value: string | number) {
  return accountBalance(value);
}

export function accountBalance(value: string | number) {
  const amount = Number(value);
  if (!Number.isFinite(amount)) return '0.00';
  const [integer, decimals] = amount.toFixed(2).split('.');
  return `${integer.replace(/\B(?=(\d{3})+(?!\d))/g, ' ')}.${decimals}`;
}

export function todayInputValue() {
  return new Date().toISOString().slice(0, 10);
}
