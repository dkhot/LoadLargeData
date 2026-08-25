# LoadLargeData

Order reporting application built to browse large volumes of order data with
good performance: server-side paging/search/sort on the .NET API, paired with
an Angular UI that avoids firing redundant requests while the user types or
pages.

## Projects

- [OrderReport/](OrderReport/) — Backend (.NET 10)
  - `OrderReportApi` — ASP.NET Core Web API (EF Core + SQL Server) exposing order report endpoints. Paging is done server-side (`COUNT` + `OFFSET`/`FETCH`), so the client never loads more than one page of rows at a time.
  - `SeedBogusData` — Console app that seeds the database with generated test data (via Bogus)
- [reportui/](reportui/) — Frontend (Angular 20) that consumes the API
  - The order list search box is wired through RxJS `debounceTime` + `distinctUntilChanged` (+ `takeUntilDestroyed` to unsubscribe automatically) so a search request is only sent once the user pauses typing and the term actually changed — not on every keystroke or on a no-op edit.
  - Paging includes First/Prev/Next/Last controls driven by the server-reported `totalPages`, so jumping to either end of a large result set is a single request instead of paging through every page in between.

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
