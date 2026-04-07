# 👥 Modul 6 — Manajemen Penyewa

Modul ini menampilkan dan memfilter daftar penyewa kost. **Hanya bisa diakses oleh Admin.**

---

## 📁 File yang Terlibat

| File | Tipe | Fungsi |
|------|------|--------|
| `DataPenyewa.cs` | UserControl | Logika fetch, filter, tampilkan penyewa |
| `DataPenyewa.Designer.cs` | Auto-generated | Komponen UI |
| `Penyewa.cs` | Model | Blueprint data penyewa |
| `Session.cs` | State | Cek role user sebelum load data |

---

## 6.1 — Komponen UI di DataPenyewa

| Komponen | Nama | Fungsi |
|----------|------|--------|
| DataGridView | `dataGridView1` | Tabel daftar penyewa |
| TextBox | `txtSearch` | Pencarian berdasarkan nama |

---

## 6.2 — Alur Kerja Modul Penyewa

```
DataPenyewa_Load()
        │
        ▼
Cek Session.UserRole == "admin"?
        │
   ┌────┴────┐
   │         │
  Ya        Tidak
   │         │
   │         └─► MessageBox "Akses Ditolak"
   │
   ▼
LoadDataPenyewa()
        │
        ▼
GET /api/tenants  (pakai ApiClient.Client → session terbawa)
        │
        ▼
Parse response (adaptif: Array atau Object wrapper)
        │
        ▼
Simpan ke fullListPenyewa
        │
        ▼
Tampilkan ke DataGridView via BindingList
        │
        └─► txtSearch_TextChanged → filter real-time (nama penyewa)
```

---

## 6.3 — Kode Lengkap DataPenyewa.cs

```csharp
// File: DataPenyewa.cs
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    public partial class DataPenyewa : UserControl
    {
        // List yang terhubung ke DataGridView (berubah saat filter)
        private BindingList<Penyewa> bindingListPenyewa = new BindingList<Penyewa>();

        // Data master dari server (tidak berubah)
        private List<Penyewa> fullListPenyewa = new List<Penyewa>();

        public DataPenyewa()
        {
            InitializeComponent();
        }

        // ── Event: Halaman load ───────────────────────────────────────────
        private void DataPenyewa_Load(object sender, EventArgs e)
        {
            // Periksa role user — hanya admin yang diizinkan
            if (Session.UserRole?.ToLower() == "admin")
            {
                LoadDataPenyewa();
            }
            else
            {
                MessageBox.Show("Akses Ditolak! Anda bukan Admin.\nRole Anda: " + Session.UserRole);
            }
        }

        // ── Fungsi utama: Fetch data penyewa dari API ─────────────────────
        private async void LoadDataPenyewa()
        {
            try
            {
                string url = "https://rahmatzaw.elarisnoir.my.id/api/tenants";

                // Memakai ApiClient.Client → cookie login ikut terkirim
                HttpResponseMessage response = await ApiClient.Client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Parse dengan JToken agar fleksibel terhadap format JSON
                    var result = Newtonsoft.Json.Linq.JToken.Parse(jsonResponse);
                    Newtonsoft.Json.Linq.JToken listPenyewaRaw = null;

                    // Cek format: Array langsung atau Object wrapper?
                    if (result is Newtonsoft.Json.Linq.JArray)
                    {
                        // Format: [ { "id": 1, ... }, { "id": 2, ... } ]
                        listPenyewaRaw = result;
                    }
                    else
                    {
                        // Format: { "tenants": [...] } atau { "penyewas": [...] }
                        listPenyewaRaw = result["tenants"] ?? result["penyewas"] ?? result["data"];
                    }

                    if (listPenyewaRaw != null)
                    {
                        fullListPenyewa = listPenyewaRaw.ToObject<List<Penyewa>>();

                        // Update UI harus di UI Thread
                        this.Invoke((MethodInvoker)delegate {
                            bindingListPenyewa = new BindingList<Penyewa>(fullListPenyewa);
                            dataGridView1.DataSource = null;      // Reset dulu
                            dataGridView1.DataSource = bindingListPenyewa;
                            dataGridView1.Refresh();
                        });
                    }
                    else
                    {
                        MessageBox.Show("Data tidak ketemu di JSON. Isi aslinya:\n" + jsonResponse);
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Sesi login habis atau tidak sah. Silakan login ulang.");
                }
                else
                {
                    MessageBox.Show("Gagal ambil data. Status: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat bongkar data: " + ex.Message);
            }
        }

        // ── Event: Filter real-time berdasarkan nama ──────────────────────
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                // Tidak ada keyword → tampilkan semua
                bindingListPenyewa = new BindingList<Penyewa>(fullListPenyewa);
            }
            else
            {
                // Filter berdasarkan nama lengkap
                var filtered = fullListPenyewa
                    .Where(p => p.NAMA_LENGKAP != null &&
                                p.NAMA_LENGKAP.ToLower().Contains(keyword))
                    .ToList();

                bindingListPenyewa = new BindingList<Penyewa>(filtered);
            }

            dataGridView1.DataSource = bindingListPenyewa;
        }
    }
}
```

---

## 6.4 — Penjelasan Teknis Penting

### 🔐 Mengapa Role-Based Access Control (RBAC) di Sini?

```csharp
// Data penyewa bersifat SENSITIF (NIK, email)
// Hanya admin yang boleh melihatnya
if (Session.UserRole?.ToLower() == "admin")
{
    LoadDataPenyewa();  // ✅ Lanjutkan fetch
}
else
{
    // ❌ Blok akses → tampilkan pesan
    MessageBox.Show("Akses Ditolak! Role Anda: " + Session.UserRole);
}
```

### ↩️ Mengapa `dataGridView1.DataSource = null` Sebelum Set Baru?

```csharp
// Tanpa reset, DataGridView kadang tidak refresh dengan benar
// jika DataSource diganti dengan list baru yang bertipe sama
dataGridView1.DataSource = null;          // Reset
dataGridView1.DataSource = bindingListPenyewa; // Set baru
dataGridView1.Refresh();                  // Paksa repaint
```

### 🔍 Perbedaan Filter di DataPenyewa vs DataKamar

| | DataKamar | DataPenyewa |
|---|---|---|
| Filter | Status + Tipe + Keyword | Hanya Keyword (nama) |
| Komponen Filter | 2 ComboBox + 1 TextBox | 1 TextBox saja |
| Fungsi Filter | `ApplyFilters()` terpusat | Langsung di event handler |

---

## 6.5 — Kolom yang Tampil di DataGridView

Kolom tampil otomatis sesuai properti `Penyewa`:

| Kolom di Grid | Properti C# | Field JSON |
|---------------|-------------|-----------|
| ID | `ID` | `"id"` |
| NAMA_LENGKAP | `NAMA_LENGKAP` | `"nama_lengkap"` |
| NIK | `NIK` | `"nik"` |
| KONTAK | `KONTAK` | `"email"` |
| PERAN | `PERAN` | `"role"` |

---

## 6.6 — Pengembangan yang Disarankan

```csharp
// Filter tambahan yang berguna:
private void ApplyFilters()
{
    string keyword = txtSearch.Text.ToLower().Trim();
    var filtered = fullListPenyewa.AsEnumerable();

    // Filter berdasarkan nama ATAU NIK
    if (!string.IsNullOrEmpty(keyword))
    {
        filtered = filtered.Where(p =>
            (p.NAMA_LENGKAP != null && p.NAMA_LENGKAP.ToLower().Contains(keyword)) ||
            (p.NIK != null && p.NIK.Contains(keyword))
        );
    }

    // Filter berdasarkan role (hanya tampilkan "tenant", bukan "admin")
    filtered = filtered.Where(p =>
        p.PERAN?.ToLower() == "tenant"
    );

    bindingListPenyewa = new BindingList<Penyewa>(filtered.ToList());
    dataGridView1.DataSource = bindingListPenyewa;
}
```
