# 💳 Modul 7 — Pembayaran

Modul ini menangani data transaksi pembayaran sewa kamar, termasuk daftar pembayaran dan kalkulasi total.

---

## 📁 File yang Terlibat

| File | Tipe | Status | Fungsi |
|------|------|--------|--------|
| `PembayaranForm.cs` | UserControl | 🚧 Skeleton | UI & logika pembayaran |
| `PaymentResponse.cs` | Model | ✅ Aktif | Blueprint data pembayaran |
| `BerandaPage.cs` | UserControl | ✅ Aktif | Total pendapatan (pakai model Pembayaran) |

---

## 7.1 — Status Pembayaran

| Status | Keterangan |
|--------|-----------|
| `"Confirmed"` | Pembayaran sudah dikonfirmasi oleh admin |
| `"Pending"` | Pembayaran menunggu konfirmasi |
| `"Rejected"` | Pembayaran ditolak / dibatalkan |

---

## 7.2 — Model Data (PaymentResponse.cs)

```csharp
// File: PaymentResponse.cs
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kost_SiguraGura
{
    // Wrapper untuk response: { "pembayarans": [...] }
    public class PaymentResponse
    {
        [JsonProperty("pembayarans")]
        public List<Pembayaran> Pembayarans { get; set; }
    }

    public class Pembayaran
    {
        [JsonProperty("jumlah_bayar")]
        public long JumlahBayar { get; set; }       // Nomimal dalam Rupiah

        [JsonProperty("status_pembayaran")]
        public string StatusPembayaran { get; set; } // "Confirmed" / "Pending" / "Rejected"
    }
}
```

---

## 7.3 — Kode PembayaranForm.cs (Kondisi Saat Ini)

```csharp
// File: PembayaranForm.cs
// ⚠️ Status: SKELETON — hanya constructor, belum ada implementasi
namespace Kost_SiguraGura
{
    public partial class PembayaranForm : UserControl
    {
        public PembayaranForm()
        {
            InitializeComponent();
        }
    }
}
```

---

## 7.4 — Implementasi yang Disarankan

### Komponen UI yang Perlu Ditambah di Designer

| Komponen | Nama | Fungsi |
|----------|------|--------|
| DataGridView | `dataGridView1` | Tabel daftar pembayaran |
| Label | `lblTotalConfirmed` | Total pembayaran dikonfirmasi |
| Label | `lblTotalPending` | Total pembayaran tertunda |
| ComboBox | `cmbFilterStatus` | Filter berdasarkan status |
| Button | `btnRefresh` | Muat ulang data |

### Kode Implementasi Lengkap yang Disarankan

```csharp
// File: PembayaranForm.cs (implementasi lengkap yang disarankan)
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    public partial class PembayaranForm : UserControl
    {
        private List<Pembayaran> fullListPembayaran = new List<Pembayaran>();
        private BindingList<Pembayaran> bindingList = new BindingList<Pembayaran>();

        public PembayaranForm()
        {
            InitializeComponent();
        }

        // ── Event: Load halaman ───────────────────────────────────────────
        private async void PembayaranForm_Load(object sender, EventArgs e)
        {
            SetupFilterComboBox();
            await LoadDataPembayaran();
        }

        // ── Setup pilihan filter ──────────────────────────────────────────
        private void SetupFilterComboBox()
        {
            cmbFilterStatus.Items.Clear();
            cmbFilterStatus.Items.Add("Semua Status");
            cmbFilterStatus.Items.Add("Confirmed");
            cmbFilterStatus.Items.Add("Pending");
            cmbFilterStatus.Items.Add("Rejected");
            cmbFilterStatus.SelectedIndex = 0;
        }

        // ── Fetch data pembayaran dari API ────────────────────────────────
        private async Task LoadDataPembayaran()
        {
            string url = "https://rahmatzaw.elarisnoir.my.id/api/payments";
            try
            {
                var response = await ApiClient.Client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.Linq.JToken.Parse(json);

                    Newtonsoft.Json.Linq.JToken listRaw;

                    // Deteksi format response
                    if (result is Newtonsoft.Json.Linq.JArray)
                        listRaw = result;
                    else
                        listRaw = result["pembayarans"] ?? result["data"] ?? result;

                    if (listRaw != null)
                    {
                        fullListPembayaran = listRaw.ToObject<List<Pembayaran>>();
                        ApplyFilters();
                        UpdateSummaryLabels();
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Sesi habis, silakan login ulang.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // ── Filter data sesuai pilihan status ────────────────────────────
        private void ApplyFilters()
        {
            string selectedStatus = cmbFilterStatus.SelectedItem?.ToString() ?? "Semua Status";
            var filtered = fullListPembayaran.AsEnumerable();

            if (selectedStatus != "Semua Status")
            {
                filtered = filtered.Where(p =>
                    p.StatusPembayaran?.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase) == true);
            }

            this.Invoke((MethodInvoker)delegate {
                bindingList = new BindingList<Pembayaran>(filtered.ToList());
                dataGridView1.DataSource = bindingList;
            });
        }

        // ── Update label ringkasan ────────────────────────────────────────
        private void UpdateSummaryLabels()
        {
            long totalConfirmed = fullListPembayaran
                .Where(p => p.StatusPembayaran?.ToLower() == "confirmed")
                .Sum(p => p.JumlahBayar);

            long totalPending = fullListPembayaran
                .Where(p => p.StatusPembayaran?.ToLower() == "pending")
                .Sum(p => p.JumlahBayar);

            this.Invoke((MethodInvoker)delegate {
                lblTotalConfirmed.Text = FormatRupiah(totalConfirmed);
                lblTotalPending.Text   = FormatRupiah(totalPending);
            });
        }

        // ── Format angka ke Rupiah ────────────────────────────────────────
        private string FormatRupiah(long nominal)
        {
            return nominal.ToString("C0", new CultureInfo("id-ID"));
        }

        // ── Event: Perubahan filter ───────────────────────────────────────
        private void cmbFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        // ── Event: Tombol refresh ─────────────────────────────────────────
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDataPembayaran();
        }
    }
}
```

---

## 7.5 — Kalkulasi Pendapatan di Beranda

Modul Beranda menggunakan data pembayaran untuk menghitung total income. Ini adalah **contoh nyata** penggunaan model `Pembayaran`:

```csharp
// Dari BerandaPage.cs — memanfaatkan data pembayaran:

// 1. Ambil semua pembayaran
var listPembayaran = listPembayaranRaw.ToObject<List<Pembayaran>>();

// 2. Filter yang sudah dikonfirmasi lalu jumlahkan
long total = listPembayaran
    .Where(p => p.StatusPembayaran != null &&
                p.StatusPembayaran.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
    .Sum(p => p.JumlahBayar);

// 3. Format dan tampilkan
lblIncome.Text = FormatKeRupiahSingkat(total); // → "Rp 3,2 jt"
```

---

## 7.6 — Contoh JSON Response

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
    },
    {
      "jumlah_bayar": 800000,
      "status_pembayaran": "Confirmed"
    }
  ]
}
```

**Dari data di atas:**
- Total Confirmed: Rp 800.000 + Rp 800.000 = **Rp 1.600.000**
- Total Pending: **Rp 1.500.000**
