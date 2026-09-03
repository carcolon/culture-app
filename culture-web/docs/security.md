# Frontend Security

## XSS

- React text interpolation is the default rendering strategy.
- Do not use `dangerouslySetInnerHTML` directly.
- If rich HTML is required, sanitize it through `src/shared/security/sanitizeHtml.ts`.
- CSP is configured in `staticwebapp.config.json`; production must not allow `unsafe-inline` or `unsafe-eval`.

## CSRF

- Mutating requests use cookie authentication and must send `X-CSRF-TOKEN`.
- The token is fetched from `/api/auth/csrf` and cached in memory only.
- Requests always use `credentials: 'include'`.
