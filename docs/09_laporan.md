# 📊 Modul 9 — Laporan

Modul ini menampilkan **ringkasan dan laporan keuangan** kost, termasuk total pendapatan, statistik kamar, dan data penyewa aktif.

---

## 📁 File yang Terlibat

| File | Tipe | Status | Fungsi |
|------|------|--------|--------|
| `Report.cs` | UserControl | 🚧 Skeleton | UI & logika laporan |
| `Report.Designer.cs` | Auto-generated | - | Komponen UI |
| `PaymentResponse.cs` | Model | ✅ Aktif | Data pembayaran untuk kalkulasi |
| `Kamar.cs` | Model | ✅ Aktif | Data kamar untuk statistik |

---

## 9.1 — Kode Report.cs (Kondisi Saat Ini)

```csharp
// File: Report.cs
// ⚠️ Status: SKELETON — belum ada implementasi
namespace Kost_SiguraGura
{
    public partial class Report : UserControl
    {
        public Report()
        {
            InitializeComponent();
        }
    }
}
```

---

## 9.2 — Konsep Data Laporan

```
Laporan Keuangan Kost
        │
        ├─► Pendapatan
        │       ├─► Total Confirmed (sudah masuk kas)
        │       ├─► Total Pending (belum dikonfirmasi)
        │       └─► Breakdown per bulan (jika ada field tanggal)
        │
        ├─► Statistik Kamar
        │       ├─► Jumlah kamar tersedia
        │       ├─► Jumlah kamar terisi (Penuh)
        │       ├─► Tingkat hunian (Occupancy Rate %)
        │       └─► Breakdown per tipe (Standard vs Premium)
        │
        └─► Data Penyewa
                ├─► Total penyewa aktif
                └─► Jumlah admin
```

---

## 9.3 — Implementasi yang Disarankan

### Komponen UI yang Perlu Ditambah di Designer

| Komponen | Nama | Fungsi |
|----------|------|--------|
| Label | `lblTotalPendapatan` | Total pendapatan confirmed |
| Label | `lblTotalPending` | Total pembayaran pending |
| Label | `lblKamarTersedia` | Jumlah kamar tersedia |
| Label | `lblKamarPenuh` | Jumlah kamar terisi |
| Label | `lblOccupancy` | Persentase hunian |
| Label | `lblTotalPenyewa` | Total penyewa aktif |
| DataGridView | `dgvLaporan` | Tabel rincian pembayaran |
| Button | `btnExport` | Ekspor ke file |

### Kode Implementasi Lengkap yang Disarankan

```csharp
// File: Report.cs (implementasi lengkap yang disarankan)
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
    public partial class Report : UserControl
    {
        public Report()
        {
            InitializeComponent();
        }

        // ── Event: Load halaman ───────────────────────────────────────────
        private async void Report_Load(object sender, EventArgs e)
        {
            // Jalankan semua fetch secara paralel untuk performa lebih baik
            await Task.WhenAll(
                LoadLaporanPembayaran(),
                LoadStatistikKamar()
            );
        }

        // ── 1. Laporan Pembayaran ─────────────────────────────────────────
        private async Task LoadLaporanPembayaran()
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
                    if (result is Newtonsoft.Json.Linq.JArray)
                        listRaw = result;
                    else
                        listRaw = result["pembayarans"] ?? result["data"] ?? result;

                    if (listRaw != null)
                    {
                        var list = listRaw.ToObject<List<Pembayaran>>();

                        // Kalkulasi total berdasarkan status
                        long totalConfirmed = list
                            .Where(p => p.StatusPembayaran?.ToLower() == "confirmed")
                            .Sum(p => p.JumlahBayar);

                        long totalPending = list
                            .Where(p => p.StatusPembayaran?.ToLower() == "pending")
                            .Sum(p => p.JumlahBayar);

                        long totalRejected = list
                            .Where(p => p.StatusPembayaran?.ToLower() == "rejected")
                            .Sum(p => p.JumlahBayar);

                        // Update UI
                        this.Invoke((MethodInvoker)delegate {
                            lblTotalPendapatan.Text = FormatRupiah(totalConfirmed);
                            lblTotalPending.Text    = FormatRupiah(totalPending);

                            // Tampilkan di DataGridView
                            dgvLaporan.DataSource = list;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load laporan pembayaran: " + ex.Message);
            }
        }

        // ── 2. Statistik Kamar ────────────────────────────────────────────
        private async Task LoadStatistikKamar()
        {
            string url = "https://rahmatzaw.elarisnoir.my.id/api/kamar";
            try
            {
                var response = await ApiClient.Client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var listKamar = JsonConvert.DeserializeObject<List<Kamar>>(json);

                    if (listKamar != null && listKamar.Count > 0)
                    {
                        int totalKamar   = listKamar.Count;
                        int tersedia     = listKamar.Count(k =>
                            k.STATUS?.ToLower() == "tersedia");
                        int penuh        = listKamar.Count(k =>
                            k.STATUS?.ToLower() == "penuh");
                        int perbaikan    = listKamar.Count(k =>
                            k.STATUS?.ToLower() == "perbaikan");

                        // Occupancy Rate = (kamar terisi / total kamar) × 100%
                        double occupancy = totalKamar > 0
                            ? (double)penuh / totalKamar * 100
                            : 0;

                        // Update UI
                        this.Invoke((MethodInvoker)delegate {
                            lblKamarTersedia.Text = tersedia.ToString();
                            lblKamarPenuh.Text    = penuh.ToString();
                            lblOccupancy.Text     = $"{occupancy:0.0}%";
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load statistik kamar: " + ex.Message);
            }
        }

        // ── Helper: Format angka ke Rupiah ────────────────────────────────
        private string FormatRupiah(long nominal)
        {
            return nominal.ToString("C0", new CultureInfo("id-ID"));
        }

        // ── Ekspor ke CSV ─────────────────────────────────────────────────
        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = "Laporan_Kost_" + DateTime.Now.ToString("yyyyMMdd");

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    EksporKeCsv(sfd.FileName);
                }
            }
        }

        private void EksporKeCsv(string filePath)
        {
            try
            {
                var sb = new System.Text.StringBuilder();

                // Header kolom
                sb.AppendLine("Jumlah Bayar,Status Pembayaran");

                // Data dari DataGridView
                foreach (DataGridViewRow row in dgvLaporan.Rows)
                {
                    if (row.IsNewRow) continue;
                    sb.AppendLine($"{row.Cells["JumlahBayar"].Value},{row.Cells["StatusPembayaran"].Value}");
                }

                System.IO.File.WriteAllText(filePath, sb.ToString(), System.Text.Encoding.UTF8);
                MessageBox.Show("Berhasil diekspor ke:\n" + filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal ekspor: " + ex.Message);
            }
        }
    }
}
```

---

## 9.4 — Formula Laporan

```
Occupancy Rate (Tingkat Hunian):
    = (Jumlah Kamar "Penuh" / Total Kamar) × 100%

Contoh: 8 kamar penuh dari 12 total
    = (8 / 12) × 100% = 66,7%

─────────────────────────────────────────────────

Total Pendapatan Bersih:
    = SUM(JumlahBayar WHERE StatusPembayaran == "Confirmed")

Total Pending:
    = SUM(JumlahBayar WHERE StatusPembayaran == "Pending")
```

---

## 9.5 — Ringkasan Laporan (Output yang Diharapkan)

```
┌─────────────────────────────────────────────────────────┐
│              LAPORAN KOST SIGURA GURA                   │
│                                                         │
│  💰 KEUANGAN                                            │
│  ├─ Total Pendapatan (Confirmed) : Rp 6.400.000         │
│  ├─ Menunggu Konfirmasi          : Rp 1.500.000         │
│  └─ Total Transaksi              : 9 transaksi          │
│                                                         │
│  🛏️ KAMAR                                               │
│  ├─ Total Kamar     : 12                                │
│  ├─ Tersedia        : 4                                 │
│  ├─ Terisi          : 8                                 │
│  ├─ Perbaikan       : 0                                 │
│  └─ Occupancy Rate  : 66,7%                             │
│                                                         │
└─────────────────────────────────────────────────────────┘
```
