# LoadLargeData

Order reporting application with a .NET backend API and an Angular frontend.

## Projects

- [OrderReport/](OrderReport/) — Backend (.NET 10)
  - `OrderReportApi` — ASP.NET Core Web API (EF Core + SQL Server) exposing order report endpoints
  - `SeedBogusData` — Console app that seeds the database with generated test data (via Bogus)
- [reportui/](reportui/) — Frontend (Angular 20) that consumes the API

## Prerequisites

- .NET 10 SDK
- Node.js + npm
- SQL Server (local or accessible instance) with connection settings configured in `OrderReport/OrderReportApi/appsettings.json` / `appsettings.Development.json`

## Running the backend

```bash
cd OrderReport/OrderReportApi
dotnet run
```

The API applies CORS for `http://localhost:4200` (the Angular dev server).

### Seeding test data

```bash
cd OrderReport/SeedBogusData
dotnet run
```

## Running the frontend

```bash
cd reportui
npm install
ng serve
```

Navigate to `http://localhost:4200/`.
