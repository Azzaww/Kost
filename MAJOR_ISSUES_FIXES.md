# 🔨 MAJOR ISSUES FIXES
## Implementation Guide for Issues #9-15

---

## MAJOR ISSUE #9: Pagination UI Not Implemented in DataPenyewa

### Current State
```csharp
// DataPenyewa.cs
private int currentPage = 1;
private int currentLimit = 20;
private int totalPages = 1;

// ❌ NO: Pagination buttons in UI
```

### Missing UI Elements
- Button "Previous" (Navigate to previous page)
- Label "Page X of Y"
- Button "Next" (Navigate to next page)
- ComboBox for "Items per page" (10, 20, 50)

### Implementation

**Add UI Controls** (in Designer or manually):
```csharp
// Panel untuk pagination
Panel panelPagination = new Panel();
panelPagination.Dock = DockStyle.Bottom;
panelPagination.Height = 40;
panelPagination.BackColor = SystemColors.ControlLight;

// Label "Page X of Y"
Label lblPageInfo = new Label();
lblPageInfo.Text = "Page 1 of 1";
lblPageInfo.AutoSize = true;
lblPageInfo.Location = new Point(20, 10);
panelPagination.Controls.Add(lblPageInfo);

// Button Previous
Button btnPrevious = new Button();
btnPrevious.Text = "◀ Previous";
btnPrevious.Location = new Point(150, 8);
btnPrevious.Width = 100;
btnPrevious.Click += (s, e) => NavigatePrevious();
panelPagination.Controls.Add(btnPrevious);

// Button Next
Button btnNext = new Button();
btnNext.Text = "Next ▶";
btnNext.Location = new Point(260, 8);
btnNext.Width = 100;
btnNext.Click += (s, e) => NavigateNext();
panelPagination.Controls.Add(btnNext);

// ComboBox limit
ComboBox cbLimit = new ComboBox();
cbLimit.Items.AddRange(new[] { "10", "20", "50" });
cbLimit.SelectedItem = "20";
cbLimit.Location = new Point(380, 10);
cbLimit.Width = 70;
cbLimit.SelectedIndexChanged += (s, e) => ChangeLimit();
panelPagination.Controls.Add(cbLimit);

this.Controls.Add(panelPagination);
```

**Add Methods** (in DataPenyewa.cs):
```csharp
private void NavigatePrevious()
{
	if (currentPage > 1)
	{
		currentPage--;
		LoadDataWithSearch();
		UpdatePaginationUI();
	}
}

private void NavigateNext()
{
	if (currentPage < totalPages)
	{
		currentPage++;
		LoadDataWithSearch();
		UpdatePaginationUI();
	}
}

private void ChangeLimit()
{
	if (int.TryParse(cbLimit.SelectedItem.ToString(), out int newLimit))
	{
		currentLimit = newLimit;
		currentPage = 1;
		LoadDataWithSearch();
		UpdatePaginationUI();
	}
}

private void UpdatePaginationUI()
{
	this.Invoke((MethodInvoker)delegate {
		lblPageInfo.Text = $"Page {currentPage} of {totalPages}";
		btnPrevious.Enabled = (currentPage > 1);
		btnNext.Enabled = (currentPage < totalPages);
	});
}
```

---

## MAJOR ISSUE #10: Date Filtering Timezone Issues in Report

### Problem
```csharp
var filteredPayments = allPayments
	.Where(p => p.TanggalBayar.HasValue && 
				p.TanggalBayar.Value.Date >= selectedStartDate.Date && 
				p.TanggalBayar.Value.Date <= selectedEndDate.Date)
	.ToList();
```

**Issues**:
- DateTime.Now bisa timezone-dependent
- Midnight edge case tidak handled konsisten

### Solution
```csharp
private DateTime NormalizeStartDate(DateTime date)
{
	// Normalize to start of day (00:00:00)
	return date.Date; // yyyy-MM-dd 00:00:00
}

private DateTime NormalizeEndDate(DateTime date)
{
	// Normalize to end of day (23:59:59)
	return date.Date.AddDays(1).AddSeconds(-1); // yyyy-MM-dd 23:59:59
}

private List<Pembayaran> FilterPaymentsByDateRange(List<Pembayaran> payments, DateTime startDate, DateTime endDate)
{
	DateTime normalizedStart = NormalizeStartDate(startDate);
	DateTime normalizedEnd = NormalizeEndDate(endDate);

	return payments
		.Where(p => p.TanggalBayar.HasValue && 
					p.TanggalBayar.Value >= normalizedStart && 
					p.TanggalBayar.Value <= normalizedEnd)
		.ToList();
}

// Usage
private void UpdateReportStatCards()
{
	var filteredPayments = FilterPaymentsByDateRange(
		allPayments, 
		selectedStartDate, 
		selectedEndDate
	);

	// ... rest of logic
}
```

---

## MAJOR ISSUE #11: Missing Room Occupancy Rate Calculation

### Current State
```csharp
// ❌ Occupancy rate tidak calculated/displayed
// Backend support ini tapi C# tidak display
```

### Implementation

**Add Property to Display**:
```csharp
// Create new UI element for occupancy rate
Label lblOccupancyRate = new Label();
lblOccupancyRate.Text = "Occupancy Rate: 0%";
lblOccupancyRate.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
// Add to dashboard

// Calculate occupancy
private decimal CalculateOccupancyRate(int occupiedRooms, int totalRooms)
{
	if (totalRooms == 0)
		return 0;

	return (decimal)occupiedRooms / totalRooms * 100;
}

// Display in BerandaPage_Load or UpdateKPICards
private void UpdateOccupancyRate()
{
	decimal occupancyRate = CalculateOccupancyRate(
		allRooms.Count(r => r.STATUS?.Contains("Penuh") ?? false),
		allRooms.Count
	);

	lblOccupancyRate.Text = $"Occupancy Rate: {occupancyRate:F1}%";

	// Color coding
	if (occupancyRate >= 80)
		lblOccupancyRate.ForeColor = Color.Green;
	else if (occupancyRate >= 50)
		lblOccupancyRate.ForeColor = Color.Orange;
	else
		lblOccupancyRate.ForeColor = Color.Red;
}
```

---

## MAJOR ISSUE #12: EditKamar Cannot Upload New Images

### Current State
```csharp
// EditKamar.cs
private string[] selectedImagePaths = new string[3];

// ❌ MISSING: Logic untuk upload images dalam edit form
```

### Implementation

**Add image upload to EditKamar**:
```csharp
private async void btnUpdateImages_Click(object sender, EventArgs e)
{
	try
	{
		// Jika ada selected images, upload
		if (selectedImagePaths.Any(p => !string.IsNullOrEmpty(p)))
		{
			using (var client = new HttpClient())
			{
				client.Timeout = TimeSpan.FromSeconds(30);

				var form = new MultipartFormDataContent();

				// Add images
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

				// POST to /api/kamar/{id}/images
				string url = $"https://rahmatzaw.elarisnoir.my.id/api/kamar/{originalKamar.NO}/images";
				var response = await client.PostAsync(url, form);

				if (response.IsSuccessStatusCode)
				{
					MessageBox.Show("✅ Images berhasil di-upload!");
				}
				else
				{
					throw new Exception($"API Error: {response.StatusCode}");
				}
			}
		}
	}
	catch (Exception ex)
	{
		MessageBox.Show($"❌ Error uploading images: {ex.Message}");
	}
}

// Add button ke UI
Button btnUpdateImages = new Button();
btnUpdateImages.Text = "Update Images";
btnUpdateImages.Click += btnUpdateImages_Click;
// Add to form
```

---

## MAJOR ISSUE #13: GalleryForm Delete Not Implemented

### Current State
```csharp
// GalleryForm.cs
// ❌ NO: Delete button atau logic
```

### Implementation

**Add Delete Button & Logic**:
```csharp
private void GalleryForm_Load(object sender, EventArgs e)
{
	// ... existing code ...

	// Wire up delete event
	this.btnDeleteImage.Click += BtnDeleteImage_Click;  // Add button ke designer
}

private async void BtnDeleteImage_Click(object sender, EventArgs e)
{
	try
	{
		// Get selected gallery
		if (filteredGalleries.Count == 0)
		{
			MessageBox.Show("No gallery selected");
			return;
		}

		// For now, delete first in list (should implement selection)
		Gallery selectedGallery = filteredGalleries[0];

		DialogResult result = MessageBox.Show(
			$"Delete gallery: {selectedGallery.title}?",
			"Confirm Delete",
			MessageBoxButtons.YesNo,
			MessageBoxIcon.Question
		);

		if (result == DialogResult.Yes)
		{
			using (var client = new HttpClient())
			{
				string url = $"https://rahmatzaw.elarisnoir.my.id/api/galleries/{selectedGallery.id}";
				var response = await client.DeleteAsync(url);

				if (response.IsSuccessStatusCode)
				{
					MessageBox.Show("✅ Gallery deleted successfully!");
					LoadGalleries(); // Reload
				}
				else
				{
					throw new Exception($"API Error: {response.StatusCode}");
				}
			}
		}
	}
	catch (Exception ex)
	{
		MessageBox.Show($"❌ Error: {ex.Message}");
	}
}
```

---

## MAJOR ISSUE #14: TenantDetailForm Not Integrated

### Current State
```csharp
// File ada: TenantDetailForm.cs
// ❌ Tidak ada reference dari DataPenyewa
```

### Implementation

**Wire up in DataPenyewa.cs**:
```csharp
private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
{
	if (e.RowIndex < 0) return;

	try
	{
		// Get selected tenant
		Penyewa selectedTenant = fullListPenyewa[e.RowIndex];

		// Open TenantDetailForm modal
		TenantDetailForm detailForm = new TenantDetailForm(selectedTenant, this);
		detailForm.ShowDialog();

		// Refresh data after detail form closed
		LoadDataPenyewa();
	}
	catch (Exception ex)
	{
		MessageBox.Show($"Error: {ex.Message}");
	}
}

// Register event dalam Load
private void DataPenyewa_Load(object sender, EventArgs e)
{
	// ...
	dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
}
```

---

## MAJOR ISSUE #15: AddGallery Form Not Implemented

### Current State
```csharp
// GalleryForm.cs line 30
this.btnAddImage.Click += btnAddImage_Click;

// ❌ MISSING: Implementation
```

### Implementation

**Create AddGallery.cs**:
```csharp
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Kost_SiguraGura
{
	public partial class AddGallery : Form
	{
		private string selectedImagePath = "";

		public AddGallery()
		{
			InitializeComponent();
			this.Load += AddGallery_Load;
			this.btnSelectImage.Click += BtnSelectImage_Click;
			this.btnUpload.Click += BtnUpload_Click;
			this.btnCancel.Click += (s, e) => this.Close();
		}

		private void AddGallery_Load(object sender, EventArgs e)
		{
			// Setup combobox categories
			cbCategory.Items.Clear();
			cbCategory.Items.Add("Interior");
			cbCategory.Items.Add("Exterior");
			cbCategory.Items.Add("Fasilitas");
			cbCategory.SelectedIndex = 0;
		}

		private void BtnSelectImage_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
				ofd.Title = "Select Gallery Image";
				ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					selectedImagePath = ofd.FileName;
					lblImagePreview.Text = $"Selected: {Path.GetFileName(selectedImagePath)}";
				}
			}
		}

		private async void BtnUpload_Click(object sender, EventArgs e)
		{
			try
			{
				// Validate
				if (string.IsNullOrEmpty(txtTitle.Text))
				{
					MessageBox.Show("Title is required");
					return;
				}

				if (string.IsNullOrEmpty(selectedImagePath))
				{
					MessageBox.Show("Please select an image");
					return;
				}

				// Upload
				using (var client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromSeconds(30);

					var form = new MultipartFormDataContent();
					form.Add(new StringContent(txtTitle.Text), "title");
					form.Add(new StringContent(cbCategory.SelectedItem.ToString()), "category");

					var fileContent = new StreamContent(File.OpenRead(selectedImagePath));
					fileContent.Headers.ContentType = 
						new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
					form.Add(fileContent, "image", Path.GetFileName(selectedImagePath));

					string url = "https://rahmatzaw.elarisnoir.my.id/api/galleries";
					var response = await client.PostAsync(url, form);

					if (response.IsSuccessStatusCode)
					{
						MessageBox.Show("✅ Gallery uploaded successfully!");
						this.Close();
					}
					else
					{
						throw new Exception($"API Error: {response.StatusCode}");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"❌ Error: {ex.Message}");
			}
		}
	}
}
```

**Add event handler in GalleryForm.cs**:
```csharp
private void btnAddImage_Click(object sender, EventArgs e)
{
	try
	{
		AddGallery addForm = new AddGallery();
		addForm.ShowDialog();

		// Reload galleries after upload
		LoadGalleries();
	}
	catch (Exception ex)
	{
		MessageBox.Show($"Error: {ex.Message}");
	}
}
```

---

## Summary: Major Issues Fixes

| Issue | Estimated Effort | Priority |
|-------|-----------------|----------|
| #9 - Pagination UI | 2 hours | HIGH |
| #10 - Date Filter | 1 hour | HIGH |
| #11 - Occupancy Rate | 1 hour | HIGH |
| #12 - Edit Images | 2 hours | HIGH |
| #13 - Delete Gallery | 1 hour | MEDIUM |
| #14 - TenantDetailForm Integration | 1 hour | HIGH |
| #15 - AddGallery Form | 3 hours | HIGH |

**Total Estimated**: 11 hours untuk semua major fixes.

---

Next: Implement Minor Issues (#16-18) untuk polish dan optimization.
