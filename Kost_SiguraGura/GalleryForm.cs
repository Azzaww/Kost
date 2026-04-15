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
    public partial class GalleryForm : UserControl
    {
        // Data source
        private List<Gallery> allGalleries = new List<Gallery>();
        private List<Gallery> filteredGalleries = new List<Gallery>();

        public GalleryForm()
        {
            InitializeComponent();
        }

        private void GalleryForm_Load(object sender, EventArgs e)
        {
            // Hide dummy panels - they are only for design preview
            this.guna2Panel1.Visible = false;
            this.guna2Panel2.Visible = false;

            LoadGalleries();

            // Wire up events
            this.btnAddImage.Click += btnAddImage_Click;
            this.txtSearch.TextChanged += txtSearch_TextChanged;
        }

        /// <summary>
        /// Load semua gallery images dari API
        /// </summary>
        private async void LoadGalleries()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    string url = "https://rahmatzaw.elarisnoir.my.id/api/galleries";

                    System.Diagnostics.Debug.WriteLine($"🔄 Loading galleries from: {url}");

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"📄 API Response Length: {jsonResponse.Length} bytes");
                        System.Diagnostics.Debug.WriteLine($"📄 API Response Sample: {jsonResponse.Substring(0, Math.Min(300, jsonResponse.Length))}");

                        // Validate response is not empty
                        if (string.IsNullOrWhiteSpace(jsonResponse))
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ API returned empty response!");
                            MessageBox.Show("⚠️ API mengembalikan data kosong. Coba tambahkan gallery terlebih dahulu.", "Info",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        allGalleries = JsonConvert.DeserializeObject<List<Gallery>>(jsonResponse) ?? new List<Gallery>();

                        // Apply filters
                        ApplyFilters();

                        System.Diagnostics.Debug.WriteLine($"✅ Gallery loaded: {allGalleries.Count} images");
                        foreach (var gal in allGalleries)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - ID: {gal.id}, Title: {gal.title}, Category: {gal.category}, Image: {gal.image_url}, Created: {gal.created_at}");
                        }
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"❌ API Error: {response.StatusCode} - {errorContent}");
                        MessageBox.Show($"❌ Failed to load galleries: {response.StatusCode}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Network error: {httpEx.Message}");
                MessageBox.Show($"❌ Network error loading galleries: {httpEx.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (JsonException jsonEx)
            {
                System.Diagnostics.Debug.WriteLine($"❌ JSON Parse error: {jsonEx.Message}");
                MessageBox.Show($"❌ Error parsing gallery data: {jsonEx.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading galleries: {ex.Message}");
                MessageBox.Show($"❌ Error loading galleries: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Apply filters dan display di UI
        /// </summary>
        private void ApplyFilters()
        {
            string searchKeyword = txtSearch?.Text?.ToLower().Trim() ?? "";

            // Filter by search keyword (title atau category)
            filteredGalleries = allGalleries
                .Where(g => string.IsNullOrEmpty(searchKeyword) || 
                           (g.title != null && g.title.ToLower().Contains(searchKeyword)) ||
                           (g.category != null && g.category.ToLower().Contains(searchKeyword)))
                .ToList();

            System.Diagnostics.Debug.WriteLine($"🔍 Filter applied: '{searchKeyword}' - Found {filteredGalleries.Count} galleries");

            DisplayGalleries();
        }

        /// <summary>
        /// Display galleries sebagai card view di FlowLayoutPanel
        /// </summary>
        private void DisplayGalleries()
        {
            // Assuming there's a FlowLayoutPanel named flowLayoutPanel1
            // If tidak ada, kita perlu add via designer

            if (this.Controls.Find("flowLayoutPanel1", false).FirstOrDefault() is FlowLayoutPanel flowPanel)
            {
                flowPanel.Controls.Clear();

                System.Diagnostics.Debug.WriteLine($"📊 Displaying {filteredGalleries.Count} galleries in FlowLayoutPanel");

                foreach (var gallery in filteredGalleries)
                {
                    // Create card panel - optimized for 3 cards per 777px row
                    Panel cardPanel = new Panel();
                    cardPanel.Width = 280;
                    cardPanel.Height = 280;
                    cardPanel.BorderStyle = BorderStyle.FixedSingle;
                    cardPanel.BackColor = Color.White;
                    cardPanel.Margin = new Padding(8);
                    cardPanel.Tag = gallery; // Store gallery object untuk reference
                    cardPanel.DoubleClick += (s, e) => ShowGalleryDetails(gallery);

                    // Create image picture box
                    PictureBox picImage = new PictureBox();
                    picImage.Top = 0;
                    picImage.Left = 0;
                    picImage.Width = 280;
                    picImage.Height = 160;
                    picImage.BackColor = Color.LightGray;
                    picImage.SizeMode = PictureBoxSizeMode.Zoom;
                    picImage.BorderStyle = BorderStyle.None;
                    picImage.DoubleClick += (s, e) => ShowGalleryDetails(gallery);

                    // Load image async (fire and forget pattern with proper error handling)
                    _ = LoadImageAsync(picImage, gallery.image_url);

                    // Create title label
                    Label lblTitle = new Label();
                    lblTitle.Text = gallery.title ?? "No Title";
                    lblTitle.Top = 160;
                    lblTitle.Left = 0;
                    lblTitle.Width = 500;
                    lblTitle.AutoSize = false;
                    lblTitle.Height = 50;
                    lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    lblTitle.ForeColor = Color.Black;
                    lblTitle.TextAlign = ContentAlignment.TopLeft;
                    lblTitle.Padding = new Padding(5);
                    lblTitle.DoubleClick += (s, e) => ShowGalleryDetails(gallery);

                    // Create category label
                    Label lblCategory = new Label();
                    lblCategory.Text = $"📁 {gallery.category ?? "No Category"}";
                    lblCategory.Top = 210;
                    lblCategory.Left = 0;
                    lblCategory.Width = 280;
                    lblCategory.AutoSize = false;
                    lblCategory.Height = 25;
                    lblCategory.Font = new Font("Segoe UI", 9);
                    lblCategory.ForeColor = Color.Orange;
                    lblCategory.TextAlign = ContentAlignment.TopLeft;
                    lblCategory.Padding = new Padding(5, 0, 5, 5);
                    lblCategory.DoubleClick += (s, e) => ShowGalleryDetails(gallery);

                    // Create date label
                    Label lblDate = new Label();
                    lblDate.Text = gallery.created_at.ToString("dd MMM yyyy");
                    lblDate.Top = 235;
                    lblDate.Left = 0;
                    lblDate.Width = 500;
                    lblDate.AutoSize = false;
                    lblDate.Height = 20;
                    lblDate.Font = new Font("Segoe UI", 8);
                    lblDate.ForeColor = Color.Gray;
                    lblDate.TextAlign = ContentAlignment.TopLeft;
                    lblDate.Padding = new Padding(5, 0, 5, 5);

                    // Add controls to card
                    cardPanel.Controls.Add(picImage);
                    cardPanel.Controls.Add(lblTitle);
                    cardPanel.Controls.Add(lblCategory);
                    cardPanel.Controls.Add(lblDate);

                    // Add card to flow panel
                    flowPanel.Controls.Add(cardPanel);
                }

                System.Diagnostics.Debug.WriteLine($"✅ Successfully displayed {filteredGalleries.Count} galleries");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ FlowLayoutPanel not found! Please add 'flowLayoutPanel1' to form designer.");
            }
        }

        /// <summary>
        /// Load image dari URL async dengan better memory management
        /// Handle both Full CDN URL dan Local Path fallback
        /// </summary>
        private async Task LoadImageAsync(PictureBox pictureBox, string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Empty image URL");
                    return;
                }

                // Handle local path fallback (jika backend return /gallery/12345.jpg)
                string finalUrl = imageUrl;
                if (imageUrl.StartsWith("/"))
                {
                    // Prepend backend base URL untuk local path
                    finalUrl = "https://rahmatzaw.elarisnoir.my.id" + imageUrl;
                    System.Diagnostics.Debug.WriteLine($"📷 Converting local path to full URL: {finalUrl}");
                }

                System.Diagnostics.Debug.WriteLine($"📷 Loading image: {finalUrl}");

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    byte[] imageBytes = await client.GetByteArrayAsync(finalUrl);

                    // Keep reference to old image for disposal
                    Image oldImage = null;

                    using (var ms = new System.IO.MemoryStream(imageBytes))
                    {
                        var img = System.Drawing.Image.FromStream(ms);

                        // Must be on UI thread
                        if (pictureBox.InvokeRequired)
                        {
                            pictureBox.Invoke(new Action(() =>
                            {
                                oldImage = pictureBox.Image;
                                pictureBox.Image = img;
                                oldImage?.Dispose();
                            }));
                        }
                        else
                        {
                            oldImage = pictureBox.Image;
                            pictureBox.Image = img;
                            oldImage?.Dispose();
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"✅ Image loaded successfully: {finalUrl}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Network error loading image: {httpEx.Message}");
                if (pictureBox.InvokeRequired)
                {
                    pictureBox.Invoke(new Action(() => pictureBox.BackColor = Color.LightGray));
                }
                else
                {
                    pictureBox.BackColor = Color.LightGray;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Error loading image: {ex.Message}");
                if (pictureBox.InvokeRequired)
                {
                    pictureBox.Invoke(new Action(() => pictureBox.BackColor = Color.LightGray));
                }
                else
                {
                    pictureBox.BackColor = Color.LightGray;
                }
            }
        }

        /// <summary>
        /// Show gallery details di modal (untuk delete)
        /// </summary>
        private void ShowGalleryDetails(Gallery gallery)
        {
            // Create modal form
            Form modalForm = new Form();
            modalForm.Text = "Gallery Details";
            modalForm.Width = 500;
            modalForm.Height = 550;
            modalForm.StartPosition = FormStartPosition.CenterParent;
            modalForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            modalForm.MaximizeBox = false;
            modalForm.MinimizeBox = false;

            // Image display
            PictureBox picDisplay = new PictureBox();
            picDisplay.Width = 450;
            picDisplay.Height = 250;
            picDisplay.Top = 20;
            picDisplay.Left = 25;
            picDisplay.SizeMode = PictureBoxSizeMode.Zoom;
            picDisplay.BorderStyle = BorderStyle.FixedSingle;
            // Load image async (fire and forget)
            _ = LoadImageAsync(picDisplay, gallery.image_url);

            // Title label + textbox
            Label lblTitle = new Label();
            lblTitle.Text = "Title:";
            lblTitle.Top = 280;
            lblTitle.Left = 25;
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            TextBox txtTitle = new TextBox();
            txtTitle.Text = gallery.title ?? "";
            txtTitle.ReadOnly = true; // Read-only untuk view only
            txtTitle.Top = 305;
            txtTitle.Left = 25;
            txtTitle.Width = 450;
            txtTitle.Height = 30;
            txtTitle.Font = new Font("Segoe UI", 10);

            // Category label + textbox
            Label lblCategory = new Label();
            lblCategory.Text = "Category:";
            lblCategory.Top = 340;
            lblCategory.Left = 25;
            lblCategory.AutoSize = true;
            lblCategory.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            TextBox txtCategory = new TextBox();
            txtCategory.Text = gallery.category ?? "";
            txtCategory.ReadOnly = true; // Read-only untuk view only
            txtCategory.Top = 365;
            txtCategory.Left = 25;
            txtCategory.Width = 450;
            txtCategory.Height = 30;
            txtCategory.Font = new Font("Segoe UI", 10);

            // Delete button
            Button btnDelete = new Button();
            btnDelete.Text = "🗑️ Delete";
            btnDelete.Top = 410;
            btnDelete.Left = 25;
            btnDelete.Width = 450;
            btnDelete.Height = 35;
            btnDelete.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDelete.BackColor = Color.FromArgb(255, 107, 107); // Red
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.Click += async (s, e) =>
            {
                DialogResult confirmDelete = MessageBox.Show(
                    $"Apakah Anda yakin ingin menghapus gambar '{gallery.title}'?",
                    "Konfirmasi Hapus",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmDelete == DialogResult.Yes)
                {
                    await DeleteGalleryAsync(gallery.id);
                    modalForm.Close();
                }
            };

            // Add controls to modal
            modalForm.Controls.Add(picDisplay);
            modalForm.Controls.Add(lblTitle);
            modalForm.Controls.Add(txtTitle);
            modalForm.Controls.Add(lblCategory);
            modalForm.Controls.Add(txtCategory);
            modalForm.Controls.Add(btnDelete);

            // Show modal
            modalForm.ShowDialog();
        }

        /// <summary>
        /// Delete gallery async
        /// </summary>
        private async Task DeleteGalleryAsync(int galleryId)
        {
            try
            {
                string url = $"https://rahmatzaw.elarisnoir.my.id/api/galleries/{galleryId}";
                HttpResponseMessage response = await ApiClient.Client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("✅ Gambar berhasil dihapus!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Remove dari local list
                    allGalleries.RemoveAll(g => g.id == galleryId);

                    // Refresh display
                    ApplyFilters();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("❌ Session expired! Silakan login kembali.", "Auth Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("❌ Gambar tidak ditemukan!", "Not Found Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"❌ Gagal menghapus gambar!\n\n{errorContent}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"❌ Network Error: {httpEx.Message}", "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tombol Add Image - buka form AddGallery
        /// </summary>
        private void btnAddImage_Click(object sender, EventArgs e)
        {
            AddGallery form = new AddGallery();
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Refresh gallery setelah upload baru
                LoadGalleries();
            }
        }

        // Event handler for search
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }
    }

    /// <summary>
    /// Gallery DTO untuk deserialize JSON response dari API
    /// </summary>
    public class Gallery
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("category")]
        public string category { get; set; }

        [JsonProperty("image_url")]
        public string image_url { get; set; }

        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
    }
}
