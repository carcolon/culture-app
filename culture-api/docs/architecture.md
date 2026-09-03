# Architecture

The backend starts as a modular monolith. This gives clean domain boundaries without the operational cost of early microservices.

## Layers

- `Culture.SharedKernel`: primitives shared by every module.
- `Culture.Domain`: business entities, value objects and domain events.
- `Culture.Application`: use-case contracts, ports and validation.
- `Culture.Infrastructure`: EF Core, persistence, external adapters and integration services.
- `Culture.Api`: HTTP endpoints, security middleware, OpenAPI and composition root.

## Security Baseline

- Cookies must be `HttpOnly`, `Secure` and `SameSite=None` when used cross-origin.
- Every mutating request must validate CSRF and origin.
- CORS must list exact SWA origins.
- Admin auth is designed for Microsoft Entra.
- Buddy auth is designed for a native credential flow with lockout and rate limits.
- Secrets belong in Azure Key Vault, never in source control.

## Implemented Security Controls

- Native buddy login uses ASP.NET Core `PasswordHasher`.
- Buddy lockout is enforced after configurable failed attempts.
- Sessions use the `__Host-culture-session` cookie with `HttpOnly`, `Secure`, `SameSite=None` and path `/`.
- Mutating requests are protected by ASP.NET Core antiforgery validation through `X-CSRF-TOKEN`.
- Mutating requests must include a trusted `Origin` or `Referer`.
- Login has a dedicated rate limiter in addition to the global limiter.
- Logout invalidates the server-side authentication cookie.
- Admin endpoints support Microsoft Entra JWT validation when `AzureAd` settings are provided.
- API responses include restrictive security headers and CSP.
