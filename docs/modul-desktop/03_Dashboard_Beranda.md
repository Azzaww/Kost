# 03 - TUTORIAL MODUL DASHBOARD / BERANDA

**File Terkait:** `BerandaPage.cs`, `DashboardStats.cs`, `DataSyncManager.cs`

---

## 📖 Pendahuluan

Modul Dashboard/Beranda adalah "Command Center" aplikasi Anda. Saat user pertama kali *login*, halaman ini yang akan muncul. Tutorial ini akan mengajarkan cara merancang dashboard untuk memanggil banyak data API secara asinkron (*parallel*), mengkalkulasi matriks ringkasan (Total Revenue, dll), dan memvisualisasikannya menjadi *Chart* dan *Grid*.

### Fungsi Utama
1. **KPI Cards** - 4 Matriks ringkasan cepat: Total Pendapatan, Rata-rata Pendapatan, Penyewa Aktif, Pembayaran Tertunda.
2. **Parallel Loading** - Pemanggilan API serentak menggunakan `Task.WhenAll` agar performa lebih cepat.
3. **Data Visualization** - Merender Grafik *Line Chart* (Tren Pendapatan) & *Pie Chart* (Okupansi Kamar).

---

## Langkah 1: Memahami Alur (Flow Diagram)

```text
┌─────────────────────────────────────────────────────────┐
│  USER MASUK KE BERANDA (Load Module)                    │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│  Tampilkan Loading Indicator (Spinner/ProgressBar)      │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│  PARALLEL FETCH DATA DARI API (async)                   │
│  - Task 1: Fetch Data Payments                          │
│  - Task 2: Fetch Data Rooms                             │
│  - Task 3: Fetch Data Tenants                           │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│  HITUNG STATISTIK / METRIK (Data Processing)            │
│  Sembunyikan Loading Indicator                          │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│  UPDATE UI KONTROL (KPI Cards, Charts, & DataGrid)      │
└─────────────────────────────────────────────────────────┘
```

---

## Langkah 2: Pembuatan Struktur UI

Buat `UserControl` bernama `BerandaPage.cs`. Di dalamnya, tambahkan:
- 4 Buah Label untuk KPI (contoh: `lblTotalRevenue`, `lblAvgIncome`).
- 2 Buah komponen `Chart` (contoh: `chartRevenue`, `chartOccupancy`).
- 1 Buah `DataGridView` (contoh: `dataGridPayments` untuk transaksi terbaru).
- 1 Buah ProgressBar (contoh: `progressBar1`) untuk Loading State.

---

## Langkah 3: Setup State & Method Loading Parallel

Mulai dari inisialisasi class. Kita perlu membuat penampung data (*cache*) dan pemanggilan secara paralel.

```csharp
public partial class BerandaPage : UserControl
{
    private List<Pembayaran> allPayments = new List<Pembayaran>();
    private List<Kamar> allRooms = new List<Kamar>();
    private List<Penyewa> allTenants = new List<Penyewa>();

    public BerandaPage()
    {
        InitializeComponent();
    }

    private async void BerandaPage_Load(object sender, EventArgs e)
    {
        await LoadAllDashboardData();
    }

    // --- Eksekusi Parallel ---
    private async Task LoadAllDashboardData()
    {
        try
        {
            ShowLoadingState(); // Tampilkan ProgressBar
            
            // Eksekusi API Calls serentak (Parallel Async)
            await Task.WhenAll(
                LoadPaymentsAsync(),
                LoadRoomsAsync(),
                LoadTenantsAsync()
            );

            // Jika semua selesai, Update UI
            UpdateKPICards();
            SetupCharts();
            UpdateDataGrids();
            
            HideLoadingState(); // Sembunyikan ProgressBar
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
            HideLoadingState();
        }
    }
}
```
> [!TIP]
> Penggunaan `Task.WhenAll` jauh lebih efisien ketimbang memanggil API satu per satu menggunakan `await` di setiap baris.

---

## Langkah 4: Mengambil Data API (Contoh: Payments)

Tulis metode untuk mengambil masing-masing data dari backend API:

```csharp
private async Task LoadPaymentsAsync()
{
    try
    {
        string url = "https://rahmatzaw.elarisnoir.my.id/api/payments";
        var response = await ApiClient.Client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var result = Newtonsoft.Json.Linq.JToken.Parse(jsonResponse);

            // Ekstrak list JSON (menangani wrapper 'data' / 'pembayarans')
            var listRaw = result is Newtonsoft.Json.Linq.JArray ? result : (result["pembayarans"] ?? result["data"] ?? result);
            allPayments = listRaw.ToObject<List<Pembayaran>>() ?? new List<Pembayaran>();
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error LoadPayments: {ex.Message}");
        allPayments = new List<Pembayaran>();
    }
}
// Buat juga untuk LoadRoomsAsync() dan LoadTenantsAsync()
```

---

## Langkah 5: Mengolah Metrik (KPI Cards)

Gunakan `System.Linq` untuk melakukan rekapitulasi data dan memasukkannya ke elemen Label.

```csharp
private void UpdateKPICards()
{
    // Total Revenue (Hanya yang confirmed)
    decimal totalRevenue = allPayments.Where(p => p.Status?.ToLower() == "confirmed").Sum(p => p.Amount ?? 0);
    lblTotalRevenue.Text = $"Rp {totalRevenue:N0}";

    // Average Income (Contoh: rata-rata 6 bulan)
    decimal avgIncome = totalRevenue / 6;
    lblAvgIncome.Text = $"Rp {avgIncome:N0}";

    // Active Tenants
    int activeTenants = allTenants.Count(t => t.Status?.ToLower() == "aktif");
    lblActiveTenants.Text = activeTenants.ToString();

    // Pending Payments
    decimal pendingAmount = allPayments.Where(p => p.Status?.ToLower() == "pending").Sum(p => p.Amount ?? 0);
    lblPendingPayments.Text = $"Rp {pendingAmount:N0}";
}
```

---

## Langkah 6: Implementasi Grafik (Charts)

Visualisasikan ke form chart.

```csharp
private void SetupOccupancyChart()
{
    // Kelompokkan ruangan berdasarkan status
    var occupancyData = allRooms.GroupBy(r => r.STATUS)
                                .Select(g => new { Status = g.Key, Count = g.Count() })
                                .ToList();

    chartOccupancy.Series[0].Points.Clear();
    
    foreach (var item in occupancyData)
    {
        chartOccupancy.Series[0].Points.AddXY(item.Status, item.Count);
    }
}
```

---

## Langkah 7: Skenario Pengujian (Testing)

- [ ] **Test Asynchronous Loading**: Buka halaman Beranda, perhatikan apakah indikator loading tampil terlebih dahulu sebelum data mendadak muncul (tanpa nge-*freeze* aplikasi).
- [ ] **Test Kalkulasi Logika**: Coba hitung manual total bayar di server vs kalkulasi di KPI Cards `Total Revenue`.
- [ ] **Test Tampilan Grid**: Pastikan riwayat pembayaran terbaru muncul dengan benar di `DataGridView`.

---

## 📚 Navigasi Dokumentasi

**← Sebelumnya:** [02_Navigasi_Sidebar](02_Navigasi_Sidebar) - Modul Navigasi & Sidebar  
**Berikutnya →** [04_Manajemen_Kamar_List](04_Manajemen_Kamar_List) - Modul Manajemen Kamar - List View

**Daftar Lengkap:** [README_DOKUMENTASI](README_DOKUMENTASI)