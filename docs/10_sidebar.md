# 🧭 Modul 10 — Navigasi (Sidebar)

Modul ini adalah **main shell** atau kerangka utama aplikasi setelah login. Sidebar mengelola navigasi antar halaman dan menampilkan identitas user yang sedang login.

---

## 📁 File yang Terlibat

| File | Tipe | Fungsi |
|------|------|--------|
| `Sidebar.cs` | Form | Shell utama aplikasi + navigasi |
| `Sidebar.Designer.cs` | Auto-generated | Komponen UI |
| `Session.cs` | State | Menampilkan nama user di sidebar |

---

## 10.1 — Tombol Navigasi Sidebar

| Tombol | Nama | Halaman yang Ditampilkan |
|--------|------|--------------------------|
| `guna2Button1` | 🏠 Beranda | `BerandaPage` |
| `guna2Button2` | 🛏️ Kamar | `DataKamar` |
| `guna2Button3` | 👥 Penyewa | `DataPenyewa` |
| `guna2Button4` | 💳 Pembayaran | `PembayaranForm` |
| `guna2Button5` | 📊 Laporan | `Report` |
| `guna2Button6` | 🖼️ Galeri | `GalleryForm` |
| `guna2Button7` | 🔓 Logout | Tutup Sidebar, buka Form1 |

---

## 10.2 — Sistem Icon Sidebar (Active / Inactive)

```
Kondisi INACTIVE (tidak dipilih):
  ├─ FillColor     = Color.Transparent
  ├─ ForeColor     = Color.Gray
  ├─ BorderThickness = 0px
  └─ Icon          = versi _gray  (abu-abu)

Kondisi ACTIVE (sedang dipilih):
  ├─ FillColor     = #FBBF24 dengan opacity 40 (amber transparan)
  ├─ ForeColor     = #FBBF24 (amber kuning)
  ├─ BorderLeft    = 4px solid #FBBF24
  └─ Icon          = versi _yellow (kuning)
```

Resources yang dibutuhkan:
- `home_gray` / `home_yellow`
- `room_gray` / `room_yellow`
- `tenant_gray` / `tenant_yellow`
- `payment_gray` / `payment_yellow`
- `report_gray` / `report_yellow`
- `gallery_gray` / `gallery_yellow`

---

## 10.3 — Kode Lengkap Sidebar.cs

```csharp
// File: Sidebar.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    public partial class Sidebar : Form
    {
        public Sidebar()
        {
            InitializeComponent();

            // Tampilkan username dari session di label sidebar
            lblName.Text = Session.Username;
        }

        // ── Event: Sidebar selesai dimuat ─────────────────────────────────
        private void Sidebar_Load(object sender, EventArgs e)
        {
            // Default: tampilkan halaman Beranda saat pertama buka
            ShowUc(new BerandaPage());
            SetActiveButton(guna2Button1);
        }

        // ── Navigasi: Tampilkan UserControl di area konten utama ──────────
        void ShowUc(UserControl uc)
        {
            // Bersihkan konten sebelumnya
            guna2Panel2.Controls.Clear();

            // UserControl akan mengisi seluruh panel
            uc.Dock = DockStyle.Fill;

            // Tambahkan ke panel konten
            guna2Panel2.Controls.Add(uc);
        }

        // ── Ubah tampilan tombol menjadi "aktif" ──────────────────────────
        private void SetActiveButton(object sender)
        {
            Color amberYellow = ColorTranslator.FromHtml("#FBBF24");

            // ── Reset SEMUA tombol ke kondisi default (inactive) ──────────
            foreach (Control c in guna2Panel1.Controls)
            {
                if (c is Guna.UI2.WinForms.Guna2Button btn)
                {
                    btn.FillColor = Color.Transparent;
                    btn.ForeColor = Color.Gray;
                    btn.CustomBorderThickness = new Padding(0, 0, 0, 0);

                    // Kembalikan ikon ke versi abu-abu
                    if (btn.Name == "guna2Button1") btn.Image = Properties.Resources.home_gray;
                    if (btn.Name == "guna2Button2") btn.Image = Properties.Resources.room_gray;
                    if (btn.Name == "guna2Button3") btn.Image = Properties.Resources.tenant_gray;
                    if (btn.Name == "guna2Button4") btn.Image = Properties.Resources.payment_gray;
                    if (btn.Name == "guna2Button5") btn.Image = Properties.Resources.report_gray;
                    if (btn.Name == "guna2Button6") btn.Image = Properties.Resources.gallery_gray;
                }
            }

            // ── Tandai tombol yang diklik sebagai ACTIVE ──────────────────
            Guna.UI2.WinForms.Guna2Button selectedBtn = (Guna.UI2.WinForms.Guna2Button)sender;

            selectedBtn.FillColor = Color.FromArgb(40, amberYellow); // Transparan 40/255
            selectedBtn.ForeColor = amberYellow;
            selectedBtn.CustomBorderColor = amberYellow;
            selectedBtn.CustomBorderThickness = new Padding(4, 0, 0, 0); // Border kiri saja

            // Ganti ikon ke versi kuning
            if (selectedBtn.Name == "guna2Button1") selectedBtn.Image = Properties.Resources.home_yellow;
            if (selectedBtn.Name == "guna2Button2") selectedBtn.Image = Properties.Resources.room_yellow;
            if (selectedBtn.Name == "guna2Button3") selectedBtn.Image = Properties.Resources.tenant_yellow;
            if (selectedBtn.Name == "guna2Button4") selectedBtn.Image = Properties.Resources.payment_yellow;
            if (selectedBtn.Name == "guna2Button5") selectedBtn.Image = Properties.Resources.report_yellow;
            if (selectedBtn.Name == "guna2Button6") selectedBtn.Image = Properties.Resources.gallery_yellow;
        }

        // ── Event handlers navigasi ───────────────────────────────────────
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            ShowUc(new BerandaPage());
            SetActiveButton(sender);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ShowUc(new DataKamar());
            SetActiveButton(sender);
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            ShowUc(new DataPenyewa());
            SetActiveButton(sender);
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            ShowUc(new PembayaranForm());
            SetActiveButton(sender);
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            ShowUc(new Report());
            SetActiveButton(sender);
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            ShowUc(new GalleryForm());
            SetActiveButton(sender);
        }

        // ── Logout ────────────────────────────────────────────────────────
        private void guna2Button7_Click(object sender, EventArgs e)
        {
            this.Close();       // Tutup main shell
            new Form1().Show(); // Buka kembali halaman login
        }
    }
}
```

---

## 10.4 — Penjelasan Teknis Penting

### 🔄 Pattern ShowUc() — Single-Page Application Style

```csharp
void ShowUc(UserControl uc)
{
    guna2Panel2.Controls.Clear(); // ← Hapus halaman lama
    uc.Dock = DockStyle.Fill;     // ← Halaman baru mengisi seluruh panel
    guna2Panel2.Controls.Add(uc); // ← Tampilkan yang baru
}
```

**Efeknya:** Setiap klik navigasi = halaman lama dihancurkan, halaman baru dibuat ulang dari awal. Ini berarti data selalu fresh (fetch ulang dari API).

**Trade-off:**
- ✅ Data selalu up-to-date
- ❌ Tidak ada caching → setiap pindah halaman = request API lagi

### 🎨 Color Scheme Sidebar

```csharp
// Warna tema: Amber Yellow (#FBBF24)
Color amberYellow = ColorTranslator.FromHtml("#FBBF24");

// Background active: transparan 40/255 ≈ 16% opacity
Color.FromArgb(40, amberYellow)
// ↑ Alpha=40, R=251, G=191, B=36 (dari #FBBF24)
```

### 🖇️ Hubungan Sidebar dengan UserControl

```
Sidebar (Form)
├── guna2Panel1  ← Sidebar kiri (tombol navigasi)
│       ├── guna2Button1 (Beranda)
│       ├── guna2Button2 (Kamar)
│       ├── ...
│       └── guna2Button7 (Logout)
│
└── guna2Panel2  ← Area konten kanan
        └── [UserControl yang aktif]
                ├── BerandaPage
                ├── DataKamar
                ├── DataPenyewa
                ├── PembayaranForm
                ├── Report
                └── GalleryForm
```

---

## 10.5 — Alur Tampilan Visual Sidebar

```
┌──────────────────────────────────────────────────────────┐
│                   KOST SIGURA GURA                      │
│  ┌────────────────┬────────────────────────────────────┐ │
│  │  👤 Admin      │                                    │ │
│  │─────────────── │       Area Konten Utama            │ │
│  │ ▶ 🏠 Beranda   │       (guna2Panel2)                │ │
│  │   🛏️ Kamar     │                                    │ │
│  │   👥 Penyewa   │   [UserControl yang sedang aktif]   │ │
│  │   💳 Bayar     │                                    │ │
│  │   📊 Laporan   │                                    │ │
│  │   🖼️ Galeri    │                                    │ │
│  │─────────────── │                                    │ │
│  │   🔓 Logout   │                                    │ │
│  └────────────────┴────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────┘
       Sidebar kiri              Konten kanan
     (guna2Panel1)              (guna2Panel2)
```

---

## 🏠 Kembali ke Awal
Semua modul telah dibahas. Anda bisa kembali meninjau arsitektur global di **[Overview Proyek](./00_overview.md)**.
