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
        private FlowLayoutPanel flowLayoutPanelPayments;
        private List<PaymentCardControl> paymentCards = new List<PaymentCardControl>();
        private Panel panelPaymentTimeline;

        public PembayaranForm()
        {
            InitializeComponent();
            SetupFlowLayoutPanel();
            this.Load += PembayaranForm_Load;
        }

        private void SetupFlowLayoutPanel()
        {
            // Create Timeline container panel
            panelPaymentTimeline = new Panel();
            panelPaymentTimeline.AutoSize = false;
            panelPaymentTimeline.Dock = DockStyle.Bottom;
            panelPaymentTimeline.Height = 500;
            panelPaymentTimeline.BackColor = Color.FromArgb(245, 245, 245);
            panelPaymentTimeline.BorderStyle = BorderStyle.None;
            panelPaymentTimeline.Padding = new Padding(20);

            // Add label "Payment Timeline"
            Label lblTimeline = new Label();
            lblTimeline.Text = "Payment Timeline";
            lblTimeline.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTimeline.ForeColor = Color.Black;
            lblTimeline.AutoSize = true;
            lblTimeline.Location = new Point(20, 10);
            panelPaymentTimeline.Controls.Add(lblTimeline);

            // Create FlowLayoutPanel for vertical card layout
            flowLayoutPanelPayments = new FlowLayoutPanel();
            flowLayoutPanelPayments.AutoScroll = true;
            flowLayoutPanelPayments.Dock = DockStyle.Fill;
            flowLayoutPanelPayments.Padding = new Padding(15);
            flowLayoutPanelPayments.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelPayments.WrapContents = false;
            flowLayoutPanelPayments.BackColor = Color.White;
            flowLayoutPanelPayments.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanelPayments.Margin = new Padding(20, 40, 20, 20);

            panelPaymentTimeline.Controls.Add(flowLayoutPanelPayments);
        }

        private void PembayaranForm_Load(object sender, EventArgs e)
        {
            if (Session.UserRole?.ToLower() == "admin")
            {
                // Add timeline panel to parent form
                if (!this.Controls.Contains(panelPaymentTimeline))
                {
                    this.Controls.Add(panelPaymentTimeline);
                    panelPaymentTimeline.BringToFront();
                }

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
            // Clear existing cards
            flowLayoutPanelPayments.Controls.Clear();
            paymentCards.Clear();

            // Filter payments based on current filter
            List<Pembayaran> filteredPayments = GetFilteredPayments();

            // Generate payment card for each payment (vertical layout)
            foreach (var payment in filteredPayments)
            {
                try
                {
                    PaymentCardControl card = new PaymentCardControl(payment, this);
                    card.Width = flowLayoutPanelPayments.Width - 40; // Full width minus padding
                    card.Height = 100;
                    flowLayoutPanelPayments.Controls.Add(card);
                    paymentCards.Add(card);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating payment card: {ex.Message}");
                }
            }

            // If no payments, show message
            if (filteredPayments.Count == 0)
            {
                Label lblNoPayments = new Label();
                lblNoPayments.Text = "Tidak ada pembayaran untuk ditampilkan";
                lblNoPayments.AutoSize = true;
                lblNoPayments.Font = new Font("Segoe UI", 11F);
                lblNoPayments.ForeColor = Color.Gray;
                flowLayoutPanelPayments.Controls.Add(lblNoPayments);
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

        public void SetFilter(string filter)
        {
            currentFilter = filter;
            RefreshPaymentList();
        }
    }
}

