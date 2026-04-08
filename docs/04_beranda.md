# 🏠 Modul 4 — Dashboard (Beranda)

Halaman pertama yang ditampilkan setelah login berhasil. Menampilkan **ringkasan statistik** kost secara real-time dari API.

---

## 📁 File yang Terlibat

| File | Tipe | Fungsi |
|------|------|--------|
| `BerandaPage.cs` | UserControl | Logika dashboard & fetch data |
| `BerandaPage.Designer.cs` | Auto-generated | Definisi komponen UI |
| `PaymentResponse.cs` | Model | Struktur data pembayaran |

---

## 4.1 — Komponen UI di Beranda

| Komponen | Nama | Konten |
|----------|------|--------|
| Label | `lblIncome` | Total pendapatan bulan ini |
| Layout | `tableLayoutPanel3` | Container cards statistik |

---

## 4.2 — Alur Load Data Dashboard

```
BerandaPage_Load() dipanggil saat halaman ditampilkan
        │
        ▼
await UpdateTotalPendapatan()
        │
        ▼
GET /api/payments  (pakai ApiClient.Client agar session terbawa)
        │
        ▼
Terima JSON response
        │
        ▼
Deteksi format: Array langsung atau Object wrapper?
        │
   ┌────┴────┐
   │         │
 Array    Object
   │         │
   │         └─► Ambil key "pembayarans" / "data"
   │
   ▼
ToObject<List<Pembayaran>>()
        │
        ▼
LINQ: Filter status == "Confirmed" → Sum(JumlahBayar)
        │
        ▼
FormatKeRupiahSingkat(total) → tampilkan ke lblIncome
```

---

## 4.3 — Kode Lengkap BerandaPage.cs

```csharp
// File: BerandaPage.cs
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    public partial class BerandaPage : UserControl
    {
        public BerandaPage()
        {
            InitializeComponent();
        }

        // ── Event: Halaman selesai dimuat ──────────────────────────────────
        private async void BerandaPage_Load(object sender, EventArgs e)
        {
            await UpdateTotalPendapatan();
        }

        // ── Fungsi utama: Ambil & hitung total pendapatan ──────────────────
        private async Task UpdateTotalPendapatan()
        {
            string url = "https://rahmatzaw.elarisnoir.my.id/api/payments";

            try
            {
                // Gunakan ApiClient agar session login tetap terbawa (Cookie)
                HttpResponseMessage response = await ApiClient.Client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.Linq.JToken.Parse(jsonResponse);

                    Newtonsoft.Json.Linq.JToken listPembayaranRaw;

                    // Deteksi format response:
                    // Tipe A: langsung array  → [{ "jumlah_bayar": ... }, ...]
                    // Tipe B: wrapped object  → { "pembayarans": [...] }
                    if (result is Newtonsoft.Json.Linq.JArray)
                    {
                        listPembayaranRaw = result;
                    }
                    else
                    {
                        listPembayaranRaw = result["pembayarans"] ?? result["data"] ?? result;
                    }

                    if (listPembayaranRaw != null)
                    {
                        var listPembayaran = listPembayaranRaw.ToObject<List<Pembayaran>>();

                        if (listPembayaran != null && listPembayaran.Count > 0)
                        {
                            // LINQ: Hanya hitung yang statusnya sudah "Confirmed"
                            long total = listPembayaran
                                .Where(p => p.StatusPembayaran != null &&
                                            p.StatusPembayaran.Equals("Confirmed",
                                                StringComparison.OrdinalIgnoreCase))
                                .Sum(p => p.JumlahBayar);

                            // Update UI dari thread async → wajib pakai Invoke
                            this.Invoke((MethodInvoker)delegate {
                                lblIncome.Text = FormatKeRupiahSingkat(total);
                            });
                        }
                        else
                        {
                            lblIncome.Text = "Rp 0";
                        }
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    lblIncome.Text = "No Access";
                    MessageBox.Show("Sesi login habis, silakan login ulang.");
                }
                else
                {
                    lblIncome.Text = "Error " + (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                lblIncome.Text = "Error";
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }

        // ── Helper: Format angka ke Rupiah singkat ─────────────────────────
        private string FormatKeRupiahSingkat(long nominal)
        {
            var culture = new CultureInfo("id-ID");

            if (nominal >= 1_000_000)
            {
                double juta = (double)nominal / 1_000_000;
                return string.Format(culture, "Rp {0:0.#} jt", juta);
                // Contoh output: "Rp 2,5 jt"
            }
            else if (nominal >= 1_000)
            {
                double ribu = (double)nominal / 1_000;
                return string.Format(culture, "Rp {0:0.#} rb", ribu);
                // Contoh output: "Rp 800 rb"
            }

            return nominal.ToString("C0", culture);
            // Contoh output: "Rp 500"
        }
    }
}
```

---

## 4.4 — Penjelasan Teknis Penting

### 🔒 Mengapa Pakai `this.Invoke()`?

```csharp
// ❌ SALAH — Akan crash dengan "Cross-thread operation not valid"
lblIncome.Text = FormatKeRupiahSingkat(total);

// ✅ BENAR — Update UI harus dilakukan di UI Thread
this.Invoke((MethodInvoker)delegate {
    lblIncome.Text = FormatKeRupiahSingkat(total);
});
```

**Penjelasan:** `await` menjalankan kode di background thread. Komponen UI (seperti `lblIncome`) hanya boleh disentuh dari UI thread. `Invoke()` memastikan kode tersebut berjalan di thread yang benar.

### 📊 Logika LINQ Filter Pembayaran

```csharp
long total = listPembayaran
    .Where(p => p.StatusPembayaran != null &&
                p.StatusPembayaran.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
    //          ↑ null check             ↑ case-insensitive: "confirmed" = "Confirmed" = "CONFIRMED"
    .Sum(p => p.JumlahBayar);
    // ↑ Jumlahkan field JumlahBayar dari semua item yang lolos filter
```

### 💱 Tabel Konversi Format Rupiah

| Input | Output |
|-------|--------|
| `2_500_000` | `"Rp 2,5 jt"` |
| `800_000` | `"Rp 800 rb"` |
| `1_500_000` | `"Rp 1,5 jt"` |
| `500` | `"Rp 500"` |

---

## 4.5 — Pengembangan yang Disarankan

```csharp
// Tambahkan kartu statistik lain di BerandaPage_Load():

private async void BerandaPage_Load(object sender, EventArgs e)
{
    // Jalankan semua fetch secara paralel untuk performa lebih baik
    await Task.WhenAll(
        UpdateTotalPendapatan(),  // Total income
        UpdateJumlahKamar(),      // Total kamar tersedia
        UpdateJumlahPenyewa()     // Total penyewa aktif
    );
}

private async Task UpdateJumlahKamar()
{
    string url = "https://rahmatzaw.elarisnoir.my.id/api/kamar";
    var response = await ApiClient.Client.GetAsync(url);
    if (response.IsSuccessStatusCode)
    {
        string json = await response.Content.ReadAsStringAsync();
        var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Kamar>>(json);

        int tersedia = list?.Count(k =>
            k.STATUS?.Equals("Tersedia", StringComparison.OrdinalIgnoreCase) == true) ?? 0;

        this.Invoke((MethodInvoker)delegate {
            lblKamarTersedia.Text = tersedia.ToString();
        });
    }
}
```

---

## ⏭️ Langkah Berikutnya
Pelajari pengelolaan data aset utama aplikasi, yaitu kamar kost, di **[Modul 5 — Manajemen Kamar](./05_kamar.md)**.
