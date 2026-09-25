# Design System

## Design direction

**Dense, quiet, table-first, operational UI.**

The application is used by staff for extended periods. Optimize for scanability and reliable data entry rather than marketing polish.

## Brand

Primary brand color:

```text
Forest
#285943
```

## Semantic colors

| Meaning | Color |
|---|---|
| Success | `#2F7D4A` |
| Warning | `#A66A00` |
| Danger | `#B43B35` |

## Neutrals

| Purpose | Color |
|---|---|
| Page background | Near-white |
| Surface | White |
| Border | `#D9DDD9` |
| Primary text | `#202522` |
| Muted text | `#5F6862` |

Exact page-background and surface hex values were not specified in the source material; preserve the visual relationship rather than inventing values.

## Typography

Compact application type scale:

| Element | Size |
|---|---:|
| Page title | 20px |
| Table header | 12px |
| Table body | 13px |
| Form label | 12px |

## Spacing

Use a compact spacing rhythm:

```text
4 / 8 / 12 / 16 / 20–24px
```

## Shape

- Border radius: approximately 4–8px.
- Minimal shadows.
- Subtle borders.
- Avoid decorative containers.

## Tables

Tables are the primary information surface.

Rules:

- Hairline borders.
- Muted gray header row.
- Compact row padding.
- Numeric columns right aligned.
- Status badges only where useful.
- Favor density over excessive whitespace.

## Stock status badges

Use the same pattern wherever quantity is presented as a status:

- Green = healthy / in stock
- Amber = at or near reorder level / low stock
- Red = at or below reorder level / out of stock

Status labels:

```text
In Stock
Low Stock
Out of Stock
```

## Dashboard cards

Metric cards should use:

- Muted label above the value.
- Large numeric value.
- Subtle background fill.
- No prominent border.
- Minimal visual decoration.

## Currency display

```text
UGX 125,000
```

## Quantity display

```text
1,250 kg
```

Always include the base unit `kg`.

## Receipt layout

Receipt print layout should be distinct from the normal application interface.

Required visual principles:

- Centered header.
- Thin dividers.
- Right-aligned amounts.
- Clear paid/balance line at the bottom.

Thermal versus A4 is still an open decision.

## Navigation

The core sidebar order is:

1. Dashboard
2. Sales
3. Purchases
4. Commodities
5. Stock Ledger

The icon set specified for the core navigation is Tabler icons.

## Explicit exclusions

The core application UI should not introduce:

- Illustrations
- Mascots
- Marketing-style visual polish
- Excessive whitespace
- Decorative UI that reduces information density
