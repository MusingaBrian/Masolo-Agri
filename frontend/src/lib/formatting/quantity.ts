/**
 * Presentation formatting only. Stock is stored in kg in C#; these helpers
 * turn kg values into display strings. Bags and tonnes are conversions
 * shown next to kg, never stored.
 */
export function formatKg(kilograms: number): string {
  return `${kilograms.toLocaleString("en-US")} kg`;
}

export function kgToBags(kilograms: number, bagKg: number): number {
  return kilograms / bagKg;
}

export function kgToTonnes(kilograms: number): number {
  return kilograms / 1000;
}
