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
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Kost_SiguraGura
{
    public partial class DataKamar : UserControl
    {
        private BindingList<Kamar> bindingListKamar = new BindingList<Kamar>();
        private List<Kamar> fullListKamar = new List<Kamar>();

        public DataKamar()
        {
            InitializeComponent();
            SetupComboBox(); // Tambahkan inisialisasi item combobox
        }

        // --- TAMBAHAN: Setup isi ComboBox ---
        private void SetupComboBox()
        {
            // Setup ComboBox Status (Support Bilingual: Indonesian + English)
            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.Add("Semua Status");
            guna2ComboBox1.Items.Add("Tersedia / Available");
            guna2ComboBox1.Items.Add("Penuh / Full");
            guna2ComboBox1.Items.Add("Perbaikan / Maintenance");
            guna2ComboBox1.SelectedIndex = 0;

            // Tambahkan Setup ComboBox Tipe Kamar
            guna2ComboBox2.Items.Clear();
            guna2ComboBox2.Items.Add("Semua Tipe");
            guna2ComboBox2.Items.Add("Standard");
            guna2ComboBox2.Items.Add("Premium");
            guna2ComboBox2.SelectedIndex = 0;
        }

        /// <summary>
        /// Normalize status value untuk support bilingual (Indonesia + English)
        /// Handles both single language and bilingual formats
        /// Contoh: "Tersedia" atau "Available" atau "Tersedia / Available" → kesemuanya akan cocok
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

            // Map Indonesian status ke normalized form
            if (status == "tersedia" || status == "available")
                return "tersedia_available";
            else if (status == "penuh" || status == "full")
                return "penuh_full";
            else if (status == "perbaikan" || status == "maintenance")
                return "perbaikan_maintenance";

            return status;
        }

        // --- TAMBAHAN: Fungsi Filter Terpusat ---
        private void ApplyFilters()
        {
            string keyword = txtSearch.Text.ToLower().Trim();
            string selectedStatus = guna2ComboBox1.SelectedItem?.ToString() ?? "Semua Status";
            string selectedType = guna2ComboBox2.SelectedItem?.ToString() ?? "Semua Tipe";

            // Ambil data dari list utama (fullListKamar)
            var filtered = fullListKamar.AsEnumerable();

            // 1. Filter Status (guna2ComboBox1) - Support Bilingual
            if (selectedStatus != "Semua Status")
            {
                // Normalize filter value
                string normalizedFilter = NormalizeStatus(selectedStatus);

                filtered = filtered.Where(k => k.STATUS != null &&
                           NormalizeStatus(k.STATUS) == normalizedFilter);
            }

            // 2. Filter Tipe Kamar (guna2ComboBox2)
            if (selectedType != "Semua Tipe")
            {
                filtered = filtered.Where(k => k.TYPE != null &&
                           k.TYPE.Equals(selectedType, StringComparison.OrdinalIgnoreCase));
            }

            // 3. Filter Keyword (TextBox)
            if (!string.IsNullOrEmpty(keyword))
            {
                filtered = filtered.Where(k =>
                    (k.ROOM != null && k.ROOM.ToLower().Contains(keyword)) ||
                    (k.TYPE != null && k.TYPE.ToLower().Contains(keyword))
                );
            }

            // Update tampilan ke DataGridView
            bindingListKamar = new BindingList<Kamar>(filtered.ToList());
            dataGridView1.DataSource = bindingListKamar;
        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters(); // Tambahkan pemanggilan filter di sini
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters(); // Panggil filter saat teks pencarian berubah
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void DataKamar_Load(object sender, EventArgs e)
        {
            // ✅ Configure DataGridView untuk single cell selection (bukan full row)
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;

            LoadDataKamar();
            txtSearch.PlaceholderText = "Search Rooms...";

            // ✅ PENTING: Register event handlers untuk edit & delete
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellMouseMove += dataGridView1_CellMouseMove; // Show hand cursor on delete column
        }

        /// <summary>
        /// Show hand cursor saat hover ke delete column
        /// </summary>
        private void dataGridView1_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 1 || dataGridView1.Columns[e.ColumnIndex]?.Name == "THUMBNAIL")
            {
                dataGridView1.Cursor = Cursors.Hand; // Hand cursor untuk clickable
            }
            else
            {
                dataGridView1.Cursor = Cursors.Default;
            }
        }

        private async void LoadDataKamar()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "https://rahmatzaw.elarisnoir.my.id/api/kamar";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var listData = JsonConvert.DeserializeObject<List<Kamar>>(jsonResponse);

                        if (listData != null)
                        {
                            fullListKamar.Clear(); // Bersihkan list cadangan

                            foreach (var k in listData)
                            {
                                if (!string.IsNullOrEmpty(k.ThumbnailUrl))
                                {
                                    try
                                    {
                                        byte[] imageBytes = await client.GetByteArrayAsync(k.ThumbnailUrl);
                                        using (var ms = new System.IO.MemoryStream(imageBytes))
                                        {
                                            k.THUMBNAIL = new Bitmap(System.Drawing.Image.FromStream(ms));
                                        }
                                    }
                                    catch { k.THUMBNAIL = null; }
                                }
                                fullListKamar.Add(k); // Simpan ke list cadangan
                            }

                            // Tampilkan data menggunakan fungsi filter agar sinkron dengan status awal combobox
                            ApplyFilters();

                            if (dataGridView1.Columns["ThumbnailUrl"] != null)
                                dataGridView1.Columns["ThumbnailUrl"].Visible = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan: " + ex.Message);
                }
            }
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters(); // Panggil filter saat pilihan status berubah
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            AddKamar form = new AddKamar();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadDataKamar(); // Refresh data setelah tambah
            }
        }

        // ===== TAMBAHAN: EVENT GRID ROW DOUBLE CLICK =====
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // Skip header row

            // Ambil data kamar dari row yang diklik
            var selectedKamar = bindingListKamar[e.RowIndex];

            // Buka form Edit
            EditKamar editForm = new EditKamar(selectedKamar);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadDataKamar(); // Refresh data setelah edit
            }
        }

        // ===== TAMBAHAN: DELETE BUTTON DI GRID =====
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Skip if clicking header row or invalid row
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            try
            {
                // Get column name yang diklik
                string columnName = dataGridView1.Columns[e.ColumnIndex].Name;

                System.Diagnostics.Debug.WriteLine($"CellClick - RowIndex: {e.RowIndex}, ColumnIndex: {e.ColumnIndex}, ColumnName: {columnName}");

                // Check jika kolom THUMBNAIL (atau bisa juga cek jika value berisi X icon)
                if (columnName == "THUMBNAIL" || e.ColumnIndex == 1)
                {
                    var selectedKamar = bindingListKamar[e.RowIndex];
                    System.Diagnostics.Debug.WriteLine($"Delete clicked for kamar: {selectedKamar.ROOM}");
                    DeleteKamar(selectedKamar);
                }
                // Alternative: cek jika cell value adalah X atau ❌
                else if (dataGridView1[e.ColumnIndex, e.RowIndex]?.Value?.ToString()?.Contains("X") == true ||
                         dataGridView1[e.ColumnIndex, e.RowIndex]?.Value?.ToString()?.Contains("❌") == true)
                {
                    var selectedKamar = bindingListKamar[e.RowIndex];
                    System.Diagnostics.Debug.WriteLine($"Delete clicked (by content) for kamar: {selectedKamar.ROOM}");
                    DeleteKamar(selectedKamar);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CellClick: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // ===== FUNGSI DELETE KAMAR =====
        private async void DeleteKamar(Kamar kamar)
        {
            DialogResult result = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus kamar '{kamar.ROOM}'?",
                "Konfirmasi Hapus",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                string url = $"https://rahmatzaw.elarisnoir.my.id/api/kamar/{kamar.NO}";
                HttpResponseMessage response = await ApiClient.Client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"✅ Kamar '{kamar.ROOM}' berhasil dihapus!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataKamar(); // Refresh data
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("❌ Session expired! Silakan login kembali.", "Auth Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show("❌ Kamar tidak ditemukan di server!", "Not Found Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"❌ Gagal menghapus kamar!\n\n{errorContent}", "Error",
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
        /// Export data kamar ke file PDF
        /// </summary>
        private void exportButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // Get data yang akan di-export (data yang currently di-display)
                var dataToExport = bindingListKamar.ToList();

                if (dataToExport.Count == 0)
                {
                    MessageBox.Show("❌ Tidak ada data untuk di-export!", "Export Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Open SaveFileDialog untuk pilih lokasi simpan
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
                    sfd.FileName = $"Kamar_Export_{DateTime.Now:yyyy-MM-dd_HHmmss}.pdf";
                    sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        ExportToPDF(sfd.FileName, dataToExport);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error during export: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Generate PDF file dari data kamar
        /// </summary>
        private void ExportToPDF(string filePath, List<Kamar> data)
        {
            try
            {
                // Create PDF document
                iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate());
                PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                doc.Open();

                // Add title
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("📋 LAPORAN DATA KAMAR (ROOM REPORT)", titleFont);
                title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                doc.Add(title);

                // Add export date
                iTextSharp.text.Font dateFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10);
                iTextSharp.text.Paragraph dateInfo = new iTextSharp.text.Paragraph($"Tanggal Export: {DateTime.Now:dd MMMM yyyy HH:mm:ss}", dateFont);
                dateInfo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                doc.Add(dateInfo);

                // Add spacing
                doc.Add(new iTextSharp.text.Paragraph(" "));

                // Create table
                PdfPTable table = new PdfPTable(11); // 11 columns
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1, 1.5f, 1.5f, 1.2f, 1.2f, 1, 1, 1, 1, 1.5f, 1.5f });

                // Header cells style
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.WHITE);
                iTextSharp.text.BaseColor headerBackColor = new iTextSharp.text.BaseColor(70, 130, 180); // Steel Blue

                string[] headers = { "NO", "ROOM", "TYPE", "PRICE", "FLOOR", "CAPACITY", "SIZE", "BEDS", "BATHS", "FACILITIES", "STATUS" };

                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                    cell.BackgroundColor = headerBackColor;
                    cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);
                }

                // Data rows
                iTextSharp.text.Font cellFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9);

                foreach (var kamar in data)
                {
                    // NO
                    PdfPCell noCell = new PdfPCell(new iTextSharp.text.Phrase(kamar.NO.ToString(), cellFont));
                    noCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    table.AddCell(noCell);

                    // ROOM
                    table.AddCell(new PdfPCell(new iTextSharp.text.Phrase(kamar.ROOM ?? "-", cellFont)));

                    // TYPE
                    table.AddCell(new PdfPCell(new iTextSharp.text.Phrase(kamar.TYPE ?? "-", cellFont)));

                    // PRICE
                    PdfPCell priceCell = new PdfPCell(new iTextSharp.text.Phrase("Rp " + kamar.PRICE.ToString("N0"), cellFont));
                    priceCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                    table.AddCell(priceCell);

                    // FLOOR
                    PdfPCell floorCell = new PdfPCell(new iTextSharp.text.Phrase(kamar.FLOOR.ToString(), cellFont));
                    floorCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    table.AddCell(floorCell);

                    // CAPACITY
                    PdfPCell capacityCell = new PdfPCell(new iTextSharp.text.Phrase(kamar.KAPASITAS.ToString(), cellFont));
                    capacityCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    table.AddCell(capacityCell);

                    // SIZE
                    table.AddCell(new PdfPCell(new iTextSharp.text.Phrase(kamar.SIZE ?? "-", cellFont)));

                    // BEDROOMS
                    PdfPCell bedsCell = new PdfPCell(new iTextSharp.text.Phrase(kamar.BEDROOMS.ToString(), cellFont));
                    bedsCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    table.AddCell(bedsCell);

                    // BATHROOMS
                    PdfPCell bathsCell = new PdfPCell(new iTextSharp.text.Phrase(kamar.BATHROOMS.ToString(), cellFont));
                    bathsCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    table.AddCell(bathsCell);

                    // FACILITIES
                    table.AddCell(new PdfPCell(new iTextSharp.text.Phrase(kamar.FACILITIES ?? "-", cellFont)));

                    // STATUS
                    string status = kamar.STATUS ?? "-";
                    PdfPCell statusCell = new PdfPCell(new iTextSharp.text.Phrase(status, cellFont));
                    statusCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    table.AddCell(statusCell);
                }

                // Add table ke document
                doc.Add(table);

                // Add footer
                doc.Add(new iTextSharp.text.Paragraph(" "));
                iTextSharp.text.Font footerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.ITALIC);
                iTextSharp.text.Paragraph footer = new iTextSharp.text.Paragraph($"Total Kamar: {data.Count}", footerFont);
                footer.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                doc.Add(footer);

                // Close document
                doc.Close();

                // Show success message
                MessageBox.Show($"✅ Export berhasil!\n\nFile: {filePath}\nTotal Data: {data.Count} kamar", "Export Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Optional: Open file jika user ingin
                DialogResult openFile = MessageBox.Show("Buka file PDF sekarang?", "Open File",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (openFile == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error creating PDF: {ex.Message}", "PDF Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
