# 02 - TUTORIAL MODUL NAVIGASI & SIDEBAR

**File Terkait:** `Sidebar.cs`, `Sidebar.Designer.cs`, `MainForm.cs`

---

## 📖 Pendahuluan

Modul Navigasi & Sidebar adalah fondasi antarmuka utama (UI) yang bertindak sebagai "menu kendali" aplikasi. Di tutorial ini, kita akan mempelajari cara membuat sistem navigasi dinamis, di mana user dapat berpindah menu (Beranda, Kamar, Penyewa, dll) tanpa harus membuka window baru, melainkan mengganti *UserControl* di dalam satu panel utama.

### Fungsi Utama
1. **Menu Navigation** - Tombol/link untuk navigasi antar modul.
2. **Dynamic Loading** - Melakukan unload/load modul secara dinamis dalam satu form (`MainForm`).
3. **Active Indicator** - Menyorot tombol menu yang sedang aktif diakses.
4. **Logout Handling** - Proses membersihkan *Session* dan kembali ke form Login.

---

## Langkah 1: Memahami Alur (Flow Diagram)

```text
┌──────────────────────────────────────────────────┐
│   MAINFORM INITIALIZATION                        │
│   1. Load Sidebar & Content Panel di MainForm    │
│   2. Load module default (BerandaPage)           │
└──────────────────────────────────────────────────┘
              ↓
┌──────────────────────────────────────────────────┐
│   USER KLIK MENU DI SIDEBAR (misal: "Kamar")     │
│   1. Unload modul saat ini (Dispose)             │
│   2. Beri warna "Active" ke tombol Kamar         │
│   3. Load modul "DataKamar" ke Content Panel     │
└──────────────────────────────────────────────────┘
```

---

## Langkah 2: Pembuatan Struktur UI

Untuk struktur ini, Anda memerlukan:
1. Sebuah `Form` bernama `MainForm.cs`.
2. Sebuah `UserControl` bernama `Sidebar.cs` yang memuat list `Button` (`btnBeranda`, `btnKamar`, dll) dan `Label` info user.

---

## Langkah 3: Setup Container & Inisialisasi Sidebar (MainForm)

Di `MainForm.cs`, kita perlu mendefinisikan kontainer layout untuk menampung sidebar di sisi kiri dan konten dinamis di sisi kanan.

```csharp
public partial class MainForm : Form
{
    private Sidebar sidebar;
    private Panel contentPanel;
    private UserControl currentModule = null;

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        // 1. Validasi Keamanan (Cegah akses tanpa login)
        if (!Session.IsLoggedIn())
        {
            MessageBox.Show("Session expired. Please login again.");
            this.Close(); return;
        }

        // 2. Setup layout form (Fullscreen)
        this.Text = $"Kos Admin - {Session.UserName}";
        this.WindowState = FormWindowState.Maximized;

        // 3. Setup Layout Kontainer
        CreateAndSetupSidebar();
        CreateContentPanel();

        // 4. Load Modul Default
        LoadModule(new BerandaPage(), "Beranda");
    }
}
```

---

## Langkah 4: Menulis Logika Pembuatan Sidebar & Panel Konten

Masih di dalam `MainForm.cs`, mari implementasikan pembuatan *layout* secara prosedural:

```csharp
private void CreateContentPanel()
{
    contentPanel = new Panel();
    contentPanel.Dock = DockStyle.Fill; // Mengisi penuh sisa layar kanan
    contentPanel.BackColor = Color.White;
    contentPanel.Padding = new Padding(10);
    this.Controls.Add(contentPanel);
}

private void CreateAndSetupSidebar()
{
    sidebar = new Sidebar();
    sidebar.Dock = DockStyle.Left; // Tetap diam di sisi kiri
    sidebar.Width = 250;
    this.Controls.Add(sidebar);
    
    // Binding Event Click Menu
    Button btnKamar = sidebar.Controls.Find("btnKamar", true).FirstOrDefault() as Button;
    if (btnKamar != null)
    {
        btnKamar.Click += (s, e) => 
        {
            LoadModule(new DataKamar(), "Kamar");
            SetActiveMenuButton(btnKamar);
        };
    }
    
    // Binding Tombol Logout
    Button btnLogout = sidebar.Controls.Find("btnLogout", true).FirstOrDefault() as Button;
    if (btnLogout != null)
    {
        btnLogout.Click += (s, e) => HandleLogout();
    }
}
```

---

## Langkah 5: Implementasi Dynamic Module Switching

Ini adalah *core logic* dari navigasi. Fungsinya untuk membersihkan panel dari modul lama, lalu me-*load* (memuat) `UserControl` modul baru.

```csharp
private void LoadModule(UserControl newModule, string moduleName)
{
    try
    {
        // 1. Cleanup modul lama (Cegah memory leak)
        if (currentModule != null)
        {
            currentModule.Dispose();
            currentModule = null;
        }

        contentPanel.Controls.Clear();

        // 2. Load modul baru
        newModule.Dock = DockStyle.Fill;
        contentPanel.Controls.Add(newModule);
        currentModule = newModule;

        this.Text = $"Kos Admin - {moduleName} | {Session.UserName}";
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Gagal membuka modul {moduleName}: {ex.Message}");
    }
}
```

Untuk memberi *Feedback Visual* (tombol mana yang sedang aktif):

```csharp
private void SetActiveMenuButton(Button activeButton)
{
    // Reset semua button (jadikan abu-abu)
    foreach (Control control in sidebar.Controls)
    {
        if (control is Button btn)
        {
            btn.BackColor = Color.LightGray;
            btn.ForeColor = Color.Black;
        }
    }

    // Highlight button yang aktif
    if (activeButton != null)
    {
        activeButton.BackColor = Color.DodgerBlue;
        activeButton.ForeColor = Color.White;
    }
}
```

---

## Langkah 6: Logika Logout

Fungsi untuk membersihkan sesi dan kembali ke `Form1`.

```csharp
private void HandleLogout()
{
    var result = MessageBox.Show("Yakin ingin logout?", "Confirm Logout", MessageBoxButtons.YesNo);
    if (result == DialogResult.Yes)
    {
        Session.Clear(); // Bersihkan memori sesi
        this.Close();    // Tutup MainForm
        new Form1().Show(); // Buka form Login
    }
}
```

---

## Langkah 7: Skenario Pengujian (Testing)

- [ ] **Test Transisi Modul**: Klik modul "Kamar", periksa apakah antarmuka "Kamar" ter-*load*. Kemudian pindah ke modul lain dan periksa apakah modul lama dibersihkan tanpa meninggalkan *error*.
- [ ] **Test Indikator Aktif**: Periksa apakah warna menu berubah ketika diklik (warna biru).
- [ ] **Test Logout**: Pastikan muncul peringatan dialog, dan ketika dipilih "Yes", form tertutup dengan rapi dan mengembalikan user ke halaman login.

---

## 📚 Navigasi Dokumentasi

**← Sebelumnya:** [01_Autentikasi_Login](01_Autentikasi_Login) - Modul Autentikasi & Login  
**Berikutnya →** [03_Dashboard_Beranda](03_Dashboard_Beranda) - Modul Dashboard / Beranda

**Daftar Lengkap:** [README_DOKUMENTASI](README_DOKUMENTASI)