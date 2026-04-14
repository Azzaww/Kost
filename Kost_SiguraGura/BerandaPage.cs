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

namespace Kost_SiguraGura
{
    public partial class BerandaPage : UserControl
    {
        public BerandaPage()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void BerandaPage_Load(object sender, EventArgs e)
        {
            await UpdateTotalPendapatan();
        }
        private async Task UpdateTotalPendapatan()
        {
            string url = "https://rahmatzaw.elarisnoir.my.id/api/payments";

            try
            {
                // Gunakan ApiClient agar session login tetap terbawa (Cookie/Token)
                HttpResponseMessage response = await ApiClient.Client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.Linq.JToken.Parse(jsonResponse);

                    Newtonsoft.Json.Linq.JToken listPembayaranRaw;

                    // Cek apakah JSON berbentuk Array [] atau Object {}
                    if (result is Newtonsoft.Json.Linq.JArray)
                    {
                        listPembayaranRaw = result;
                    }
                    else
                    {
                        // Jika Object, cari key pembayarans atau data
                        listPembayaranRaw = result["pembayarans"] ?? result["data"] ?? result;
                    }

                    if (listPembayaranRaw != null)
                    {
                        var listPembayaran = listPembayaranRaw.ToObject<List<Pembayaran>>();

                        if (listPembayaran != null && listPembayaran.Count > 0)
                        {
                            // LINQ: Filter hanya yang statusnya "Confirmed" lalu jumlahkan
                            decimal total = listPembayaran
                                .Where(p => p.StatusPembayaran != null &&
                                            p.StatusPembayaran.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
                                .Sum(p => p.JumlahBayar);

                            // Update UI lewat Invoke agar aman dari cross-thread error
                            this.Invoke((MethodInvoker)delegate {
                                lblIncome.Text = FormatKeRupiahSingkat((long)total);
                            });
                        }
                        else
                        {
                            lblIncome.Text = "Rp 0";
                        }
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    lblIncome.Text = "No Access";
                    MessageBox.Show("Sesi login habis, silakan login ulang.");
                }
                else
                {
                    lblIncome.Text = "Error " + (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                lblIncome.Text = "Error";
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
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

