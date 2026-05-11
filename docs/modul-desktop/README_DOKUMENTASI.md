# 📚 README DOKUMENTASI MODUL DESKTOP

Selamat datang di pusat dokumentasi teknis untuk **Kost_SiguraGura (Modul Desktop)**! File ini berfungsi sebagai gerbang utama sebelum Anda mempelajari kode sumber dan alur logika sistem.

---

## 🎯 Pengenalan Aplikasi & Tujuan

**Kost_SiguraGura (Desktop)** adalah aplikasi antarmuka administrator untuk manajemen kos-kosan. Aplikasi ini digunakan oleh pengelola kos untuk memantau data operasional bisnis mereka dengan antarmuka yang bersahabat dan interaktif.

**Tujuan Aplikasi:**
1. **Pusat Kendali Bisnis**: Mengelola ketersediaan kamar, pencatatan data penyewa, dan riwayat pembayaran melalui satu pintu (*All-in-One*).
2. **Monitoring Terpusat**: Menyajikan Dashboard analitik untuk melihat pendapatan, jumlah penyewa aktif, dan tren okupansi secara *real-time*.
3. **Koneksi *Cloud***: Bertindak sebagai *Front-End* klien yang berkomunikasi secara langsung ke *Backend Server* melalui REST API, sehingga data aman dan selalu tersinkronisasi.

---

## 🛠️ Tools & Teknologi yang Digunakan

Aplikasi desktop ini dibangun menggunakan standar tumpukan teknologi modern untuk C# sebagai berikut:
- **Bahasa Pemrograman**: C# (C-Sharp).
- **Framework Desktop**: .NET Framework 4.8 (Windows Forms).
- **UI & Styling Library**: Guna.UI2.WinForms (Library pihak ketiga untuk membuat tampilan form, tombol, dan tabel terlihat mewah dan *flat-design*).
- **Komunikasi Jaringan**: `System.Net.Http.HttpClient` (Untuk mengeksekusi *request* data HTTP dari dan ke Backend).
- **Parsing Data**: `Newtonsoft.Json` (Untuk mengonversi teks data JSON dari API menjadi Objek *Class* di dalam C#).
- **API Backend**: Terkoneksi pada REST API pusat di URL `https://rahmatzaw.elarisnoir.my.id/api`.

---

## 📑 Daftar Isi Modul (Table of Contents)

Seluruh dokumentasi telah kami susun dan refaktor ke dalam format **Tutorial Step-by-Step**. Berikut adalah daftar modul tutorial yang bisa Anda baca:

- [00_INDEX](00_INDEX) - Pengantar Struktur Folder & Navigasi Umum
- [01_Autentikasi_Login](01_Autentikasi_Login) - Tutorial Login & Manajemen Sesi (*HTTP Cookies*)
- [02_Navigasi_Sidebar](02_Navigasi_Sidebar) - Tutorial Membuat Navigasi Menu Dinamis
- [03_Dashboard_Beranda](03_Dashboard_Beranda) - Tutorial Membuat Dashboard Analitik & Paralelisasi Data
- [04_Manajemen_Kamar_List](04_Manajemen_Kamar_List) - Tutorial Menampilkan Tabel Data Kamar & Logika Filter
- [05_Edit_Kamar](05_Edit_Kamar) - Tutorial Membuat Dialog Form Edit & Validasi Data Sisi Klien
- [06_Integrasi_API](06_Integrasi_API) - Tutorial Konfigurasi Singleton `ApiClient` & Operasi CRUD (REST API)
- [07_Model_Data](07_Model_Data) - Tutorial Pembuatan Struktur Objek (DTO) & Anotasi JSON
- [08_QUICK_REFERENCE](08_QUICK_REFERENCE) - Panduan Pintas, Kode *Snippets*, dan Pemecahan Masalah Error

---

## 🎓 Rekomendasi Belajar (Learning Path)

Jika Anda adalah developer yang baru bergabung untuk memelihara (*maintenance*) atau menambah fitur di dalam proyek ini, kami sangat menyarankan Anda membaca dokumentasi dengan urutan tahapan berikut agar alur logika dapat ditangkap dengan sempurna:

### 1. Tahap Dasar (Wajib)
Mulailah dari jantung aplikasinya.
- Pahami konsep model struktur datanya di **[07_Model_Data](07_Model_Data)**.
- Pahami cara kerja aplikasi menarik data dari server luar di **[06_Integrasi_API](06_Integrasi_API)**.
- Pahami alur masuk (Autentikasi) dan penyimpanan riwayat masuk di **[01_Autentikasi_Login](01_Autentikasi_Login)**.

### 2. Tahap Menengah (UI & Interaksi Pengguna)
Pahami bagaimana antarmuka (layar) digambar.
- Pelajari cara berpindah menu di layar **[02_Navigasi_Sidebar](02_Navigasi_Sidebar)**.
- Pahami proses menampilkan data dalam bentuk kolom tabel (DataGrid) dan teknik pemilahannya (Filter) di **[04_Manajemen_Kamar_List](04_Manajemen_Kamar_List)**.
- Pelajari teknik pengambilan input pengguna melalui form dan validasinya di **[05_Edit_Kamar](05_Edit_Kamar)**.

### 3. Tahap Lanjutan (Performa & Optimasi)
- Pelajari bagaimana cara menembakkan banyak request data sekaligus secara efisien (*Parallel Async*) tanpa membuat antarmuka membeku (nge-*hang*) di modul **[03_Dashboard_Beranda](03_Dashboard_Beranda)**.
- Jangan lupa meletakkan halaman **[08_QUICK_REFERENCE](08_QUICK_REFERENCE)** di menu penanda (Bookmark) Anda untuk persiapan mencari referensi *error-solving* cepat.

---

## 📚 Navigasi Dokumentasi

Silakan tekan tombol **Berikutnya** di bawah ini untuk memulai pembelajaran dokumentasi Anda dari halaman dasar!

**Berikutnya →** [00_INDEX](00_INDEX) - Pengantar Struktur & Navigasi Umum
