# 📋 Kost SiguraGura — Dokumentasi Proyek

Aplikasi manajemen kost berbasis **Desktop (Windows Forms C#)** yang terhubung ke backend REST API.

---

## 🗂️ Daftar Modul

| No | Modul | File Utama | Status |
|----|-------|-----------|--------|
| 1 | [Core & Infrastruktur](./01_core.md) | `ApiClient.cs`, `Session.cs` | ✅ Aktif |
| 2 | [Models / Blueprint Data](./02_models.md) | `Kamar.cs`, `Penyewa.cs`, `PaymentResponse.cs` | ✅ Aktif |
| 3 | [Autentikasi (Login)](./03_auth.md) | `Form1.cs`, `LoginRequest.cs` | ✅ Aktif |
| 4 | [Dashboard (Beranda)](./04_beranda.md) | `BerandaPage.cs` | ✅ Aktif |
| 5 | [Manajemen Kamar](./05_kamar.md) | `DataKamar.cs` | ✅ Aktif |
| 6 | [Manajemen Penyewa](./06_penyewa.md) | `DataPenyewa.cs` | ✅ Aktif |
| 7 | [Pembayaran](./07_pembayaran.md) | `PembayaranForm.cs`, `PaymentResponse.cs` | 🚧 Skeleton |
| 8 | [Galeri Foto](./08_gallery.md) | `GalleryForm.cs` | 🚧 Skeleton |
| 9 | [Laporan](./09_laporan.md) | `Report.cs` | 🚧 Skeleton |
| 10 | [Navigasi (Sidebar)](./10_sidebar.md) | `Sidebar.cs` | ✅ Aktif |

---

## 🏗️ Arsitektur Global

```
┌─────────────────────────────────────────────────────────┐
│                     UI Layer (Forms)                    │
│  Form1 (Login) → Sidebar (Shell) → UserControl (Pages) │
└────────────────────┬────────────────────────────────────┘
                     │ event & method call
┌────────────────────▼────────────────────────────────────┐
│                  Service / Logic Layer                  │
│    Fetch API → Deserialize JSON → Manipulate data       │
└────────────────────┬────────────────────────────────────┘
                     │ HTTP Request
┌────────────────────▼────────────────────────────────────┐
│                   Core Infrastructure                   │
│         ApiClient (HttpClient) + Session (State)        │
└────────────────────┬────────────────────────────────────┘
                     │ HTTPS / REST API
┌────────────────────▼────────────────────────────────────┐
│              Backend Server (Laravel / Go)              │
│         https://rahmatzaw.elarisnoir.my.id/api/         │
└─────────────────────────────────────────────────────────┘
```

---

## 📡 Daftar Endpoint API

| Endpoint | Method | Modul | Keterangan |
|----------|--------|-------|-----------|
| `/api/auth/login` | POST | Auth | Login user |
| `/api/kamar` | GET | Kamar | Ambil semua data kamar |
| `/api/tenants` | GET | Penyewa | Ambil semua data penyewa (Admin only) |
| `/api/payments` | GET | Pembayaran | Ambil semua data pembayaran |

---

## 🧩 Teknologi yang Digunakan

| Komponen | Teknologi |
|----------|-----------|
| UI Framework | Windows Forms (.NET Framework) |
| UI Library | Guna.UI2.WinForms |
| HTTP Client | `System.Net.Http.HttpClient` |
| JSON Parser | `Newtonsoft.Json` |
| State Management | Static class (`Session.cs`) |
| Backend | REST API (Laravel / Go) |

---

## 🔄 Flow Login → Dashboard

```
[Program.cs]
    │
    └─► Buka Form1 (Login Page)
              │
              └─► User input username + password
                        │
                        └─► POST /api/auth/login
                                  │
                          ┌───────▼────────┐
                          │ Sukses?        │
                          └───────┬────────┘
                         Ya       │       Tidak
                          │       │         │
                          ▼       │         ▼
                    Simpan ke   (wait)  Tampilkan
                    Session.cs          MessageBox Error
                          │
                          └─► Buka Sidebar (Main Shell)
                                    │
                                    └─► Load BerandaPage (default)
```
