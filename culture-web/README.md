# Culture Web

React 19 + TypeScript + Vite frontend for the Culture app.

## Product Shells

- `/buddy`: mobile-first tablet experience for buddy activity execution.
- `/admin`: desktop-first administration, reporting and configuration.

## Commands

```powershell
npm install
npm run dev
npm run build
npm run test
```

Local security defaults expect the API at `https://localhost:7193` because session cookies are `Secure`.

## Environments

- `npm run dev` or `npm run dev:local`: uses `.env.local`.
- `npm run dev:dev`: uses `.env.dev`.
- `npm run build:main`: uses `.env.main`.
