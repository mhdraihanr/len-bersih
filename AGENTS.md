# Len Bersih – Agent Guide

## What changed recently (26 Sep 2025)

- `Pages/Report/ReportForm.*` now uses a gradient outer shell and tighter spacing. Respect the existing border, padding, and section gap tuning when modifying the report form.
- Mobile tweaks keep form controls balanced at ≤768px. Preserve the responsive padding and captcha column stack when iterating on layout.
- README already documents these UI changes—sync any future styling updates there as well.

## Solution layout

- **LenBersih.Api** – ASP.NET Core backend, primary entry is `Program.cs` with `/api/reports` handled in `Controllers/ReportsController.cs` (in-memory storage, captcha cleanup, evidence validation).
- **LenBersih.Web** – Blazor WASM frontend. Hot reload via `dotnet watch --project LenBersih.Web`.
- **LenBersih.Shared** – Contains `Report` DTO and `ReportMetadata` constants. Update this first when adding categories or evidence rules so both API and Web stay aligned.

## Pages folder restructure

- Each page now sits in its own folder:
  - `Pages/Home/Home.razor` & `.css`
  - `Pages/Report/ReportForm.razor` & `.css`
  - `Pages/CekStatus/CekStatus.razor` & `.css`
- Component-scoped CSS lives beside its Razor file. Keep breakpoints and animations (`data-aos` attributes, keyframes) in sync with `ANIMATION_GUIDE.md`.

## Animation & assets

- AOS initialized in `wwwroot/index.html`; JS glue lives in `Home.razor`. Use the documented `AOS.init` defaults.
- Custom keyframes (`slideInFromLeft`, `fadeInUp`) power hero/CTA reveals—extend them in the relevant page-level CSS rather than a global file.

## Build & verify

- Restore/build: `dotnet restore` then `dotnet build LenBersih.sln`.
- Run API: `dotnet run --project LenBersih.Api` (listens on https://localhost:7000 ).
- Run WASM: `dotnet run --project LenBersih.Web` (from root to avoid duplicate HttpClient registrations).
- No automated tests yet; manually exercise the report form submission path and mobile breakpoints after UI changes.

## Collaboration etiquette

- Mirror README “Pembaruan Terbaru” entries when introducing notable UI or structural changes.
- When moving/shared code across projects, confirm `LenBersih.Web/_Imports.razor` still exposes required namespaces.
- Maintain anonymous-report behavior: toggling `IsAnonymous` must preserve the “Anonim” placeholder and email requirement checks on both client and server.
