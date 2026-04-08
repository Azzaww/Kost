# 📦 Modul 2 — Models / Blueprint Data

Model adalah **kelas C# yang strukturnya identik dengan data JSON** yang dikirim oleh server. Saat JSON di-*deserialize*, data otomatis masuk ke properti-properti ini.

---

## 📁 File yang Terlibat

| File | Kelas di Dalamnya | Endpoint API |
|------|-------------------|-------------|
| `Kamar.cs` | `Kamar`, `KamarResponse` | `GET /api/kamar` |
| `Penyewa.cs` | `Penyewa`, `PenyewaResponse` | `GET /api/tenants` |
| `PaymentResponse.cs` | `Pembayaran`, `PaymentResponse` | `GET /api/payments` |
| `LoginRequest.cs` | `LoginRequest` | `POST /api/auth/login` |

---

## 2.1 — Kamar.cs

### 🎯 Tujuan
Blueprint untuk data satu unit kamar kost yang diterima dari server.

### 📄 Kode

```csharp
// File: Kamar.cs
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;

namespace Kost_SiguraGura
{
    namespace Kost_SiguraGura  // ⚠️ Catatan: namespace nested ini bisa disederhanakan
    {
        public class Kamar
        {
            // "id" dari JSON → properti NO
            [JsonProperty("id")]
            public int NO { get; set; }

            // URL gambar kamar dari server (string teks)
            [JsonProperty("image_url")]
            public string ThumbnailUrl { get; set; }

            // [JsonIgnore] → properti ini TIDAK di-mapping dari JSON.
            // Diisi manual setelah gambar di-download dari ThumbnailUrl.
            [JsonIgnore]
            public Image THUMBNAIL { get; set; }

            [JsonProperty("nomor_kamar")]
            public string ROOM { get; set; }

            [JsonProperty("tipe_kamar")]
            public string TYPE { get; set; }

            [JsonProperty("harga_per_bulan")]
            public decimal PRICE { get; set; }

            [JsonProperty("floor")]
            public int FLOOR { get; set; }

            // Nilai: "Tersedia", "Penuh", atau "Perbaikan"
            [JsonProperty("status")]
            public string STATUS { get; set; }
        }
    }

    // Wrapper jika server mengirim JSON berbentuk: { "kamars": [...] }
    public class KamarResponse
    {
        [JsonProperty("kamars")]
        public List<Kamar> Kamars { get; set; }
    }
}
```

### 📨 Contoh JSON dari Server

```json
[
  {
    "id": 1,
    "image_url": "https://example.com/kamar1.jpg",
    "nomor_kamar": "A-101",
    "tipe_kamar": "Standard",
    "harga_per_bulan": 800000,
    "floor": 1,
    "status": "Tersedia"
  },
  {
    "id": 2,
    "nomor_kamar": "B-201",
    "tipe_kamar": "Premium",
    "harga_per_bulan": 1500000,
    "floor": 2,
    "status": "Penuh"
  }
]
```

### 🔄 Proses Deserialisasi

```csharp
// Teks JSON mentah → List objek Kamar C#
string jsonResponse = await response.Content.ReadAsStringAsync();
var listData = JsonConvert.DeserializeObject<List<Kamar>>(jsonResponse);

// Setelah dapat list, download gambarnya satu per satu
foreach (var kamar in listData)
{
    if (!string.IsNullOrEmpty(kamar.ThumbnailUrl))
    {
        byte[] imageBytes = await client.GetByteArrayAsync(kamar.ThumbnailUrl);
        using (var ms = new System.IO.MemoryStream(imageBytes))
        {
            kamar.THUMBNAIL = new Bitmap(Image.FromStream(ms)); // isi properti Image
        }
    }
}
```

---

## 2.2 — Penyewa.cs

### 🎯 Tujuan
Blueprint untuk data seorang penyewa / tenant kost.

### 📄 Kode

```csharp
// File: Penyewa.cs
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kost_SiguraGura
{
    public class Penyewa
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("nama_lengkap")]
        public string NAMA_LENGKAP { get; set; }

        // Nomor Induk Kependudukan (KTP)
        [JsonProperty("nik")]
        public string NIK { get; set; }

        // Field "email" dari JSON → properti KONTAK
        [JsonProperty("email")]
        public string KONTAK { get; set; }

        // Role user: "admin" atau "tenant"
        [JsonProperty("role")]
        public string PERAN { get; set; }
    }

    // Wrapper — server mungkin mengirim dalam dua format berbeda:
    public class PenyewaResponse
    {
        // Format 1: { "tenants": [...] }  ← sesuai URL /api/tenants
        [JsonProperty("tenants")]
        public List<Penyewa> Tenants { get; set; }

        // Format 2: { "penyewas": [...] }  ← kemungkinan format lain
        [JsonProperty("penyewas")]
        public List<Penyewa> Penyewas { get; set; }
    }
}
```

### 📨 Contoh JSON dari Server

```json
{
  "tenants": [
    {
      "id": 1,
      "nama_lengkap": "Budi Santoso",
      "nik": "3201234567890001",
      "email": "budi@gmail.com",
      "role": "tenant"
    },
    {
      "id": 2,
      "nama_lengkap": "Admin Utama",
      "nik": "3201234567890002",
      "email": "admin@kost.id",
      "role": "admin"
    }
  ]
}
```

### 🔄 Proses Deserialisasi (Adaptif)

```csharp
// Karena format JSON tidak selalu konsisten, gunakan JToken untuk fleksibilitas:
var result = Newtonsoft.Json.Linq.JToken.Parse(jsonResponse);
Newtonsoft.Json.Linq.JToken listPenyewaRaw = null;

if (result is Newtonsoft.Json.Linq.JArray)
{
    // Server kirim langsung array: [{ "id": 1, ... }, ...]
    listPenyewaRaw = result;
}
else
{
    // Server kirim wrapped object: { "tenants": [...] }
    listPenyewaRaw = result["tenants"] ?? result["penyewas"] ?? result["data"];
}

var listPenyewa = listPenyewaRaw.ToObject<List<Penyewa>>();
```

---

## 2.3 — PaymentResponse.cs (Pembayaran)

### 🎯 Tujuan
Blueprint untuk data transaksi pembayaran sewa kamar.

### 📄 Kode

```csharp
// File: PaymentResponse.cs
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kost_SiguraGura
{
    // Wrapper: { "pembayarans": [...] }
    public class PaymentResponse
    {
        [JsonProperty("pembayarans")]
        public List<Pembayaran> Pembayarans { get; set; }
    }

    public class Pembayaran
    {
        // Jumlah nominal pembayaran dalam Rupiah
        [JsonProperty("jumlah_bayar")]
        public long JumlahBayar { get; set; }

        // Status: "Confirmed", "Pending", "Rejected"
        [JsonProperty("status_pembayaran")]
        public string StatusPembayaran { get; set; }
    }
}
```

### 📨 Contoh JSON dari Server

```json
{
  "pembayarans": [
    {
      "jumlah_bayar": 800000,
      "status_pembayaran": "Confirmed"
    },
    {
      "jumlah_bayar": 1500000,
      "status_pembayaran": "Pending"
    }
  ]
}
```

### 📌 Field yang Direkomendasikan untuk Ditambahkan

```csharp
// Rekomendasi pengembangan model Pembayaran yang lebih lengkap:
public class Pembayaran
{
    [JsonProperty("id")]
    public int ID { get; set; }

    [JsonProperty("jumlah_bayar")]
    public long JumlahBayar { get; set; }

    [JsonProperty("status_pembayaran")]
    public string StatusPembayaran { get; set; }

    // Field tambahan yang disarankan:
    [JsonProperty("tanggal_bayar")]
    public string TanggalBayar { get; set; }      // Kapan dibayar

    [JsonProperty("metode_pembayaran")]
    public string MetodePembayaran { get; set; }  // Transfer / Cash / dll

    [JsonProperty("nama_penyewa")]
    public string NamaPenyewa { get; set; }       // Siapa yang bayar

    [JsonProperty("nomor_kamar")]
    public string NomorKamar { get; set; }        // Kamar mana
}
```

---

## 2.4 — LoginRequest.cs

### 🎯 Tujuan
Blueprint untuk data yang **dikirim ke server** saat proses login (bukan menerima, melainkan mengirim).

### 📄 Kode

```csharp
// File: LoginRequest.cs
namespace Kost_SiguraGura
{
    public class LoginRequest
    {
        // Harus sesuai dengan nama field yang diharapkan server
        public string username { get; set; }
        public string password { get; set; }
    }
}
```

### 🔄 Cara Penggunaan

```csharp
// Di Form1.cs (Login Page):

// 1. Buat objek LoginRequest dari input user
var dataLogin = new LoginRequest
{
    username = txtUsername.Text,
    password = txtPassword.Text
};

// 2. Ubah objek menjadi teks JSON
string jsonString = JsonConvert.SerializeObject(dataLogin);
// Hasil: {"username":"admin","password":"rahasia"}

// 3. Bungkus dalam HTTP content dengan encoding UTF-8
var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

// 4. Kirim ke server
HttpResponseMessage response = await ApiClient.Client.PostAsync(loginUrl, content);
```

---

## 🗺️ Hubungan Antar Model

```
LoginRequest ──► POST /api/auth/login ──► [session tersimpan di Session.cs]
                                                    │
                            ┌───────────────────────┼──────────────────────┐
                            │                       │                      │
               GET /api/kamar          GET /api/tenants       GET /api/payments
                            │                       │                      │
                         List<Kamar>          List<Penyewa>       List<Pembayaran>
```

---

## ⏭️ Langkah Berikutnya
Dengan blueprint data yang sudah siap, pelajari proses autentikasi user ke server di **[Modul 3 — Autentikasi (Login)](./03_auth.md)**.
