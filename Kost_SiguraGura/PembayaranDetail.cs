using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;

namespace Kost_SiguraGura
{
    public partial class PaymentDetail : Form
    {
        private Pembayaran currentPayment;
        private PembayaranForm parentForm;

        public PaymentDetail()
        {
            InitializeComponent();
        }

        public PaymentDetail(Pembayaran payment, PembayaranForm parent)
        {
            InitializeComponent();
            currentPayment = payment;
            parentForm = parent;
        }

        private void PaymentDetail_Load(object sender, EventArgs e)
        {
            if (currentPayment != null)
            {
                DisplayPaymentDetails();
                // Load image asynchronously to avoid blocking UI
                LoadReceiptImageAsync();
            }
        }

        private async void LoadReceiptImageAsync()
        {
            if (currentPayment == null || string.IsNullOrEmpty(currentPayment.BuktiTransfer))
                return;

            try
            {
                string imageUrl = ConstructFullImageUrl(currentPayment.BuktiTransfer);
                Debug.WriteLine($"[PembayaranDetail] Starting async image load from: {imageUrl}");

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(15);
                    var imageBytes = await client.GetByteArrayAsync(imageUrl);

                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        using (var ms = new System.IO.MemoryStream(imageBytes))
                        {
                            var image = Image.FromStream(ms);
                            pictureBox1.Image = image;
                            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                            Debug.WriteLine($"[PembayaranDetail] Image loaded successfully");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"[PembayaranDetail] Image bytes empty");
                        pictureBox1.Text = "Gambar tidak tersedia";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PembayaranDetail] Error loading image async: {ex.Message}");
                pictureBox1.Text = $"Error: {ex.Message}";
            }
        }

        private void DisplayPaymentDetails()
        {
            try
            {
                // Display payment info
                lblId.Text = currentPayment.Id.ToString();
                lblTenant.Text = currentPayment.Pemesanan?.Penyewa?.NamaPenyewa ?? "Unknown";
                lblRoom.Text = $"Kamar {currentPayment.Pemesanan?.Kamar?.ROOM}";
                lblHarga.Text = $"Rp {currentPayment.JumlahBayar:N0}";
                lblDate.Text = currentPayment.TanggalBayar?.ToString("dd/MM/yyyy") ?? "N/A";

                // Use bilingual display for method and category
                lblMethod.Text = ApiClient.GetBilingualMethod(currentPayment.MetodePembayaran);
                lblKategoriBayar.Text = ApiClient.GetBilingualCategory(currentPayment.TipePembayaran);
                lblDeskripsi.Text = $"Pembayaran untuk kamar {currentPayment.Pemesanan?.Kamar?.ROOM}";

                Debug.WriteLine($"[PembayaranDetail] Payment details displayed. BuktiTransfer: {currentPayment.BuktiTransfer}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PembayaranDetail] Error displaying details: {ex.Message}");
                MessageBox.Show("Error saat menampilkan detail: " + ex.Message);
            }
        }

        private async void ConfirmPayment()
        {
            if (currentPayment == null) return;

            try
            {
                bool success = await ApiClient.ConfirmPayment(currentPayment.Id);
                if (success)
                {
                    MessageBox.Show("Pembayaran berhasil dikonfirmasi!");
                    currentPayment.StatusPembayaran = "Confirmed";
                    this.Close();
                    parentForm?.RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengkonfirmasi pembayaran: " + ex.Message);
            }
        }

        private async void RejectPayment()
        {
            if (currentPayment == null) return;

            try
            {
                bool success = await ApiClient.RejectPayment(currentPayment.Id);
                if (success)
                {
                    MessageBox.Show("Pembayaran berhasil ditolak!");
                    currentPayment.StatusPembayaran = "Rejected";
                    this.Close();
                    parentForm?.RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menolak pembayaran: " + ex.Message);
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            ConfirmPayment();
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            RejectPayment();
        }

        /// <summary>
        /// Construct full absolute URL from bukti_transfer field
        /// Handles both absolute URLs and relative paths
        /// </summary>
        private string ConstructFullImageUrl(string buktiTransfer)
        {
            if (string.IsNullOrEmpty(buktiTransfer))
                return "";

            // If already absolute URL, return as-is
            if (buktiTransfer.StartsWith("http://") || buktiTransfer.StartsWith("https://"))
            {
                return buktiTransfer;
            }

            // If relative path, construct full URL
            string baseUrl = "https://rahmatzaw.elarisnoir.my.id";

            // Ensure relative path starts with /
            if (!buktiTransfer.StartsWith("/"))
            {
                buktiTransfer = "/" + buktiTransfer;
            }

            return baseUrl + buktiTransfer;
        }
    }
}
