# 🔧 DETAILED FIXES & IMPLEMENTATION GUIDE
## Critical Issues Resolution

---

## CRITICAL ISSUE #1: Duplicate PaymentResponse.cs

### Problem
```
File listing menunjukkan:
  Kost_SiguraGura\PaymentResponse.cs (line 1)
  Kost_SiguraGura\PaymentResponse.cs (line 2)
```

### Solution
**Action**: Delete satu file yang duplikat, simpan hanya satu
```bash
# Di project, hapus file yang duplikat
# Verify hanya satu PaymentResponse.cs yang ada
```

---

## CRITICAL ISSUE #2: Missing TenantPaymentHistory Model

### Problem
```csharp
// ApiClient.cs line 235
public static async Task<List<TenantPaymentHistory>> GetTenantPaymentHistory(int tenantId)
{
	// TenantPaymentHistory tidak didefinisikan!
}
```

### Solution
**File to Create**: `TenantPaymentHistory.cs`

```csharp
using Newtonsoft.Json;
using System;

namespace Kost_SiguraGura
{
	public class TenantPaymentHistory
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("pemesanan_id")]
		public int PemesananId { get; set; }

		[JsonProperty("jumlah_bayar")]
		public decimal JumlahBayar { get; set; }

		[JsonProperty("tanggal_bayar")]
		public DateTime? TanggalBayar { get; set; }

		[JsonProperty("bukti_transfer")]
		public string BuktiTransfer { get; set; }

		[JsonProperty("status_pembayaran")]
		public string StatusPembayaran { get; set; } // Pending, Confirmed, Rejected

		[JsonProperty("metode_pembayaran")]
		public string MetodePembayaran { get; set; }

		[JsonProperty("tipe_pembayaran")]
		public string TipePembayaran { get; set; }

		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public DateTime UpdatedAt { get; set; }
	}
}
```

---

## CRITICAL ISSUE #3: PenyewaDetail Form Kosong

### Problem
```csharp
// PenyewaDetail.cs - hanya skeleton
public partial class PenyewaDetail : Form
{
	public PenyewaDetail()
	{
		InitializeComponent();
	}
	// TIDAK ADA LOGIC
}
```

### Solution
**Implement PenyewaDetail.cs** dengan:

1. Constructor yang accept Penyewa object dan parent form
2. Load tenant data saat form open
3. Display payment history dalam DataGridView
4. Button untuk deactivate tenant

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
	public partial class PenyewaDetail : Form
	{
		private Penyewa _tenantData;
		private PembayaranForm _parentForm;
		private List<TenantPaymentHistory> _paymentHistory;

		public PenyewaDetail(Penyewa tenant, PembayaranForm parent)
		{
			InitializeComponent();
			_tenantData = tenant;
			_parentForm = parent;
			this.Load += PenyewaDetail_Load;
		}

		private async void PenyewaDetail_Load(object sender, EventArgs e)
		{
			try
			{
				// Load tenant data
				DisplayTenantInfo();

				// Load payment history
				await LoadPaymentHistory();

				// Setup DataGridView
				SetupPaymentGrid();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading detail: {ex.Message}");
			}
		}

		private void DisplayTenantInfo()
		{
			// Display left column: Tenant Info
			lblNamaLengkap.Text = _tenantData.NAMA_LENGKAP ?? "N/A";
			lblEmail.Text = _tenantData.KONTAK ?? "N/A";
			lblNomorHP.Text = _tenantData.NOMOR_HP ?? "N/A";
			lblAlamatAsal.Text = _tenantData.ALAMAT_ASAL ?? "N/A";
			lblJenisKelamin.Text = _tenantData.JENIS_KELAMIN ?? "N/A";
			lblRole.Text = _tenantData.PERAN ?? "N/A";
			lblCreatedAt.Text = _tenantData.CREATED_AT?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

			// Profile photo if available
			if (!string.IsNullOrEmpty(_tenantData.FOTO_PROFIL))
			{
				try
				{
					using (var client = new System.Net.Http.HttpClient())
					{
						var imageBytes = client.GetByteArrayAsync(_tenantData.FOTO_PROFIL).Result;
						using (var ms = new System.IO.MemoryStream(imageBytes))
						{
							pictureBoxProfile.Image = System.Drawing.Image.FromStream(ms);
						}
					}
				}
				catch { /* silently fail */ }
			}
		}

		private async Task LoadPaymentHistory()
		{
			try
			{
				_paymentHistory = await ApiClient.GetTenantPaymentHistory(_tenantData.ID);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading payment history: {ex.Message}");
				_paymentHistory = new List<TenantPaymentHistory>();
			}
		}

		private void SetupPaymentGrid()
		{
			dataGridViewPayments.DataSource = null;
			dataGridViewPayments.DataSource = _paymentHistory;

			// Configure columns
			dataGridViewPayments.Columns["Id"].Visible = false;
			dataGridViewPayments.Columns["PemesananId"].Visible = false;

			dataGridViewPayments.Columns["JumlahBayar"].HeaderText = "Jumlah Bayar";
			dataGridViewPayments.Columns["JumlahBayar"].DefaultCellStyle.Format = "C0";

			dataGridViewPayments.Columns["TanggalBayar"].HeaderText = "Tanggal Bayar";
			dataGridViewPayments.Columns["TanggalBayar"].DefaultCellStyle.Format = "dd/MM/yyyy";

			dataGridViewPayments.Columns["StatusPembayaran"].HeaderText = "Status";
			dataGridViewPayments.Columns["MetodePembayaran"].HeaderText = "Metode";
			dataGridViewPayments.Columns["TipePembayaran"].HeaderText = "Tipe";
		}

		private async void BtnDeactivate_Click(object sender, EventArgs e)
		{
			try
			{
				DialogResult result = MessageBox.Show(
					$"Apakah Anda yakin ingin deactivate tenant: {_tenantData.NAMA_LENGKAP}?",
					"Konfirmasi",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question
				);

				if (result == DialogResult.Yes)
				{
					bool success = await ApiClient.SuspendTenant(_tenantData.ID);
					if (success)
					{
						MessageBox.Show("Tenant deactivated successfully!");
						_parentForm?.RefreshData();
						this.Close();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}");
			}
		}
	}
}
```

---

## CRITICAL ISSUE #4: Missing Gallery Model

### Problem
```csharp
// GalleryForm.cs line 50
allGalleries = JsonConvert.DeserializeObject<List<Gallery>>(jsonResponse);
// Gallery class tidak ada atau properties tidak lengkap
```

### Solution
**File to Create**: `Gallery.cs`

```csharp
using Newtonsoft.Json;
using System;

namespace Kost_SiguraGura
{
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
		public DateTime? created_at { get; set; }

		[JsonProperty("updated_at")]
		public DateTime? updated_at { get; set; }

		[JsonProperty("deleted_at")]
		public DateTime? deleted_at { get; set; }
	}
}
```

---

## CRITICAL ISSUE #5: Dashboard Stats API Not Implemented

### Problem
```csharp
// BerandaPage.cs - Load data terpisah-pisah (inefficient)
await Task.WhenAll(
	LoadPaymentsAsync(),
	LoadRoomsAsync()
);
// Seharusnya 1 API call ke /api/dashboard/stats
```

### Solution
**Add to ApiClient.cs**:

```csharp
/// <summary>
/// Get comprehensive dashboard statistics from a single API call
/// </summary>
public static async Task<DashboardStats> GetDashboardStats()
{
	try
	{
		string url = $"{BaseUrl}/dashboard/stats";
		HttpResponseMessage response = await Client.GetAsync(url);

		if (response.IsSuccessStatusCode)
		{
			string jsonResponse = await response.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<DashboardStats>(jsonResponse);
			return result ?? new DashboardStats();
		}
		else if (response.StatusCode == HttpStatusCode.Unauthorized)
		{
			throw new Exception("Sesi login habis. Silakan login ulang.");
		}
		else
		{
			throw new Exception($"Gagal mengambil dashboard stats. Status: {response.StatusCode}");
		}
	}
	catch (Exception ex)
	{
		throw new Exception($"Error saat mengambil dashboard stats: {ex.Message}");
	}
}
```

**Create DashboardStats.cs**:

```csharp
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kost_SiguraGura
{
	public class DashboardStats
	{
		[JsonProperty("total_revenue")]
		public decimal TotalRevenue { get; set; }

		[JsonProperty("pending_revenue")]
		public decimal PendingRevenue { get; set; }

		[JsonProperty("available_rooms")]
		public int AvailableRooms { get; set; }

		[JsonProperty("occupied_rooms")]
		public int OccupiedRooms { get; set; }

		[JsonProperty("pending_payments")]
		public int PendingPayments { get; set; }

		[JsonProperty("occupancy_rate")]
		public decimal OccupancyRate { get; set; }

		[JsonProperty("active_tenants")]
		public int ActiveTenants { get; set; }
	}
}
```

**Update BerandaPage.cs**:

```csharp
private async Task LoadAllDashboardData()
{
	try
	{
		// ✅ Efficient: Single API call
		var stats = await ApiClient.GetDashboardStats();

		// Update UI
		UpdateKPICardsFromStats(stats);
	}
	catch (Exception ex)
	{
		MessageBox.Show($"Error loading dashboard: {ex.Message}");
	}
}

private void UpdateKPICardsFromStats(DashboardStats stats)
{
	this.Invoke((MethodInvoker)delegate {
		lblIncome.Text = FormatKeRupiahSingkat((long)stats.TotalRevenue);
		lblActiveTenants.Text = stats.ActiveTenants.ToString();
		lblAvailableRooms.Text = stats.AvailableRooms.ToString();
		lblPendingPayments.Text = stats.PendingPayments.ToString();
		lblOccupancyRate.Text = $"{stats.OccupancyRate:F1}%";
	});
}
```

---

## CRITICAL ISSUE #6: Room Status Bilingual Logic Bug

### Problem
```csharp
// DataKamar.cs - Setup correct, but normalization has edge cases
private string NormalizeStatus(string status)
{
	// Bug: if API returns only "Available", tidak match dengan 
	// kombobox "Tersedia / Available"
}
```

### Solution
**Fix NormalizeStatus in DataKamar.cs, EditKamar.cs, AddKamar.cs**:

```csharp
/// <summary>
/// Normalize status value untuk support bilingual (Indonesia + English)
/// Maps "Tersedia"/"Available"/"Tersedia / Available" ke standard form
/// </summary>
private string NormalizeStatusForComparison(string status)
{
	if (string.IsNullOrEmpty(status))
		return "";

	// Remove extra whitespace
	status = System.Text.RegularExpressions.Regex.Replace(status.Trim(), @"\s+", " ").ToLower();

	// Handle bilingual format "Tersedia / Available" → take first part
	if (status.Contains("/"))
	{
		status = status.Split('/')[0].Trim().ToLower();
	}

	// Normalize all variations to internal representation
	if (status == "tersedia" || status == "available")
		return "tersedia";
	else if (status == "penuh" || status == "full")
		return "penuh";
	else if (status == "perbaikan" || status == "maintenance" || status == "maintenance" || status == "perbaikan")
		return "perbaikan";

	return status;
}

/// <summary>
/// Convert internal status ke bilingual display format
/// </summary>
private string ConvertStatusToDisplay(string internalStatus)
{
	string normalized = NormalizeStatusForComparison(internalStatus);

	switch (normalized)
	{
		case "tersedia":
			return "Tersedia / Available";
		case "penuh":
			return "Penuh / Full";
		case "perbaikan":
			return "Perbaikan / Maintenance";
		default:
			return internalStatus;
	}
}

// Gunakan dalam filter:
private void ApplyFilters()
{
	string keyword = txtSearch.Text.ToLower().Trim();
	string selectedStatus = guna2ComboBox1.SelectedItem?.ToString() ?? "Semua Status";
	string selectedType = guna2ComboBox2.SelectedItem?.ToString() ?? "Semua Tipe";

	var filtered = fullListKamar.AsEnumerable();

	// 1. Filter Status (support bilingual)
	if (selectedStatus != "Semua Status")
	{
		string normalizedFilter = NormalizeStatusForComparison(selectedStatus);
		filtered = filtered.Where(k => k.STATUS != null &&
				   NormalizeStatusForComparison(k.STATUS) == normalizedFilter);
	}

	// 2. Filter Tipe Kamar
	if (selectedType != "Semua Tipe")
	{
		filtered = filtered.Where(k => k.TYPE != null &&
				   k.TYPE.Equals(selectedType, StringComparison.OrdinalIgnoreCase));
	}

	// 3. Filter Keyword
	if (!string.IsNullOrEmpty(keyword))
	{
		filtered = filtered.Where(k =>
			(k.ROOM != null && k.ROOM.ToLower().Contains(keyword)) ||
			(k.TYPE != null && k.TYPE.ToLower().Contains(keyword))
		);
	}

	bindingListKamar = new BindingList<Kamar>(filtered.ToList());
	dataGridView1.DataSource = bindingListKamar;
}
```

---

## CRITICAL ISSUE #7: AddKamar Multiple Image Upload Missing

### Problem
```csharp
// AddKamar.cs - Ada UI untuk select 3 gambar, tapi upload logic tidak ada
private string[] selectedImagePaths = new string[3];

// ❌ MISSING: btnCreate_Click tidak upload gambar
```

### Solution
**Implement upload logic in AddKamar.cs btnCreate_Click**:

```csharp
private async void btnCreate_Click(object sender, EventArgs e)
{
	try
	{
		// 1. Validate input
		if (!ValidateInput())
			return;

		// 2. Validate images
		if (!ValidateImages())
			return;

		// 3. Prepare multipart form data
		using (var client = new HttpClient())
		{
			client.Timeout = TimeSpan.FromSeconds(30);

			var form = new MultipartFormDataContent();

			// Add room data
			form.Add(new StringContent(txtRoomName.Text), "nomor_kamar");
			form.Add(new StringContent(cbType.SelectedItem.ToString()), "tipe_kamar");
			form.Add(new StringContent(NuPrice.Value.ToString("F2")), "harga_per_bulan");
			form.Add(new StringContent(NormalizeStatusForAPI(CbStatus.SelectedItem.ToString())), "status");
			form.Add(new StringContent(NuFloor.Value.ToString()), "floor");
			form.Add(new StringContent(NuCapacity.Value.ToString()), "capacity");
			form.Add(new StringContent(txtSize.Text), "size");
			form.Add(new StringContent(NuBedrooms.Value.ToString()), "bedrooms");
			form.Add(new StringContent(NuBathrooms.Value.ToString()), "bathrooms");
			form.Add(new StringContent(rtbFacilities.Text), "fasilitas");
			form.Add(new StringContent(rtbDescription.Text), "description");

			// Add images (minimum 3)
			for (int i = 0; i < 3; i++)
			{
				string imagePath = selectedImagePaths[i];
				if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
				{
					var fileContent = new StreamContent(File.OpenRead(imagePath));
					fileContent.Headers.ContentType = 
						new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

					form.Add(fileContent, "images", Path.GetFileName(imagePath));
				}
			}

			// 4. POST to API
			string url = "https://rahmatzaw.elarisnoir.my.id/api/kamar";
			var response = await client.PostAsync(url, form);

			if (response.IsSuccessStatusCode)
			{
				MessageBox.Show("✅ Kamar berhasil dibuat!", "Success", 
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				this.Close();
			}
			else
			{
				string error = await response.Content.ReadAsStringAsync();
				throw new Exception($"API Error ({response.StatusCode}): {error}");
			}
		}
	}
	catch (Exception ex)
	{
		MessageBox.Show($"❌ Error: {ex.Message}", "Error", 
			MessageBoxButtons.OK, MessageBoxIcon.Error);
	}
}

/// <summary>
/// Convert bilingual status to API format (just first part before "/")
/// </summary>
private string NormalizeStatusForAPI(string displayStatus)
{
	if (string.IsNullOrEmpty(displayStatus))
		return "";

	// Extract first part before "/"
	if (displayStatus.Contains("/"))
		return displayStatus.Split('/')[0].Trim().ToLower();

	return displayStatus.ToLower();
}

private bool ValidateImages()
{
	for (int i = 0; i < 3; i++)
	{
		if (string.IsNullOrEmpty(selectedImagePaths[i]))
		{
			MessageBox.Show($"Image {i + 1} must be selected", "Validation Error", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		if (!File.Exists(selectedImagePaths[i]))
		{
			MessageBox.Show($"Image {i + 1} file not found: {selectedImagePaths[i]}", 
				"File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}
	}
	return true;
}
```

---

## CRITICAL ISSUE #8: Payment Confirmation Response Not Handled

### Problem
```csharp
// ApiClient.cs - ConfirmPayment tidak capture response data
public static async Task<bool> ConfirmPayment(int paymentId)
{
	// Return hanya true/false, tidak parse Pembayaran object
}
```

### Solution
**Update ApiClient.cs**:

```csharp
/// <summary>
/// Confirm a payment and return updated payment data
/// </summary>
public static async Task<Pembayaran> ConfirmPayment(int paymentId)
{
	try
	{
		string url = $"{BaseUrl}/payments/{paymentId}/confirm";
		HttpResponseMessage response = await Client.PutAsync(url, null);

		if (response.IsSuccessStatusCode)
		{
			string jsonResponse = await response.Content.ReadAsStringAsync();

			// Parse response - bisa wrapper atau direct object
			var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

			// Extract payment data
			Pembayaran updatedPayment = null;

			if (result["data"] != null)
			{
				updatedPayment = JsonConvert.DeserializeObject<Pembayaran>(
					result["data"].ToString()
				);
			}
			else
			{
				updatedPayment = JsonConvert.DeserializeObject<Pembayaran>(jsonResponse);
			}

			return updatedPayment;
		}
		else if (response.StatusCode == HttpStatusCode.Unauthorized)
		{
			throw new Exception("Sesi login habis. Silakan login ulang.");
		}
		else
		{
			string error = await response.Content.ReadAsStringAsync();
			throw new Exception($"Gagal konfirmasi pembayaran ({response.StatusCode}): {error}");
		}
	}
	catch (Exception ex)
	{
		throw new Exception($"Error saat konfirmasi pembayaran: {ex.Message}");
	}
}

/// <summary>
/// Reject a payment and return updated payment data
/// </summary>
public static async Task<Pembayaran> RejectPayment(int paymentId)
{
	try
	{
		string url = $"{BaseUrl}/payments/{paymentId}/reject";
		HttpResponseMessage response = await Client.PutAsync(url, null);

		if (response.IsSuccessStatusCode)
		{
			string jsonResponse = await response.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

			Pembayaran updatedPayment = null;
			if (result["data"] != null)
			{
				updatedPayment = JsonConvert.DeserializeObject<Pembayaran>(
					result["data"].ToString()
				);
			}
			else
			{
				updatedPayment = JsonConvert.DeserializeObject<Pembayaran>(jsonResponse);
			}

			return updatedPayment;
		}
		else if (response.StatusCode == HttpStatusCode.Unauthorized)
		{
			throw new Exception("Sesi login habis. Silakan login ulang.");
		}
		else
		{
			string error = await response.Content.ReadAsStringAsync();
			throw new Exception($"Gagal reject pembayaran ({response.StatusCode}): {error}");
		}
	}
	catch (Exception ex)
	{
		throw new Exception($"Error saat reject pembayaran: {ex.Message}");
	}
}
```

**Update PaymentCardControl.cs**:

```csharp
private async void BtnConfirm_Click(object sender, EventArgs e)
{
	try
	{
		Pembayaran updatedPayment = await ApiClient.ConfirmPayment(PaymentData.Id);
		if (updatedPayment != null)
		{
			// Update local data
			PaymentData = updatedPayment;
			LoadPaymentData();

			MessageBox.Show("✅ Pembayaran berhasil dikonfirmasi!", "Success", 
				MessageBoxButtons.OK, MessageBoxIcon.Information);

			OnConfirmClicked?.Invoke(this, EventArgs.Empty);
			ParentForm?.RefreshData();
		}
	}
	catch (Exception ex)
	{
		MessageBox.Show($"❌ Error: {ex.Message}", "Error", 
			MessageBoxButtons.OK, MessageBoxIcon.Error);
	}
}

private async void BtnReject_Click(object sender, EventArgs e)
{
	try
	{
		DialogResult result = MessageBox.Show(
			"Apakah Anda yakin ingin menolak pembayaran ini?",
			"Konfirmasi",
			MessageBoxButtons.YesNo,
			MessageBoxIcon.Question
		);

		if (result == DialogResult.Yes)
		{
			Pembayaran updatedPayment = await ApiClient.RejectPayment(PaymentData.Id);
			if (updatedPayment != null)
			{
				PaymentData = updatedPayment;
				LoadPaymentData();

				MessageBox.Show("✅ Pembayaran berhasil ditolak!", "Success", 
					MessageBoxButtons.OK, MessageBoxIcon.Information);

				OnRejectClicked?.Invoke(this, EventArgs.Empty);
				ParentForm?.RefreshData();
			}
		}
	}
	catch (Exception ex)
	{
		MessageBox.Show($"❌ Error: {ex.Message}", "Error", 
			MessageBoxButtons.OK, MessageBoxIcon.Error);
	}
}
```

---

## Summary

Untuk menyelesaikan semua 8 CRITICAL issues:

1. ✅ Delete duplicate PaymentResponse.cs
2. ✅ Create TenantPaymentHistory.cs
3. ✅ Implement PenyewaDetail.cs with full logic
4. ✅ Create Gallery.cs with proper mapping
5. ✅ Implement Dashboard Stats API integration
6. ✅ Fix bilingual status normalization
7. ✅ Implement multi-image upload in AddKamar
8. ✅ Fix payment confirmation response handling

**Total Estimated Work**: 30-40 hours untuk semua critical fixes.
