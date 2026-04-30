using Newtonsoft.Json;
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

namespace Kost_SiguraGura
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click_1(object sender, EventArgs e)
        {

        }

        private void btnAuth_Click(object sender, EventArgs e)
        {
           
        }

        private void btnAuth_Click_1(object sender, EventArgs e)
        {

        }

        private async void btnAuth_Click_2(object sender, EventArgs e)
        {
            var dataLogin = new LoginRequest { username = txtUsername.Text, password = txtPassword.Text };
            string jsonString = JsonConvert.SerializeObject(dataLogin);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            try
            {
                string url = "https://rahmatzaw.elarisnoir.my.id/api/auth/login";
                HttpResponseMessage response = await ApiClient.Client.PostAsync(url, content);

                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Newtonsoft.Json.Linq.JObject result = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                    Newtonsoft.Json.Linq.JToken userData = null;

                    // --- LOGIKA PENCARI DATA USER (OTOMATIS) ---
                    if (result["users"] != null) userData = result["users"][0];
                    else if (result["user"] != null) userData = result["user"];
                    else if (result["data"] != null) userData = result["data"];
                    else userData = result; // Ambil root jika tidak ada pembungkus

                    if (userData != null && userData.HasValues)
                    {
                        Session.UserId = (long)(userData["id"] ?? 0);

                        // ✅ FIX Issue #3: CRITICAL - Remove default "admin" role
                        // Validate role is present and has valid value
                        string userRole = userData["role"]?.ToString();
                        if (string.IsNullOrEmpty(userRole))
                        {
                            MessageBox.Show("❌ SECURITY ERROR: User role tidak ditemukan dalam response dari server.\n\nTidak dapat melanjutkan login untuk alasan keamanan.", 
                                "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Validate role is one of the expected values
                        string roleLower = userRole.ToLower();
                        if (roleLower != "admin" && roleLower != "tenant" && roleLower != "guest" && roleLower != "non_active")
                        {
                            MessageBox.Show($"❌ SECURITY ERROR: Invalid user role '{userRole}' received from server.\n\nTidak dapat melanjutkan login untuk alasan keamanan.", 
                                "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        Session.UserRole = userRole;
                        Session.Username = userData["username"]?.ToString() ?? txtUsername.Text;

                        Sidebar main = new Sidebar();
                        main.Show();
                        this.Hide();
                    }
                    else
                    {
                        // Jika masih tidak ketemu, kita tampilkan isi JSON-nya biar kita tahu namanya apa
                        MessageBox.Show("Struktur JSON tidak dikenali. Isi Respon:\n" + responseBody);
                    }
                }
                else
                {
                    MessageBox.Show("Login Gagal! Username/Password salah atau Server Error.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi Kesalahan: " + ex.Message);
            }
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
