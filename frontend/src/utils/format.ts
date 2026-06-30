export function money(value: string | number) {
  return Number(value).toLocaleString('fr-MA', { style: 'currency', currency: 'MAD' });
}

export function todayInputValue() {
  return new Date().toISOString().slice(0, 10);
}
