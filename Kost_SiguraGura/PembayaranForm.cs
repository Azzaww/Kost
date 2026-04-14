using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    public partial class PembayaranForm : UserControl
    {
        private List<Pembayaran> allPayments = new List<Pembayaran>();
        private string currentFilter = "All"; // All, Pending, Confirmed, Rejected

        public PembayaranForm()
        {
            InitializeComponent();
            this.Load += PembayaranForm_Load;
        }

        private void PembayaranForm_Load(object sender, EventArgs e)
        {
            if (Session.UserRole?.ToLower() == "admin")
            {
                LoadPayments();
            }
            else
            {
                MessageBox.Show("Akses Ditolak! Anda bukan Admin. Role Anda: " + Session.UserRole);
            }
        }

        private async void LoadPayments()
        {
            try
            {
                var payments = await ApiClient.GetAllPayments();
                allPayments = payments;
                UpdateSummary();
                RefreshPaymentList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data pembayaran: " + ex.Message);
            }
        }

        private void UpdateSummary()
        {
            int pendingCount = allPayments.Count(p => p.StatusPembayaran?.ToLower() == "pending");
            int confirmedCount = allPayments.Count(p => p.StatusPembayaran?.ToLower() == "confirmed");
            int rejectedCount = allPayments.Count(p => p.StatusPembayaran?.ToLower() == "rejected");

            guna2HtmlLabel4.Text = pendingCount.ToString(); // Pending
            guna2HtmlLabel5.Text = confirmedCount.ToString(); // Confirmed
            guna2HtmlLabel6.Text = rejectedCount.ToString(); // Rejected
        }

        private void RefreshPaymentList()
        {
            // Filter payments based on current filter
            List<Pembayaran> filteredPayments = allPayments;

            if (currentFilter == "Pending")
                filteredPayments = allPayments.Where(p => p.StatusPembayaran?.ToLower() == "pending").ToList();
            else if (currentFilter == "Confirmed")
                filteredPayments = allPayments.Where(p => p.StatusPembayaran?.ToLower() == "confirmed").ToList();
            else if (currentFilter == "Rejected")
                filteredPayments = allPayments.Where(p => p.StatusPembayaran?.ToLower() == "rejected").ToList();

            // Update guna2Panel2 to show first payment as example
            if (filteredPayments.Count > 0)
            {
                Pembayaran payment = filteredPayments[0];
                DisplayPaymentInPanel(payment);
            }
        }

        private void DisplayPaymentInPanel(Pembayaran payment)
        {
            // Display payment info in the panel
            guna2HtmlLabel9.Text = payment.Pemesanan?.Penyewa?.NamaPenyewa ?? "Unknown";
            guna2HtmlLabel10.Text = $"Kamar {payment.Pemesanan?.Kamar?.ROOM} - {payment.MetodePembayaran}";
            guna2HtmlLabel11.Text = $"Rp {payment.JumlahBayar:N0}";
            guna2HtmlLabel12.Text = payment.TanggalBayar?.ToString("dd/MM/yyyy") ?? "N/A";
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            // Get first payment from filtered list to show detail
            List<Pembayaran> filteredPayments = GetFilteredPayments();
            if (filteredPayments.Count > 0)
            {
                PembayaranDetail detail = new PembayaranDetail(filteredPayments[0], this);
                detail.Show();
            }
            else
            {
                MessageBox.Show("Tidak ada pembayaran untuk ditampilkan.");
            }
        }

        private List<Pembayaran> GetFilteredPayments()
        {
            List<Pembayaran> filtered = allPayments;

            if (currentFilter == "Pending")
                filtered = allPayments.Where(p => p.StatusPembayaran?.ToLower() == "pending").ToList();
            else if (currentFilter == "Confirmed")
                filtered = allPayments.Where(p => p.StatusPembayaran?.ToLower() == "confirmed").ToList();
            else if (currentFilter == "Rejected")
                filtered = allPayments.Where(p => p.StatusPembayaran?.ToLower() == "rejected").ToList();

            return filtered;
        }

        public void RefreshData()
        {
            LoadPayments();
        }
    }
}
