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
                // ✅ FIX: Use ActiveBaseUrl instead of hardcoded production URL
                string url = $"{ApiClient.ActiveBaseUrl}/auth/login";
                System.Diagnostics.Debug.WriteLine($"📍 Attempting login to: {url}");
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

                        // ✅ NEW: Extract and save tokens from response
                        // Backend returns: { "user": {...}, "accessToken": "...", "refreshToken": "...", "expiresIn": 900 }
                        string accessToken = result["accessToken"]?.ToString() ?? result["access_token"]?.ToString();
                        string refreshToken = result["refreshToken"]?.ToString() ?? result["refresh_token"]?.ToString();

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            Session.Token = accessToken;
                        }
                        else
                        {
                            MessageBox.Show("❌ SECURITY ERROR: accessToken tidak ditemukan dalam response dari server.\n\nTidak dapat melanjutkan login untuk alasan keamanan.", 
                                "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (!string.IsNullOrEmpty(refreshToken))
                        {
                            Session.RefreshToken = refreshToken;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("⚠️ WARNING: refreshToken tidak ditemukan dalam response. Auto-refresh mungkin tidak akan bekerja.");
                        }

                        System.Diagnostics.Debug.WriteLine($"✅ Login sukses! Token disimpan. ExpiresIn: {result["expiresIn"]}");

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
                    System.Diagnostics.Debug.WriteLine($"❌ Login Failed - Status Code: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"📄 Response Body: {responseBody}");
                    MessageBox.Show($"Login Gagal!\n\nStatus: {response.StatusCode}\n\nResponse:\n{responseBody}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Exception during login: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"📋 Stack Trace: {ex.StackTrace}");
                MessageBox.Show("Terjadi Kesalahan:\n" + ex.Message + "\n\nLihat Debug Output untuk detail lebih lengkap.");
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
