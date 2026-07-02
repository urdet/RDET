import { trLoose, trStatus } from '../i18n';

export function arText(value: string | null | undefined) {
  return trLoose(value);
}

export function arAccountName(value: string | null | undefined) {
  if (!value) return '';
  return String(value);
}

export function arStatus(value: string | null | undefined) {
  return trStatus(value);
}

export function arActionLabel(value: string | null | undefined) {
  return trLoose(value);
}
