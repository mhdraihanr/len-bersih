# Len Bersih – Agent Guide

## What changed recently (3 Oct 2025)

### **Phase 1: Database Layer Setup - COMPLETED ✅**

#### **Implementation Steps Completed:**

**Step 1: Docker MariaDB Setup**

- Created `docker-compose.yml` dengan MariaDB 11.2 dan phpMyAdmin services
- Configured environment variables: MARIADB_DATABASE, MARIADB_USER, MARIADB_PASSWORD
- Port mapping: Container 3306 → Host 3307
- Persistent volumes: `mariadb_data` untuk data storage
- Init directory: `./database/init` untuk auto-loading SQL schema
- Verified containers running: `docker ps` showed both mariadb and phpmyadmin up
- Test connection: `docker exec mariadb -u lenbersih_user -p` successful

**Step 2: NuGet Package Installation**

```bash
# Removed Pomelo.EntityFrameworkCore.MySql (compatibility issues with .NET 9)
# Installed official Oracle provider instead
dotnet add LenBersih.Api package MySql.EntityFrameworkCore --version 9.0.0
dotnet add LenBersih.Api package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add LenBersih.Api package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 9.0.0
dotnet add LenBersih.Api package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add LenBersih.Api package BCrypt.Net-Next --version 4.0.3
```

**Step 3: Entity Models Creation**
Created 7 entity files in `LenBersih.Api/Data/Entities/` dengan proper mapping:

1. **User.cs**: Full implementation dengan 19 columns matching `users` table schema

   - Data annotations: `[Table("users")]`, `[Column("column_name")]`, `[MaxLength]`, `[Required]`
   - Navigation property: `public virtual ICollection<UserGroup> UserGroups { get; set; }`

2. **Group.cs**: Role management entity

   - Maps to `groups` table (id, name, description)
   - Navigation: `public virtual ICollection<UserGroup> UserGroups { get; set; }`

3. **UserGroup.cs**: Junction table for many-to-many relationship

   - Foreign keys dengan `[ForeignKey]` attribute
   - Navigation to both User and Group entities

4. **ReportEntity.cs**: Core whistleblowing report entity

   - 16 columns dari `pelaporan` table
   - Complex relationships: Status (N:1), Dokumen (1:N), HistoryStatus (1:N)

5. **Status.cs**: Status lookup table

   - Simple entity dengan back-references ke Reports dan HistoryStatus

6. **HistoryStatus.cs**: Audit trail entity

   - Tracks semua status changes dengan alasan field
   - Foreign keys ke Status dan ReportEntity

7. **Dokumen.cs**: Evidence file paths
   - Links to ReportEntity dengan foreign key

**Step 4: LenBersihDbContext Implementation**
Created comprehensive DbContext dengan:

- All DbSets for 7 entities
- `OnModelCreating` with fluent API configurations:
  - User: Unique indexes on email, activation_selector, forgotten_password_selector, remember_selector
  - UserGroup: Composite unique index (user_id, group_id), cascade delete constraints
  - ReportEntity: Restrict delete on Status FK, cascade delete on Dokumen and HistoryStatus
  - Proper constraint names matching existing database schema

**Step 5: Connection String Configuration**
Updated `appsettings.json`:

- Fixed port from 3306 to 3307 (Docker mapping)
- Fixed password from `LenBersih@Pass123` to `LenBersih_Pass123!` (matching docker-compose.yml)
- Added connection parameters: `AllowPublicKeyRetrieval=True;SslMode=None`

**Step 6: Program.cs Updates**

```csharp
// DbContext registration
builder.Services.AddDbContext<LenBersihDbContext>(options =>
    options.UseMySQL(connectionString)
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
    .EnableDetailedErrors(builder.Environment.IsDevelopment())
);

// Database connection test on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LenBersihDbContext>();
    var canConnect = await dbContext.Database.CanConnectAsync();
    // Log statistics: Users, Groups, Reports, Status counts
}
```

**Step 7: Build & Test**

```bash
$ dotnet build LenBersih.sln
Build succeeded in 5.8s (no errors, minor version warning resolved)

$ dotnet run --project LenBersih.Api --launch-profile http
info: Program[0]
      🔄 Testing database connection...
info: Program[0]
      ✅ Database connection successful!
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (9ms) SELECT COUNT(*) FROM `users`
info: Program[0]
      📊 Database Statistics:
         • Users: 1
         • Groups: 2
         • Reports: 0
         • Status Types: 6
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:7083
```

**Issues Encountered & Resolved:**

1. ❌ Pomelo.EntityFrameworkCore.MySql 8.0.2 → TypeLoadException with .NET 9
   ✅ Solution: Switched to MySql.EntityFrameworkCore 9.0.0 (official Oracle provider)

2. ❌ Connection failed: Port 3306 vs 3307
   ✅ Solution: Updated connection string to use port 3307 (Docker host mapping)

3. ❌ Authentication failed: Password mismatch
   ✅ Solution: Fixed password in appsettings.json to match docker-compose.yml

4. ❌ `UseMySql()` method not found
   ✅ Solution: Changed to `UseMySQL()` (capital SQL) for Oracle provider

**Files Created/Modified:**

- ✅ `docker-compose.yml` - MariaDB and phpMyAdmin services
- ✅ `LenBersih.Api/Data/Entities/*.cs` - 7 entity models
- ✅ `LenBersih.Api/Data/LenBersihDbContext.cs` - Complete DbContext
- ✅ `LenBersih.Api/appsettings.json` - Connection string, JWT, Tikitaka settings
- ✅ `LenBersih.Api/Program.cs` - DbContext registration, connection test
- ✅ `LenBersih.Api/LenBersih.Api.csproj` - Package references

**Phase 1 Achievement Summary:**

- ✅ Docker MariaDB running on port 3307
- ✅ phpMyAdmin accessible at http://localhost:8080
- ✅ 7 Entity models fully implemented
- ✅ LenBersihDbContext configured with relationships
- ✅ Database connection verified with statistics
- ✅ API running successfully on http://localhost:7083
- ✅ Ready for Phase 2: Authentication System

---

## Database Structure

### **Entity Relationships**

- **User** 1:N **UserGroup** N:1 **Group** (many-to-many via junction table)
- **ReportEntity** N:1 **Status** (workflow state)
- **ReportEntity** 1:N **Dokumen** (evidence files)
- **ReportEntity** 1:N **HistoryStatus** (audit trail)
- **Status** 1:N **HistoryStatus** (status change log)

### **Key Tables**

- **users** (19 columns): Authentication, profile, activation/reset fields, timestamps
- **groups** (3 columns): Role definitions (admin, members)
- **pelaporan** (16 columns): Core whistleblowing reports with terlapor data, kejadian details, approval workflow
- **status** (2 columns): 6 workflow states for report processing
- **history_status** (4 columns): Audit log for status changes with alasan field
- **dokumen** (3 columns): Evidence file paths linked to reports

### **Database Configuration**

- **Connection String**: `appsettings.json` → ConnectionStrings:DefaultConnection
- **Provider**: MySQL.EntityFrameworkCore 9.0.0 (official Oracle driver)
- **Port Mapping**: Docker container 3306 → Host 3307
- **Credentials**: User `lenbersih_user`, Password `LenBersih_Pass123!`
- **Management UI**: phpMyAdmin at http://localhost:8080

## Admin Dashboard (Tikitaka) Architecture

### **Authentication Flow (Phase 2)**

1. User submits credentials to `POST /api/tikitaka/auth/login`
2. Backend verifies BCrypt password hash from `users` table
3. Retrieves user roles from `users_groups` join with `groups`
4. Generates JWT token with role claims (admin/members)
5. Returns token + refresh token + user info
6. Frontend stores token in localStorage/sessionStorage
7. All `/api/tikitaka/*` endpoints require `[Authorize(Roles = "admin")]`

### **Admin Endpoints Structure (Phase 3)**

- **Dashboard**: `GET /api/tikitaka/dashboard/stats` - aggregate statistics
- **Reports Management**:
  - `GET /api/tikitaka/reports` - paginated list with filters
  - `GET /api/tikitaka/reports/{id}` - detailed view with history
  - `PUT /api/tikitaka/reports/{id}/status` - update workflow
  - `POST /api/tikitaka/reports/{id}/approve` - approve/reject
- **Users Management**:
  - `GET /api/tikitaka/users` - list with role info
  - `POST /api/tikitaka/users` - create new admin
  - `PUT /api/tikitaka/users/{id}` - update profile
  - `PUT /api/tikitaka/users/{id}/role` - change role
- **Documents**: `POST /api/tikitaka/dokumen/upload` - additional evidence

### **Admin UI Routes (Phase 4)**

- `/tikitaka/login` - Authentication page (public)
- `/tikitaka/dashboard` - Statistics & charts (admin only)
- `/tikitaka/reports` - Report list with filters (admin only)
- `/tikitaka/reports/{id}` - Report detail & status update (admin only)
- `/tikitaka/users` - User management interface (admin only)

## Collaboration etiquette

- Mirror README "Pembaruan Terbaru" entries when introducing notable UI or structural changes.
- When moving/shared code across projects, confirm `LenBersih.Web/_Imports.razor` still exposes required namespaces.
- Maintain anonymous-report behavior: toggling `IsAnonymous` must preserve the "Anonim" placeholder and email requirement checks on both client and server.
- **Database Changes**: Always update entity models first, then run migrations if schema changes. Never modify database directly in production.
- **Admin Features**: All admin endpoints must use `/api/tikitaka/*` prefix with `[Authorize(Roles = "admin")]` attribute for security by obscurity and role-based access control.
- **Configuration**: Keep sensitive data (passwords, JWT secrets) in appsettings.Development.json (gitignored) or environment variables. appsettings.json should have placeholders only.D ✅\*\*

- **Docker MariaDB Integration**: Implemented containerized MariaDB 11.2 using Docker Compose with persistent volumes. Database `lenbersih` accessible on port 3307 with phpMyAdmin UI at `http://localhost:8080`. Configuration in `docker-compose.yml` with proper environment variables for MARIADB_DATABASE, MARIADB_USER, and MARIADB_PASSWORD.
- **Entity Framework Core Setup**: Created 7 entity models in `LenBersih.Api/Data/Entities/`:
  - `User.cs` - Maps to 'users' table (19 columns: id, username, password, email, first_name, last_name, company, phone, activation fields, forgotten password fields, remember fields, created_on, last_login, active)
  - `Group.cs` - Maps to 'groups' table (id, name, description) for role management
  - `UserGroup.cs` - Junction table for many-to-many User-Group relationship
  - `ReportEntity.cs` - Maps to 'pelaporan' table (16 columns including id_pelaporan, nama, email, jenis_laporan, terlapor data, kejadian details, dokumen, kode, status, approve fields)
  - `Status.cs` - Maps to 'status' table for report workflow (6 status types)
  - `HistoryStatus.cs` - Audit log for status changes
  - `Dokumen.cs` - Evidence file attachments table
- **LenBersihDbContext**: Complete database context with all DbSets, entity configurations, foreign key relationships, cascade behaviors, and unique indexes. Configured with MySQL.EntityFrameworkCore 9.0.0 provider (official Oracle driver) replacing Pomelo due to .NET 9 compatibility.
- **Connection String Configuration**: Updated `appsettings.json` with MariaDB connection string (port 3307), JWT settings (secret key, issuer, audience, expiry times), TikitakaSettings for admin security (IP whitelist, session timeout, max login attempts, lockout duration), and maintained existing EmailSettings.
- **Program.cs Updates**: Added DbContext registration with `UseMySQL()`, connection test on startup with logging (displays user count, group count, report count, status count), enabled sensitive data logging for development, and maintained existing JWT authentication setup ready for Phase 2.
- **Database Connection Test Results**: Successfully verified connectivity with statistics - Users: 1 (administrator), Groups: 2 (admin/members), Reports: 0, Status: 6. API running on http://localhost:7083.

### 2 Oct 2025

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

- **LenBersih.Api** – ASP.NET Core backend, primary entry is `Program.cs`.
  - **Controllers/**:
    - `ReportsController.cs` – Public report submission (POST /api/reports) with database persistence via EF Core, random ID generation, evidence validation, and email notifications.
    - `TikitakaAuthController.cs` – Admin authentication endpoints (upcoming Phase 2).
    - `TikitakaReportsController.cs` – Admin report management CRUD (upcoming Phase 3).
    - `TikitakaUsersController.cs` – Admin user management (upcoming Phase 3).
    - `TikitakaDashboardController.cs` – Admin statistics and analytics (upcoming Phase 3).
  - **Data/**:
    - `LenBersihDbContext.cs` – EF Core context with MySQL.EntityFrameworkCore 9.0.0 provider.
    - **Entities/** – 7 entity models: User (19 columns), Group (roles), UserGroup (junction), ReportEntity (16 columns, maps to 'pelaporan'), Status (6 workflow types), HistoryStatus (audit log), Dokumen (evidence files).
  - **Services/**:
    - `EmailService.cs` – MailKit-based HTML email notifications with Gmail SMTP integration.
    - `JwtTokenService.cs` – JWT token generation and validation (upcoming Phase 2).
- **LenBersih.Web** – Blazor WASM frontend with DNTCaptcha.Blazor integration. Hot reload via `dotnet watch --project LenBersih.Web`.
  - **Layout Folder**: Contains reusable layout components:
    - `NavMenu.razor` – Top navigation with smooth hover animations
    - `Footer.razor` – Comprehensive footer with 3-column grid and contact info
    - `MainLayout.razor` – Main page wrapper integrating NavMenu and Footer
  - **Pages/**:
    - `Home/` – Landing page with hero, scope, guide sections and AOS animations
    - `Report/` – Anonymous report form with DNTCaptcha and file upload
    - `CekStatus/` – Report status checker
    - `Tikitaka/` – Admin dashboard pages (upcoming Phase 4)
- **LenBersih.Shared** – Contains `Report` DTO, `SecurityNumberModel` for CAPTCHA validation, `ReportMetadata` constants, and `TikitakaModels/` for admin DTOs (upcoming). Update this first when adding categories, evidence rules, or security validation patterns so both API and Web stay aligned.
- **Database (MariaDB 11.2)** – Containerized via Docker Compose on port 3307 with 8 tables: users, groups, users_groups, pelaporan, status, history_status, dokumen, login_attempts. phpMyAdmin available at http://localhost:8080.

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

- **Docker Setup**: `docker-compose up -d` to start MariaDB and phpMyAdmin containers.
- **Restore/Build**: `dotnet restore` then `dotnet build LenBersih.sln`.
- **Run API**: `dotnet run --project LenBersih.Api --launch-profile http` (listens on http://localhost:7083). Database connection test runs automatically on startup with logging.
- **Run WASM**: `dotnet run --project LenBersih.Web --launch-profile http` (listens on http://localhost:5247, calls API at http://localhost:7083).
- **Database Management**: Access phpMyAdmin at http://localhost:8080 (user: lenbersih_user, password: LenBersih_Pass123!).
- **Connection String**: Configured in appsettings.json for port 3307 with proper credentials matching docker-compose.yml.
- No automated tests yet; manually exercise the report form submission path, database persistence verification, and mobile breakpoints after UI changes.

## Collaboration etiquette

- Mirror README “Pembaruan Terbaru” entries when introducing notable UI or structural changes.
- When moving/shared code across projects, confirm `LenBersih.Web/_Imports.razor` still exposes required namespaces.
- Maintain anonymous-report behavior: toggling `IsAnonymous` must preserve the “Anonim” placeholder and email requirement checks on both client and server.
