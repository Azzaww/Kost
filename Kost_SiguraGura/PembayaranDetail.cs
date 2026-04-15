using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

                // Display receipt image if available
                if (!string.IsNullOrEmpty(currentPayment.BuktiTransfer))
                {
                    try
                    {
                        pictureBox1.ImageLocation = currentPayment.BuktiTransfer;
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    catch (Exception ex)
                    {
                        pictureBox1.Text = "Gagal memuat gambar: " + ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
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
    }
}
