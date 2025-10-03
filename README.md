# Len Bersih - E-Whistleblowing System

## Deskripsi Proyek

Len Bersih ada- **Form Pelaporan**:

- Dropdown kategori, textarea deskripsi, dan submit anonim ke API tanpa login.
- **Sistem Keamanan CAPTCHA**: DNTCaptcha.Blazor dengan Security Number 4-digit, validasi client-side yang kuat, dan desain responsif mobile-first.
- **Notifikasi Email Otomatis**: Sistem email terintegrasi dengan template HTML profesional yang mengirim detail laporan lengkap ke administrator melalui Gmail SMTP.
- Upload bukti pendukung dengan validasi format dan ukuran file.
- Identitas anonim opsional dengan perlindungan data pelapor.ortal web E-Whistleblowing System resmi milik PT Len Industri (Persero). Sistem ini berfungsi sebagai kanal resmi untuk melaporkan dugaan pelanggaran secara anonim dan rahasia, seperti gratifikasi, kecurangan, konflik kepentingan, korupsi, pelanggaran hukum, pelanggaran peraturan perusahaan, pencurian, suap, atau penyalahgunaan jabatan. Laporan dikelola oleh Unit Pengelola Whistleblowing System dengan komitmen penuh kerahasiaan pelapor dan tindak lanjut sesuai tata kelola perusahaan.

Tujuan utama: Mendukung budaya integritas dan tata kelola yang bersih di lingkungan PT Len Industri.

## Pembaruan Terbaru (3 Oktober 2025)

### **Phase 1: Database Layer Setup - COMPLETED ✅**

#### **1. Docker MariaDB Integration**

- Setup containerized MariaDB 11.2 menggunakan Docker Compose dengan persistent volumes
- Database `lenbersih` running pada port 3307 (container 3306 → host 3307)
- phpMyAdmin UI accessible di `http://localhost:8080` untuk database management
- Credentials: User `lenbersih_user`, Password `LenBersih_Pass123!`
- Auto-initialization dengan SQL schema dari `./database/init` directory

#### **2. Entity Framework Core Setup**

**Package Installation:**

- `MySql.EntityFrameworkCore 9.0.0` - Official Oracle MySQL provider untuk .NET 9
- `Microsoft.EntityFrameworkCore.Design 9.0.0` - EF Core design-time tools
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.0` - Identity framework
- `Microsoft.AspNetCore.Authentication.JwtBearer 9.0.0` - JWT authentication
- `BCrypt.Net-Next 4.0.3` - Password hashing compatible dengan existing data

**7 Entity Models Created** (`LenBersih.Api/Data/Entities/`):

1. **User.cs** - Maps to `users` table

   - 19 columns: id, username, password (BCrypt), email, first_name, last_name, company, phone
   - Activation fields: activation_selector, activation_code
   - Password reset: forgotten_password_selector, forgotten_password_code, forgotten_password_time
   - Remember me: remember_selector, remember_code
   - Timestamps: created_on, last_login
   - Status: active (byte)
   - Navigation: `ICollection<UserGroup>`

2. **Group.cs** - Maps to `groups` table

   - Role definitions: id (mediumint), name (admin/members), description
   - Navigation: `ICollection<UserGroup>`

3. **UserGroup.cs** - Junction table for User-Group many-to-many

   - Maps to `users_groups` table: id, user_id (FK), group_id (FK)
   - Configured with unique composite index on (user_id, group_id)
   - Cascade delete on both foreign keys

4. **ReportEntity.cs** - Maps to `pelaporan` table

   - 16 columns untuk whistleblowing reports:
     - Pelapor: id_pelaporan (PK), nama, email, jenis_laporan
     - Terlapor: nama_terlapor, jabatan_terlapor, unit_kerja_terlapor
     - Kejadian: waktu_kejadian, lokasi_kejadian, uraian
     - System: dokumen, kode (random ID), created_date
     - Workflow: id_status (FK), approve, approved_date
   - Navigation: `Status`, `ICollection<Dokumen>`, `ICollection<HistoryStatus>`

5. **Status.cs** - Maps to `status` table

   - Workflow states: id_status (PK), status (6 types)
   - Navigation: `ICollection<ReportEntity>`, `ICollection<HistoryStatus>`

6. **HistoryStatus.cs** - Maps to `history_status` table

   - Audit log: id_history_status (PK), id_status (FK), id_pelaporan (FK), alasan, created_date
   - Navigation: `Status`, `ReportEntity`

7. **Dokumen.cs** - Maps to `dokumen` table
   - Evidence files: id_dokumen (PK), dokumen (filepath), id_pelaporan (FK)
   - Navigation: `ReportEntity`

#### **3. LenBersihDbContext Configuration**

**DbSets Configured:**

```csharp
public DbSet<User> Users { get; set; }
public DbSet<Group> Groups { get; set; }
public DbSet<UserGroup> UserGroups { get; set; }
public DbSet<ReportEntity> Reports { get; set; }
public DbSet<Status> Statuses { get; set; }
public DbSet<HistoryStatus> HistoryStatuses { get; set; }
public DbSet<Dokumen> Dokumen { get; set; }
```

**Entity Configurations:**

- User: Unique indexes pada email, activation_selector, forgotten_password_selector, remember_selector
- UserGroup: Unique composite index (user_id, group_id), cascade delete constraints
- ReportEntity: Foreign key to Status dengan Restrict delete, cascade delete to Dokumen & HistoryStatus
- HistoryStatus: Foreign key to Status dengan Restrict delete

**Connection Configuration:**

```csharp
options.UseMySQL(connectionString)
  .EnableSensitiveDataLogging(Development)
  .EnableDetailedErrors(Development)
```

#### **4. Database Connection Test Results**

**Startup Verification:**

```
🔄 Testing database connection...
✅ Database connection successful!

📊 Database Statistics:
   • Users: 1 (administrator)
   • Groups: 2 (admin, members)
   • Reports: 0 (ready for submissions)
   • Status Types: 6 (workflow configured)

🌐 Server Status:
   Now listening on: http://localhost:7083
   Hosting environment: Development
```

**SQL Queries Executed:**

- `SELECT COUNT(*) FROM users` → 1
- `SELECT COUNT(*) FROM groups` → 2
- `SELECT COUNT(*) FROM pelaporan` → 0
- `SELECT COUNT(*) FROM status` → 6

#### **5. Configuration Files Updated**

**appsettings.json:**

- ConnectionString: `Server=localhost;Port=3307;Database=lenbersih;User=lenbersih_user;Password=LenBersih_Pass123!`
- JwtSettings: SecretKey (32+ chars), Issuer, Audience, ExpiryMinutes (60), RefreshTokenExpiryDays (7)
- TikitakaSettings: IP whitelist, session timeout (30 min), max login attempts (5), lockout duration (15 min)

**Program.cs Updates:**

- DbContext registration dengan `UseMySQL()` provider
- JWT Authentication configuration dengan Bearer scheme
- Authorization policies: `AdminOnly` (requires "admin" role), `MembersOnly` (requires "members" role)
- Database connection test pada startup dengan logging
- Sensitive data logging enabled untuk Development environment

#### **6. Build & Test Results**

```bash
$ dotnet build LenBersih.sln
Build succeeded in 5.8s

$ dotnet run --project LenBersih.Api --launch-profile http
✅ Database connection successful!
📊 Users=1, Groups=2, Reports=0, Status=6
API listening on: http://localhost:7083
```

**Phase 1 Complete!** ✅ Ready untuk Phase 2: Authentication System

### 2 Oktober 2025

- **Sistem ID Laporan Random**: Implementasi generator ID laporan acak yang aman (100-999999) menggunakan `Random.Shared` dengan validasi keunikan. Halaman sukses kini menampilkan ID laporan dengan styling gradient profesional untuk referensi pelapor dalam melacak status.
- **Komponen Footer Profesional**: Penambahan footer komprehensif dengan layout grid 3-kolom responsif yang mencakup informasi brand, navigasi (termasuk link ke form pelaporan dan cek status), dan kontak. Dilengkapi animasi AOS (Animate On Scroll) yang konsisten dengan pola Home.razor (fade-up duration 800ms dengan delay bertahap), hover effects yang smooth, dan desain mobile-first yang elegan dengan gradient background.

### 30 September 2025

- **Sistem Notifikasi Email**: Implementasi lengkap sistem email otomatis menggunakan MailKit dengan template HTML profesional untuk mengirim detail laporan ke administrator melalui Gmail SMTP.
- **Integrasi Gmail**: EmailService dengan autentikasi Gmail App Password, konfigurasi SMTP melalui appsettings, dan penanganan error yang robust untuk pengiriman email.
- **Implementasi DNTCaptcha.Blazor**: Mengganti sistem CAPTCHA matematika lama dengan komponen profesional DNTCaptcha.Blazor v1.2.3 yang menggunakan Security Number 4-digit dengan tampilan responsif dan validasi yang kuat.
- **Model Keamanan**: Penambahan `SecurityNumberModel` dengan validasi Compare attribute untuk memastikan verifikasi CAPTCHA yang akurat sebelum pengiriman laporan.
- **Layout Bootstrap Input-Group**: CAPTCHA kini menggunakan desain input-group dengan ikon shield, canvas responsif, dan tombol refresh yang terintegrasi dengan tema aplikasi.
- **Animasi Navigasi Halus**: Peningkatan NavMenu dengan efek hover underline yang smooth menggunakan CSS pseudo-element dan transisi untuk pengalaman pengguna yang lebih profesional.

### 29 September 2025

- Restrukturisasi **folder Pages**: setiap halaman kini berada di subfolder masing-masing (`Pages/Home`, `Pages/Report`, `Pages/CekStatus`) bersama stylesheet scoped-nya untuk memudahkan pemeliharaan dan konsistensi gaya.

### 26 September 2025

- Penyegaran **halaman Report** dengan latar gradien lembut dan radius ekstra agar form semakin menonjol dan nyaman dibaca.
- Penataan ulang **form pelaporan**: border gradien, padding yang diperkecil, serta jarak antarelemen yang lebih rapat untuk alur input lebih ringkas.
- Perbaikan **responsivitas mobile** pada form dan layout pendukung agar tetap proporsional di layar kecil.

### 25 September 2025

- Penambahan **Scope Section** dengan grid 5 kolom yang menampilkan kategori pelaporan secara lebih seimbang dan responsif.
- Pembuatan **Guide Section** untuk memandu pelapor melalui langkah-langkah proses whistleblowing.
- Integrasi **animasi scroll menggunakan AOS** untuk pengalaman pengguna yang lebih hidup dengan tetap menjaga performa.
- Penataan ulang **navigasi satu halaman** agar tautan hero, scope, guide, dan about lebih mudah diakses.
- Penyempurnaan styling responsif (spacing, tipografi, dan keseimbangan kartu) pada halaman utama.

## Struktur Proyek

Proyek dibangun menggunakan .NET 9 dengan arsitektur tiga lapis:

### **LenBersih.Shared**

Class library untuk model bersama:

- `Report.cs` - Model laporan whistleblowing dengan 16 fields
- `SecurityNumberModel.cs` - Model validasi CAPTCHA
- `ReportMetadata.cs` - Constants untuk kategori dan validasi
- `TikitakaModels/` - DTOs untuk admin authentication (upcoming)

### **LenBersih.Api**

ASP.NET Core Web API backend:

- **Controllers/**
  - `ReportsController.cs` - Public report submission endpoints
  - `TikitakaAuthController.cs` - Admin authentication (upcoming)
  - `TikitakaReportsController.cs` - Admin report management (upcoming)
  - `TikitakaUsersController.cs` - Admin user management (upcoming)
  - `TikitakaDashboardController.cs` - Admin statistics (upcoming)
- **Data/**
  - `LenBersihDbContext.cs` - EF Core database context
  - **Entities/** - 7 entity models:
    - `User.cs` - User accounts (19 columns)
    - `Group.cs` - Roles (admin/members)
    - `UserGroup.cs` - User-role junction table
    - `ReportEntity.cs` - Whistleblowing reports (16 columns)
    - `Status.cs` - Report status workflow (6 types)
    - `HistoryStatus.cs` - Status change audit log
    - `Dokumen.cs` - Evidence file attachments
- **Services/**
  - `EmailService.cs` - MailKit email notifications
  - `JwtTokenService.cs` - JWT authentication (upcoming)

### **LenBersih.Web**

Blazor WebAssembly frontend:

- **Pages/**
  - `Home/` - Landing page dengan hero, scope, guide sections
  - `Report/` - Anonymous report submission form
  - `CekStatus/` - Report status checker
  - `Tikitaka/` - Admin dashboard pages (upcoming)
- **Layout/**
  - `MainLayout.razor` - Main layout wrapper
  - `NavMenu.razor` - Top navigation with smooth animations
  - `Footer.razor` - Comprehensive footer (3-column grid)

### **Database (MariaDB 11.2)**

Containerized via Docker Compose:

- **Connection**: `localhost:3307`
- **Database**: `lenbersih`
- **phpMyAdmin**: `http://localhost:8080`
- **8 Tables**: users, groups, users_groups, pelaporan, status, history_status, dokumen, login_attempts

## Teknologi Stack

### Backend

- **.NET 9** - Latest LTS framework
- **ASP.NET Core Web API** - RESTful API endpoints
- **Entity Framework Core 9.0** - ORM for database operations
- **MySQL.EntityFrameworkCore 9.0.0** - Official MySQL provider
- **MariaDB 11.2** - Open-source database (Docker)
- **MailKit 4.14.0** - Email notifications
- **BCrypt.Net-Next 4.0.3** - Password hashing
- **JWT Bearer Authentication** - Secure admin access

### Frontend

- **Blazor WebAssembly (WASM)** - SPA framework
- **Bootstrap 5** - Responsive UI components
- **AOS (Animate On Scroll)** - Scroll animations
- **DNTCaptcha.Blazor 1.2.3** - Security number CAPTCHA

### DevOps

- **Docker & Docker Compose** - Containerization
- **phpMyAdmin** - Database management UI
- **Git** - Version control

## Persyaratan

- .NET 9 SDK
- Docker Desktop (untuk MariaDB container)
- Browser modern (untuk Blazor WASM)

## Setup dan Run

### 1. Setup Database (Docker)

```bash
# Start MariaDB dan phpMyAdmin containers
docker-compose up -d

# Verifikasi containers running
docker ps

# Akses phpMyAdmin (opsional)
# URL: http://localhost:8080
# User: lenbersih_user
# Password: LenBersih_Pass123!
```

### 2. Clone & Restore Packages

```bash
# Clone repository
git clone <repository-url>
cd Len-Bersih

# Restore NuGet packages
dotnet restore
```

### 3. Configuration

File `appsettings.json` sudah dikonfigurasi dengan:

- **Connection String**: MariaDB di port 3307
- **JWT Settings**: Secret key untuk authentication
- **Email Settings**: Gmail SMTP untuk notifikasi
- **Tikitaka Settings**: Admin security configuration

### 4. Build Solusi

```bash
dotnet build LenBersih.sln
```

### 5. Jalankan API

```bash
# Terminal 1: API Backend
dotnet run --project LenBersih.Api --launch-profile http

# API akan berjalan di http://localhost:7083
# Database connection test akan dijalankan otomatis
```

Output yang diharapkan:

```
✅ Database connection successful!
📊 Database Statistics:
   • Users: 1
   • Groups: 2
   • Reports: 0
   • Status Types: 6
Now listening on: http://localhost:7083
```

### 6. Jalankan Web UI

```bash
# Terminal 2: Blazor WASM Frontend
dotnet run --project LenBersih.Web --launch-profile http

# UI akan berjalan di http://localhost:5247
```

### 7. Akses Aplikasi

- **Public Website**: `http://localhost:5247`
- **API Endpoints**: `http://localhost:7083/api`
- **phpMyAdmin**: `http://localhost:8080`
- **Admin Dashboard**: `http://localhost:5247/tikitaka` (upcoming)

### Default Admin User

- **Email**: admin@admin.com
- **Username**: administrator
- **Password**: (cek database untuk hash BCrypt)
- **Role**: admin

## Fitur Saat Ini

### **Public Features (Implemented ✅)**

#### **Halaman Home**

- Hero section dengan CTA jelas dan akses cepat ke form pelaporan
- Scope section 5 kolom yang menampilkan ruang lingkup kasus yang dilayani
- About section yang merinci program dan persyaratan whistleblowing
- Guide section berisi langkah-langkah proses WBS yang terstruktur
- Animasi scroll AOS dan layout responsif mobile-first
- Footer 3-kolom dengan navigasi lengkap dan kontak informasi

#### **Form Pelaporan Anonim**

- Dropdown kategori pelanggaran (7 jenis)
- Textarea deskripsi kejadian dengan validasi
- Data terlapor (nama, jabatan, unit kerja)
- Waktu dan lokasi kejadian
- Upload bukti pendukung (validasi format dan ukuran)
- **CAPTCHA Security**: DNTCaptcha.Blazor 4-digit Security Number
- **Random Report ID**: Generator ID acak (100-999999) dengan display profesional
- **Email Notification**: Notifikasi otomatis ke admin via Gmail SMTP
- Opsi laporan anonim dengan perlindungan identitas

#### **API Backend**

- `GET /api/reports` - List all reports
- `POST /api/reports` - Submit new report with email notification
- Random ID generation dengan uniqueness validation
- Evidence file validation (format & size)
- Database persistence via Entity Framework Core

### **Admin Features (Upcoming 🔄)**

#### **Phase 2: Authentication System** (Next)

- JWT-based authentication untuk admin
- BCrypt password verification
- Refresh token mechanism
- Role-based authorization (admin/members)
- Login endpoint: `POST /api/tikitaka/auth/login`

#### **Phase 3: Admin Dashboard** (Planned)

- `GET /api/tikitaka/dashboard/stats` - Statistics & analytics
- `GET /api/tikitaka/reports` - Report list dengan filters dan pagination
- `GET /api/tikitaka/reports/{id}` - Report detail view
- `PUT /api/tikitaka/reports/{id}/status` - Update workflow status
- `POST /api/tikitaka/reports/{id}/approve` - Approve/reject reports
- `GET /api/tikitaka/users` - User management
- `POST /api/tikitaka/dokumen/upload` - Upload additional evidence

#### **Phase 4: Admin UI** (Planned)

- `/tikitaka/login` - Admin authentication page
- `/tikitaka/dashboard` - Statistics dashboard
- `/tikitaka/reports` - Report list dengan search & filter
- `/tikitaka/reports/{id}` - Report detail dengan status update
- `/tikitaka/users` - User management interface

## Database Schema

### **users** (Authentication & Profile)

```sql
CREATE TABLE users (
  id INT(11) UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
  ip_address VARCHAR(45) NOT NULL,
  username VARCHAR(100) DEFAULT NULL,
  password VARCHAR(255) NOT NULL,  -- BCrypt hash ($2y$10$...)
  email VARCHAR(254) NOT NULL UNIQUE,
  activation_selector VARCHAR(255) DEFAULT NULL UNIQUE,
  activation_code VARCHAR(255) DEFAULT NULL,
  forgotten_password_selector VARCHAR(255) DEFAULT NULL UNIQUE,
  forgotten_password_code VARCHAR(255) DEFAULT NULL,
  forgotten_password_time INT(11) UNSIGNED DEFAULT NULL,
  remember_selector VARCHAR(255) DEFAULT NULL UNIQUE,
  remember_code VARCHAR(255) DEFAULT NULL,
  created_on INT(11) UNSIGNED NOT NULL,  -- Unix timestamp
  last_login INT(11) UNSIGNED DEFAULT NULL,  -- Unix timestamp
  active TINYINT(1) UNSIGNED DEFAULT NULL,  -- 1=active, 0=inactive
  first_name VARCHAR(50) DEFAULT NULL,
  last_name VARCHAR(50) DEFAULT NULL,
  company VARCHAR(100) DEFAULT NULL,
  phone VARCHAR(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

**Purpose**: System users dengan authentication credentials dan profile information  
**Indexes**: `uc_email`, `uc_activation_selector`, `uc_forgotten_password_selector`, `uc_remember_selector`  
**Default User**: `administrator` / `admin@admin.com` / password: `admin123` (BCrypt hashed)

### **groups** (Roles)

```sql
CREATE TABLE groups (
  id MEDIUMINT(8) UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(20) NOT NULL,  -- 'admin', 'members'
  description VARCHAR(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

**Purpose**: Role definitions untuk authorization  
**Data**: (1, 'admin', 'Administrator'), (2, 'members', 'General User')

### **users_groups** (User-Role Junction)

```sql
CREATE TABLE users_groups (
  id INT(11) UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
  user_id INT(11) UNSIGNED NOT NULL,
  group_id MEDIUMINT(8) UNSIGNED NOT NULL,
  UNIQUE KEY uc_users_groups (user_id, group_id),
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  FOREIGN KEY (group_id) REFERENCES groups(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

**Purpose**: Many-to-many relationship between users and groups  
**Constraints**: Composite unique index (user_id, group_id), cascade delete

### **pelaporan** (Reports)

```sql
CREATE TABLE pelaporan (
  id_pelaporan INT(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  nama VARCHAR(150) NOT NULL,  -- Reporter name (or 'Anonim')
  email VARCHAR(150) NOT NULL,
  jenis_laporan VARCHAR(150) NOT NULL,  -- Category of violation
  nama_terlapor VARCHAR(150) NOT NULL,  -- Reported person name
  jabatan_terlapor VARCHAR(150) NOT NULL,  -- Reported person position
  unit_kerja_terlapor VARCHAR(150) NOT NULL,  -- Reported person unit
  waktu_kejadian DATE NOT NULL,  -- Incident date
  lokasi_kejadian VARCHAR(150) NOT NULL,  -- Incident location
  uraian TEXT NOT NULL,  -- Detailed description
  dokumen VARCHAR(255) NOT NULL,  -- Evidence file path
  kode VARCHAR(5) NOT NULL,  -- Random report code (100-999999)
  created_date DATE NOT NULL,
  id_status INT(11) NOT NULL DEFAULT 1,
  approve INT(11) NOT NULL DEFAULT 0,  -- 0=pending, 1=approved, 2=rejected
  approved_date DATETIME NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
```

**Purpose**: Core whistleblowing report submissions  
**Indexes**: `idx_kode`, `idx_created_date`, `idx_email`, `idx_nama_terlapor`

### **status** (Workflow States)

```sql
CREATE TABLE status (
  id_status INT(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  status VARCHAR(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
```

**Purpose**: Report workflow status definitions  
**Data** (6 workflow stages):

1. `Validasi Laporan Tim Sekretariat DITERIMA`
2. `Validasi Laporan Tim Sekretariat DITOLAK`
3. `Analisa Tim Pengawas DITINDAK LANJUTI`
4. `Analisa Tim Pengawas DIHENTIKAN`
5. `Audit Tim Investigasi TERBUKTI`
6. `Audit Tim Investigasi TIDAK TERBUKTI`

### **history_status** (Audit Log)

```sql
CREATE TABLE history_status (
  id_history_status INT(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  id_status INT(11) NOT NULL,
  id_pelaporan INT(11) NOT NULL,
  alasan TEXT NOT NULL,  -- Reason for status change
  created_date DATE NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
```

**Purpose**: Audit trail untuk semua status changes dengan reasoning  
**Indexes**: `idx_id_pelaporan`, `idx_id_status`

### **dokumen** (Evidence Files)

```sql
CREATE TABLE dokumen (
  id_dokumen INT(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  dokumen VARCHAR(255) NOT NULL,  -- File path/name
  id_pelaporan INT(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
```

**Purpose**: Multiple evidence file attachments per report  
**Indexes**: `idx_id_pelaporan`

### **login_attempts** (Security)

```sql
CREATE TABLE login_attempts (
  id INT(11) UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
  ip_address VARCHAR(45) NOT NULL,
  login VARCHAR(100) NOT NULL,  -- Username/email attempted
  time INT(11) UNSIGNED DEFAULT NULL  -- Unix timestamp
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

**Purpose**: Track failed login attempts untuk brute force protection  
**Indexes**: `idx_ip_address`, `idx_time`

### **Entity Relationships**

```
users (1) ←→ (N) users_groups (N) ←→ (1) groups
pelaporan (N) → (1) status
pelaporan (1) → (N) dokumen
pelaporan (1) → (N) history_status
status (1) → (N) history_status
```

## Configuration Files

### **appsettings.json** (Production)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3307;Database=lenbersih;..."
  },
  "JwtSettings": {
    "SecretKey": "LenBersih-Super-Secret-JWT-Key...",
    "Issuer": "LenBersih.Api",
    "Audience": "LenBersih.Web",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "TikitakaSettings": {
    "EnableIpWhitelist": false,
    "SessionTimeoutMinutes": 30,
    "MaxLoginAttempts": 5,
    "LockoutDurationMinutes": 15
  }
}
```

### **docker-compose.yml**

```yaml
services:
  mariadb:
    image: mariadb:11.2
    ports: ["3307:3306"]
    environment:
      MARIADB_DATABASE: lenbersih
      MARIADB_USER: lenbersih_user
      MARIADB_PASSWORD: LenBersih_Pass123!

  phpmyadmin:
    image: phpmyadmin:latest
    ports: ["8080:80"]
```

## Rencana Masa Depan

### **Phase 2: Authentication System** (In Progress)

- ✅ JWT configuration ready
- ✅ Database entities & relationships configured
- 🔄 Implement TikitakaAuthController
- 🔄 Create authentication DTOs
- 🔄 JWT token service dengan refresh mechanism

### **Phase 3: Admin Dashboard API**

- Dashboard statistics endpoint
- Report management CRUD operations
- User management endpoints
- File upload untuk evidence tambahan
- Status workflow management

### **Phase 4: Admin UI (Blazor)**

- Admin login page dengan JWT authentication
- Dashboard dengan charts & statistics
- Report list dengan advanced filters
- Report detail view dengan status update
- User management interface

### **Phase 5: Deployment**

- **API**: Azure App Service / IIS Server
- **Web UI**: Azure Static Web Apps / CDN
- **Database**: Azure Database for MySQL / On-premise MariaDB
- **CI/CD**: GitHub Actions untuk automated deployment

## Kontribusi

Silakan buka issue atau pull request untuk kontribusi. Pastikan mengikuti standar coding .NET dan Blazor.
