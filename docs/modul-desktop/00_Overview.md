# 00 - PENGANTAR & ARSITEKTUR SISTEM DESKTOP

Setelah membaca pengenalan dan rekomendasi belajar pada halaman sebelumnya, halaman ini akan membedah secara teknis bagaimana aplikasi desktop ini beroperasi di belakang layar. Memahami arsitektur dan ekosistem teknologi di bawah ini sangat penting sebelum Anda mulai membaca dan mengubah modul-modul kodenya.

---

## 🏢 Overview Proyek Desktop

Aplikasi desktop ini berfungsi sebagai **Panel Admin (Client)** yang digunakan oleh pemilik atau pengelola kos untuk memantau data secara terpusat. Aplikasi ini **tidak memiliki database lokal** (seperti SQL Server atau SQLite di dalam PC), melainkan beroperasi secara penuh dengan menarik dan mengirim data ke **Cloud Backend Server** melalui koneksi internet. 

Sistem ini dirancang dengan antarmuka (UI) yang modern, performa tinggi (*asynchronous*), dan keamanan berbasis Sesi (Session/Cookies).

---

## 🛠️ Tools & Teknologi (Tech Stack)

Aplikasi desktop ini dikembangkan menggunakan ekosistem Microsoft .NET dengan beberapa penyesuaian library modern:

| Kategori | Teknologi yang Digunakan | Fungsi |
|----------|-------------------------|--------|
| **Platform Inti** | C# & .NET Framework 4.8 | Bahasa pemrograman dan runtime utama aplikasi desktop Windows. |
| **Antarmuka (UI)** | Windows Forms (WinForms) | Framework pembuatan jendela aplikasi. |
| **Tema & Komponen** | `Guna.UI2.WinForms` | Library pihak ketiga (NuGet) untuk merombak tampilan tombol, textbox, dan tabel agar terlihat modern (sudut melengkung, bayangan, dll). |
| **Komunikasi Jaringan** | `HttpClient` | Sistem bawaan C# untuk melakukan pemanggilan *REST API* (GET, POST, PUT, DELETE) ke server. |
| **Pengolah Data** | `Newtonsoft.Json` (v13+) | Library untuk mengurai (*parsing*) teks JSON dari server menjadi Objek C# dan sebaliknya. |

---

## 🏗️ Bagaimana Sistem Ini Bekerja? (Arsitektur)

Aplikasi ini menggunakan pola arsitektur **Client-Server**. Berikut adalah diagram alur kerjanya:

```text
┌──────────────────────────────────────────┐
│       DESKTOP ADMIN (CLIENT)             │
│                                          │
│  1. Layar Antarmuka (UI Layer)           │
│     (Form Login, Sidebar, Beranda)       │
│                  ↓                       │
│  2. Pengelola Sesi & API (Logic Layer)   │
│     (ApiClient.cs, Session.cs)           │
└──────────────────────────────────────────┘
                  ↕ (Koneksi Internet / HTTPS)
┌──────────────────────────────────────────┐
│          BACKEND SERVER (API)            │
│  https://rahmatzaw.elarisnoir.my.id/api  │
│                                          │
│  1. API Controller (Menerima Request)    │
│  2. Autentikasi (Verifikasi Akses)       │
│  3. Database (MySQL/PostgreSQL)          │
└──────────────────────────────────────────┘
```

**Penjelasan Sistem:**
1. **Autentikasi Otomatis**: Saat Admin melakukan Login, Server memberikan *Cookie* (Tanda Pengenal). Aplikasi Desktop menyimpan Cookie ini di memori (`CookieContainer`) dan selalu melampirkannya saat meminta data Kamar atau Penyewa agar Server tahu bahwa aplikasi ini punya hak akses.
2. **Asynchronous Processing**: Agar aplikasi tidak *Not Responding* atau nge-hang saat menunggu balasan data dari server yang lambat, semua komunikasi jaringan menggunakan metode `async/await`. 
3. **Navigasi Tunggal (Single-Page Desktop)**: Alih-alih membuka banyak *Window* baru yang menumpuk di Taskbar, aplikasi ini menggunakan satu jendela utama (`MainForm`). Ketika menu diklik, aplikasi hanya menukar panel di tengah layar (menggunakan `UserControl`).

---

## 📋 Daftar Isi Modul (Tutorial Kode)

Pilih modul di bawah ini untuk mulai mempelajari kode sumber aplikasi secara bertahap:

### Konsep Dasar & Infrastruktur
- [01_Autentikasi_Login](01_Autentikasi_Login) - Cara kerja sistem Login dan Penyimpanan Sesi.
- [06_Integrasi_API](06_Integrasi_API) - Cara kerja `HttpClient` dan koneksi ke Server.
- [07_Model_Data](07_Model_Data) - Memahami struktur DTO dan Mapping JSON.

### Antarmuka & Layar (UI)
- [02_Navigasi_Sidebar](02_Navigasi_Sidebar) - Teknik memuat halaman dinamis dalam satu *Window*.
- [03_Dashboard_Beranda](03_Dashboard_Beranda) - Teknik memanggil banyak data API serentak dan menggambar grafik.
- [04_Manajemen_Kamar_List](04_Manajemen_Kamar_List) - Memasukkan data ke Tabel, *Debouncing Search*, dan Filter.
- [05_Edit_Kamar](05_Edit_Kamar) - Menyiapkan dialog modal dan teknik Validasi Data sisi Klien.

### Referensi Tambahan
- [08_QUICK_REFERENCE](08_QUICK_REFERENCE) - Contek kode cepat dan panduan *Debug* saat terjadi error.

---

## 📚 Navigasi Dokumentasi

**← Sebelumnya:** [README_DOKUMENTASI](README_DOKUMENTASI) - Pengenalan Modul & Learning Path  
**Berikutnya →** [01_Autentikasi_Login](01_Autentikasi_Login) - Modul Autentikasi & Login