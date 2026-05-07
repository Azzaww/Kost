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
        private DashboardStats currentStats = null;
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
                System.Diagnostics.Debug.WriteLine("[LoadAllReportData] Starting initial data load...");

                await Task.WhenAll(
                    LoadDashboardStatsAsync(),
                    LoadPaymentsAsync(),
                    LoadRoomsAsync()
                );

                System.Diagnostics.Debug.WriteLine($"[LoadAllReportData] Data load completed - Stats loaded: {(currentStats != null)}, Payments: {allPayments.Count}, Rooms: {allRooms.Count}");

                // Save debug log after loading
                SaveDebugLog();

                // Ensure UI update happens on UI thread
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
                System.Diagnostics.Debug.WriteLine($"[LoadAllReportData] Error loading report data: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Save comprehensive debug log to Desktop
        /// </summary>
        private void SaveDebugLog()
        {
            try
            {
                string debugLog = $"========== REPORT DEBUG LOG ==========\n";
                debugLog += $"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}\n\n";

                debugLog += $"========== LOADED DATA ==========\n";
                debugLog += $"Total Payments: {allPayments.Count}\n";
                debugLog += $"Total Rooms: {allRooms.Count}\n\n";

                debugLog += $"========== PAYMENT BREAKDOWN ==========\n";
                debugLog += $"Confirmed: {allPayments.Count(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed")}\n";
                debugLog += $"Pending: {allPayments.Count(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending")}\n";
                debugLog += $"Rejected: {allPayments.Count(p => p.StatusPembayaran == "Rejected" || p.StatusPembayaran == "rejected")}\n\n";

                debugLog += $"========== REVENUE BREAKDOWN ==========\n";
                var confirmedTotal = allPayments.Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed").Sum(p => p.JumlahBayar);
                var pendingTotal = allPayments.Where(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending").Sum(p => p.JumlahBayar);
                debugLog += $"Confirmed Total: {confirmedTotal} (Expected: 16000000)\n";
                debugLog += $"Pending Total: {pendingTotal} (Expected: 5000000)\n";
                debugLog += $"ALL Total: {allPayments.Sum(p => p.JumlahBayar)} (Expected: 21000000)\n\n";

                debugLog += $"========== FIRST 10 PAYMENTS ==========\n";
                foreach (var p in allPayments.Take(10))
                {
                    debugLog += $"ID: {p.Id} | Amount: {p.JumlahBayar} | Status: {p.StatusPembayaran} | Date: {p.TanggalBayar}\n";
                }

                string debugPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Report_Debug_Log.txt");
                File.WriteAllText(debugPath, debugLog);
                System.Diagnostics.Debug.WriteLine($"[SaveDebugLog] Debug log saved to: {debugPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SaveDebugLog] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Load dashboard statistics from /api/dashboard/stats endpoint
        /// This endpoint provides pre-calculated metrics to avoid frontend calculation errors
        /// </summary>
        private async Task LoadDashboardStatsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[LoadDashboardStatsAsync] Starting dashboard stats fetch...");
                var response = await ApiClient.Client.GetAsync("https://rahmatzaw.elarisnoir.my.id/api/dashboard/stats");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine($"[LoadDashboardStatsAsync] Raw JSON response length: {content.Length} chars");
                    System.Diagnostics.Debug.WriteLine($"[LoadDashboardStatsAsync] First 500 chars:\n{content.Substring(0, Math.Min(500, content.Length))}");

                    try
                    {
                        currentStats = JsonConvert.DeserializeObject<DashboardStats>(content,
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                        if (currentStats != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"[LoadDashboardStatsAsync] ✓ SUCCESS! Stats loaded:");
                            System.Diagnostics.Debug.WriteLine($"  - Total Revenue: {currentStats.TotalRevenue}");
                            System.Diagnostics.Debug.WriteLine($"  - Pending Revenue: {currentStats.PendingRevenue}");
                            System.Diagnostics.Debug.WriteLine($"  - Active Tenants: {currentStats.ActiveTenants}");
                            System.Diagnostics.Debug.WriteLine($"  - Occupied Rooms: {currentStats.OccupiedRooms}/{currentStats.OccupiedRooms + currentStats.AvailableRooms}");
                            System.Diagnostics.Debug.WriteLine($"  - Type Breakdown items: {currentStats.TypeBreakdown?.Count ?? 0}");
                            System.Diagnostics.Debug.WriteLine($"  - Demographics items: {currentStats.Demographics?.Count ?? 0}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[LoadDashboardStatsAsync] ✗ Deserialization returned null");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[LoadDashboardStatsAsync] ✗ Deserialization error: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"[LoadDashboardStatsAsync] Exception: {ex.GetType().Name}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[LoadDashboardStatsAsync] API returned error status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadDashboardStatsAsync] Error loading stats: {ex.Message}\n{ex.StackTrace}");
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

                    System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] Raw JSON response length: {content.Length} chars");
                    System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] Raw JSON response (first 1000 chars):\n{content.Substring(0, Math.Min(1000, content.Length))}");

                    // Save raw JSON to file for inspection
                    try
                    {
                        string debugPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "API_Response_Debug.json");
                        File.WriteAllText(debugPath, content);
                        System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] Raw JSON saved to: {debugPath}");
                    }
                    catch { }

                    // Use robust deserialization
                    allPayments = SafeDeserializePayments(content);

                    System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] *** RESULT: Successfully loaded {allPayments.Count} payments from API ***");

                    // Log detailed breakdown
                    if (allPayments.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] Payment breakdown:");
                        System.Diagnostics.Debug.WriteLine($"  - Total items: {allPayments.Count}");
                        System.Diagnostics.Debug.WriteLine($"  - Confirmed: {allPayments.Count(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed")}");
                        System.Diagnostics.Debug.WriteLine($"  - Pending: {allPayments.Count(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending")}");
                        System.Diagnostics.Debug.WriteLine($"  - Total Revenue (Confirmed): {allPayments.Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed").Sum(p => p.JumlahBayar)}");

                        System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] First 5 payments:");
                        foreach (var p in allPayments.Take(5))
                        {
                            System.Diagnostics.Debug.WriteLine($"  ID: {p.Id}, Amount: {p.JumlahBayar}, Status: {p.StatusPembayaran}, Date: {p.TanggalBayar}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] ❌ WARNING: NO PAYMENTS PARSED!");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] API returned error status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadPaymentsAsync] Error loading payments: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Safely deserialize payment response from API - handles RAW ARRAY format (main format)
        /// </summary>
        private List<Pembayaran> SafeDeserializePayments(string jsonContent)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SafeDeserializePayments] ========== START PARSING ==========");
                System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] JSON Content Length: {jsonContent?.Length ?? 0} chars");
                System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] First 200 chars: {jsonContent?.Substring(0, Math.Min(200, jsonContent.Length))}...");

                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    System.Diagnostics.Debug.WriteLine("[SafeDeserializePayments] ✗ JSON content is empty/null");
                    return new List<Pembayaran>();
                }

                // ============================================
                // PRIORITY 1: Try direct array (main format)
                // ============================================
                System.Diagnostics.Debug.WriteLine("[SafeDeserializePayments] Attempt 1: Parse as direct array...");
                try
                {
                    var directArray = JsonConvert.DeserializeObject<List<Pembayaran>>(jsonContent, 
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    if (directArray != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] ✓ SUCCESS! Direct array - {directArray.Count} items");

                        // Log first 3 items for verification
                        if (directArray.Count > 0)
                        {
                            System.Diagnostics.Debug.WriteLine("[SafeDeserializePayments] First 3 items:");
                            foreach (var payment in directArray.Take(3))
                            {
                                System.Diagnostics.Debug.WriteLine($"  - ID: {payment.Id}, Amount: {payment.JumlahBayar}, Status: {payment.StatusPembayaran}");
                            }
                        }

                        return directArray;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] ✗ Direct array parsing failed: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] Exception detail: {ex.GetType().Name}");
                }

                // ============================================
                // PRIORITY 2: Try wrapper formats
                // ============================================
                System.Diagnostics.Debug.WriteLine("[SafeDeserializePayments] Attempt 2: Parse as wrapper object...");
                try
                {
                    var jToken = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JToken>(jsonContent);

                    if (jToken == null)
                    {
                        System.Diagnostics.Debug.WriteLine("[SafeDeserializePayments] ✗ JSON token is null");
                        return new List<Pembayaran>();
                    }

                    // Try different wrapper keys in order
                    string[] possibleKeys = { "data", "pembayarans", "payments", "payment", "list", "items", "result" };

                    foreach (var key in possibleKeys)
                    {
                        try
                        {
                            var dataToken = jToken[key];
                            if (dataToken != null)
                            {
                                System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] Trying key '{key}' (type: {dataToken.Type})...");

                                if (dataToken.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                                {
                                    var payments = dataToken.ToObject<List<Pembayaran>>();
                                    if (payments != null && payments.Count > 0)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] ✓ SUCCESS with key '{key}' - {payments.Count} items");
                                        return payments;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] Key '{key}' failed: {ex.Message}");
                        }
                    }

                    // If jToken itself is array
                    if (jToken.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                    {
                        try
                        {
                            var payments = jToken.ToObject<List<Pembayaran>>();
                            if (payments != null && payments.Count > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] ✓ SUCCESS as jToken array - {payments.Count} items");
                                return payments;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] jToken array parsing failed: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] Wrapper parsing critical error: {ex.Message}");
                }

                System.Diagnostics.Debug.WriteLine("[SafeDeserializePayments] ✗ ALL parsing attempts FAILED - returning empty list");
                System.Diagnostics.Debug.WriteLine("[SafeDeserializePayments] ========== END PARSING ==========");
                return new List<Pembayaran>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] CRITICAL ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SafeDeserializePayments] StackTrace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("[SafeDeserializePayments] ========== END PARSING ==========");
                return new List<Pembayaran>();
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

        /// <summary>
        /// Refresh report data dari API - digunakan untuk real-time update saat filter berubah
        /// </summary>
        private async Task RefreshReportDataAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[RefreshReportDataAsync] Starting data refresh from API...");

                // Fetch latest data including dashboard stats
                await Task.WhenAll(
                    LoadDashboardStatsAsync(),
                    LoadPaymentsAsync(),
                    LoadRoomsAsync()
                );

                System.Diagnostics.Debug.WriteLine($"[RefreshReportDataAsync] Data refreshed! Total payments: {allPayments.Count}, Total rooms: {allRooms.Count}");

                // Log first few payments for verification
                if (allPayments.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("[RefreshReportDataAsync] First 5 payments from API:");
                    foreach (var p in allPayments.Take(5))
                    {
                        System.Diagnostics.Debug.WriteLine($"  ID: {p.Id}, Status: {p.StatusPembayaran}, Amount: {p.JumlahBayar}, Date: {p.TanggalBayar}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[RefreshReportDataAsync] WARNING: No payments received from API!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RefreshReportDataAsync] Error: {ex.Message}");
            }
        }

        private void UpdateReportStatCards()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] ========== START UPDATE STAT CARDS ==========");

                // Check if stats loaded
                if (currentStats == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] ⚠️ WARNING: Dashboard stats not loaded yet!");
                    // Fallback to manual calculation if needed
                    UpdateReportStatCardsFromManualData();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] Using pre-calculated stats from API");
                System.Diagnostics.Debug.WriteLine($"  - Total Revenue: {currentStats.TotalRevenue}");
                System.Diagnostics.Debug.WriteLine($"  - Pending Revenue: {currentStats.PendingRevenue}");
                System.Diagnostics.Debug.WriteLine($"  - Active Tenants: {currentStats.ActiveTenants}");
                System.Diagnostics.Debug.WriteLine($"  - Occupancy: {currentStats.OccupiedRooms}/{currentStats.OccupiedRooms + currentStats.AvailableRooms}");

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        // Total Revenue - direct from stats (no calculation needed)
                        TotalRevenueHtmlLabel12.Text = FormatCurrency(currentStats.TotalRevenue);

                        // Pending Payments - count
                        guna2HtmlLabel13.Text = currentStats.PendingPayments.ToString();

                        // Average Rate - calculate from type breakdown if available
                        decimal avgRate = 0;
                        if (currentStats.TypeBreakdown != null && currentStats.TypeBreakdown.Count > 0)
                        {
                            // Average = Total Revenue / Total Rooms
                            int totalRooms = currentStats.TypeBreakdown.Sum(t => t.Count);
                            if (totalRooms > 0)
                            {
                                avgRate = currentStats.TypeBreakdown.Sum(t => t.Revenue) / totalRooms;
                            }
                        }
                        guna2HtmlLabel16.Text = FormatCurrency(avgRate);

                        // Occupancy Rate
                        int totalRoomCount = currentStats.OccupiedRooms + currentStats.AvailableRooms;
                        double occupancyRate = totalRoomCount > 0 
                            ? (currentStats.OccupiedRooms / (double)totalRoomCount) * 100 
                            : 0;
                        guna2HtmlLabel15.Text = $"{occupancyRate:F1} %";

                        // Percentage change from last month - using allPayments data if available
                        if (allPayments.Count > 0)
                        {
                            var lastMonthPayments = allPayments
                                .Where(p => p.TanggalBayar.HasValue && 
                                    p.TanggalBayar.Value.Date >= DateTime.Now.AddMonths(-1).Date && 
                                    p.TanggalBayar.Value.Date < DateTime.Now.Date)
                                .Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed")
                                .Sum(p => p.JumlahBayar);

                            double percentageChange = lastMonthPayments > 0 
                                ? ((double)currentStats.TotalRevenue - (double)lastMonthPayments) / (double)lastMonthPayments * 100
                                : 0;

                            guna2HtmlLabel2.Text = percentageChange >= 0
                                ? $"+ {percentageChange:F1} % from last mth"
                                : $"- {Math.Abs(percentageChange):F1} % from last mth";
                        }

                        // Update Payment Status using stats
                        UpdatePaymentStatusFromStats();
                    }));
                }
                else
                {
                    // Total Revenue
                    TotalRevenueHtmlLabel12.Text = FormatCurrency(currentStats.TotalRevenue);
                    guna2HtmlLabel13.Text = currentStats.PendingPayments.ToString();

                    decimal avgRate = 0;
                    if (currentStats.TypeBreakdown != null && currentStats.TypeBreakdown.Count > 0)
                    {
                        int totalRooms = currentStats.TypeBreakdown.Sum(t => t.Count);
                        if (totalRooms > 0)
                        {
                            avgRate = currentStats.TypeBreakdown.Sum(t => t.Revenue) / totalRooms;
                        }
                    }
                    guna2HtmlLabel16.Text = FormatCurrency(avgRate);

                    int totalRoomCount = currentStats.OccupiedRooms + currentStats.AvailableRooms;
                    double occupancyRate = totalRoomCount > 0 
                        ? (currentStats.OccupiedRooms / (double)totalRoomCount) * 100 
                        : 0;
                    guna2HtmlLabel15.Text = $"{occupancyRate:F1} %";
                }

                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] ========== END UPDATE STAT CARDS ==========");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCards] Error updating stat cards: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Fallback method to update stats from manual calculation if DashboardStats not available
        /// </summary>
        private void UpdateReportStatCardsFromManualData()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCardsFromManualData] Falling back to manual calculation...");
                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCardsFromManualData] Total payments: {allPayments.Count}, Total rooms: {allRooms.Count}");

                if (allPayments.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCardsFromManualData] ⚠️ No payments loaded, cannot calculate");
                    return;
                }

                // Filter payments by date range
                var filteredPayments = allPayments
                    .Where(p => p.TanggalBayar.HasValue && 
                        p.TanggalBayar.Value.Date >= selectedStartDate.Date && 
                        p.TanggalBayar.Value.Date <= selectedEndDate.Date)
                    .ToList();

                var confirmedPayments = filteredPayments
                    .Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed")
                    .ToList();

                var pendingPayments = filteredPayments
                    .Where(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending")
                    .ToList();

                decimal totalRevenue = confirmedPayments.Sum(p => p.JumlahBayar);
                decimal totalPendingRevenue = pendingPayments.Sum(p => p.JumlahBayar);

                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCardsFromManualData] Calculated - Confirmed: {FormatCurrency(totalRevenue)}, Pending: {FormatCurrency(totalPendingRevenue)}");

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        TotalRevenueHtmlLabel12.Text = FormatCurrency(totalRevenue);
                        guna2HtmlLabel13.Text = pendingPayments.Count.ToString();

                        decimal avgRate = allRooms.Count > 0 ? allRooms.Average(r => r.PRICE) : 0;
                        guna2HtmlLabel16.Text = FormatCurrency(avgRate);

                        var occupiedRooms = allRooms.Where(r => r.STATUS == "Penuh" || r.STATUS == "Full").Count();
                        var totalRooms = allRooms.Count;
                        double occupancyRate = totalRooms > 0 ? (occupiedRooms / (double)totalRooms) * 100 : 0;
                        guna2HtmlLabel15.Text = $"{occupancyRate:F1} %";
                    }));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateReportStatCardsFromManualData] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update payment status display using DashboardStats
        /// </summary>
        private void UpdatePaymentStatusFromStats()
        {
            try
            {
                if (currentStats == null) return;

                // Update Confirmed status labels
                guna2HtmlLabel27.Text = "Dikonfirmasi";
                decimal confirmedRevenue = currentStats.TotalRevenue;
                guna2HtmlLabel25.Text = $"{currentStats.PendingPayments} TRANSAKSI";
                guna2HtmlLabel24.Text = FormatCurrency(confirmedRevenue);

                // Update Pending status labels
                guna2HtmlLabel30.Text = "Menunggu";
                guna2HtmlLabel29.Text = $"{currentStats.PendingPayments} TRANSAKSI";
                guna2HtmlLabel28.Text = FormatCurrency(currentStats.PendingRevenue);

                // Update Total Potential labels
                guna2HtmlLabel33.Text = "Potensi Total";
                guna2HtmlLabel32.Text = "Jika terisi penuh";
                guna2HtmlLabel31.Text = FormatCurrency(currentStats.PotentialRevenue);
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
                System.Diagnostics.Debug.WriteLine("[SetupRevenueByTypeChart] Setting up revenue by type chart...");

                // Use stats data if available, otherwise use manual calculation
                if (currentStats == null || currentStats.TypeBreakdown == null || currentStats.TypeBreakdown.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[SetupRevenueByTypeChart] Stats not available, using manual data");
                    SetupRevenueByTypeChartManual();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[SetupRevenueByTypeChart] Using stats data - {currentStats.TypeBreakdown.Count} types");

                // Show chart1 
                chart1.Visible = true;

                chart1.Series.Clear();
                chart1.ChartAreas.Clear();

                var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("Default");
                chart1.ChartAreas.Add(chartArea);

                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Revenue",
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column
                };

                // Add data from type breakdown
                foreach (var item in currentStats.TypeBreakdown)
                {
                    System.Diagnostics.Debug.WriteLine($"[SetupRevenueByTypeChart] Adding: {item.Type} = {FormatCurrency(item.Revenue)}");
                    series.Points.AddXY(item.Type, item.Revenue);
                }

                chart1.Series.Add(series);

                // Format Y-axis labels as currency
                chartArea.AxisY.LabelStyle.Format = "#,##0";

                // Setup Revenue Breakdown cards di guna2Panel9
                DisplayRevenueBreakdownFromStats();

                System.Diagnostics.Debug.WriteLine("[SetupRevenueByTypeChart] Chart setup complete");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SetupRevenueByTypeChart] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Fallback method to setup chart from manual calculation
        /// </summary>
        private void SetupRevenueByTypeChartManual()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SetupRevenueByTypeChartManual] Using manual data calculation");

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

                chart1.Visible = true;
                chart1.Series.Clear();
                chart1.ChartAreas.Clear();

                var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("Default");
                chart1.ChartAreas.Add(chartArea);

                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Revenue",
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column
                };

                series.Points.DataBind(revenueByType.Take(2).ToList(), "Type", "Revenue", "");
                chart1.Series.Add(series);

                chartArea.AxisY.LabelStyle.Format = "#,##0";

                System.Diagnostics.Debug.WriteLine("[SetupRevenueByTypeChartManual] Manual chart setup complete");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SetupRevenueByTypeChartManual] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Display revenue breakdown cards from DashboardStats
        /// </summary>
        private void DisplayRevenueBreakdownFromStats()
        {
            try
            {
                if (currentStats == null || currentStats.TypeBreakdown == null) return;

                System.Diagnostics.Debug.WriteLine($"[DisplayRevenueBreakdownFromStats] Displaying {currentStats.TypeBreakdown.Count} breakdown cards");

                var revenueBreakdownData = new List<dynamic>();

                foreach (var item in currentStats.TypeBreakdown)
                {
                    revenueBreakdownData.Add(new
                    {
                        Type = item.Type,
                        Revenue = item.Revenue,
                        Occupied = item.Occupied,
                        Total = item.Count,
                        OccupancyText = $"{item.Occupied}/{item.Count} occupied"
                    });
                }

                DisplayRevenueBreakdownInPanel9(revenueBreakdownData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DisplayRevenueBreakdownFromStats] Error: {ex.Message}");
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
                        BorderRadius = 8, // Sedikit lebih melengkung agar terlihat modern
                        BorderThickness = 1,
                        FillColor = System.Drawing.Color.FromArgb(250, 250, 250),
                        Location = new System.Drawing.Point(10, yPosition),
                        Width = guna2Panel10.ClientSize.Width - 45,
                        Height = 75,
                        Padding = new Padding(10), // KUNCI UTAMA: Isi kartu tidak akan keluar dari zona ini
                        Anchor = AnchorStyles.Top | AnchorStyles.Left,
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
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
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
                        Location = new System.Drawing.Point(12, 48),
                        Width = cardPanel.Width - 24,
                        Height = 8,
                        BackColor = System.Drawing.Color.FromArgb(230, 230, 230),
                        ForeColor = System.Drawing.Color.FromArgb(245, 158, 11),
                        Value = (item.Total > 0) ? (int)((item.Occupied / (double)item.Total) * 100) : 0,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        Visible = true
                    };
                    cardPanel.Controls.Add(progressBar);

                    guna2Panel10.Controls.Add(cardPanel);
                    System.Diagnostics.Debug.WriteLine($"Added card at Y={yPosition}, Total controls in panel now: {guna2Panel10.Controls.Count}");

                    yPosition += 85;
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
                System.Diagnostics.Debug.WriteLine("[SetupRoomDemographicsChart] Setting up demographics chart...");

                // Use stats data if available
                if (currentStats == null || currentStats.Demographics == null || currentStats.Demographics.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[SetupRoomDemographicsChart] Stats not available, using manual data");
                    SetupRoomDemographicsChartManual();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[SetupRoomDemographicsChart] Using stats data - {currentStats.Demographics.Count} age groups");

                chart2.Series.Clear();
                chart2.ChartAreas.Clear();

                var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("Default");
                chart2.ChartAreas.Add(chartArea);

                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Age Distribution",
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie
                };

                foreach (var item in currentStats.Demographics)
                {
                    System.Diagnostics.Debug.WriteLine($"[SetupRoomDemographicsChart] Adding: {item.Name} = {item.Value}");
                    series.Points.AddXY(item.Name, item.Value);
                }

                chart2.Series.Add(series);

                System.Diagnostics.Debug.WriteLine("[SetupRoomDemographicsChart] Chart setup complete");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SetupRoomDemographicsChart] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Fallback method to setup demographics chart from manual data
        /// </summary>
        private void SetupRoomDemographicsChartManual()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SetupRoomDemographicsChartManual] Using manual data");

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

                System.Diagnostics.Debug.WriteLine("[SetupRoomDemographicsChartManual] Manual chart setup complete");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SetupRoomDemographicsChartManual] Error: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine("[SetupMonthlyTrendChart] Setting up monthly trend chart...");

                // Use stats data if available
                if (currentStats == null || currentStats.MonthlyTrend == null || currentStats.MonthlyTrend.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[SetupMonthlyTrendChart] Stats not available, using manual data");
                    SetupMonthlyTrendChartManual();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[SetupMonthlyTrendChart] Using stats data - {currentStats.MonthlyTrend.Count} months");

                chart4.Series.Clear();
                chart4.ChartAreas.Clear();

                var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea("Default");
                chart4.ChartAreas.Add(chartArea);

                var series = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Monthly Revenue",
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
                };

                foreach (var item in currentStats.MonthlyTrend)
                {
                    System.Diagnostics.Debug.WriteLine($"[SetupMonthlyTrendChart] Adding: {item.Month} = {FormatCurrency(item.Revenue)}");
                    series.Points.AddXY(item.Month, item.Revenue);
                }

                chart4.Series.Add(series);
                chartArea.AxisY.LabelStyle.Format = "#,##0";

                System.Diagnostics.Debug.WriteLine("[SetupMonthlyTrendChart] Chart setup complete");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SetupMonthlyTrendChart] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Fallback method to setup monthly trend chart from manual data
        /// </summary>
        private void SetupMonthlyTrendChartManual()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SetupMonthlyTrendChartManual] Using manual data");

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

                System.Diagnostics.Debug.WriteLine("[SetupMonthlyTrendChartManual] Manual chart setup complete");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SetupMonthlyTrendChartManual] Error: {ex.Message}");
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

                // 🔄 REFRESH DATA FROM API BEFORE UPDATING UI
                System.Diagnostics.Debug.WriteLine("[ShowDateRangeDialog] User changed filter - refreshing data from API...");

                Task.Run(async () =>
                {
                    try
                    {
                        // Refresh payment dan room data dari API
                        await RefreshReportDataAsync();

                        // After refresh, update UI
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                UpdateReportStatCards();
                                SetupCharts();
                                System.Diagnostics.Debug.WriteLine("[ShowDateRangeDialog] Data refreshed and UI updated");
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
                        System.Diagnostics.Debug.WriteLine($"[ShowDateRangeDialog] Error during refresh: {ex.Message}");

                        if (InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                MessageBox.Show($"Error refreshing data: {ex.Message}", "Error", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                        }
                    }
                });
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

        /// <summary>
        /// Fetch dan tampilkan raw JSON response dari API untuk diagnostic
        /// </summary>
        private async void ShowRawApiResponseDiagnostic()
        {
            try
            {
                // Create a form to show the raw response
                var diagnosticForm = new Form
                {
                    Text = "🔍 Raw API Response Diagnostic",
                    Width = 1000,
                    Height = 700,
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.Sizable,
                    ShowIcon = false
                };

                // Create tab control
                var tabControl = new TabControl
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(10)
                };

                // TAB 1: RAW JSON
                var tabRawJson = new TabPage { Text = "Raw JSON" };
                var textBoxRawJson = new TextBox
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Both,
                    WordWrap = false,
                    Font = new System.Drawing.Font("Courier New", 10),
                    Dock = DockStyle.Fill
                };
                tabRawJson.Controls.Add(textBoxRawJson);

                // TAB 2: PARSING INFO
                var tabParsingInfo = new TabPage { Text = "Parsing Info" };
                var textBoxParsingInfo = new TextBox
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Both,
                    Font = new System.Drawing.Font("Courier New", 9),
                    Dock = DockStyle.Fill,
                    ReadOnly = true
                };
                tabParsingInfo.Controls.Add(textBoxParsingInfo);

                // TAB 3: CURRENT LOADED DATA
                var tabCurrentData = new TabPage { Text = "Current Loaded Data" };
                var textBoxCurrentData = new TextBox
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Both,
                    Font = new System.Drawing.Font("Courier New", 9),
                    Dock = DockStyle.Fill,
                    ReadOnly = true
                };
                tabCurrentData.Controls.Add(textBoxCurrentData);

                tabControl.TabPages.Add(tabRawJson);
                tabControl.TabPages.Add(tabParsingInfo);
                tabControl.TabPages.Add(tabCurrentData);

                diagnosticForm.Controls.Add(tabControl);

                // Button panel
                var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };

                var btnCopy = new Button
                {
                    Text = "Copy JSON",
                    Width = 100,
                    Height = 30,
                    Location = new Point(10, 10)
                };
                btnCopy.Click += (s, e) =>
                {
                    try
                    {
                        Clipboard.SetText(textBoxRawJson.Text);
                        MessageBox.Show("JSON copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch { }
                };

                var btnClose = new Button
                {
                    Text = "Close",
                    Width = 100,
                    Height = 30,
                    Location = new Point(120, 10),
                    DialogResult = DialogResult.Cancel
                };

                buttonPanel.Controls.Add(btnCopy);
                buttonPanel.Controls.Add(btnClose);
                diagnosticForm.Controls.Add(buttonPanel);

                diagnosticForm.Show();

                // Fetch data asynchronously
                Task.Run(async () =>
                {
                    try
                    {
                        // Fetch raw JSON
                        System.Diagnostics.Debug.WriteLine("[ShowRawApiResponseDiagnostic] Fetching raw JSON from API...");
                        var response = await ApiClient.Client.GetAsync("https://rahmatzaw.elarisnoir.my.id/api/payments");
                        var rawJson = await response.Content.ReadAsStringAsync();

                        // Format JSON for display
                        var prettyJson = rawJson;
                        try
                        {
                            var jsonObj = Newtonsoft.Json.Linq.JToken.Parse(rawJson);
                            prettyJson = jsonObj.ToString();
                        }
                        catch { }

                        // Populate Raw JSON tab
                        diagnosticForm.Invoke((MethodInvoker)delegate
                        {
                            textBoxRawJson.Text = prettyJson;
                        });

                        // Attempt parsing and show details
                        string parsingInfo = "========== PARSING DIAGNOSTIC ==========\n\n";
                        parsingInfo += $"Response Length: {rawJson.Length} characters\n";
                        parsingInfo += $"Response Status: {response.StatusCode}\n\n";

                        // Try direct array
                        parsingInfo += "--- Attempt 1: Direct Array ---\n";
                        try
                        {
                            var directArray = JsonConvert.DeserializeObject<List<Pembayaran>>(rawJson);
                            if (directArray != null)
                            {
                                parsingInfo += $"✓ SUCCESS: Parsed {directArray.Count} items\n";
                                if (directArray.Count > 0)
                                {
                                    parsingInfo += $"  First item: ID={directArray[0].Id}, Amount={directArray[0].JumlahBayar}, Status={directArray[0].StatusPembayaran}\n";
                                }
                            }
                            else
                            {
                                parsingInfo += "✗ Result is null\n";
                            }
                        }
                        catch (Exception ex)
                        {
                            parsingInfo += $"✗ FAILED: {ex.Message}\n";
                        }

                        parsingInfo += "\n--- Attempt 2: Wrapper Object ---\n";
                        try
                        {
                            var jToken = Newtonsoft.Json.Linq.JToken.Parse(rawJson);
                            parsingInfo += $"Root token type: {jToken.Type}\n";

                            if (jToken.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                            {
                                var obj = (Newtonsoft.Json.Linq.JObject)jToken;
                                parsingInfo += $"Object keys: {string.Join(", ", obj.Properties().Select(p => p.Name))}\n";

                                // Try to find array in known keys
                                foreach (var key in new[] { "data", "pembayarans", "payments", "payment", "list", "items", "result" })
                                {
                                    if (obj[key] != null)
                                    {
                                        parsingInfo += $"\n Found key '{key}' (type: {obj[key].Type})\n";
                                        if (obj[key].Type == Newtonsoft.Json.Linq.JTokenType.Array)
                                        {
                                            try
                                            {
                                                var items = obj[key].ToObject<List<Pembayaran>>();
                                                parsingInfo += $" ✓ Parsed {items.Count} items from key '{key}'\n";
                                            }
                                            catch (Exception ex)
                                            {
                                                parsingInfo += $" ✗ Failed to parse: {ex.Message}\n";
                                            }
                                        }
                                    }
                                }
                            }
                            else if (jToken.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                            {
                                parsingInfo += "Root is array - direct array parsing should have worked\n";
                            }
                        }
                        catch (Exception ex)
                        {
                            parsingInfo += $"✗ FAILED: {ex.Message}\n";
                        }

                        // Populate Parsing Info tab
                        diagnosticForm.Invoke((MethodInvoker)delegate
                        {
                            textBoxParsingInfo.Text = parsingInfo;
                        });

                        // Populate Current Data tab
                        string currentData = "========== CURRENT IN-MEMORY DATA ==========\n\n";
                        currentData += $"Total Payments Loaded: {allPayments.Count}\n";
                        currentData += $"Total Rooms Loaded: {allRooms.Count}\n\n";

                        currentData += "--- Status Breakdown ---\n";
                        currentData += $"Confirmed: {allPayments.Count(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed")}\n";
                        currentData += $"Pending: {allPayments.Count(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending")}\n";
                        currentData += $"Rejected: {allPayments.Count(p => p.StatusPembayaran == "Rejected" || p.StatusPembayaran == "rejected")}\n\n";

                        currentData += "--- Revenue Breakdown ---\n";
                        var confirmedTotal = allPayments.Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed").Sum(p => p.JumlahBayar);
                        var pendingTotal = allPayments.Where(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending").Sum(p => p.JumlahBayar);
                        var totalAll = allPayments.Sum(p => p.JumlahBayar);

                        currentData += $"Confirmed Total: {FormatCurrency(confirmedTotal)}\n";
                        currentData += $"Pending Total: {FormatCurrency(pendingTotal)}\n";
                        currentData += $"ALL Total: {FormatCurrency(totalAll)}\n\n";

                        currentData += "--- Expected from Database ---\n";
                        currentData += $"Confirmed Total: Rp 16.000.000 (22 items)\n";
                        currentData += $"Pending Total: Rp 5.000.000 (6 items)\n";
                        currentData += $"ALL Total: Rp 21.000.000 (31 items)\n\n";

                        if (confirmedTotal == 100000)
                        {
                            currentData += "⚠️ WARNING: Only Rp 100.000 loaded! Only 1 payment (ID=30 or 28) parsed!\n";
                            currentData += "Parsing issue detected!\n";
                        }

                        // First 10 items
                        currentData += "\n--- First 10 Loaded Payments ---\n";
                        foreach (var p in allPayments.Take(10))
                        {
                            currentData += $"ID: {p.Id} | Amount: {p.JumlahBayar} | Status: {p.StatusPembayaran} | Date: {p.TanggalBayar}\n";
                        }

                        diagnosticForm.Invoke((MethodInvoker)delegate
                        {
                            textBoxCurrentData.Text = currentData;
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ShowRawApiResponseDiagnostic] Error: {ex.Message}");
                        diagnosticForm.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Show detailed API response and data parsing info
        /// </summary>
        private void ShowApiDataDebugInfo()
        {
            try
            {
                // Create comprehensive debug message
                string debugInfo = $"📊 API DATA DEBUG INFO\n\n" +
                    $"========== CURRENT IN-MEMORY DATA ==========\n" +
                    $"Total Payments Loaded: {allPayments.Count}\n" +
                    $"Total Rooms Loaded: {allRooms.Count}\n\n" +

                    $"========== PAYMENT STATUS BREAKDOWN ==========\n" +
                    $"Confirmed: {allPayments.Count(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed")}\n" +
                    $"Pending: {allPayments.Count(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending")}\n" +
                    $"Rejected: {allPayments.Count(p => p.StatusPembayaran == "Rejected" || p.StatusPembayaran == "rejected")}\n" +
                    $"Other: {allPayments.Count(p => p.StatusPembayaran != "Confirmed" && p.StatusPembayaran != "confirmed" && p.StatusPembayaran != "Pending" && p.StatusPembayaran != "pending" && p.StatusPembayaran != "Rejected" && p.StatusPembayaran != "rejected")}\n\n" +

                    $"========== REVENUE CALCULATION ==========\n" +
                    $"Confirmed Total: {FormatCurrency(allPayments.Where(p => p.StatusPembayaran == "Confirmed" || p.StatusPembayaran == "confirmed").Sum(p => p.JumlahBayar))}\n" +
                    $"Pending Total: {FormatCurrency(allPayments.Where(p => p.StatusPembayaran == "Pending" || p.StatusPembayaran == "pending").Sum(p => p.JumlahBayar))}\n" +
                    $"ALL Total: {FormatCurrency(allPayments.Sum(p => p.JumlahBayar))}\n\n" +

                    $"========== FIRST 10 PAYMENTS ==========\n";

                foreach (var payment in allPayments.Take(10))
                {
                    debugInfo += $"ID: {payment.Id} | Amount: {payment.JumlahBayar} | Status: {payment.StatusPembayaran} | Date: {payment.TanggalBayar}\n";
                }

                debugInfo += $"\n========== EXPECTED FROM DATABASE ==========\n" +
                    $"Confirmed Total: Rp 16.000.000 (22 items)\n" +
                    $"Pending Total: Rp 5.000.000 (6 items)\n" +
                    $"ALL Total: Rp 21.000.000 (31 items)\n\n" +
                    $"If showing different values, parsing issue detected!";

                // Show in message box
                MessageBox.Show(debugInfo, "🔍 API Data Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing debug info: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TotalRevenueHtmlLabel12_Click(object sender, EventArgs e)
        {
            // Show raw API diagnostic
            ShowRawApiResponseDiagnostic();
        }
    }
}
