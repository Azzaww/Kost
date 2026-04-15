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

namespace Kost_SiguraGura
{
    public partial class Report : UserControl
    {
        private List<Pembayaran> allPayments = new List<Pembayaran>();
        private List<Kamar> allRooms = new List<Kamar>();
        private DateTime selectedStartDate = DateTime.Now.AddMonths(-6);
        private DateTime selectedEndDate = DateTime.Now;

        public Report()
        {
            InitializeComponent();
        }

        private void Report_Load(object sender, EventArgs e)
        {
            LoadAllReportData();
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
                var response = await ApiClient.Client.GetAsync("https://rahmatzaw.elarisnoir.my.id/api/payments");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    allPayments = JsonConvert.DeserializeObject<List<Pembayaran>>(content) ?? new List<Pembayaran>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading payments: {ex.Message}");
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
                // Filter payments by date range
                var filteredPayments = allPayments
                    .Where(p => p.TanggalBayar.HasValue && p.TanggalBayar.Value.Date >= selectedStartDate.Date && p.TanggalBayar.Value.Date <= selectedEndDate.Date)
                    .ToList();

                // Calculate metrics
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

                // Update stat cards with calculated values
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        // Total Revenue - guna2HtmlLabel12
                        guna2HtmlLabel12.Text = FormatCurrency(totalRevenue);

                        // Pending Payments - guna2HtmlLabel13
                        guna2HtmlLabel13.Text = pendingPayments.ToString();

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
                    }));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating stat cards: {ex.Message}");
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
                    .OrderByDescending(x => x.Revenue)
                    .ToList();

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

                series.Points.DataBind(revenueByType, "Type", "Revenue", "");
                chart1.Series.Add(series);

                // Format Y-axis labels as currency
                chartArea.AxisY.LabelStyle.Format = "#,##0";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up revenue by type chart: {ex.Message}");
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
    }
}
