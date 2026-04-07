# 🔧 Modul 1 — Core & Infrastruktur

Modul ini adalah **pondasi utama** aplikasi. Semua modul lain bergantung pada komponen di sini.

---

## 📁 File yang Terlibat

| File | Tipe | Fungsi |
|------|------|--------|
| `ApiClient.cs` | Static Class | Menyediakan satu instance `HttpClient` global yang membawa sesi login |
| `Session.cs` | Static Class | Menyimpan data user yang sedang login (state global) |

---

## 1.1 — ApiClient.cs

### 🎯 Tujuan
Menyediakan **satu instance `HttpClient`** yang dipakai oleh seluruh form dan UserControl di aplikasi.

### ❓ Mengapa Tidak Buat `new HttpClient()` di Setiap Form?
Setiap `HttpClient` baru = sesi baru = cookie login hilang. Dengan satu instance bersama, cookie (sesi login) dari server akan otomatis terbawa di semua request berikutnya.

### 📄 Kode

```csharp
// File: ApiClient.cs
using System.Net;
using System.Net.Http;

namespace Kost_SiguraGura
{
    public static class ApiClient
    {
        // Container untuk menyimpan 'izin' / cookie dari server setelah login
        private static readonly CookieContainer cookieContainer = new CookieContainer();
        
        // Handler yang menghubungkan HttpClient dengan CookieContainer
        private static readonly HttpClientHandler handler = new HttpClientHandler()
        {
            CookieContainer = cookieContainer
        };

        // Gunakan satu Client ini untuk SEMUA Form dan UserControl
        public static readonly HttpClient Client = new HttpClient(handler);
    }
}
```

### 🔁 Cara Penggunaan di Form Lain

```csharp
// Di BerandaPage.cs, DataPenyewa.cs, dll — cukup panggil:
HttpResponseMessage response = await ApiClient.Client.GetAsync(url);

// BUKAN ini (salah — session hilang):
// HttpClient client = new HttpClient();
// HttpResponseMessage response = await client.GetAsync(url);
```

### 📌 Alur Kerja Cookie

```
Form1 (Login)
    │
    └─► ApiClient.Client.PostAsync("/api/auth/login") 
              │
              └─► Server kirim cookie sesi → otomatis disimpan di CookieContainer
                          │
                          ▼
              Semua request berikutnya → cookie terbawa otomatis
              (BerandaPage, DataKamar, DataPenyewa, dll)
```

---

## 1.2 — Session.cs

### 🎯 Tujuan
Menyimpan **data user yang sedang login** agar bisa diakses dari mana saja dalam aplikasi tanpa perlu passing parameter antar form.

### 📄 Kode

```csharp
// File: Session.cs
namespace Kost_SiguraGura
{
    internal class Session
    {
        // ID numerik user dari database server
        public static long UserId { get; set; }

        // Role user: "admin" atau "tenant" (menentukan akses fitur)
        public static string UserRole { get; set; }

        // Username yang ditampilkan di sidebar
        public static string Username { get; set; }

        // Token autentikasi (Bearer Token jika API pakai JWT)
        public static string Token { get; set; }
    }
}
```

### 🔁 Cara Penggunaan

```csharp
// ✅ Menyimpan data setelah login berhasil (di Form1.cs)
Session.UserId   = (long)(userData["id"] ?? 0);
Session.UserRole = userData["role"]?.ToString() ?? "admin";
Session.Username = userData["username"]?.ToString() ?? txtUsername.Text;

// ✅ Membaca data di form lain (contoh di DataPenyewa.cs)
if (Session.UserRole?.ToLower() == "admin")
{
    LoadDataPenyewa();  // Hanya admin yang bisa load data penyewa
}
else
{
    MessageBox.Show("Akses Ditolak! Role Anda: " + Session.UserRole);
}

// ✅ Menampilkan nama user di Sidebar
lblName.Text = Session.Username;
```

### ⚙️ Role-Based Access

| `Session.UserRole` | Akses |
|-------------------|-------|
| `"admin"` | Semua fitur (Kamar, Penyewa, Pembayaran, Laporan) |
| `"tenant"` | Terbatas (hanya bisa lihat kamar & pembayaran miliknya) |

---

## 📌 Catatan Pengembangan Selanjutnya

> Tambahkan `AppConstants.cs` untuk menyimpan base URL API secara terpusat, agar tidak hardcode di setiap file:

```csharp
// File yang direkomendasikan: AppConstants.cs
namespace Kost_SiguraGura
{
    public static class AppConstants
    {
        public const string BaseUrl = "https://rahmatzaw.elarisnoir.my.id/api";

        // Endpoint lengkap
        public const string LoginUrl      = BaseUrl + "/auth/login";
        public const string KamarUrl      = BaseUrl + "/kamar";
        public const string TenantsUrl    = BaseUrl + "/tenants";
        public const string PaymentsUrl   = BaseUrl + "/payments";
    }
}
```
