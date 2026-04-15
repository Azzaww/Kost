using System;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    public partial class PaymentCardControl : UserControl
    {
        public Pembayaran PaymentData { get; set; }
        public PembayaranForm ParentForm { get; set; }

        public event EventHandler OnDetailsClicked;
        public event EventHandler OnConfirmClicked;
        public event EventHandler OnRejectClicked;

        public PaymentCardControl(Pembayaran payment, PembayaranForm parent)
        {
            InitializeComponent();
            PaymentData = payment;
            ParentForm = parent;

            // Wire up button events
            btnReceipt.Click += BtnReceipt_Click;
            btnConfirm.Click += BtnConfirm_Click;
            btnReject.Click += BtnReject_Click;

            // Load payment data
            LoadPaymentData();
        }

        private async void LoadPaymentData()
        {
            try
            {
                // Display tenant info (left column)
                lblTenantName.Text = PaymentData.Pemesanan?.Penyewa?.NamaPenyewa ?? "Unknown";
                lblRoomInfo.Text = $"Kamar {PaymentData.Pemesanan?.Kamar?.ROOM} - {PaymentData.MetodePembayaran ?? "-"}";

                // Display amount (right column)
                lblAmount.Text = $"Rp {PaymentData.JumlahBayar:N0}";
                lblDate.Text = PaymentData.TanggalBayar?.ToString("dd/MM/yyyy") ?? "N/A";

                // Display status with button visibility
                SetStatusDisplay();

                // Show/hide receipt button based on payment method
                ShowReceiptIfBankTransfer();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payment data: {ex.Message}");
            }
        }

        private void SetStatusDisplay()
        {
            string status = PaymentData.StatusPembayaran?.ToLower() ?? "";

            if (status == "pending")
            {
                btnConfirm.Visible = true;
                btnReject.Visible = true;
            }
            else
            {
                btnConfirm.Visible = false;
                btnReject.Visible = false;
            }
        }

        private void ShowReceiptIfBankTransfer()
        {
            bool isBankTransfer = PaymentData.MetodePembayaran?.ToLower() == "transfer bank" ||
                PaymentData.MetodePembayaran?.ToLower() == "bank transfer" ||
                (PaymentData.MetodePembayaran?.ToLower().Contains("transfer") ?? false);

            btnReceipt.Visible = isBankTransfer && !string.IsNullOrEmpty(PaymentData.BuktiTransfer);
        }

        private void BtnReceipt_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(PaymentData.BuktiTransfer))
                {
                    MessageBox.Show("Bukti transfer tidak tersedia");
                    return;
                }

                // Open PembayaranDetail modal (sudah include payment details + receipt image)
                PembayaranDetail detail = new PembayaranDetail(PaymentData, ParentForm);
                detail.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void BtnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = await ApiClient.ConfirmPayment(PaymentData.Id);
                if (success)
                {
                    MessageBox.Show("Payment confirmed successfully!");
                    OnConfirmClicked?.Invoke(this, EventArgs.Empty);
                    ParentForm?.RefreshData();
                }
                else
                {
                    MessageBox.Show("Failed to confirm payment");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void BtnReject_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = await ApiClient.RejectPayment(PaymentData.Id);
                if (success)
                {
                    MessageBox.Show("Payment rejected successfully!");
                    OnRejectClicked?.Invoke(this, EventArgs.Empty);
                    ParentForm?.RefreshData();
                }
                else
                {
                    MessageBox.Show("Failed to reject payment");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
