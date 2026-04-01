using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    public partial class DataPenyewa : UserControl
    {
        private BindingList<Penyewa> bindingListPenyewa = new BindingList<Penyewa>();
        private List<Penyewa> fullListPenyewa = new List<Penyewa>();
        public DataPenyewa()
        {
            InitializeComponent();
        }

        private void DataPenyewa_Load(object sender, EventArgs e)
        {
            if (Session.UserRole?.ToLower() == "admin")
            {
                LoadDataPenyewa();
            }
            else
            {
                MessageBox.Show("Akses Ditolak! Anda bukan Admin. Role Anda: " + Session.UserRole);
            }
        }

        private async void LoadDataPenyewa()
        {
            try
            {
                string url = "https://rahmatzaw.elarisnoir.my.id/api/tenants";

                // Memakai ApiClient.Client agar cookie login ikut terkirim
                HttpResponseMessage response = await ApiClient.Client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Kita bongkar pakai JObject/JArray biar lebih sakti
                    var result = Newtonsoft.Json.Linq.JToken.Parse(jsonResponse);
                    Newtonsoft.Json.Linq.JToken listPenyewaRaw = null;

                    // Cek: Apakah JSON-nya langsung list [{},{}] ?
                    if (result is Newtonsoft.Json.Linq.JArray)
                    {
                        listPenyewaRaw = result;
                    }
                    // Atau dibungkus object {"tenants": []} atau {"penyewas": []} ?
                    else
                    {
                        listPenyewaRaw = result["tenants"] ?? result["penyewas"] ?? result["data"];
                    }

                    if (listPenyewaRaw != null)
                    {
                        // Convert data mentah tadi ke List<Penyewa>
                        fullListPenyewa = listPenyewaRaw.ToObject<List<Penyewa>>();

                        this.Invoke((MethodInvoker)delegate {
                            bindingListPenyewa = new BindingList<Penyewa>(fullListPenyewa);
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = bindingListPenyewa;

                            // Supaya grid otomatis update tampilan
                            dataGridView1.Refresh();
                        });
                    }
                    else
                    {
                        // Jika masih gagal, kita intip isi JSON-nya
                        MessageBox.Show("Data tidak ketemu di JSON. Isi aslinya:\n" + jsonResponse);
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Sesi login habis atau tidak sah. Silakan login ulang.");
                }
                else
                {
                    MessageBox.Show("Gagal ambil data. Status: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat bongkar data: " + ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                bindingListPenyewa = new BindingList<Penyewa>(fullListPenyewa);
            }
            else
            {
                var filtered = fullListPenyewa.Where(p =>
                    (p.NAMA_LENGKAP != null && p.NAMA_LENGKAP.ToLower().Contains(keyword))
                ).ToList();

                bindingListPenyewa = new BindingList<Penyewa>(filtered);
            }

            dataGridView1.DataSource = bindingListPenyewa;
        }
    }
}
