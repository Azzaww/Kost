# 🛏️ Modul 5 — Manajemen Kamar

Modul ini menampilkan, memfilter, dan mengelola data kamar kost yang diambil dari server.

---

## 📁 File yang Terlibat

| File | Tipe | Fungsi |
|------|------|--------|
| `DataKamar.cs` | UserControl | Logika utama fetch, filter, tampilkan kamar |
| `DataKamar.Designer.cs` | Auto-generated | Komponen UI |
| `Kamar.cs` | Model | Blueprint data kamar |
| `GalleryForm.cs` | UserControl | Tampilan gallery foto kamar |

---

## 5.1 — Komponen UI di DataKamar

| Komponen | Nama | Fungsi |
|----------|------|--------|
| DataGridView | `dataGridView1` | Tabel data kamar |
| ComboBox | `guna2ComboBox1` | Filter berdasarkan status |
| ComboBox | `guna2ComboBox2` | Filter berdasarkan tipe kamar |
| TextBox | `txtSearch` | Pencarian real-time |

---

## 5.2 — Alur Kerja Modul Kamar

```
DataKamar_Load()
        │
        ├─► SetupComboBox()       ← Isi pilihan filter
        │
        └─► LoadDataKamar()       ← Fetch data dari API
                  │
                  ▼
          GET /api/kamar
                  │
                  ▼
          Deserialize JSON → List<Kamar>
                  │
                  ▼
          Loop: Download gambar dari ThumbnailUrl
                  │
                  ▼
          Simpan ke fullListKamar (cadangan utama)
                  │
                  ▼
          ApplyFilters() → tampilkan ke DataGridView
                  │
                  └─► (dijalankan ulang setiap kali filter/search berubah)
```

---

## 5.3 — Kode Lengkap DataKamar.cs

```csharp
// File: DataKamar.cs
using Kost_SiguraGura.Kost_SiguraGura;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    public partial class DataKamar : UserControl
    {
        // BindingList untuk DataGridView (yang ditampilkan — bisa berubah saat filter)
        private BindingList<Kamar> bindingListKamar = new BindingList<Kamar>();

        // Sumber data asli — TIDAK boleh dimodifikasi saat filter
        private List<Kamar> fullListKamar = new List<Kamar>();

        public DataKamar()
        {
            InitializeComponent();
            SetupComboBox();
        }

        // ── Inisialisasi pilihan ComboBox ─────────────────────────────────
        private void SetupComboBox()
        {
            // ComboBox Filter Status Kamar
            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.Add("Semua Status");
            guna2ComboBox1.Items.Add("Tersedia");
            guna2ComboBox1.Items.Add("Penuh");
            guna2ComboBox1.Items.Add("Perbaikan");
            guna2ComboBox1.SelectedIndex = 0;

            // ComboBox Filter Tipe Kamar
            guna2ComboBox2.Items.Clear();
            guna2ComboBox2.Items.Add("Semua Tipe");
            guna2ComboBox2.Items.Add("Standard");
            guna2ComboBox2.Items.Add("Premium");
            guna2ComboBox2.SelectedIndex = 0;
        }

        // ── Event: Halaman load ───────────────────────────────────────────
        private void DataKamar_Load(object sender, EventArgs e)
        {
            LoadDataKamar();
            txtSearch.PlaceholderText = "Search Rooms...";
        }

        // ── Fungsi utama: Fetch data kamar dari API ───────────────────────
        private async void LoadDataKamar()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "https://rahmatzaw.elarisnoir.my.id/api/kamar";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        // Deserialize JSON → List<Kamar>
                        var listData = JsonConvert.DeserializeObject<List<Kamar>>(jsonResponse);

                        if (listData != null)
                        {
                            fullListKamar.Clear();

                            foreach (var kamar in listData)
                            {
                                // Download gambar dari URL jika ada
                                if (!string.IsNullOrEmpty(kamar.ThumbnailUrl))
                                {
                                    try
                                    {
                                        byte[] imageBytes = await client.GetByteArrayAsync(kamar.ThumbnailUrl);
                                        using (var ms = new System.IO.MemoryStream(imageBytes))
                                        {
                                            // Buat Bitmap baru (clone) agar stream bisa di-dispose
                                            kamar.THUMBNAIL = new Bitmap(Image.FromStream(ms));
                                        }
                                    }
                                    catch
                                    {
                                        kamar.THUMBNAIL = null; // Gambar gagal load → null
                                    }
                                }

                                fullListKamar.Add(kamar);
                            }

                            // Tampilkan sesuai filter yang sudah dipilih
                            ApplyFilters();

                            // Sembunyikan kolom URL (hanya teks, tidak perlu tampil)
                            if (dataGridView1.Columns["ThumbnailUrl"] != null)
                                dataGridView1.Columns["ThumbnailUrl"].Visible = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan: " + ex.Message);
                }
            }
        }

        // ── Fungsi terpusat: Terapkan semua filter sekaligus ──────────────
        private void ApplyFilters()
        {
            string keyword       = txtSearch.Text.ToLower().Trim();
            string selectedStatus = guna2ComboBox1.SelectedItem?.ToString() ?? "Semua Status";
            string selectedType  = guna2ComboBox2.SelectedItem?.ToString() ?? "Semua Tipe";

            // Mulai dari data lengkap (fullListKamar)
            var filtered = fullListKamar.AsEnumerable();

            // Filter 1: Status kamar
            if (selectedStatus != "Semua Status")
            {
                filtered = filtered.Where(k =>
                    k.STATUS != null &&
                    k.STATUS.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase));
            }

            // Filter 2: Tipe kamar
            if (selectedType != "Semua Tipe")
            {
                filtered = filtered.Where(k =>
                    k.TYPE != null &&
                    k.TYPE.Equals(selectedType, StringComparison.OrdinalIgnoreCase));
            }

            // Filter 3: Kata kunci pencarian (nomor kamar / tipe)
            if (!string.IsNullOrEmpty(keyword))
            {
                filtered = filtered.Where(k =>
                    (k.ROOM != null && k.ROOM.ToLower().Contains(keyword)) ||
                    (k.TYPE != null && k.TYPE.ToLower().Contains(keyword))
                );
            }

            // Update DataGridView dengan hasil filter
            bindingListKamar = new BindingList<Kamar>(filtered.ToList());
            dataGridView1.DataSource = bindingListKamar;
        }

        // ── Event handlers: Setiap perubahan filter memanggil ApplyFilters ─
        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }
    }
}
```

---

## 5.4 — Penjelasan Teknis Penting

### 🗄️ Dua List: `fullListKamar` vs `bindingListKamar`

```
fullListKamar  ← Data MASTER dari server. Tidak pernah diubah. Selalu utuh.
      │
      │  ApplyFilters() membuat salinan yang sudah difilter
      ▼
bindingListKamar  ← Data yang TAMPIL di DataGridView. Berubah setiap filter.
```

Pendekatan ini penting agar saat user menghapus filter, data asli masih ada di `fullListKamar`.

### 🖼️ Kenapa Dibuat `new Bitmap()` dari Stream?

```csharp
// ❌ BERMASALAH — stream di-dispose, gambar hilang dari memori
kamar.THUMBNAIL = Image.FromStream(ms);

// ✅ BENAR — Bitmap baru dibuat dari data gambar (independen dari stream)
kamar.THUMBNAIL = new Bitmap(Image.FromStream(ms));
```

### 🔗 Chaining Filter dengan LINQ

```csharp
var filtered = fullListKamar.AsEnumerable();  // Sumber

// Setiap .Where() menambahkan kondisi tanpa replace yang sebelumnya
filtered = filtered.Where(k => k.STATUS == "Tersedia");  // Tambah filter status
filtered = filtered.Where(k => k.TYPE == "Premium");      // Tambah filter tipe
filtered = filtered.Where(k => k.ROOM.Contains("A"));    // Tambah filter kata kunci

// Eksekusi nyata baru terjadi di sini (lazy evaluation):
var result = filtered.ToList();
```

---

## 5.5 — Nilai Status Kamar

| Status | Arti |
|--------|------|
| `"Tersedia"` | Kamar kosong, bisa disewa |
| `"Penuh"` | Sedang ditempati penyewa |
| `"Perbaikan"` | Sedang dalam maintenance |

---

## 5.6 — GalleryForm.cs (Skeleton)

```csharp
// File: GalleryForm.cs
// Status: SKELETON — belum ada implementasi
namespace Kost_SiguraGura
{
    public partial class GalleryForm : UserControl
    {
        public GalleryForm()
        {
            InitializeComponent();
        }
    }
}
```

### 📌 Rencana Implementasi Gallery

```csharp
// Implementasi yang disarankan untuk GalleryForm.cs:
private async void GalleryForm_Load(object sender, EventArgs e)
{
    string url = "https://rahmatzaw.elarisnoir.my.id/api/kamar";
    var response = await ApiClient.Client.GetAsync(url);

    if (response.IsSuccessStatusCode)
    {
        string json = await response.Content.ReadAsStringAsync();
        var list = JsonConvert.DeserializeObject<List<Kamar>>(json);

        foreach (var kamar in list)
        {
            if (!string.IsNullOrEmpty(kamar.ThumbnailUrl))
            {
                // Buat PictureBox per kamar dan tambahkan ke FlowLayoutPanel
                var pb = new PictureBox
                {
                    Size = new Size(200, 150),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    ImageLocation = kamar.ThumbnailUrl
                };
                pb.LoadAsync(); // Load gambar secara async
                flowLayoutPanel1.Controls.Add(pb);
            }
        }
    }
}
```

---

## ⏭️ Langkah Berikutnya
Setelah manajemen kamar, pelajari bagaimana mengelola data orang yang menyewa kamar di **[Modul 6 — Manajemen Penyewa](./06_penyewa.md)**.
