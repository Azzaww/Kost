using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace Kost_SiguraGura
{
    public partial class AddKamar : Form
    {
        // Array untuk menyimpan 3 path gambar
        private string[] selectedImagePaths = new string[3];
        private int currentImageIndex = 0;  // Index gambar saat ini (0, 1, 2)

        public AddKamar()
        {
            InitializeComponent();
            // Initialize array kosong
            for (int i = 0; i < 3; i++)
            {
                selectedImagePaths[i] = string.Empty;
            }

            // ✅ RESET currentImageIndex setiap kali form dibuka
            currentImageIndex = 0;

            // ✅ SET MAXIMUM VALUE untuk NumericUpDown agar bisa handle nilai besar
            NuPrice.Maximum = 10000000; // 10 juta
            NuCapacity.Maximum = 1000;
            NuFloor.Maximum = 100;
            NuBedrooms.Maximum = 100;
            NuBathrooms.Maximum = 100;

            // Setup ComboBox Status dengan bilingual options
            SetupStatusComboBox();

            // Wiring events manually if not done in designer
            this.btnCreate.Click += btnCreate_Click;
            this.btnCancel.Click += (s, e) => this.Close();
            this.btnImage.Click += btnImage_Click;
        }

        /// <summary>
        /// Setup ComboBox Status dengan bilingual options (Indonesia/English)
        /// </summary>
        private void SetupStatusComboBox()
        {
            CbStatus.Items.Clear();
            CbStatus.Items.Add("Tersedia / Available");
            CbStatus.Items.Add("Penuh / Full");
            CbStatus.Items.Add("Perbaikan / Maintenance");
            CbStatus.SelectedIndex = 0; // Default ke Tersedia
        }

        /// <summary>
        /// Tombol untuk memilih gambar dari local storage
        /// Minimal 3 gambar harus dipilih (API requirement)
        /// </summary>
        private void btnImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files(*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp";
                ofd.Title = $"Pilih Gambar Kamar (Gambar {currentImageIndex + 1}/3)";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Simpan path ke array
                        selectedImagePaths[currentImageIndex] = ofd.FileName;

                        // Update display di ImageTextBox3 dengan info ketiga gambar
                        UpdateImagePreviewDisplay();

                        // Auto-move ke gambar berikutnya jika belum semua
                        if (currentImageIndex < 2)
                        {
                            currentImageIndex++;
                            MessageBox.Show($"✅ Gambar {currentImageIndex}/3 berhasil dipilih!\n\nSilakan pilih gambar berikutnya.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("✅ Ketiga gambar sudah dipilih!\n\nSiap untuk di-upload. Klik Create untuk melanjutkan.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
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
        /// Update display preview gambar di ImageTextBox3
        /// Menampilkan status ketiga gambar
        /// </summary>
        private void UpdateImagePreviewDisplay()
        {
            StringBuilder preview = new StringBuilder();

            for (int i = 0; i < 3; i++)
            {
                if (string.IsNullOrEmpty(selectedImagePaths[i]))
                {
                    preview.AppendLine($"Image {i + 1}: ❌ Not selected");
                }
                else if (!File.Exists(selectedImagePaths[i]))
                {
                    preview.AppendLine($"Image {i + 1}: ⚠️ File not found");
                }
                else
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(selectedImagePaths[i]);
                        double fileSizeMB = fileInfo.Length / (1024.0 * 1024.0);
                        string fileName = Path.GetFileName(selectedImagePaths[i]);
                        preview.AppendLine($"Image {i + 1}: ✅ {fileName} ({fileSizeMB:F2} MB)");
                    }
                    catch
                    {
                        preview.AppendLine($"Image {i + 1}: ⚠️ Error reading");
                    }
                }
            }

            ImageTextBox3.Text = preview.ToString().Trim();
        }

        /// <summary>
        /// Validasi semua input sebelum submit
        /// </summary>
        private bool ValidateInput()
        {
            // 1. Cek Room Name
            if (string.IsNullOrWhiteSpace(txtRoomName.Text))
            {
                MessageBox.Show("❌ Room Name wajib diisi!", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRoomName.Focus();
                return false;
            }

            // 2. Cek Harga
            if (NuPrice.Value <= 0)
            {
                MessageBox.Show("❌ Harga harus lebih dari 0!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NuPrice.Focus();
                return false;
            }

            // 3. Cek Tipe Kamar
            if (CbType.SelectedIndex < 0)
            {
                MessageBox.Show("❌ Pilih Tipe Kamar!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CbType.Focus();
                return false;
            }

            // 4. Cek Status
            if (CbStatus.SelectedIndex < 0)
            {
                MessageBox.Show("❌ Pilih Status Kamar!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CbStatus.Focus();
                return false;
            }

            // 5. Cek Kapasitas
            if (NuCapacity.Value <= 0)
            {
                MessageBox.Show("❌ Kapasitas harus lebih dari 0!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NuCapacity.Focus();
                return false;
            }

            // 6. Cek Floor
            if (NuFloor.Value <= 0)
            {
                MessageBox.Show("❌ Lantai harus lebih dari 0!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NuFloor.Focus();
                return false;
            }

            // 7. Cek Size
            if (string.IsNullOrWhiteSpace(txtSize.Text))
            {
                MessageBox.Show("❌ Ukuran Kamar wajib diisi!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSize.Focus();
                return false;
            }

            // 8. Cek Bedrooms
            if (NuBedrooms.Value <= 0)
            {
                MessageBox.Show("❌ Jumlah Kamar Tidur harus lebih dari 0!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NuBedrooms.Focus();
                return false;
            }

            // 9. Cek Bathrooms
            if (NuBathrooms.Value <= 0)
            {
                MessageBox.Show("❌ Jumlah Kamar Mandi harus lebih dari 0!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NuBathrooms.Focus();
                return false;
            }

            // 10. Cek Facilities
            if (string.IsNullOrWhiteSpace(guna2TextBox1.Text))
            {
                MessageBox.Show("❌ Fasilitas wajib diisi!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox1.Focus();
                return false;
            }

            // 11. Cek Description
            if (string.IsNullOrWhiteSpace(guna2TextBox2.Text))
            {
                MessageBox.Show("❌ Deskripsi wajib diisi!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox2.Focus();
                return false;
            }

            // 12. ✅ CEK 3 GAMBAR (API REQUIREMENT)
            for (int i = 0; i < 3; i++)
            {
                System.Diagnostics.Debug.WriteLine($"VALIDATION DEBUG: Image {i + 1} path: '{selectedImagePaths[i]}'");

                if (string.IsNullOrEmpty(selectedImagePaths[i]))
                {
                    MessageBox.Show($"❌ Gambar {i + 1} belum dipilih!\n\nAPI memerlukan minimal 3 gambar kamar.",
                        "Missing Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!File.Exists(selectedImagePaths[i]))
                {
                    MessageBox.Show($"❌ File gambar {i + 1} tidak ditemukan!\n\nSilakan pilih ulang.",
                        "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    selectedImagePaths[i] = string.Empty;
                    return false;
                }

                // ✅ FIX Issue #4: Add file size validation (max 5MB per image)
                try
                {
                    FileInfo fileInfo = new FileInfo(selectedImagePaths[i]);
                    long fileSizeBytes = fileInfo.Length;
                    double fileSizeMB = fileSizeBytes / (1024.0 * 1024.0);
                    const long MAX_FILE_SIZE_BYTES = 5 * 1024 * 1024; // 5MB

                    if (fileSizeBytes > MAX_FILE_SIZE_BYTES)
                    {
                        MessageBox.Show($"❌ File gambar {i + 1} terlalu besar!\n\n" +
                            $"Ukuran: {fileSizeMB:F2} MB\n" +
                            $"Maksimal: 5 MB\n\n" +
                            $"Silakan pilih gambar dengan ukuran lebih kecil.",
                            "File Too Large", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Error checking file size untuk gambar {i + 1}: {ex.Message}",
                        "File Size Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"VALIDATION DEBUG: Image {i + 1} found and valid");
            }

            System.Diagnostics.Debug.WriteLine("VALIDATION DEBUG: All 3 images validated successfully");

            return true; // ✅ Semua valid
        }

        /// <summary>
        /// Tombol Create - submit data kamar + 3 gambar ke API
        /// </summary>
        private async void btnCreate_Click(object sender, EventArgs e)
        {
            // Validasi input
            if (!ValidateInput())
            {
                return;  // Batalkan jika tidak valid
            }

            // Disable button saat proses
            btnCreate.Enabled = false;
            btnCreate.Text = "Creating...";

            MultipartFormDataContent formData = null;
            try
            {
                // Buat MultipartFormDataContent untuk upload file + data
                formData = new MultipartFormDataContent();

                // Extract status value (hanya bagian Indonesia, sebelum "/")
                string statusValue = "Tersedia"; // Default
                if (CbStatus.SelectedItem != null)
                {
                    string selectedStatus = CbStatus.SelectedItem.ToString();
                    // Split by "/" dan ambil bagian pertama (Indonesia)
                    string[] parts = selectedStatus.Split('/');
                    statusValue = parts[0].Trim();
                }

                // Tambahkan field text biasa
                formData.Add(new StringContent(txtRoomName.Text), "nomor_kamar");
                formData.Add(new StringContent(NuPrice.Value.ToString()), "harga_per_bulan");
                formData.Add(new StringContent(CbType.SelectedItem?.ToString() ?? "Standard"), "tipe_kamar");
                formData.Add(new StringContent(statusValue), "status"); // ✅ Send only Indonesian value
                formData.Add(new StringContent(NuCapacity.Value.ToString()), "capacity");
                formData.Add(new StringContent(NuFloor.Value.ToString()), "floor");
                formData.Add(new StringContent(txtSize.Text), "size");
                formData.Add(new StringContent(NuBedrooms.Value.ToString()), "bedrooms");
                formData.Add(new StringContent(NuBathrooms.Value.ToString()), "bathrooms");
                formData.Add(new StringContent(guna2TextBox1.Text), "fasilitas");
                formData.Add(new StringContent(guna2TextBox2.Text), "description");

                // ✅ UPLOAD 3 GAMBAR (API REQUIREMENT)
                int imageCount = 0;
                for (int i = 0; i < 3; i++)
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG: Checking image {i + 1}, Path: '{selectedImagePaths[i]}'");

                    if (!string.IsNullOrEmpty(selectedImagePaths[i]) && File.Exists(selectedImagePaths[i]))
                    {
                        try
                        {
                            // Baca seluruh file ke byte array terlebih dahulu
                            byte[] fileBytes = File.ReadAllBytes(selectedImagePaths[i]);
                            imageCount++;
                            System.Diagnostics.Debug.WriteLine($"DEBUG: Image {i + 1} loaded successfully. Size: {fileBytes.Length} bytes");

                            // Buat ByteArrayContent dari byte array (lebih reliable daripada File.OpenRead)
                            var fileContent = new ByteArrayContent(fileBytes);

                            // Tentukan content type berdasarkan extension
                            // ⚠️ PENTING: Backend memvalidasi bahwa Content-Type harus dimulai dengan "image/"
                            string extension = Path.GetExtension(selectedImagePaths[i]).ToLower();
                            switch (extension)
                            {
                                case ".jpg":
                                case ".jpeg":
                                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                                    break;
                                case ".png":
                                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                                    break;
                                case ".gif":
                                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/gif");
                                    break;
                                case ".bmp":
                                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/bmp");
                                    break;
                                case ".webp":
                                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/webp");
                                    break;
                                default:
                                    // Default ke image/jpeg jika extension tidak dikenal
                                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                                    break;
                            }

                            // ✅ PERBAIKAN UTAMA: Backend API mengharapkan parameter name "images" (bukan "image1", "image2", "image3")
                            // Semua 3 file ditambahkan dengan nama kunci yang SAMA: "images"
                            formData.Add(fileContent, "images", Path.GetFileName(selectedImagePaths[i]));
                            System.Diagnostics.Debug.WriteLine($"DEBUG: Image {i + 1} added to formData with parameter name 'images'");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"❌ Error reading image {i + 1}: {ex.Message}", "File Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"DEBUG: Image {i + 1} not found or path is empty");
                    }
                }
                System.Diagnostics.Debug.WriteLine($"DEBUG: Total images added: {imageCount}");

                // POST ke API dengan multipart form data
                string url = "https://rahmatzaw.elarisnoir.my.id/api/kamar";
                System.Diagnostics.Debug.WriteLine($"DEBUG: Sending POST request to {url}");
                System.Diagnostics.Debug.WriteLine($"DEBUG: FormData contains {formData.Count()} items");

                HttpResponseMessage response = await ApiClient.Client.PostAsync(url, formData);

                System.Diagnostics.Debug.WriteLine($"DEBUG: Response status: {response.StatusCode}");


                // Cek response status
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("✅ Kamar berhasil ditambahkan dengan 3 gambar!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show("❌ Session expired!\nSilakan login kembali.", "Auth Error",
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
            }
            // Handle exception spesifik
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
                // Dispose formData properly
                if (formData != null)
                {
                    formData.Dispose();
                }

                // Re-enable button
                btnCreate.Enabled = true;
                btnCreate.Text = "Create";
            }
        }

        /// <summary>
        /// Event handler dari Designer (btnImage Click)
        /// </summary>
        private void btnImage_Click_1(object sender, EventArgs e)
        {
            btnImage_Click(sender, e);
        }

        /// <summary>
        /// Event handler dari Designer (ImageTextBox3 TextChanged)
        /// </summary>
        private void ImageTextBox3_TextChanged(object sender, EventArgs e)
        {
            // Optional: Bisa tambahkan logic jika text berubah
        }
    }
}
