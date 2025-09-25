# Len Bersih - E-Whistleblowing System

## Deskripsi Proyek

Len Bersih adalah portal web E-Whistleblowing System resmi milik PT Len Industri (Persero). Sistem ini berfungsi sebagai kanal resmi untuk melaporkan dugaan pelanggaran secara anonim dan rahasia, seperti gratifikasi, kecurangan, konflik kepentingan, korupsi, pelanggaran hukum, pelanggaran peraturan perusahaan, pencurian, suap, atau penyalahgunaan jabatan. Laporan dikelola oleh Unit Pengelola Whistleblowing System dengan komitmen penuh kerahasiaan pelapor dan tindak lanjut sesuai tata kelola perusahaan.

Tujuan utama: Mendukung budaya integritas dan tata kelola yang bersih di lingkungan PT Len Industri.

## Struktur Proyek

Proyek dibangun menggunakan .NET 9 dengan arsitektur tiga lapis:

- **LenBersih.Shared**: Class library untuk model bersama (e.g., `Report` model dengan fields: Id, Category, Description, DateReported).
- **LenBersih.Api**: ASP.NET Core Web API untuk backend, dengan stub controller untuk endpoint GET/POST reports (menggunakan in-memory storage sementara).
- **LenBersih.Web**: Blazor WebAssembly untuk frontend UI modern dan clean, dengan halaman Home (overview sistem) dan ReportForm (form pelaporan anonim).

Tidak ada autentikasi saat ini; pelaporan bersifat anonim. Tidak ada integrasi email atau upload file.

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

- Halaman Home: Deskripsi sistem whistleblowing, daftar kategori pelanggaran, dan tombol untuk mulai laporan.
- Form Pelaporan: Dropdown kategori, textarea deskripsi, submit ke API (anonim, tanpa login).
- API Stubs: GET /api/reports (daftar laporan), POST /api/reports (buat laporan baru).

## Rencana Masa Depan

- Integrasi database MariaDB menggunakan Entity Framework Core untuk penyimpanan laporan yang persisten.
- Tambah autentikasi (JWT untuk admin, tetap anonim untuk pelapor).
- Fitur tambahan: Upload bukti/file, notifikasi email untuk laporan baru, dashboard admin untuk pengelola.
- Deployment: Host API di server (e.g., Azure/IIS), UI di static hosting (e.g., Azure Static Web Apps).

## Kontribusi

Silakan buka issue atau pull request untuk kontribusi. Pastikan mengikuti standar coding .NET dan Blazor.
