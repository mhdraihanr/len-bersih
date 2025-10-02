# Len Bersih – Agent Guide

## What changed recently (2 Oct 2025)

- **Random Report ID Generation**: Implemented cryptographically secure random ID generation (100-999999) using `Random.Shared` in ReportsController with uniqueness validation. Success message now displays generated report ID with professional gradient styling for user reference.
- **Professional Footer Component**: Added comprehensive footer in `Layout/Footer.razor` with 3-column responsive grid layout including brand section, navigation links, and contact information. Features AOS (Animate On Scroll) animations consistent with Home.razor patterns (fade-up with 800ms duration and staggered delays), smooth hover effects, and mobile-first responsive design.

### 30 September 2025

- **Email Notification System**: Implemented complete email notification using MailKit for automatic report submissions. Professional HTML email template sends detailed reports to configured Gmail account with SMTP authentication.
- **Gmail Integration**: Added EmailService with Gmail App Password authentication, SMTP configuration via appsettings, and proper error handling for email delivery failures.
- **DNTCaptcha.Blazor Integration**: Replaced custom math CAPTCHA with professional DNTCaptcha.Blazor v1.2.3 component using Security Number (4-digit display) with English language configuration.
- **SecurityNumberModel Implementation**: Added dedicated DTO with Compare attribute validation for CAPTCHA text verification in LenBersih.Shared project.
- **Bootstrap Input Group Layout**: CAPTCHA now uses input-group design with shield icon, responsive canvas display, and refresh button styling.
- **Mobile CAPTCHA Responsive**: Enhanced mobile breakpoints (≤768px) with column stacking, larger touch targets, and proper canvas scaling.
- **Smooth Navigation Animations**: Enhanced NavMenu with smooth underline hover effects using CSS pseudo-elements and transitions for professional user experience.

### 26 September 2025

- `Pages/Report/ReportForm.*` now uses a gradient outer shell and tighter spacing. Respect the existing border, padding, and section gap tuning when modifying the report form.
- Mobile tweaks keep form controls balanced at ≤768px. Preserve the responsive padding and captcha column stack when iterating on layout.
- README already documents these UI changes—sync any future styling updates there as well.

## Solution layout

- **LenBersih.Api** – ASP.NET Core backend, primary entry is `Program.cs` with `/api/reports` handled in `Controllers/ReportsController.cs` (in-memory storage, evidence validation, email notifications via EmailService).
- **LenBersih.Web** – Blazor WASM frontend with DNTCaptcha.Blazor integration. Hot reload via `dotnet watch --project LenBersih.Web`.
  - **Layout Folder**: Contains reusable layout components:
    - `NavMenu.razor` – Top navigation with smooth hover animations
    - `Footer.razor` – Comprehensive footer with 4-column grid, social links, and contact info
    - `MainLayout.razor` – Main page wrapper integrating NavMenu and Footer
- **LenBersih.Shared** – Contains `Report` DTO, `SecurityNumberModel` for CAPTCHA validation, and `ReportMetadata` constants. Update this first when adding categories, evidence rules, or security validation patterns so both API and Web stay aligned.
- **EmailService** – MailKit-based service for sending HTML email notifications with professional template formatting and Gmail SMTP integration.

## Pages folder restructure

- Each page now sits in its own folder:
  - `Pages/Home/Home.razor` & `.css`
  - `Pages/Report/ReportForm.razor` & `.css`
  - `Pages/CekStatus/CekStatus.razor` & `.css`
- Layout components in `Layout/` folder:
  - `NavMenu.razor` & `.css` – Navigation with smooth underline animations
  - `Footer.razor` & `.css` – Professional footer with gradient background and responsive grid
  - `MainLayout.razor` & `.css` – Main layout wrapper
- Component-scoped CSS lives beside its Razor file. Keep breakpoints and animations (`data-aos` attributes, keyframes) in sync with design patterns.

## Email Notification System

- **EmailService Implementation**: Professional MailKit-based service with HTML email templates for automatic report notifications.
- **Gmail SMTP Integration**: Configured for Gmail App Password authentication with proper SMTP settings (smtp.gmail.com:587).
- **HTML Email Templates**: Professional-styled emails with complete report information, anonymous status handling, and corporate branding.
- **Error Handling**: Graceful email failure handling - reports save successfully even if email delivery fails.
- **Configuration**: SMTP settings in appsettings.Development.json with secure App Password authentication.

## CAPTCHA & Security

- **DNTCaptcha.Blazor v1.2.3**: Professional canvas-based CAPTCHA with 4-digit Security Number display (1000-9999 range).
- **Language Configuration**: Set to `NumberToWordLanguage.English` with `DisplayMode.ShowDigits` to ensure proper digit display instead of Persian/Arabic text.
- **SecurityNumberModel**: Uses Compare attribute validation pattern matching `EnteredCaptchaText` against `CaptchaText` from component.
- **Bootstrap Integration**: Input-group layout with shield icon, refresh button, and mobile-responsive canvas sizing.
- **Validation Flow**: Client-side validation before API submission, automatic CAPTCHA regeneration on validation failure.

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
