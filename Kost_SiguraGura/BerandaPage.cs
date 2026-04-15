using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Kost_SiguraGura
{
    /// <summary>
    /// BerandaPage - Dashboard dengan real-time data integration
    /// Menampilkan KPI (4 stat cards), Charts, dan Recent Activities
    /// </summary>
    public partial class BerandaPage : UserControl
    {
        // Data cache untuk dashboard
        private List<Pembayaran> allPayments = new List<Pembayaran>();
        private List<Kamar> allRooms = new List<Kamar>();

        public BerandaPage()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void BerandaPage_Load(object sender, EventArgs e)
        {
            // Load semua data dashboard secara parallel
            await LoadAllDashboardData();
        }
        /// <summary>
        /// Load semua dashboard data dari API secara parallel
        /// </summary>
        private async Task LoadAllDashboardData()
        {
            try
            {
                // Load data secara parallel untuk performa lebih baik
                await Task.WhenAll(
                    LoadPaymentsAsync(),
                    LoadRoomsAsync()
                );

                // Update semua UI components dengan data yang sudah ter-load
                UpdateKPICards();
                SetupCharts();
                UpdateDataGrids();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading dashboard: {ex.Message}");
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load payments data dari API
        /// </summary>
        private async Task LoadPaymentsAsync()
        {
            try
            {
                string url = "https://rahmatzaw.elarisnoir.my.id/api/payments";
                HttpResponseMessage response = await ApiClient.Client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.Linq.JToken.Parse(jsonResponse);

                    Newtonsoft.Json.Linq.JToken listRaw = result is Newtonsoft.Json.Linq.JArray ? result : (result["pembayarans"] ?? result["data"] ?? result);

                    if (listRaw != null)
                    {
                        allPayments = listRaw.ToObject<List<Pembayaran>>() ?? new List<Pembayaran>();
                        System.Diagnostics.Debug.WriteLine($"✅ Loaded {allPayments.Count} payments");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading payments: {ex.Message}");
            }
        }

        /// <summary>
        /// Load rooms data dari API
        /// </summary>
        private async Task LoadRoomsAsync()
        {
            try
            {
                string url = "https://rahmatzaw.elarisnoir.my.id/api/kamar";
                HttpResponseMessage response = await ApiClient.Client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.Linq.JToken.Parse(jsonResponse);

                    Newtonsoft.Json.Linq.JToken listRaw = result is Newtonsoft.Json.Linq.JArray ? result : (result["kamars"] ?? result["data"] ?? result);

                    if (listRaw != null)
                    {
                        allRooms = listRaw.ToObject<List<Kamar>>() ?? new List<Kamar>();
                        System.Diagnostics.Debug.WriteLine($"✅ Loaded {allRooms.Count} rooms");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading rooms: {ex.Message}");
            }
        }

        /// <summary>
        /// Update KPI Cards (4 stat cards) dengan data terbaru dari API
        /// Card 1: Total Income, Card 2: Active Tenants
        /// Card 3: Available Rooms, Card 4: Pending Payments
        /// </summary>
        private void UpdateKPICards()
        {
            this.Invoke((MethodInvoker)delegate {
                try
                {
                    // ===== CARD 1: TOTAL INCOME =====
                    // Sum semua payments yang statusnya "Confirmed"
                    decimal totalRevenue = allPayments
                        .Where(p => p.StatusPembayaran != null && 
                                    p.StatusPembayaran.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
                        .Sum(p => p.JumlahBayar);
                    lblIncome.Text = FormatKeRupiahSingkat((long)totalRevenue);

                    // ===== CARD 2: ACTIVE TENANTS =====
                    // Count distinct tenants dari pemesanan yang active
                    int activeTenants = allPayments
                        .Where(p => p.Pemesanan != null && 
                                    p.Pemesanan.StatusPemesanan != null &&
                                    p.Pemesanan.StatusPemesanan.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        .Select(p => p.Pemesanan.PenyewaId)
                        .Distinct()
                        .Count();
                    guna2HtmlLabel13.Text = activeTenants.ToString();
                    guna2HtmlLabel16.Text = $"Total {activeTenants} active tenant(s)";

                    // ===== CARD 3: AVAILABLE ROOMS =====
                    // Count rooms dengan status "Tersedia" atau "Available"
                    int availableRooms = allRooms
                        .Where(r => r.STATUS != null && 
                                   (r.STATUS.Equals("Tersedia", StringComparison.OrdinalIgnoreCase) ||
                                    r.STATUS.Equals("Available", StringComparison.OrdinalIgnoreCase)))
                        .Count();
                    int totalRooms = allRooms.Count;
                    guna2HtmlLabel14.Text = availableRooms.ToString();
                    guna2HtmlLabel17.Text = $"From {totalRooms} room(s)";

                    // ===== CARD 4: PENDING PAYMENTS =====
                    // Count payments dengan status "Pending"
                    int pendingPayments = allPayments
                        .Where(p => p.StatusPembayaran != null &&
                                    p.StatusPembayaran.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                        .Count();
                    guna2HtmlLabel15.Text = pendingPayments.ToString();

                    string pendingStatus = pendingPayments == 0 ? "All bills paid" : $"{pendingPayments} payment(s) awaiting";
                    guna2HtmlLabel18.Text = pendingStatus;

                    System.Diagnostics.Debug.WriteLine($"✅ KPI Updated - Revenue: Rp{totalRevenue:N0}, Tenants: {activeTenants}, Available: {availableRooms}, Pending: {pendingPayments}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error updating KPI cards: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Setup charts dengan data dari API
        /// chart1 = Monthly Revenue Trend (Line Chart)
        /// chart2 = Room Occupancy Status (Pie Chart)
        /// </summary>
        private void SetupCharts()
        {
            try
            {
                SetupMonthlyRevenueChart();
                SetupRoomOccupancyChart();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error setting up charts: {ex.Message}");
            }
        }

        /// <summary>
        /// Setup Monthly Revenue Trend Chart (chart1 - Line Chart)
        /// Menampilkan revenue trend untuk 6 bulan terakhir
        /// </summary>
        private void SetupMonthlyRevenueChart()
        {
            try
            {
                // Clear existing data
                chart1.Series.Clear();
                chart1.ChartAreas.Clear();

                // Create chart area
                ChartArea chartArea = new ChartArea("MainArea");
                chartArea.AxisX.Title = "Month";
                chartArea.AxisY.Title = "Revenue (Rp)";
                chart1.ChartAreas.Add(chartArea);

                // Create line series untuk revenue
                Series revenueSeries = new Series("Revenue")
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 2,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 6
                };
                chart1.Series.Add(revenueSeries);

                // Get monthly revenue untuk 6 bulan terakhir
                var monthlyRevenue = GetMonthlyRevenue(6);

                foreach (var month in monthlyRevenue)
                {
                    revenueSeries.Points.AddXY(month.Key, month.Value);
                }

                System.Diagnostics.Debug.WriteLine($"✅ Monthly Revenue Chart Updated - {monthlyRevenue.Count} months");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error setting up revenue chart: {ex.Message}");
            }
        }

        /// <summary>
        /// Setup Room Occupancy Chart (chart2 - Pie Chart)
        /// Menampilkan status kamar: Occupied, Available, Maintenance
        /// </summary>
        private void SetupRoomOccupancyChart()
        {
            try
            {
                // Clear existing data
                chart2.Series.Clear();
                chart2.ChartAreas.Clear();

                // Create chart area
                ChartArea chartArea = new ChartArea("MainArea");
                chart2.ChartAreas.Add(chartArea);

                // Create pie series untuk occupancy
                Series occupancySeries = new Series("Occupancy")
                {
                    ChartType = SeriesChartType.Pie
                };
                chart2.Series.Add(occupancySeries);

                // Count rooms by status
                int occupied = allRooms.Where(r => r.STATUS != null && 
                                                   (r.STATUS.Equals("Penuh", StringComparison.OrdinalIgnoreCase) ||
                                                    r.STATUS.Equals("Full", StringComparison.OrdinalIgnoreCase) ||
                                                    r.STATUS.Equals("Terisi", StringComparison.OrdinalIgnoreCase))).Count();
                int available = allRooms.Where(r => r.STATUS != null && 
                                                    (r.STATUS.Equals("Tersedia", StringComparison.OrdinalIgnoreCase) ||
                                                     r.STATUS.Equals("Available", StringComparison.OrdinalIgnoreCase))).Count();
                int maintenance = allRooms.Where(r => r.STATUS != null && 
                                                      (r.STATUS.Equals("Perbaikan", StringComparison.OrdinalIgnoreCase) ||
                                                       r.STATUS.Equals("Maintenance", StringComparison.OrdinalIgnoreCase))).Count();

                occupancySeries.Points.AddXY("Occupied", occupied);
                occupancySeries.Points.AddXY("Available", available);
                occupancySeries.Points.AddXY("Maintenance", maintenance);

                System.Diagnostics.Debug.WriteLine($"✅ Room Occupancy Chart Updated - Occupied: {occupied}, Available: {available}, Maintenance: {maintenance}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error setting up occupancy chart: {ex.Message}");
            }
        }

        /// <summary>
        /// Get monthly revenue untuk last N months
        /// Return dictionary dengan bulan sebagai key dan revenue sebagai value
        /// </summary>
        private Dictionary<string, decimal> GetMonthlyRevenue(int months)
        {
            var result = new Dictionary<string, decimal>();
            var now = DateTime.Now;

            for (int i = months - 1; i >= 0; i--)
            {
                var date = now.AddMonths(-i);
                string monthKey = date.ToString("MMM yyyy");

                decimal monthRevenue = allPayments
                    .Where(p => p.StatusPembayaran != null &&
                                p.StatusPembayaran.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) &&
                                p.TanggalBayar.HasValue &&
                                p.TanggalBayar.Value.Month == date.Month &&
                                p.TanggalBayar.Value.Year == date.Year)
                    .Sum(p => p.JumlahBayar);

                result[monthKey] = monthRevenue;
            }

            return result;
        }

        /// <summary>
        /// Update DataGridViews dengan recent data
        /// dataGridView1: Recent Payments
        /// dataGridView2: Room Status
        /// </summary>
        private void UpdateDataGrids()
        {
            this.Invoke((MethodInvoker)delegate {
                try
                {
                    // DataGridView 1: Recent Payments (Top 10 terbaru)
                    var recentPayments = allPayments
                        .OrderByDescending(p => p.TanggalBayar)
                        .Take(10)
                        .ToList();

                    dataGridView1.DataSource = recentPayments.Select(p => new
                    {
                        ID = p.Id,
                        TenantName = p.Pemesanan?.Penyewa?.NamaPenyewa ?? "Unknown",
                        Amount = $"Rp {p.JumlahBayar:N0}",
                        Status = p.StatusPembayaran ?? "Unknown",
                        Date = p.TanggalBayar?.ToString("dd/MM/yyyy") ?? "N/A",
                        RoomType = p.Pemesanan?.Kamar?.TYPE ?? "N/A"
                    }).ToList();

                    // DataGridView 2: Room Status (Top 10)
                    var roomStatus = allRooms.Take(10).ToList();

                    dataGridView2.DataSource = roomStatus.Select(r => new
                    {
                        RoomNo = r.ROOM,
                        Type = r.TYPE,
                        Status = r.STATUS,
                        Price = $"Rp {r.PRICE:N0}/month",
                        Floor = r.FLOOR,
                        Capacity = r.KAPASITAS
                    }).ToList();

                    System.Diagnostics.Debug.WriteLine($"✅ DataGrids Updated - Payments: {recentPayments.Count}, Rooms: {roomStatus.Count}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error updating DataGrids: {ex.Message}");
                }
            });
        }

        private string FormatKeRupiahSingkat(long nominal)
        {
            var culture = new CultureInfo("id-ID");

            if (nominal >= 1000000)
            {
                double juta = (double)nominal / 1000000;
                return string.Format(culture, "Rp {0:0.#} jt", juta);
            }
            else if (nominal >= 1000)
            {
                double ribu = (double)nominal / 1000;
                return string.Format(culture, "Rp {0:0.#} rb", ribu);
            }

            return nominal.ToString("C0", culture);
        }

    }
}

