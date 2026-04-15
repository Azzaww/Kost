using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Kost_SiguraGura
{
    public partial class AddGallery : Form
    {
        // Variabel untuk menyimpan path gambar yang dipilih
        private string selectedImagePath = string.Empty;

        public AddGallery()
        {
            InitializeComponent();
        }

        private void AddGallery_Load(object sender, EventArgs e)
        {
            // Set placeholder text untuk TextBox
            if (txtImageTitle.Text == "")
                txtImageTitle.PlaceholderText = "e.g., Deluxe Room Interior";

            if (txtCategory.Text == "")
                txtCategory.PlaceholderText = "e.g., Interior, Facilities";

            // Wire up event handlers
            this.btnImage.Click += btnImage_Click;
            this.btnUpload.Click += btnUpload_Click;
        }

        /// <summary>
        /// Tombol untuk memilih gambar dari local storage
        /// </summary>
        private void btnImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp";
                ofd.Title = "Pilih Gambar untuk Gallery";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        selectedImagePath = ofd.FileName;

                        // Display file info
                        FileInfo fileInfo = new FileInfo(selectedImagePath);
                        string fileName = Path.GetFileName(selectedImagePath);
                        double fileSizeMB = fileInfo.Length / (1024.0 * 1024.0);

                        // Update ImageTextBox3 dengan info file
                        ImageTextBox3.Text = $"✅ File Dipilih:\n\n" +
                            $"Nama: {fileName}\n" +
                            $"Ukuran: {fileSizeMB:F2} MB\n" +
                            $"Path: {selectedImagePath}";

                        MessageBox.Show($"✅ Gambar berhasil dipilih!\n\nNama: {fileName}\nUkuran: {fileSizeMB:F2} MB",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ Error reading file: {ex.Message}", "File Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Validasi input sebelum upload
        /// </summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtImageTitle.Text))
            {
                MessageBox.Show("❌ Image Title wajib diisi!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtImageTitle.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("❌ Category wajib diisi!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategory.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(selectedImagePath))
            {
                MessageBox.Show("❌ Silakan pilih gambar terlebih dahulu!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!File.Exists(selectedImagePath))
            {
                MessageBox.Show("❌ File gambar tidak ditemukan!", "File Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                selectedImagePath = string.Empty;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tombol Upload - submit gambar ke API
        /// </summary>
        private async void btnUpload_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            btnUpload.Enabled = false;
            btnUpload.Text = "Uploading...";

            try
            {
                // Create multipart form data
                var formData = new MultipartFormDataContent();

                // Add title field
                formData.Add(new StringContent(txtImageTitle.Text.Trim()), "title");

                // Add category field
                formData.Add(new StringContent(txtCategory.Text.Trim()), "category");

                // Add image file
                byte[] imageBytes = File.ReadAllBytes(selectedImagePath);
                var fileContent = new ByteArrayContent(imageBytes);

                // Determine content type
                string extension = Path.GetExtension(selectedImagePath).ToLower();
                switch (extension)
                {
                    case ".jpg":
                    case ".jpeg":
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                        break;
                    case ".png":
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                        break;
                    case ".gif":
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/gif");
                        break;
                    case ".bmp":
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/bmp");
                        break;
                    default:
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                        break;
                }

                // ✅ PENTING: Field name HARUS "image" sesuai backend API
                formData.Add(fileContent, "image", Path.GetFileName(selectedImagePath));

                // POST ke API
                string url = "https://rahmatzaw.elarisnoir.my.id/api/galleries";
                HttpResponseMessage response = await ApiClient.Client.PostAsync(url, formData);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var galleryResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    MessageBox.Show($"✅ Gambar berhasil di-upload!\n\n" +
                        $"Title: {galleryResponse["title"]}\n" +
                        $"Category: {galleryResponse["category"]}\n" +
                        $"Upload Time: {galleryResponse["created_at"]}", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset form
                    ResetForm();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"❌ Data tidak valid!\n\n{errorContent}", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("❌ Session expired! Silakan login kembali.", "Auth Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    MessageBox.Show("❌ Server error!\nHubungi administrator.", "Server Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"❌ Error: {(int)response.StatusCode} - {response.ReasonPhrase}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                formData.Dispose();
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"❌ Network Error:\n{httpEx.Message}\n\nPastikan internet connection aktif.",
                    "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show("❌ Request timeout!\nServer tidak merespons dalam waktu lama.\n\nCoba lagi.",
                    "Timeout Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ioEx)
            {
                MessageBox.Show($"❌ File Error:\n{ioEx.Message}\n\nPastikan file gambar valid.",
                    "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Unexpected Error:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnUpload.Enabled = true;
                btnUpload.Text = "Upload Asset";
            }
        }

        /// <summary>
        /// Reset form ke state awal
        /// </summary>
        private void ResetForm()
        {
            txtImageTitle.Text = "";
            txtCategory.Text = "";
            ImageTextBox3.Text = "";
            selectedImagePath = string.Empty;
        }

        private void txtImageTitle_TextChanged(object sender, EventArgs e)
        {
            // Placeholder handling if needed
        }

        private void txtCategory_TextChanged(object sender, EventArgs e)
        {
            // Placeholder handling if needed
        }

        private void ImageTextBox3_TextChanged(object sender, EventArgs e)
        {
            // Read-only display of selected image info
        }
    }
}
