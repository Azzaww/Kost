using Kost_SiguraGura.Kost_SiguraGura;
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
            // Setup ComboBox Status
            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.Add("Semua Status");
            guna2ComboBox1.Items.Add("Tersedia");
            guna2ComboBox1.Items.Add("Penuh");
            guna2ComboBox1.Items.Add("Perbaikan");
            guna2ComboBox1.SelectedIndex = 0;

            // Tambahkan Setup ComboBox Tipe Kamar
            guna2ComboBox2.Items.Clear();
            guna2ComboBox2.Items.Add("Semua Tipe");
            guna2ComboBox2.Items.Add("Standard");
            guna2ComboBox2.Items.Add("Premium");
            guna2ComboBox2.SelectedIndex = 0;
        }

        // --- TAMBAHAN: Fungsi Filter Terpusat ---
        private void ApplyFilters()
        {
            string keyword = txtSearch.Text.ToLower().Trim();
            string selectedStatus = guna2ComboBox1.SelectedItem?.ToString() ?? "Semua Status";
            string selectedType = guna2ComboBox2.SelectedItem?.ToString() ?? "Semua Tipe";

            // Ambil data dari list utama (fullListKamar)
            var filtered = fullListKamar.AsEnumerable();

            // 1. Filter Status (guna2ComboBox1)
            if (selectedStatus != "Semua Status")
            {
                filtered = filtered.Where(k => k.STATUS != null &&
                           k.STATUS.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase));
            }

            // 2. Filter Tipe Kamar (guna2ComboBox2) - TAMBAHAN BARU
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
            LoadDataKamar();
            txtSearch.PlaceholderText = "Search Rooms...";
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
                                            k.THUMBNAIL = new Bitmap(Image.FromStream(ms));
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
            new AddKamar().Show();
        }
    }
}