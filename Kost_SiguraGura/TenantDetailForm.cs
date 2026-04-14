using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace Kost_SiguraGura
{
    public partial class TenantDetailForm : Form
    {
        private Penyewa tenantData;
        private DataPenyewa parentForm;
        private List<TenantPaymentHistory> paymentHistory;

        public TenantDetailForm(Penyewa tenant, DataPenyewa parent)
        {
            InitializeComponent();
            tenantData = tenant;
            parentForm = parent;
            paymentHistory = new List<TenantPaymentHistory>();

            // Setup event handlers
            this.btnSuspend.Click += BtnSuspend_Click;
            this.btnClose.Click += (s, e) => this.Close();

            // Load tenant detail
            LoadTenantDetail();
        }

        private async void LoadTenantDetail()
        {
            try
            {
                // Display tenant basic information
                lblIdValue.Text = tenantData.ID.ToString();
                lblNamaValue.Text = tenantData.NAMA_LENGKAP ?? "-";
                lblEmailValue.Text = tenantData.KONTAK ?? "-";
                lblNoHpValue.Text = tenantData.NOMOR_HP ?? "-";
                lblNikValue.Text = tenantData.NIK ?? "-";
                lblTglLahirValue.Text = tenantData.TANGGAL_LAHIR?.ToString("dd/MM/yyyy") ?? "-";
                lblAlamatValue.Text = tenantData.ALAMAT_ASAL ?? "-";
                lblGenderValue.Text = tenantData.JENIS_KELAMIN ?? "-";
                lblRoleValue.Text = tenantData.PERAN ?? "-";

                // Load and display profile photo if available
                if (!string.IsNullOrEmpty(tenantData.FOTO_PROFIL))
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            var response = await client.GetAsync(tenantData.FOTO_PROFIL);
                            if (response.IsSuccessStatusCode)
                            {
                                var imageData = await response.Content.ReadAsByteArrayAsync();
                                using (var ms = new System.IO.MemoryStream(imageData))
                                {
                                    pictureBoxProfile.Image = Image.FromStream(ms);
                                    pictureBoxProfile.SizeMode = PictureBoxSizeMode.Zoom;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // If image fails to load, just skip
                    }
                }

                // Load payment history
                await LoadPaymentHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading tenant detail: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadPaymentHistory()
        {
            try
            {
                paymentHistory = await ApiClient.GetTenantPaymentHistory(tenantData.ID);

                if (paymentHistory != null && paymentHistory.Count > 0)
                {
                    this.Invoke((MethodInvoker)delegate {
                        // Populate DataGridView with payment history
                        dataGridViewPayments.DataSource = null;
                        dataGridViewPayments.DataSource = paymentHistory;

                        // Format columns
                        dataGridViewPayments.AutoResizeColumns();
                    });
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate {
                        lblPaymentHistoryValue.Text = "Belum ada riwayat pembayaran";
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payment history: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSuspend_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show(
                    $"Apakah Anda yakin ingin suspend tenant: {tenantData.NAMA_LENGKAP}?",
                    "Konfirmasi Suspend",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool success = await ApiClient.SuspendTenant(tenantData.ID);

                    if (success)
                    {
                        MessageBox.Show("Tenant berhasil di-suspend!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh parent form data
                        if (parentForm != null)
                        {
                            parentForm.RefreshTenantData();
                        }

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Gagal untuk suspend tenant!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
