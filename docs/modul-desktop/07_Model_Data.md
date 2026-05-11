# 07 - TUTORIAL MODEL DATA & STRUKTUR DTO

**File Terkait:** `Kamar.cs`, `Penyewa.cs`, `Pembayaran.cs`, `LoginRequest.cs`, `DashboardStats.cs`

---

## 📖 Pendahuluan

Di dalam aplikasi yang terkoneksi API, server Backend merespon dalam bentuk teks panjang berformat JSON. Untuk bisa membaca, mengubah, dan memvalidasi JSON tersebut secara aman di C# (Type-Safe), kita harus mengubahnya (Deserialisasi) ke dalam wujud **Class / Objek C#**. Objek-objek ini dikenal sebagai **Data Models** atau **DTO (Data Transfer Object)**.

Tutorial ini akan memandu Anda untuk menulis Model C# yang dihubungkan dengan library `Newtonsoft.Json`.

---

## Langkah 1: Memahami DTO & JSON Mapping

Contoh dari server kita mendapat JSON:
```json
{
  "nomor_kamar": "101",
  "harga_per_bulan": 500000
}
```

Bagaimana cara agar C# tahu kalau `harga_per_bulan` JSON itu merujuk ke variabel `PRICE` di aplikasi kita?
Jawabannya adalah **Attribute [JsonProperty]**.
```csharp
[JsonProperty("harga_per_bulan")]
public decimal PRICE { get; set; }
```

---

## Langkah 2: Membuat Model Kamar

Buat *class* `Kamar.cs`. Perhatikan penggunaan tipe datanya (gunakan `decimal` untuk mata uang).

```csharp
using Newtonsoft.Json;
using System.Drawing;

namespace Kost_SiguraGura
{
    public class Kamar
    {
        [JsonProperty("id")]
        public int NO { get; set; }

        [JsonProperty("nomor_kamar")]
        public string ROOM { get; set; }

        [JsonProperty("tipe_kamar")]
        public string TYPE { get; set; }

        // MATA UANG -> Wajib Decimal
        [JsonProperty("harga_per_bulan")]
        public decimal PRICE { get; set; }

        [JsonProperty("floor")]
        public int FLOOR { get; set; }

        [JsonProperty("status")]
        public string STATUS { get; set; }

        [JsonProperty("capacity")]
        public int KAPASITAS { get; set; }

        // IGNORE: Memberitahu C# agar properti ini diabaikan saat JSON Deserialize
        // Ini murni properti memory UI internal saja
        [JsonIgnore]
        public Image THUMBNAIL { get; set; }
    }
}
```

---

## Langkah 3: Membuat Model Relasional (Pembayaran & Pemesanan)

Dalam beberapa kasus, backend mengirimkan entitas turunan (Object di dalam Object).
Misalnya ketika `Pembayaran` menyertakan info detil tentang `Penyewa`.

```csharp
public class Pembayaran
{
    [JsonProperty("id")]
    public int NO { get; set; }

    [JsonProperty("amount")]
    public decimal? Amount { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    // Jika JSON mengirimkan object bersarang (nested):
    [JsonProperty("pemesanan")]
    public Pemesanan Pemesanan { get; set; }
}

// Child Class yang direferensikan
public class Pemesanan
{
    [JsonProperty("id")]
    public int NO { get; set; }

    [JsonProperty("penyewa")]
    public Penyewa Penyewa { get; set; } // Referensi Class Penyewa
    
    [JsonProperty("kamar")]
    public Kamar Kamar { get; set; }     // Referensi Class Kamar
}
```

---

## Langkah 4: Best Practices Mapping Model

Setiap kali Anda mendesain kelas DTO, patuhi standar ini:

1. **Gunakan Nullable Types `?`**
   Untuk data JSON yang bisa jadi bernilai `null` dari sisi server, tambahkan tanda tanya pada tipe data C# untuk menghindari aplikasi *Crash*.
   ```csharp
   [JsonProperty("check_out_date")]
   public DateTime? CheckOutDate { get; set; } // Tanggal bisa kosong / null
   ```

2. **Konsistensi Penamaan**
   Usahakan seluruh *property* Anda menggunakan kaidah `PascalCase` di dalam C# (walaupun properti aslinya di C# mungkin ditulis `UPPERCASE` untuk beberapa kolom warisan tabel lama). Namun kuncinya ada pada pendefinisian di dalam kurung tanda kutip properti: `[JsonProperty("snake_case_backend_key")]`.

3. **Performa Wrapper Array**
   Jika struktur backend dibungkus layaknya `{"data": [{...}]}`, Anda tidak wajib membuat class Wrapper `DataResponse` apabila logika mapping manual `JToken.Parse()` sudah ditangani di langkah *Integrasi API*. Class DTO Anda murni untuk isi entitasnya saja.

---

## 📚 Navigasi Dokumentasi

**← Sebelumnya:** [06_Integrasi_API](06_Integrasi_API) - Integrasi API REST  
**Berikutnya →** [08_QUICK_REFERENCE](08_QUICK_REFERENCE) - Quick Reference (Panduan Singkat)

**Daftar Lengkap:** [README_DOKUMENTASI](README_DOKUMENTASI)
