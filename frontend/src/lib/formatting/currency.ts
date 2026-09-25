/**
 * Presentation formatting only. Money math stays in C#; these helpers turn
 * whole shilling amounts into display strings.
 */
export function formatUgx(amountShillings: number): string {
  const rounded = Math.trunc(amountShillings);
  return `UGX ${rounded.toLocaleString("en-US")}`;
}
