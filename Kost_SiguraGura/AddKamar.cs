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
using Newtonsoft.Json;

namespace Kost_SiguraGura
{
    public partial class AddKamar : Form
    {
        public AddKamar()
        {
            InitializeComponent();
            // Wiring events manually if not done in designer
            this.btnCreate.Click += btnCreate_Click;
            this.btnCancel.Click += (s, e) => this.Close();
            this.btnImage.Click += btnImage_Click;
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    guna2TextBox3.Text = ofd.FileName;
                }
            }
        }

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRoomName.Text))
            {
                MessageBox.Show("Room Name is required!");
                return;
            }

            var data = new
            {
                nomor_kamar = txtRoomName.Text,
                harga_per_bulan = NuPrice.Value,
                tipe_kamar = CbType.SelectedItem?.ToString() ?? "Standard",
                status = CbStatus.SelectedItem?.ToString() ?? "Tersedia",
                kapasitas = NuCapacity.Value,
                floor = NuFloor.Value,
                size = txtSize.Text,
                bedrooms = NuBedrooms.Value,
                bathrooms = NuBathrooms.Value,
                facilities = guna2TextBox1.Text,
                description = guna2TextBox2.Text
                // Catatan: Implementasi upload file gambar biasanya membutuhkan MultipartFormDataContent
            };

            try
            {
                string json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                string url = "https://rahmatzaw.elarisnoir.my.id/api/kamar";
                HttpResponseMessage response = await ApiClient.Client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Room created successfully!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to save data. Status: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
