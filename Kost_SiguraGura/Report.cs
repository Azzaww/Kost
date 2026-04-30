using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Kost_SiguraGura
{
    public partial class Report : UserControl
    {
        private List<Pembayaran> allPayments = new List<Pembayaran>();
        private List<Kamar> allRooms = new List<Kamar>();
        private DateTime selectedStartDate = DateTime.Now.AddMonths(-6);
        private DateTime selectedEndDate = DateTime.Now;

        // Data synchronization manager
        private DataSyncManager _dataSyncManager;
        // Auto-sync interval taken from SyncConfiguration (default 15 seconds)

        public Report()
        {
            InitializeComponent();
        }

        private void Report_Load(object sender, EventArgs e)
        {
            // Initialize sync configuration (load from app.config if available)
            SyncConfiguration.Initialize();

            LoadAllReportData();
            InitializeAutoSync();
        }

        private void InitializeAutoSync()
        {
            try
            {
                // Initialize sync manager with configured interval (or use default)
                _dataSyncManager = new DataSyncManager();

                // Subscribe to refresh events
                _dataSyncManager.PaymentsRefreshed += DataSyncManager_PaymentsRefreshed;
                _dataSyncManager.RoomsRefreshed += DataSyncManager_RoomsRefreshed;
                _dataSyncManager.SyncStatusChanged += DataSyncManager_SyncStatusChanged;
                _dataSyncManager.SyncErrorOccurred += DataSyncManager_SyncErrorOccurred;

                // Start automatic synchronization
                _dataSyncManager.StartAutoSync();

                System.Diagnostics.Debug.WriteLine("Auto-sync initialized for Report data");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing auto-sync: {ex.Message}");
            }
        }

        private void DataSyncManager_PaymentsRefreshed(object sender, DataRefreshEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[Report] DataSyncManager_PaymentsRefreshed triggered - Success: {e.Success}, DataCount: {e.DataCount}");

            if (!e.Success) return;

            // Fetch latest payments data asynchronously
            Task.Run(async () =>
            {
                try
                {
                    await LoadPaymentsAsync();

                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            UpdateReportStatCards();
                            SetupCharts();
                        }));
                    }
                    else
                    {
                        UpdateReportStatCards();
                        SetupCharts();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Report] Error in PaymentsRefreshed handler: {ex.Message}");
                }
            });
        }

        private void DataSyncManager_RoomsRefreshed(object sender, DataRefreshEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[Report] DataSyncManager_RoomsRefreshed triggered - Success: {e.Success}, DataCount: {e.DataCount}");

            if (!e.Success) return;

            // Fetch latest rooms data asynchronously
            Task.Run(async () =>
            {
                try
                {
                    await LoadRoomsAsync();

                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            UpdateReportStatCards();
                            SetupCharts();
                        }));
                    }
                    else
                    {
                        UpdateReportStatCards();
                        SetupCharts();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Report] Error in RoomsRefreshed handler: {ex.Message}");
                }
            });
        }

        private void DataSyncManager_SyncStatusChanged(object sender, SyncStatusChangedEventArgs e)
        {
            // Log sync status changes
            System.Diagnostics.Debug.WriteLine($"[Sync Status] {e.Status}: {e.Message}");
        }

        private void DataSyncManager_SyncErrorOccurred(object sender, SyncErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[Sync Error] {e.Message}: {e.Exception?.Message}");
        }

        private async void LoadAllReportData()
        {
            try
            {
                await Task.WhenAll(
                    LoadPaymentsAsync(),
                    LoadRoomsAsync()
                );

                UpdateReportStatCards();
                SetupCharts();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading report data: {ex.Message}");
            }
        }

        private async Task LoadPaymentsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] Starting payment data fetch...");
                var response = await ApiClient.Client.GetAsync("https://rahmatzaw.elarisnoir.my.id/api/payments");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    allPayments = JsonConvert.DeserializeObject<List<Pembayaran>>(content) ?? new List<Pembayaran>();

                    System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] Successfully loaded {allPayments.Count} payments from API");

                    // Log first few payments for debugging
                    foreach (var p in allPayments.Take(3))
                    {
                        System.Diagnostics.Debug.WriteLine($"  - Payment ID: {p.Id}, Status: {p.StatusPembayaran}, Amount: {p.JumlahBayar}, Date: {p.TanggalBayar}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] API returned error status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] Error loading payments: {ex.Message}");
            }
        }

        private async Task LoadRoomsAsync()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync("https://rahmatzaw.elarisnoir.my.id/api/kamar");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    allRooms = JsonConvert.DeserializeObject<List<Kamar>>(content) ?? new List<Kamar>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading rooms: {ex.Message}");
            }
        }

        private void UpdateReportStatCards()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] Starting calculation with {allPayments.Count} total payments");
                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] Date range filter: {selectedStartDate:yyyy-MM-dd} to {selectedEndDate:yyyy-MM-dd}");

                // Filter payments by date range
                var filteredPayments = allPayments
                    .Where(p => p.TanggalBayar.HasValue && p.TanggalBayar.Value.Date >= selectedStartDate.Date && p.TanggalBayar.Value.Date <= selectedEndDate.Date)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] After date filter: {filteredPayments.Count} payments");

                // Log status breakdown
                var confirmedPayments = filteredPayments.Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed").ToList();
                var pendingPayments = filteredPayments.Where(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending").ToList();
                var otherPayments = filteredPayments.Where(p => p.StatusPembayaran != "Confirmed" && p.StatusPembayaran != "confirmed" && p.StatusPembayaran != "Pending" && p.StatusPembayaran != "pending").ToList();

                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] Status breakdown - Confirmed: {confirmedPayments.Count}, Pending: {pendingPayments.Count}, Other: {otherPayments.Count}");
                if (otherPayments.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] Other statuses found: {string.Join(", ", otherPayments.Select(p => p.StatusPembayaran).Distinct())}");
                }

                // Calculate metrics
                decimal totalRevenue = confirmedPayments.Sum(p => p.JumlahBayar);

                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] Total Revenue Calculation: {confirmedPayments.Count} confirmed payments, Sum = {totalRevenue}");
                if (confirmedPayments.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] Confirmed payment amounts: {string.Join(", ", confirmedPayments.Select(p => p.JumlahBayar))}");
                }

                int pendingPaymentsCount = filteredPayments
                    .Where(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending")
                    .Count();

                decimal avgRate = allRooms.Count > 0 ? allRooms.Average(r => r.PRICE) : 0;

                var occupiedRooms = allRooms.Where(r => r.STATUS == "Penuh" || r.STATUS == "Full").Count();
                var totalRooms = allRooms.Count;
                double occupancyRate = totalRooms > 0 ? (occupiedRooms / (double)totalRooms) * 100 : 0;

                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] Occupancy: {occupiedRooms}/{totalRooms} rooms = {occupancyRate:F1}%");

                // Update stat cards with calculated values
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        // Total Revenue - guna2HtmlLabel12
                        guna2HtmlLabel12.Text = FormatCurrency(totalRevenue);

                        // Pending Payments - guna2HtmlLabel13
                        guna2HtmlLabel13.Text = pendingPaymentsCount.ToString();

                        // Average Rate - guna2HtmlLabel16
                        guna2HtmlLabel16.Text = FormatCurrency(avgRate);

                        // Occupancy Rate - guna2HtmlLabel15
                        guna2HtmlLabel15.Text = $"{occupancyRate:F1} %";

                        // Percentage change from last month
                        var lastMonthPayments = allPayments
                            .Where(p => p.TanggalBayar.HasValue && p.TanggalBayar.Value.Date >= DateTime.Now.AddMonths(-1).Date && p.TanggalBayar.Value.Date < DateTime.Now.Date)
                            .Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed")
                            .Sum(p => p.JumlahBayar);

                        double percentageChange = lastMonthPayments > 0 
                            ? ((double)totalRevenue - (double)lastMonthPayments) / (double)lastMonthPayments * 100
                            : 0;

                        guna2HtmlLabel2.Text = percentageChange >= 0
                            ? $"+ {percentageChange:F1} % from last mth"
                            : $"- {Math.Abs(percentageChange):F1} % from last mth";

                        // Update Payment Status
                        UpdatePaymentStatus(filteredPayments);
                    }));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] Error updating stat cards: {ex.Message}");
            }
        }

        private void UpdatePaymentStatus(List<Pembayaran> filteredPayments)
        {
            try
            {
                // Count transactions and revenue by status
                int confirmedCount = filteredPayments.Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed").Count();
                decimal confirmedRevenue = filteredPayments.Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed").Sum(p => p.JumlahBayar);

                int pendingCount = filteredPayments.Where(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending").Count();
                decimal pendingRevenue = filteredPayments.Where(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending").Sum(p => p.JumlahBayar);

                // Calculate total potential (all rooms fully occupied)
                decimal totalPotentialRevenue = allRooms.Sum(r => r.PRICE);

                // Update Confirmed status labels
                guna2HtmlLabel27.Text = "Dikonfirmasi"; // Status label
                guna2HtmlLabel25.Text = $"{confirmedCount} TRANSAKSI"; // Transaction count
                guna2HtmlLabel24.Text = FormatCurrency(confirmedRevenue); // Amount

                // Update Pending status labels
                guna2HtmlLabel30.Text = "Menunggu"; // Status label
                guna2HtmlLabel29.Text = $"{pendingCount} TRANSAKSI"; // Transaction count
                guna2HtmlLabel28.Text = FormatCurrency(pendingRevenue); // Amount

                // Update Total Potential labels
                guna2HtmlLabel33.Text = "Potensi Total"; // Status label
                guna2HtmlLabel32.Text = "Jika terisi penuh"; // Description
                guna2HtmlLabel31.Text = FormatCurrency(totalPotentialRevenue); // Amount
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating payment status: {ex.Message}");
            }
        }

        private void SetupCharts()
        {
            try
            {
                SetupRevenueByTypeChart();
                SetupRoomDemographicsChart();
                SetupMonthlyTrendChart();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up charts: {ex.Message}");
            }
        }

        private void SetupRevenueByTypeChart()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetupRevenueByTypeChart()));
                return;
            }

            try
            {
                var filteredPayments = allPayments
                    .Where(p => p.TanggalBayar.HasValue && p.TanggalBayar.Value.Date >= selectedStartDate.Date && p.TanggalBayar.Value.Date <= selectedEndDate.Date)
                    .Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed")
                    .ToList();

                var revenueByType = filteredPayments
                    .GroupBy(p => p.Pemesanan?.Kamar?.TYPE ?? "Unknown")
                    .Select(g => new { Type = g.Key, Revenue = g.Sum(p => p.JumlahBayar) })
                    .Where(x => x.Type.Contains("Standard") || x.Type.Contains("Premium") || x.Type.Contains("standard") || x.Type.Contains("premium"))
                    .OrderByDescending(x => x.Revenue)
                    .ToList();

                // Ensure we have exactly 2 entries (Standard and Premium)
                if (revenueByType.Count == 0)
                {
                    revenueByType.Add(new { Type = "Premium", Revenue = 0m });
                    revenueByType.Add(new { Type = "Standard", Revenue = 0m });
                }
                else if (revenueByType.Count == 1)
                {
                    if (revenueByType[0].Type.Contains("Standard") || revenueByType[0].Type.Contains("standard"))
                    {
                        revenueByType.Add(new { Type = "Premium", Revenue = 0m });
                    }
                    else
                    {
                        revenueByType.Insert(0, new { Type = "Standard", Revenue = 0m });
                    }
                }

                // Show chart1 again (was hidden before)
                chart1.Visible = true;

                chart1.Series.Clear();
                chart1.ChartAreas.Clear();

                var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("Default");
                chart1.ChartAreas.Add(chartArea);

                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Revenue",
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column,
                    XValueMember = "Type",
                    YValueMembers = "Revenue"
                };

                series.Points.DataBind(revenueByType.Take(2).ToList(), "Type", "Revenue", "");
                chart1.Series.Add(series);

                // Format Y-axis labels as currency
                chartArea.AxisY.LabelStyle.Format = "#,##0";

                // Setup Revenue Breakdown cards di guna2Panel9 (bagian bawah kiri)
                var revenueBreakdownData = new List<dynamic>();

                var premiumRooms = allRooms.Where(r => r.TYPE.Contains("Premium") || r.TYPE.Contains("premium")).ToList();
                var standardRooms = allRooms.Where(r => r.TYPE.Contains("Standard") || r.TYPE.Contains("standard")).ToList();

                decimal premiumRevenue = filteredPayments
                    .Where(p => p.Pemesanan?.Kamar?.TYPE != null && (p.Pemesanan.Kamar.TYPE.Contains("Premium") || p.Pemesanan.Kamar.TYPE.Contains("premium")))
                    .Sum(p => p.JumlahBayar);

                decimal standardRevenue = filteredPayments
                    .Where(p => p.Pemesanan?.Kamar?.TYPE != null && (p.Pemesanan.Kamar.TYPE.Contains("Standard") || p.Pemesanan.Kamar.TYPE.Contains("standard")))
                    .Sum(p => p.JumlahBayar);

                int premiumOccupied = premiumRooms.Where(r => r.STATUS == "Penuh" || r.STATUS == "Full").Count();
                int standardOccupied = standardRooms.Where(r => r.STATUS == "Penuh" || r.STATUS == "Full").Count();

                revenueBreakdownData.Add(new 
                { 
                    Type = "Premium Rooms", 
                    Revenue = premiumRevenue,
                    Occupied = premiumOccupied,
                    Total = premiumRooms.Count,
                    OccupancyText = $"{premiumOccupied}/{premiumRooms.Count} occupied"
                });

                revenueBreakdownData.Add(new 
                { 
                    Type = "Standard Rooms", 
                    Revenue = standardRevenue,
                    Occupied = standardOccupied,
                    Total = standardRooms.Count,
                    OccupancyText = $"{standardOccupied}/{standardRooms.Count} occupied"
                });

                // Display Revenue Breakdown di guna2Panel9
                DisplayRevenueBreakdownInPanel9(revenueBreakdownData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up revenue by type chart: {ex.Message}");
            }
        }

        private Control FindControlByName(Control container, string name)
        {
            if (container.Name == name)
                return container;

            foreach (Control control in container.Controls)
            {
                var found = FindControlByName(control, name);
                if (found != null)
                    return found;
            }

            return null;
        }

        private void DisplayRevenueBreakdownInPanel9(List<dynamic> revenueData)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"DisplayRevenueBreakdownInPanel9: Starting with {revenueData.Count} items");

                // Find guna2Panel10 menggunakan recursive search (karena nested dalam hierarchy)
                var guna2Panel10 = FindControlByName(this, "guna2Panel10");

                if (guna2Panel10 == null)
                {
                    System.Diagnostics.Debug.WriteLine("guna2Panel10 not found!");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"guna2Panel10 found. Size: {guna2Panel10.Width}x{guna2Panel10.Height}");

                var guna2Panel9 = guna2Panel10.Parent as Control;
                if (guna2Panel9 == null)
                {
                    System.Diagnostics.Debug.WriteLine("guna2Panel9 not found as parent!");
                    return;
                }

                // Hide the "Revenue Breakdown" title label
                var titleLabel = guna2Panel9.Controls.OfType<Guna.UI2.WinForms.Guna2HtmlLabel>()
                    .FirstOrDefault(l => l.Text == "Revenue Breakdown");
                if (titleLabel != null)
                    titleLabel.Visible = false;

                // Clear guna2Panel10 (old revenue breakdown content) dan remove kontrol lama
                System.Diagnostics.Debug.WriteLine($"Clearing guna2Panel10. Current controls: {guna2Panel10.Controls.Count}");
                guna2Panel10.Controls.Clear();
                guna2Panel10.Refresh();

                int yPosition = 5;

                // Display each revenue breakdown card
                foreach (var item in revenueData)
                {
                    System.Diagnostics.Debug.WriteLine($"Creating card for: {item.Type}");

                    // Minimal card design sesuai website
                    var cardPanel = new Guna.UI2.WinForms.Guna2Panel
                    {
                        BorderColor = System.Drawing.Color.FromArgb(230, 230, 230),
                        BorderRadius = 4,
                        BorderThickness = 1,
                        FillColor = System.Drawing.Color.FromArgb(250, 250, 250), // Very light gray
                        Location = new System.Drawing.Point(10, yPosition),
                        Size = new System.Drawing.Size(guna2Panel10.Width - 20, 72),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        Visible = true
                    };

                    // Room type title (left)
                    var roomTypeLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
                    {
                        Text = item.Type,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold),
                        ForeColor = System.Drawing.Color.Black,
                        Location = new System.Drawing.Point(12, 6),
                        Size = new System.Drawing.Size(200, 16),
                        BackColor = System.Drawing.Color.Transparent,
                        Visible = true
                    };
                    cardPanel.Controls.Add(roomTypeLabel);

                    // Revenue amount (right)
                    var amountLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
                    {
                        Text = FormatCurrency((decimal)item.Revenue),
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold),
                        ForeColor = System.Drawing.Color.FromArgb(245, 158, 11), // Orange color
                        AutoSize = false,
                        Location = new System.Drawing.Point(cardPanel.Width - 95, 6),
                        Size = new System.Drawing.Size(85, 16),
                        BackColor = System.Drawing.Color.Transparent,
                        Visible = true
                    };
                    cardPanel.Controls.Add(amountLabel);

                    // Occupancy info (left)
                    var occupancyLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
                    {
                        Text = item.OccupancyText,
                        Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular),
                        ForeColor = System.Drawing.Color.FromArgb(150, 150, 150), // Gray
                        Location = new System.Drawing.Point(12, 23),
                        Size = new System.Drawing.Size(150, 13),
                        BackColor = System.Drawing.Color.Transparent,
                        Visible = true
                    };
                    cardPanel.Controls.Add(occupancyLabel);

                    // Progress bar (full width, minimal height)
                    var progressBar = new ProgressBar
                    {
                        Location = new System.Drawing.Point(12, 40),
                        Size = new System.Drawing.Size(cardPanel.Width - 24, 5),
                        BackColor = System.Drawing.Color.FromArgb(230, 230, 230),
                        ForeColor = System.Drawing.Color.FromArgb(245, 158, 11),
                        Value = (item.Total > 0) ? (int)((item.Occupied / (double)item.Total) * 100) : 0,
                        Visible = true
                    };
                    cardPanel.Controls.Add(progressBar);

                    guna2Panel10.Controls.Add(cardPanel);
                    System.Diagnostics.Debug.WriteLine($"Added card at Y={yPosition}, Total controls in panel now: {guna2Panel10.Controls.Count}");

                    yPosition += 77;
                }

                // Force panel refresh dan auto-layout
                guna2Panel10.PerformLayout();
                guna2Panel10.Refresh();
                guna2Panel9.Refresh();

                System.Diagnostics.Debug.WriteLine($"DisplayRevenueBreakdownInPanel9: Completed. Final control count: {guna2Panel10.Controls.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error displaying revenue breakdown in panel9: {ex.Message}");
            }
        }

        private void SetupRoomDemographicsChart()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetupRoomDemographicsChart()));
                return;
            }

            try
            {
                var roomStatusDistribution = allRooms
                    .GroupBy(r => r.STATUS)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToList();

                chart2.Series.Clear();
                chart2.ChartAreas.Clear();

                var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("Default");
                chart2.ChartAreas.Add(chartArea);

                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Room Status",
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie
                };

                foreach (var item in roomStatusDistribution)
                {
                    series.Points.AddXY(item.Status, item.Count);
                }

                chart2.Series.Add(series);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up demographics chart: {ex.Message}");
            }
        }

        private void SetupMonthlyTrendChart()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetupMonthlyTrendChart()));
                return;
            }

            try
            {
                var monthlyData = GetMonthlyRevenueData();

                chart4.Series.Clear();
                chart4.ChartAreas.Clear();

                var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("Default");
                chart4.ChartAreas.Add(chartArea);

                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Monthly Revenue",
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
                    XValueMember = "Month",
                    YValueMembers = "Revenue"
                };

                series.Points.DataBind(monthlyData, "Month", "Revenue", "");
                chart4.Series.Add(series);

                chartArea.AxisY.LabelStyle.Format = "#,##0";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up monthly trend chart: {ex.Message}");
            }
        }

        private List<dynamic> GetMonthlyRevenueData()
        {
            var result = new List<dynamic>();
            var confirmedPayments = allPayments
                .Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed")
                .ToList();

            for (int i = -5; i <= 0; i++)
            {
                var targetMonth = DateTime.Now.AddMonths(i);
                var monthlyRevenue = confirmedPayments
                    .Where(p => p.TanggalBayar.HasValue && p.TanggalBayar.Value.Year == targetMonth.Year && 
                               p.TanggalBayar.Value.Month == targetMonth.Month)
                    .Sum(p => p.JumlahBayar);

                result.Add(new { Month = targetMonth.ToString("MMM"), Revenue = (double)monthlyRevenue });
            }

            return result;
        }

        private string FormatCurrency(decimal amount)
        {
            return "Rp " + amount.ToString("#,##0");
        }

        private void timeReportButton2_Click(object sender, EventArgs e)
        {
            ShowDateRangeDialog();
        }

        private void ShowDateRangeDialog()
        {
            var dialogForm = new Form
            {
                Text = "Pilih Rentang Tanggal",
                Width = 400,
                Height = 320,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowIcon = false,
                BackColor = System.Drawing.SystemColors.Control
            };

            var label = new Label
            {
                Text = "Pilih Periode Laporan:",
                Location = new Point(15, 20),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };

            var radioAllTime = new RadioButton
            {
                Text = "Sepanjang Waktu (All Time)",
                Location = new Point(30, 50),
                AutoSize = true,
                Checked = (selectedStartDate == DateTime.MinValue)
            };

            var radio30Days = new RadioButton
            {
                Text = "30 Hari Terakhir",
                Location = new Point(30, 80),
                AutoSize = true
            };

            var radio6Months = new RadioButton
            {
                Text = "6 Bulan Terakhir",
                Location = new Point(30, 110),
                AutoSize = true,
                Checked = (selectedStartDate == DateTime.Now.AddMonths(-6).Date && selectedEndDate.Date == DateTime.Now.Date)
            };

            var radio1Year = new RadioButton
            {
                Text = "1 Tahun Terakhir",
                Location = new Point(30, 140),
                AutoSize = true
            };

            var btnOk = new Button
            {
                Text = "Terapkan",
                DialogResult = DialogResult.OK,
                Location = new Point(150, 220),
                Width = 80,
                Height = 35
            };

            var btnCancel = new Button
            {
                Text = "Batal",
                DialogResult = DialogResult.Cancel,
                Location = new Point(245, 220),
                Width = 80,
                Height = 35
            };

            dialogForm.Controls.Add(label);
            dialogForm.Controls.Add(radioAllTime);
            dialogForm.Controls.Add(radio30Days);
            dialogForm.Controls.Add(radio6Months);
            dialogForm.Controls.Add(radio1Year);
            dialogForm.Controls.Add(btnOk);
            dialogForm.Controls.Add(btnCancel);

            if (dialogForm.ShowDialog() == DialogResult.OK)
            {
                string selectedPeriodText = "Last 6 Months";

                if (radio30Days.Checked)
                {
                    selectedStartDate = DateTime.Now.AddDays(-30);
                    selectedEndDate = DateTime.Now;
                    selectedPeriodText = "Last 30 Days";
                }
                else if (radio6Months.Checked)
                {
                    selectedStartDate = DateTime.Now.AddMonths(-6);
                    selectedEndDate = DateTime.Now;
                    selectedPeriodText = "Last 6 Months";
                }
                else if (radio1Year.Checked)
                {
                    selectedStartDate = DateTime.Now.AddYears(-1);
                    selectedEndDate = DateTime.Now;
                    selectedPeriodText = "Last Year";
                }
                else
                {
                    selectedStartDate = DateTime.MinValue;
                    selectedEndDate = DateTime.Now;
                    selectedPeriodText = "All Time";
                }

                // Update button text with selected period
                timeReportButton2.Text = selectedPeriodText;

                UpdateReportStatCards();
                SetupCharts();
            }

            dialogForm.Dispose();
        }

        private void exportButton1_Click(object sender, EventArgs e)
        {
            ExportReportToPdf();
        }

        private void ExportReportToPdf()
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"Financial_Report_{DateTime.Now:yyyy-MM-dd}.pdf",
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    Title = "Simpan Laporan Keuangan"
                };

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                var filteredPayments = allPayments
                    .Where(p => p.TanggalBayar.HasValue && p.TanggalBayar.Value.Date >= selectedStartDate.Date && p.TanggalBayar.Value.Date <= selectedEndDate.Date)
                    .OrderByDescending(p => p.TanggalBayar)
                    .ToList();

                Document doc = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(saveDialog.FileName, FileMode.Create));
                doc.Open();

                // Add Header
                AddReportHeader(doc);

                // Add Metrics Summary
                AddMetricsSummary(doc, filteredPayments);

                // Add Transaction Table
                AddTransactionTable(doc, filteredPayments);

                doc.Close();
                writer.Close();

                MessageBox.Show("Laporan berhasil diexport ke: " + saveDialog.FileName, "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat export: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddReportHeader(Document doc)
        {
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.BLACK);
            var subHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);

            var title = new Paragraph("Kost Putra Rahmat ZAW", headerFont);
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            var address = new Paragraph("Pondok Alam, Jl. Sigura - Gura No.21 Blok A2", subHeaderFont);
            address.Alignment = Element.ALIGN_CENTER;
            doc.Add(address);

            var address2 = new Paragraph("Karangbesuki, Kec. Sukun, Kota Malang, Jawa Timur 65149", subHeaderFont);
            address2.Alignment = Element.ALIGN_CENTER;
            doc.Add(address2);

            var phone = new Paragraph("Telepon: 08124911926", subHeaderFont);
            phone.Alignment = Element.ALIGN_CENTER;
            doc.Add(phone);

            doc.Add(new Paragraph("\n"));

            var reportTitle = new Paragraph("LAPORAN KEUANGAN", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14));
            reportTitle.Alignment = Element.ALIGN_CENTER;
            doc.Add(reportTitle);

            var dateRange = new Paragraph($"Periode: {selectedStartDate:dd/MM/yyyy} - {selectedEndDate:dd/MM/yyyy}", subHeaderFont);
            dateRange.Alignment = Element.ALIGN_CENTER;
            doc.Add(dateRange);

            doc.Add(new Paragraph("\n"));
        }

        private void AddMetricsSummary(Document doc, List<Pembayaran> filteredPayments)
        {
            var metricsFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            var valueFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);

            decimal totalRevenue = filteredPayments
                .Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed")
                .Sum(p => p.JumlahBayar);

            int pendingPayments = filteredPayments
                .Where(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending")
                .Count();

            decimal avgRate = allRooms.Count > 0 ? allRooms.Average(r => r.PRICE) : 0;

            var occupiedRooms = allRooms.Where(r => r.STATUS == "Penuh" || r.STATUS == "Full").Count();
            var totalRooms = allRooms.Count;
            double occupancyRate = totalRooms > 0 ? (occupiedRooms / (double)totalRooms) * 100 : 0;

            var metricsTable = new PdfPTable(2);
            metricsTable.WidthPercentage = 100;
            metricsTable.SetWidths(new float[] { 50, 50 });

            var cell1 = new PdfPCell(new Phrase("Total Pendapatan", metricsFont)) { Border = iTextSharp.text.Rectangle.BOX };
            var cellValue1 = new PdfPCell(new Phrase(FormatCurrency(totalRevenue), valueFont)) { Border = iTextSharp.text.Rectangle.BOX };
            metricsTable.AddCell(cell1);
            metricsTable.AddCell(cellValue1);

            var cell2 = new PdfPCell(new Phrase("Pendapatan Tertunda", metricsFont)) { Border = iTextSharp.text.Rectangle.BOX };
            var cellValue2 = new PdfPCell(new Phrase(pendingPayments.ToString(), valueFont)) { Border = iTextSharp.text.Rectangle.BOX };
            metricsTable.AddCell(cell2);
            metricsTable.AddCell(cellValue2);

            var cell3 = new PdfPCell(new Phrase("Tarif Rata-rata", metricsFont)) { Border = iTextSharp.text.Rectangle.BOX };
            var cellValue3 = new PdfPCell(new Phrase(FormatCurrency(avgRate), valueFont)) { Border = iTextSharp.text.Rectangle.BOX };
            metricsTable.AddCell(cell3);
            metricsTable.AddCell(cellValue3);

            var cell4 = new PdfPCell(new Phrase("Tingkat Hunian", metricsFont)) { Border = iTextSharp.text.Rectangle.BOX };
            var cellValue4 = new PdfPCell(new Phrase($"{occupancyRate:F1}%", valueFont)) { Border = iTextSharp.text.Rectangle.BOX };
            metricsTable.AddCell(cell4);
            metricsTable.AddCell(cellValue4);

            doc.Add(metricsTable);
            doc.Add(new Paragraph("\n"));
        }

        private void AddTransactionTable(Document doc, List<Pembayaran> filteredPayments)
        {
            doc.Add(new Paragraph("\nRINCI TRANSAKSI", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
            doc.Add(new Paragraph("\n"));

            var table = new PdfPTable(6);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 20, 25, 15, 12, 15, 13 });

            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
            var headerCell = new PdfPCell { BackgroundColor = new BaseColor(184, 134, 11), BorderColor = BaseColor.BLACK };

            string[] headers = { "Tanggal", "Nama Penyewa", "Tipe Kamar", "Kamar", "Status", "Jumlah" };
            foreach (var headerText in headers)
            {
                var cell = new PdfPCell(new Phrase(headerText, headerFont)) { BackgroundColor = new BaseColor(184, 134, 11) };
                table.AddCell(cell);
            }

            var rowFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
            int rowCount = 0;

            foreach (var payment in filteredPayments.Take(50))
            {
                var backgroundColor = (rowCount % 2 == 0) ? new BaseColor(245, 245, 245) : BaseColor.WHITE;
                rowCount++;

                var dateCell = new PdfPCell(new Phrase(payment.TanggalBayar?.ToString("dd/MM/yyyy") ?? "-", rowFont)) { BackgroundColor = backgroundColor };
                var nameCell = new PdfPCell(new Phrase(payment.Pemesanan?.Penyewa?.NamaPenyewa ?? "-", rowFont)) { BackgroundColor = backgroundColor };
                var typeCell = new PdfPCell(new Phrase(payment.Pemesanan?.Kamar?.TYPE ?? "-", rowFont)) { BackgroundColor = backgroundColor };
                var roomCell = new PdfPCell(new Phrase(payment.Pemesanan?.Kamar?.ROOM ?? "-", rowFont)) { BackgroundColor = backgroundColor };

                string statusText = "Pending";
                if (payment.StatusPembayaran == "Confirmed" || payment.StatusPembayaran == "confirmed")
                    statusText = "Konfirmasi";
                else if (payment.StatusPembayaran == "Rejected" || payment.StatusPembayaran == "rejected")
                    statusText = "Ditolak";

                var statusCell = new PdfPCell(new Phrase(statusText, rowFont)) { BackgroundColor = backgroundColor };
                var amountCell = new PdfPCell(new Phrase(FormatCurrency(payment.JumlahBayar), rowFont)) { BackgroundColor = backgroundColor };

                table.AddCell(dateCell);
                table.AddCell(nameCell);
                table.AddCell(typeCell);
                table.AddCell(roomCell);
                table.AddCell(statusCell);
                table.AddCell(amountCell);
            }

            doc.Add(table);

            var footer = new Paragraph($"\nLaporan dihasilkan pada: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", 
                FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY));
            footer.Alignment = Element.ALIGN_CENTER;
            doc.Add(footer);
        }
    }
}
