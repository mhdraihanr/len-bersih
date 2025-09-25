# Len Bersih - E-Whistleblowing System

## Deskripsi Proyek

Len Bersih adalah portal web E-Whistleblowing System resmi milik PT Len Industri (Persero). Sistem ini berfungsi sebagai kanal resmi untuk melaporkan dugaan pelanggaran secara anonim dan rahasia, seperti gratifikasi, kecurangan, konflik kepentingan, korupsi, pelanggaran hukum, pelanggaran peraturan perusahaan, pencurian, suap, atau penyalahgunaan jabatan. Laporan dikelola oleh Unit Pengelola Whistleblowing System dengan komitmen penuh kerahasiaan pelapor dan tindak lanjut sesuai tata kelola perusahaan.

Tujuan utama: Mendukung budaya integritas dan tata kelola yang bersih di lingkungan PT Len Industri.

## Pembaruan Terbaru (25 September 2025)

- Penambahan **Scope Section** dengan grid 5 kolom yang menampilkan kategori pelaporan secara lebih seimbang dan responsif.
- Pembuatan **Guide Section** untuk memandu pelapor melalui langkah-langkah proses whistleblowing.
- Integrasi **animasi scroll menggunakan AOS** untuk pengalaman pengguna yang lebih hidup dengan tetap menjaga performa.
- Penataan ulang **navigasi satu halaman** agar tautan hero, scope, guide, dan about lebih mudah diakses.
- Penyempurnaan styling responsif (spacing, tipografi, dan keseimbangan kartu) pada halaman utama.

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

- **Halaman Home**
  - Hero section dengan CTA jelas dan akses cepat ke form pelaporan.
  - Scope section 5 kolom yang menampilkan ruang lingkup kasus yang dilayani beserta ikon deskriptif.
  - About section yang merinci program dan persyaratan whistleblowing.
  - Guide section berisi langkah-langkah proses WBS yang terstruktur.
  - Animasi scroll AOS dan layout responsif untuk pengalaman yang modern dan intuitif.
- **Form Pelaporan**: Dropdown kategori, textarea deskripsi, dan submit anonim ke API tanpa login.
- **API Stubs**: Endpoint GET /api/reports (daftar laporan) dan POST /api/reports (buat laporan baru) menggunakan penyimpanan sementara in-memory.
- **Navigasi Satu Halaman**: Tautan navbar langsung ke hero, scope, guide, dan about untuk memudahkan eksplorasi.

## Rencana Masa Depan

- Integrasi database MariaDB menggunakan Entity Framework Core untuk penyimpanan laporan yang persisten.
- Tambah autentikasi (JWT untuk admin, tetap anonim untuk pelapor).
- Fitur tambahan: Upload bukti/file, notifikasi email untuk laporan baru, dashboard admin untuk pengelola.
- Deployment: Host API di server (e.g., Azure/IIS), UI di static hosting (e.g., Azure Static Web Apps).

## Kontribusi

Silakan buka issue atau pull request untuk kontribusi. Pastikan mengikuti standar coding .NET dan Blazor.
