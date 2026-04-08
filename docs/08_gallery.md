# 🖼️ Modul 8 — Galeri Foto Kamar

Modul ini menampilkan foto-foto kamar kost dalam tampilan galeri visual.

---

## 📁 File yang Terlibat

| File | Tipe | Status | Fungsi |
|------|------|--------|--------|
| `GalleryForm.cs` | UserControl | 🚧 Skeleton | UI galeri foto kamar |
| `GalleryForm.Designer.cs` | Auto-generated | - | Komponen UI |
| `Kamar.cs` | Model | ✅ Aktif | Sumber data URL foto (`ThumbnailUrl`) |

---

## 8.1 — Konsep Galeri

```
GET /api/kamar
      │
      ▼
List<Kamar> → ambil field ThumbnailUrl
      │
      ▼
Untuk setiap Kamar yang punya ThumbnailUrl:
      │
      └─► Download gambar dari URL
               │
               ▼
           Buat PictureBox → tambahkan ke FlowLayoutPanel
               │
               ▼
           Tampilkan grid foto kamar
```

---

## 8.2 — Kode GalleryForm.cs (Kondisi Saat Ini)

```csharp
// File: GalleryForm.cs
// ⚠️ Status: SKELETON — belum ada implementasi
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

---

## 8.3 — Implementasi yang Disarankan

### Komponen UI yang Perlu Ditambah di Designer

| Komponen | Nama | Fungsi |
|----------|------|--------|
| FlowLayoutPanel | `flowLayoutPanel1` | Container grid foto (wrap otomatis) |
| Panel | `panelTop` | Area header / judul |
| Label | `lblJudul` | Judul "Galeri Kamar" |
| ProgressBar | `progressBar1` | Menampilkan progres loading foto |

### Kode Implementasi Lengkap yang Disarankan

```csharp
// File: GalleryForm.cs (implementasi lengkap yang disarankan)
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    public partial class GalleryForm : UserControl
    {
        public GalleryForm()
        {
            InitializeComponent();
        }

        // ── Event: Load halaman ───────────────────────────────────────────
        private async void GalleryForm_Load(object sender, EventArgs e)
        {
            await LoadGallery();
        }

        // ── Fetch data kamar dan tampilkan fotonya ────────────────────────
        private async Task LoadGallery()
        {
            string url = "https://rahmatzaw.elarisnoir.my.id/api/kamar";

            try
            {
                flowLayoutPanel1.Controls.Clear(); // Bersihkan gallery lama

                var response = await ApiClient.Client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var listKamar = JsonConvert.DeserializeObject<List<Kamar>>(json);

                    if (listKamar == null || listKamar.Count == 0)
                    {
                        MessageBox.Show("Tidak ada data kamar.");
                        return;
                    }

                    // Hitung berapa kamar yang punya foto
                    int totalFoto = listKamar.FindAll(k => !string.IsNullOrEmpty(k.ThumbnailUrl)).Count;
                    int loaded = 0;

                    using (HttpClient imgClient = new HttpClient())
                    {
                        foreach (var kamar in listKamar)
                        {
                            if (string.IsNullOrEmpty(kamar.ThumbnailUrl))
                                continue;

                            // Buat panel kartu untuk setiap kamar
                            Panel card = BuatKartuKamar(kamar);

                            try
                            {
                                // Download gambar
                                byte[] imgBytes = await imgClient.GetByteArrayAsync(kamar.ThumbnailUrl);

                                using (var ms = new System.IO.MemoryStream(imgBytes))
                                {
                                    Image img = new Bitmap(Image.FromStream(ms));

                                    // Temukan PictureBox di dalam card dan set gambarnya
                                    foreach (Control c in card.Controls)
                                    {
                                        if (c is PictureBox pb)
                                            pb.Image = img;
                                    }
                                }
                            }
                            catch
                            {
                                // Gambar gagal load → biarkan kosong
                            }

                            // Tambahkan kartu ke galeri di UI Thread
                            this.Invoke((MethodInvoker)delegate {
                                flowLayoutPanel1.Controls.Add(card);
                            });

                            loaded++;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Gagal load data. Status: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // ── Membuat panel kartu foto untuk satu kamar ─────────────────────
        private Panel BuatKartuKamar(Kamar kamar)
        {
            // Container kartu
            Panel card = new Panel
            {
                Size = new Size(200, 240),
                Margin = new Padding(10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Foto kamar
            PictureBox pb = new PictureBox
            {
                Size = new Size(200, 150),
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.LightGray // Placeholder sebelum gambar load
            };

            // Label nomor kamar
            Label lblNomor = new Label
            {
                Text = "Kamar " + kamar.ROOM,
                Location = new Point(5, 155),
                Size = new Size(190, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            // Label tipe & harga
            Label lblTipe = new Label
            {
                Text = kamar.TYPE + " — Rp " + kamar.PRICE.ToString("N0"),
                Location = new Point(5, 175),
                Size = new Size(190, 20),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };

            // Label status kamar
            Label lblStatus = new Label
            {
                Text = kamar.STATUS,
                Location = new Point(5, 200),
                Size = new Size(190, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = GetStatusColor(kamar.STATUS)
            };

            card.Controls.Add(pb);
            card.Controls.Add(lblNomor);
            card.Controls.Add(lblTipe);
            card.Controls.Add(lblStatus);

            return card;
        }

        // ── Helper: Warna berdasarkan status ─────────────────────────────
        private Color GetStatusColor(string status)
        {
            switch (status?.ToLower())
            {
                case "tersedia":  return Color.Green;
                case "penuh":     return Color.Red;
                case "perbaikan": return Color.Orange;
                default:          return Color.Gray;
            }
        }
    }
}
```

---

## 8.4 — Tampilan Kartu Kamar

```
┌──────────────────────────┐
│                          │
│      [Foto Kamar]        │  ← PictureBox (200×150)
│       (150px tinggi)     │
│                          │
├──────────────────────────│
│ Kamar A-101              │  ← Label nomor kamar (Bold)
│ Standard — Rp 800.000    │  ← Label tipe & harga (Gray)
│ Tersedia                 │  ← Label status (Hijau/Merah/Orange)
└──────────────────────────┘
     200px lebar
```

---

## 8.5 — Catatan Performa

```
Masalah: Jika ada 50+ kamar, download semua gambar sekaligus = lambat

Solusi: Lazy loading dengan PictureBox.LoadAsync()
```

```csharp
// Alternatif lebih ringan: Muat gambar secara async via PictureBox bawaan
PictureBox pb = new PictureBox
{
    SizeMode = PictureBoxSizeMode.Zoom,
    Size = new Size(200, 150)
};

// LoadAsync() → PictureBox handle download sendiri, tidak blocking UI
pb.LoadAsync(kamar.ThumbnailUrl);
flowLayoutPanel1.Controls.Add(pb);
```

---

## ⏭️ Langkah Berikutnya
Gunakan data yang ada untuk menghasilkan ringkasan laporan di **[Modul 9 — Laporan](./09_laporan.md)**.
