# Len Bersih - E-Whistleblowing System

## Deskripsi Proyek

Len Bersih ada- **Form Pelaporan**:

- Dropdown kategori, textarea deskripsi, dan submit anonim ke API tanpa login.
- **Sistem Keamanan CAPTCHA**: DNTCaptcha.Blazor dengan Security Number 4-digit, validasi client-side yang kuat, dan desain responsif mobile-first.
- **Notifikasi Email Otomatis**: Sistem email terintegrasi dengan template HTML profesional yang mengirim detail laporan lengkap ke administrator melalui Gmail SMTP.
- Upload bukti pendukung dengan validasi format dan ukuran file.
- Identitas anonim opsional dengan perlindungan data pelapor.ortal web E-Whistleblowing System resmi milik PT Len Industri (Persero). Sistem ini berfungsi sebagai kanal resmi untuk melaporkan dugaan pelanggaran secara anonim dan rahasia, seperti gratifikasi, kecurangan, konflik kepentingan, korupsi, pelanggaran hukum, pelanggaran peraturan perusahaan, pencurian, suap, atau penyalahgunaan jabatan. Laporan dikelola oleh Unit Pengelola Whistleblowing System dengan komitmen penuh kerahasiaan pelapor dan tindak lanjut sesuai tata kelola perusahaan.

Tujuan utama: Mendukung budaya integritas dan tata kelola yang bersih di lingkungan PT Len Industri.

## Pembaruan Terbaru (30 September 2025)

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

- **LenBersih.Shared**: Class library untuk model bersama (e.g., `Report` model dengan fields: Id, Category, Description, DateReported).
- **LenBersih.Api**: ASP.NET Core Web API untuk backend, dengan controller untuk endpoint GET/POST reports (menggunakan in-memory storage sementara) dan EmailService untuk notifikasi otomatis.
- **LenBersih.Web**: Blazor WebAssembly untuk frontend UI modern dan clean, dengan halaman Home (overview sistem) dan ReportForm (form pelaporan anonim).

Tidak ada autentikasi saat ini; pelaporan bersifat anonim. Sistem email terintegrasi untuk notifikasi administrator.

## Persyaratan

- .NET 9 SDK
- Browser modern (untuk Blazor WASM)

## Setup dan Run

1. Clone atau buka proyek di direktori root.
2. Restore packages:
   ```
   dotnet restore
   ```
3. Build solusi:
   ```
   dotnet build
   ```
4. Jalankan API (di terminal terpisah):
   ```
   dotnet run --project LenBersih.Api
   ```
   API akan berjalan di `https://localhost:7000` (HTTPS default).
5. Jalankan Web UI (di terminal lain):
   ```
   dotnet run --project LenBersih.Web
   ```
   UI akan berjalan di `https://localhost:5001` (atau port default Blazor WASM), dan akan memanggil API di `https://localhost:7000`.

Akses UI di browser: `https://localhost:5001` (sesuaikan port jika berbeda). Navigasi ke Home untuk overview, lalu Report untuk form pelaporan.

## Fitur Saat Ini

- **Halaman Home**
  - Hero section dengan CTA jelas dan akses cepat ke form pelaporan.
  - Scope section 5 kolom yang menampilkan ruang lingkup kasus yang dilayani beserta ikon deskriptif.
  - About section yang merinci program dan persyaratan whistleblowing.
  - Guide section berisi langkah-langkah proses WBS yang terstruktur.
  - Animasi scroll AOS dan layout responsif untuk pengalaman yang modern dan intuitif.
- **Form Pelaporan**:
  - Dropdown kategori, textarea deskripsi, dan submit anonim ke API tanpa login.
  - **Sistem Keamanan CAPTCHA**: DNTCaptcha.Blazor dengan Security Number 4-digit, validasi client-side yang kuat, dan desain responsif mobile-first.
  - Upload bukti pendukung dengan validasi format dan ukuran file.
  - Identitas anonim opsional dengan perlindungan data pelapor.
- **API Backend**: Endpoint GET /api/reports (daftar laporan) dan POST /api/reports (buat laporan baru) dengan penyimpanan in-memory dan notifikasi email otomatis menggunakan EmailService.
- **Navigasi Satu Halaman**: Tautan navbar langsung ke hero, scope, guide, dan about untuk memudahkan eksplorasi.

## Rencana Masa Depan

- Integrasi database MariaDB menggunakan Entity Framework Core untuk penyimpanan laporan yang persisten.
- Tambah autentikasi (JWT untuk admin, tetap anonim untuk pelapor).
- Fitur tambahan: Upload bukti/file, notifikasi email untuk laporan baru, dashboard admin untuk pengelola.
- Deployment: Host API di server (e.g., Azure/IIS), UI di static hosting (e.g., Azure Static Web Apps).

## Kontribusi

Silakan buka issue atau pull request untuk kontribusi. Pastikan mengikuti standar coding .NET dan Blazor.
