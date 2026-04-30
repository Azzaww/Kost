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
    public partial class DataPenyewa : System.Windows.Forms.UserControl
    {
        private BindingList<Penyewa> bindingListPenyewa = new BindingList<Penyewa>();
        private List<Penyewa> fullListPenyewa = new List<Penyewa>();

        private int currentPage = 1;
        private int currentLimit = 20;
        private int totalPages = 1;
        private string currentSearchKeyword = "";
        private string currentRoleFilter = "";

        public DataPenyewa()
        {
            InitializeComponent();
            SetupStatusComboBox();
        }

        private void DataPenyewa_Load(object sender, EventArgs e)
        {
            if (Session.UserRole?.ToLower() == "admin")
            {
                // Wire up event handler untuk dropdown status
                StatusComboBox1.SelectedIndexChanged += StatusComboBox1_SelectedIndexChanged;

                // ✅ FIX Issue #1: Register CellDoubleClick event handler untuk open detail form
                dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;

                // Set default selection
                StatusComboBox1.SelectedIndex = 0; // "All Status"

                LoadDataPenyewa();
            }
            else
            {
                MessageBox.Show("Akses Ditolak! Anda bukan Admin. Role Anda: " + Session.UserRole);
            }
        }

        /// <summary>
        /// Setup ComboBox Status dengan mapping yang sesuai
        /// </summary>
        private void SetupStatusComboBox()
        {
            // Items sudah ditambahkan di Designer
            // StatusComboBox1 memiliki items: "All Status", "Active", "Non Active", "Guest Users"
        }

        private async void LoadDataPenyewa()
        {
            try
            {
                currentPage = 1;
                currentSearchKeyword = "";
                currentRoleFilter = "";

                var response = await ApiClient.GetTenants(currentPage, currentLimit, currentSearchKeyword, currentRoleFilter);

                if (response?.Data != null)
                {
                    fullListPenyewa = response.Data;
                    totalPages = response.Meta?.TotalPages ?? 1;

                    this.Invoke((MethodInvoker)delegate {
                        bindingListPenyewa = new BindingList<Penyewa>(fullListPenyewa);
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = bindingListPenyewa;
                        dataGridView1.Refresh();
                    });
                }
                else
                {
                    MessageBox.Show("Data tenant tidak ditemukan atau response format tidak sesuai.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat mengambil data tenant: " + ex.Message);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            currentSearchKeyword = txtSearch.Text.Trim();
            currentPage = 1;
            LoadDataWithSearch();
        }

        private async void LoadDataWithSearch()
        {
            try
            {
                var response = await ApiClient.GetTenants(currentPage, currentLimit, currentSearchKeyword, currentRoleFilter);

                if (response?.Data != null)
                {
                    fullListPenyewa = response.Data;
                    totalPages = response.Meta?.TotalPages ?? 1;

                    this.Invoke((MethodInvoker)delegate {
                        bindingListPenyewa = new BindingList<Penyewa>(fullListPenyewa);
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = bindingListPenyewa;
                        dataGridView1.Refresh();
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat search: " + ex.Message);
            }
        }

        /// <summary>
        /// Event handler untuk dropdown status - filter data berdasarkan status yang dipilih
        /// </summary>
        private void StatusComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string selectedStatus = StatusComboBox1.SelectedItem?.ToString() ?? "All Status";

                // Map UI dropdown values ke API role filter yang benar
                string roleFilter = "";

                if (selectedStatus == "All Status")
                {
                    roleFilter = ""; // Tampilkan semua tenant
                }
                else if (selectedStatus == "Active")
                {
                    roleFilter = "tenant"; // Active Tenants = role "tenant"
                }
                else if (selectedStatus == "Non Active")
                {
                    roleFilter = "non_active"; // Suspended tenants
                }
                else if (selectedStatus == "Guest Users")
                {
                    roleFilter = "guest"; // Guest users belum booking
                }

                currentRoleFilter = roleFilter;
                currentPage = 1;

                // Trigger data load dengan filter baru
                LoadDataWithSearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat filter status: " + ex.Message);
            }
        }

        // Event handler untuk row double-click - membuka TenantDetailForm
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < bindingListPenyewa.Count)
            {
                Penyewa selectedTenant = bindingListPenyewa[e.RowIndex];
                TenantDetailForm detailForm = new TenantDetailForm(selectedTenant, this);
                detailForm.ShowDialog();
            }
        }

        // Method publik untuk refresh data ketika dipanggil dari TenantDetailForm
        public void RefreshTenantData()
        {
            LoadDataPenyewa();
        }

        // Event handler untuk suspend button (jika ada di datagrid)
        public void SetupDataGridColumns()
        {
            // Bisa ditambahkan button column untuk suspend jika diperlukan
            // Tapi karena anda bilang jangan ubah UI, kita skip ini untuk saat ini
        }
    }
}
