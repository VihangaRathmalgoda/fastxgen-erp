# 0015. Frontend stack: React + TypeScript with Vite and custom CSS

- Status: Accepted (offline POS still open)
- Date: 2026-10-09

## Context

The product needs a web UI for owners, back-office staff and cashiers. The first screens (sign in, owner
dashboard, POS checkout, products and stock, reporting rules) were designed on the "FastXGen ERP Screens"
design canvas. That design has its own small visual language: IBM Plex Sans and Mono, a navy sidebar, one teal
accent, dense tables and large touch targets on the POS. A component library would have to be re-themed heavily
to match it and would pull in many transitive packages that the team must review and keep patched.

## Decision

1. **React + TypeScript** single-page app in `src/FastXGen.Erp.Web`, built with **Vite**. Routing with
   **React Router**. Strict TypeScript.
2. **No UI kit.** Plain CSS: design tokens in `src/styles/tokens.css` (copied from the design canvas),
   shared primitives (buttons, fields, cards, pills, tables) in `src/styles/base.css`, and one CSS module
   per page or component. Icons are inline stroke SVGs in `components/Icon.tsx`.
3. **Packages** (exact versions in `package.json`): `react`, `react-dom`, `react-router-dom`; dev only `vite`,
   `@vitejs/plugin-react`, `typescript`, `@types/react`, `@types/react-dom`. Adding any other npm package needs
   approval first (CLAUDE.md section 8).
4. **The UI never decides.** Totals, discounts, stock status and rule effects shown before an API call are
   previews; the API's response is authoritative (ADR 0005).
5. **Accessibility as a baseline:** real `button`, `a`, `label` + `input`; visible labels or `sr-only` labels;
   touch targets at least 36 px (44 px on the POS); text contrast 4.5:1.

## Alternatives considered

- MUI: strong components, but Material look fights the design; large dependency tree.
- Ant Design: good back-office tables and forms; same theming and dependency cost.
- Blazor: keeps one language, but weaker POS responsiveness and a smaller ecosystem for offline/PWA work.

## Consequences

- The screens match the design closely and the dependency surface stays small.
- We build our own table, form and dialog primitives as they are needed; keep them in `components/`.
- Still open: whether offline POS (PWA + local queue) is in the MVP. Decide before the POS sale slice.
