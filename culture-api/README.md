# Culture API

Backend .NET 10 for the Culture app.

## Architecture

- Modular monolith with hexagonal boundaries.
- Domain and SharedKernel have no infrastructure dependencies.
- Application owns use-case contracts and ports.
- Infrastructure owns EF Core, persistence and external adapters.
- Api owns HTTP, auth, middleware and endpoint composition.

## First modules

- Identity: native buddy session and Entra admin identity.
- Activities: activity lifecycle and buddy assignments.
- Surveys: survey templates, questions and execution sessions.
- Responses: answers and completion state.
- Reports: dashboard read models.
- Audit: functional traceability.

## Commands

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Culture.Api
```

## Local Credentials

- Buddy email: `buddy@solvoglobal.com`
- Buddy password: `ChangeMe123!`

Run local API with:

```powershell
dotnet run --project src/Culture.Api/Culture.Api.csproj --launch-profile https
```

The `https` launch profile uses `ASPNETCORE_ENVIRONMENT=Local` and connects to:

```text
Server=localhost\SQLEXPRESS;Database=CultureDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```
