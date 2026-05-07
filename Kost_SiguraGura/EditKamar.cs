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
    public partial class EditKamar : Form
    {
        // Data kamar yang akan diedit
        private Kamar originalKamar;

        // Array untuk menyimpan path gambar baru (opsional untuk edit)
        private string[] selectedImagePaths = new string[3];
        private int currentImageIndex = 0;

        public EditKamar(Kamar kamarToEdit)
        {
            InitializeComponent();
            originalKamar = kamarToEdit;

            // Initialize array kosong
            for (int i = 0; i < 3; i++)
            {
                selectedImagePaths[i] = string.Empty;
            }
            currentImageIndex = 0;

            // Setup ComboBox Status dengan bilingual options
            SetupStatusComboBox();

            // Wiring events manually if not done in designer
            this.btnCreate.Click += btnUpdate_Click;
            this.btnCancel.Click += (s, e) => this.Close();
            this.btnImage.Click += btnImage_Click;
            this.Load += EditKamar_Load;
        }

        /// <summary>
        /// Setup ComboBox Status dengan bilingual options (Indonesia/English)
        /// </summary>
        private void SetupStatusComboBox()
        {
            CbStatus.Items.Clear();
            CbStatus.Items.Add("Tersedia / Available");
            CbStatus.Items.Add("Penuh / Full");
            CbStatus.Items.Add("Terpesan / Booked");
            CbStatus.Items.Add("Perbaikan / Maintenance");
            CbStatus.SelectedIndex = 0; // Default ke Tersedia
        }

        /// <summary>
        /// Normalize status value untuk matching (Indonesia dan English)
        /// Handles both single language and bilingual formats
        /// </summary>
        private string NormalizeStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return "";

            // Remove extra whitespace dan convert to lowercase
            status = System.Text.RegularExpressions.Regex.Replace(status.Trim(), @"\s+", " ").ToLower();

            // If contains "/" (bilingual format), split and take first part
            if (status.Contains("/"))
            {
                var parts = status.Split('/');
                status = parts[0].Trim().ToLower();
            }

            // Map to normalized form
            if (status == "tersedia" || status == "available")
                return "tersedia_available";
            else if (status == "penuh" || status == "full")
                return "penuh_full";
            else if (status == "terpesan" || status == "booked")
                return "terpesan_booked";
            else if (status == "perbaikan" || status == "maintenance")
                return "perbaikan_maintenance";

            return status;
        }

        /// <summary>
        /// Get display text untuk status (bilingual format)
        /// </summary>
        private string GetStatusDisplayText(string apiStatus)
        {
            string normalized = NormalizeStatus(apiStatus);

            if (normalized == "tersedia_available")
                return "Tersedia / Available";
            else if (normalized == "penuh_full")
                return "Penuh / Full";
            else if (normalized == "terpesan_booked")
                return "Terpesan / Booked";
            else if (normalized == "perbaikan_maintenance")
                return "Perbaikan / Maintenance";

            return "Tersedia / Available"; // Default
        }

        private void EditKamar_Load(object sender, EventArgs e)
        {
            // Isi form dengan data kamar yang ada
            if (originalKamar != null)
            {
                System.Diagnostics.Debug.WriteLine("=== EditKamar_Load Debug ===");
                System.Diagnostics.Debug.WriteLine($"ROOM: {originalKamar.ROOM}");
                System.Diagnostics.Debug.WriteLine($"PRICE: {originalKamar.PRICE}");
                System.Diagnostics.Debug.WriteLine($"TYPE: {originalKamar.TYPE}");
                System.Diagnostics.Debug.WriteLine($"STATUS: {originalKamar.STATUS}");
                System.Diagnostics.Debug.WriteLine($"KAPASITAS: {originalKamar.KAPASITAS}");
                System.Diagnostics.Debug.WriteLine($"FLOOR: {originalKamar.FLOOR}");
                System.Diagnostics.Debug.WriteLine($"SIZE: {originalKamar.SIZE}");
                System.Diagnostics.Debug.WriteLine($"BEDROOMS: {originalKamar.BEDROOMS}");
                System.Diagnostics.Debug.WriteLine($"BATHROOMS: {originalKamar.BATHROOMS}");
                System.Diagnostics.Debug.WriteLine($"FACILITIES: {originalKamar.FACILITIES}");
                System.Diagnostics.Debug.WriteLine($"DESCRIPTION: {originalKamar.DESCRIPTION}");
                System.Diagnostics.Debug.WriteLine("==========================");

                txtRoomName.Text = originalKamar.ROOM ?? "";

                // ✅ SET MAXIMUM VALUE DULU sebelum assign value untuk NumericUpDown
                // Default maximum terlalu kecil, perlu diperbesar
                NuPrice.Maximum = 10000000; // 10 juta
                NuCapacity.Maximum = 1000;
                NuFloor.Maximum = 100;
                NuBedrooms.Maximum = 100;
                NuBathrooms.Maximum = 100;

                // Set numeric values dengan safe handling
                if (originalKamar.PRICE > 0)
                    NuPrice.Value = originalKamar.PRICE;

                // Set ComboBox untuk Type
                if (!string.IsNullOrEmpty(originalKamar.TYPE))
                {
                    try
                    {
                        CbType.SelectedItem = originalKamar.TYPE;
                    }
                    catch
                    {
                        CbType.SelectedIndex = 0; // Default ke Standard
                    }
                }

                // Set ComboBox untuk Status dengan bilingual matching
                if (!string.IsNullOrEmpty(originalKamar.STATUS))
                {
                    // Normalize status dari API dan temukan matching item
                    string normalizedApiStatus = NormalizeStatus(originalKamar.STATUS);
                    System.Diagnostics.Debug.WriteLine($"[STATUS DEBUG] API Status: '{originalKamar.STATUS}'");
                    System.Diagnostics.Debug.WriteLine($"[STATUS DEBUG] Normalized API Status: '{normalizedApiStatus}'");
                    System.Diagnostics.Debug.WriteLine($"[STATUS DEBUG] ComboBox Items Count: {CbStatus.Items.Count}");

                    bool found = false;

                    for (int i = 0; i < CbStatus.Items.Count; i++)
                    {
                        string comboItem = CbStatus.Items[i].ToString();
                        string normalizedItem = NormalizeStatus(comboItem);
                        System.Diagnostics.Debug.WriteLine($"[STATUS DEBUG] Index {i}: Item='{comboItem}' -> Normalized='{normalizedItem}'");

                        if (normalizedItem == normalizedApiStatus)
                        {
                            CbStatus.SelectedIndex = i;
                            found = true;
                            System.Diagnostics.Debug.WriteLine($"✅ [STATUS DEBUG] MATCH FOUND! Setting index to {i}");
                            break;
                        }
                    }

                    if (!found)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ [STATUS DEBUG] NO MATCH FOUND for '{originalKamar.STATUS}'. Using default index 0.");
                        CbStatus.SelectedIndex = 0; // Default ke Available
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ [STATUS DEBUG] STATUS is NULL or EMPTY!");
                    CbStatus.SelectedIndex = 0;
                }

                if (originalKamar.KAPASITAS > 0)
                    NuCapacity.Value = originalKamar.KAPASITAS;

                if (originalKamar.FLOOR > 0)
                    NuFloor.Value = originalKamar.FLOOR;

                txtSize.Text = originalKamar.SIZE ?? "";

                if (originalKamar.BEDROOMS > 0)
                    NuBedrooms.Value = originalKamar.BEDROOMS;

                if (originalKamar.BATHROOMS > 0)
                    NuBathrooms.Value = originalKamar.BATHROOMS;

                guna2TextBox1.Text = originalKamar.FACILITIES ?? "";
                guna2TextBox2.Text = originalKamar.DESCRIPTION ?? "";

                // Update button text
                btnCreate.Text = "Update";
                this.Text = $"Edit Kamar - {originalKamar.ROOM}";

                // Update preview untuk gambar (optional - tampilkan pesan bahwa gambar tidak ada)
                UpdateImagePreviewDisplay();
            }
        }

        /// <summary>
        /// Tombol untuk memilih gambar baru (opsional saat edit)
        /// </summary>
        private void btnImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files(*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp";
                ofd.Title = $"Pilih Gambar Kamar Baru (Gambar {currentImageIndex + 1}/3) - Optional";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        selectedImagePaths[currentImageIndex] = ofd.FileName;
                        UpdateImagePreviewDisplay();

                        if (currentImageIndex < 2)
                        {
                            currentImageIndex++;
                            MessageBox.Show($"✅ Gambar {currentImageIndex}/3 berhasil dipilih!\n\nSilakan pilih gambar berikutnya (Optional).",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("✅ Ketiga gambar sudah dipilih! Klik Update untuk melanjutkan.",
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
        /// Update display preview gambar
        /// </summary>
        private void UpdateImagePreviewDisplay()
        {
            StringBuilder preview = new StringBuilder();
            preview.AppendLine("📸 Gambar Baru (Optional):");

            int imageCount = 0;
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
                        imageCount++;
                    }
                    catch
                    {
                        preview.AppendLine($"Image {i + 1}: ⚠️ Error reading");
                    }
                }
            }

            preview.AppendLine();
            preview.AppendLine("💡 Catatan: Upload gambar baru hanya jika ingin mengubah. Jika kosong, gambar lama tetap digunakan.");

            ImageTextBox3.Text = preview.ToString().Trim();
        }

        /// <summary>
        /// Validasi input sebelum update
        /// </summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtRoomName.Text))
            {
                MessageBox.Show("❌ Room Name wajib diisi!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRoomName.Focus();
                return false;
            }

            if (NuPrice.Value <= 0)
            {
                MessageBox.Show("❌ Harga harus lebih dari 0!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NuPrice.Focus();
                return false;
            }

            if (CbType.SelectedIndex < 0)
            {
                MessageBox.Show("❌ Pilih Type Kamar!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CbType.Focus();
                return false;
            }

            if (CbStatus.SelectedIndex < 0)
            {
                MessageBox.Show("❌ Pilih Status Kamar!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CbStatus.Focus();
                return false;
            }

            if (NuCapacity.Value <= 0)
            {
                MessageBox.Show("❌ Kapasitas harus lebih dari 0!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NuCapacity.Focus();
                return false;
            }

            if (NuFloor.Value <= 0)
            {
                MessageBox.Show("❌ Lantai harus lebih dari 0!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NuFloor.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSize.Text))
            {
                MessageBox.Show("❌ Ukuran Kamar wajib diisi!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSize.Focus();
                return false;
            }

            if (NuBedrooms.Value <= 0)
            {
                MessageBox.Show("❌ Jumlah Kamar Tidur harus lebih dari 0!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NuBedrooms.Focus();
                return false;
            }

            if (NuBathrooms.Value <= 0)
            {
                MessageBox.Show("❌ Jumlah Kamar Mandi harus lebih dari 0!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NuBathrooms.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(guna2TextBox1.Text))
            {
                MessageBox.Show("❌ Fasilitas wajib diisi!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox1.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(guna2TextBox2.Text))
            {
                MessageBox.Show("❌ Deskripsi wajib diisi!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox2.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tombol Update - submit data kamar yang diupdate ke API
        /// </summary>
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            btnCreate.Enabled = false;
            btnCreate.Text = "Updating...";

            try
            {
                var formData = new MultipartFormDataContent();

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

                // ✅ UPLOAD gambar baru (opsional)
                int imageCount = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (!string.IsNullOrEmpty(selectedImagePaths[i]) && File.Exists(selectedImagePaths[i]))
                    {
                        try
                        {
                            byte[] fileBytes = File.ReadAllBytes(selectedImagePaths[i]);
                            var fileContent = new ByteArrayContent(fileBytes);

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
                                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                                    break;
                            }

                            // ✅ PENTING: Gunakan parameter name "images" (bukan "image1", "image2", "image3")
                            formData.Add(fileContent, "images", Path.GetFileName(selectedImagePaths[i]));
                            imageCount++;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"❌ Error reading image {i + 1}: {ex.Message}", "File Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            formData.Dispose();
                            return;
                        }
                    }
                }

                // PUT ke API untuk update
                string url = $"https://rahmatzaw.elarisnoir.my.id/api/kamar/{originalKamar.NO}";
                HttpResponseMessage response = await ApiClient.Client.PutAsync(url, formData);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("✅ Kamar berhasil diupdate!", "Success",
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
                btnCreate.Enabled = true;
                btnCreate.Text = "Update";
            }
        }
    }
}
