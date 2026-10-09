// Display formatting only. Amounts are calculated by the API (C# decides, SQL stores).
export const currency = 'Rs';

export function formatMoney(amount: number, decimals = 0): string {
  const text = amount.toLocaleString('en-US', {
    minimumFractionDigits: decimals,
    maximumFractionDigits: decimals,
  });
  return `${currency} ${text}`;
}

export function formatSignedMoney(amount: number): string {
  const sign = amount > 0 ? '+' : amount < 0 ? '−' : '';
  return sign + formatMoney(Math.abs(amount));
}
