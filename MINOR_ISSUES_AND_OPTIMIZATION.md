# 🔧 MINOR ISSUES & OPTIMIZATION
## Issues #16-18 Implementation Guide

---

## MINOR ISSUE #16: Inconsistent Error Messages

### Current Problem
```csharp
// Inconsistent bilingual format across project

// Type 1: Indonesian only
throw new Exception("Sesi login habis. Silakan login ulang.");

// Type 2: English with Indonesian variable
throw new Exception($"Error saat mengambil data pembayaran: {ex.Message}");

// Type 3: Mixed format
MessageBox.Show("Akses Ditolak! Anda bukan Admin. Role Anda: " + Session.UserRole);
```

### Solution: Create Localization Resource

**Create ErrorMessages.cs**:
```csharp
namespace Kost_SiguraGura
{
	/// <summary>
	/// Centralized error messages with bilingual support
	/// </summary>
	public static class ErrorMessages
	{
		// Authentication
		public const string SessionExpired = "Sesi login habis / Session Expired. Silakan login ulang / Please login again.";
		public const string AccessDenied = "Akses Ditolak / Access Denied!";
		public const string NotAdmin = "Anda bukan Admin / You are not an Admin.";

		// API Errors
		public const string FailedFetchPayments = "Gagal mengambil data pembayaran / Failed to fetch payments.";
		public const string FailedFetchRooms = "Gagal mengambil data kamar / Failed to fetch rooms.";
		public const string FailedFetchTenants = "Gagal mengambil data penyewa / Failed to fetch tenants.";
		public const string FailedFetchDashboard = "Gagal mengambil dashboard stats / Failed to fetch dashboard statistics.";

		// Payment
		public const string PaymentConfirmFailed = "Gagal mengkonfirmasi pembayaran / Failed to confirm payment.";
		public const string PaymentRejectFailed = "Gagal menolak pembayaran / Failed to reject payment.";
		public const string PaymentConfirmSuccess = "Pembayaran berhasil dikonfirmasi / Payment confirmed successfully!";
		public const string PaymentRejectSuccess = "Pembayaran berhasil ditolak / Payment rejected successfully!";

		// Room
		public const string RoomCreateSuccess = "Kamar berhasil dibuat / Room created successfully!";
		public const string RoomUpdateSuccess = "Kamar berhasil diupdate / Room updated successfully!";
		public const string RoomDeleteSuccess = "Kamar berhasil dihapus / Room deleted successfully!";
		public const string RoomCreateFailed = "Gagal membuat kamar / Failed to create room.";

		// Gallery
		public const string GalleryUploadSuccess = "Gallery berhasil di-upload / Gallery uploaded successfully!";
		public const string GalleryDeleteSuccess = "Gallery berhasil dihapus / Gallery deleted successfully!";
		public const string GalleryUploadFailed = "Gagal upload gallery / Failed to upload gallery.";

		// Tenant
		public const string TenantDeactivateSuccess = "Tenant berhasil di-deactivate / Tenant deactivated successfully!";
		public const string TenantDeactivateFailed = "Gagal deactivate tenant / Failed to deactivate tenant.";

		// Validation
		public const string RequiredField = "Field ini wajib diisi / This field is required.";
		public const string InvalidEmail = "Email tidak valid / Invalid email format.";
		public const string InvalidPhone = "Nomor HP tidak valid / Invalid phone number.";
		public const string SelectImage = "Pilih gambar terlebih dahulu / Please select an image first.";
		public const string MinimumThreeImages = "Minimum 3 gambar diperlukan / Minimum 3 images required.";

		/// <summary>
		/// Format API error dengan info status code
		/// </summary>
		public static string FormatApiError(int statusCode, string errorDetail = "")
		{
			string statusMsg = statusCode switch
			{
				400 => "Bad Request / Permintaan Invalid",
				401 => "Unauthorized / Tidak Terauthorisasi",
				403 => "Forbidden / Dilarang",
				404 => "Not Found / Tidak Ditemukan",
				500 => "Server Error / Error Server",
				_ => $"Error {statusCode} / Error {statusCode}"
			};

			if (!string.IsNullOrEmpty(errorDetail))
				return $"{statusMsg}: {errorDetail}";

			return statusMsg;
		}

		/// <summary>
		/// Format exception message untuk user display
		/// </summary>
		public static string FormatExceptionMessage(Exception ex)
		{
			if (ex == null)
				return "Terjadi kesalahan yang tidak diketahui / An unknown error occurred.";

			return $"Error: {ex.Message}";
		}
	}
}
```

**Update ApiClient.cs to use centralized messages**:
```csharp
// Before:
throw new Exception("Sesi login habis. Silakan login ulang.");

// After:
throw new Exception(ErrorMessages.SessionExpired);

// Before:
throw new Exception($"Gagal mengambil data pembayaran. Status: {response.StatusCode}");

// After:
throw new Exception(ErrorMessages.FormatApiError((int)response.StatusCode, 
	ErrorMessages.FailedFetchPayments));
```

**Update UI MessageBox calls**:
```csharp
// Before:
MessageBox.Show("Sesi login habis. Silakan login ulang.");

// After:
MessageBox.Show(ErrorMessages.SessionExpired, "Login", 
	MessageBoxButtons.OK, MessageBoxIcon.Warning);

// With error detail
MessageBox.Show(
	$"{ErrorMessages.PaymentConfirmFailed}\n{ex.Message}",
	"Error",
	MessageBoxButtons.OK,
	MessageBoxIcon.Error
);
```

---

## MINOR ISSUE #17: Missing Image Download Caching

### Current Problem
```csharp
// DataKamar.cs - Download image setiap kali LoadDataKamar
foreach (var k in listData)
{
	// Download dari URL tanpa cache → slow, bandwidth waste
	// Kalau offline → fail
}
```

### Solution: Implement Local Image Cache

**Create ImageCacheManager.cs**:
```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
	/// <summary>
	/// Manage image caching untuk reduce bandwidth dan speed up UI
	/// </summary>
	public static class ImageCacheManager
	{
		private static readonly string CachePath = 
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
						"KostSiguraGura", "ImageCache");

		static ImageCacheManager()
		{
			// Create cache directory if not exist
			if (!Directory.Exists(CachePath))
			{
				Directory.CreateDirectory(CachePath);
			}
		}

		/// <summary>
		/// Get cached image atau download jika tidak ada
		/// </summary>
		public static async Task<System.Drawing.Image> GetCachedImage(string imageUrl)
		{
			try
			{
				if (string.IsNullOrEmpty(imageUrl))
					return null;

				// Generate cache key dari URL
				string cacheKey = GenerateCacheKey(imageUrl);
				string cachePath = Path.Combine(CachePath, cacheKey);

				// Check cache
				if (File.Exists(cachePath))
				{
					try
					{
						return System.Drawing.Image.FromFile(cachePath);
					}
					catch
					{
						// Cache corrupted, delete dan re-download
						File.Delete(cachePath);
					}
				}

				// Download dan cache
				using (var client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromSeconds(10);
					var imageBytes = await client.GetByteArrayAsync(imageUrl);

					// Save to cache
					File.WriteAllBytes(cachePath, imageBytes);

					// Return image
					using (var ms = new System.IO.MemoryStream(imageBytes))
					{
						return System.Drawing.Image.FromStream(ms);
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Image cache error: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// Generate consistent cache filename dari URL
		/// </summary>
		private static string GenerateCacheKey(string url)
		{
			using (var md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] hashBytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(url));
				string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
				return $"{hash}.jpg";
			}
		}

		/// <summary>
		/// Clear cache (optional)
		/// </summary>
		public static void ClearCache()
		{
			try
			{
				if (Directory.Exists(CachePath))
				{
					foreach (var file in Directory.GetFiles(CachePath))
					{
						File.Delete(file);
					}
				}
			}
			catch { }
		}

		/// <summary>
		/// Get cache size
		/// </summary>
		public static long GetCacheSize()
		{
			try
			{
				if (!Directory.Exists(CachePath))
					return 0;

				return Directory.GetFiles(CachePath)
					.Sum(f => new FileInfo(f).Length);
			}
			catch
			{
				return 0;
			}
		}
	}
}
```

**Update DataKamar.cs to use cache**:
```csharp
// Before:
foreach (var k in listData)
{
	// Download image... slow
}

// After:
private async Task LoadImageAsync(Kamar kamar)
{
	try
	{
		if (!string.IsNullOrEmpty(kamar.ThumbnailUrl))
		{
			// Use cache manager
			kamar.THUMBNAIL = await ImageCacheManager.GetCachedImage(kamar.ThumbnailUrl);
		}
	}
	catch (Exception ex)
	{
		System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
	}
}

private async void LoadDataKamar()
{
	using (HttpClient client = new HttpClient())
	{
		try
		{
			// ... fetch kamar list ...

			if (listData != null)
			{
				fullListKamar.Clear();

				// Load images in background (non-blocking)
				_ = Task.Run(async () =>
				{
					foreach (var k in listData)
					{
						await LoadImageAsync(k);
					}
				});

				// Update UI immediately (images load in background)
				ApplyFilters();
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error: {ex.Message}");
		}
	}
}
```

---

## MINOR ISSUE #18: No Offline Mode / Fallback

### Current Problem
```
Jika internet down → App completely non-functional
- Tidak bisa access data
- Tidak bisa queue actions untuk sync later
```

### Solution: Implement Simple Local Cache & Queue

**Create LocalDataCache.cs**:
```csharp
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kost_SiguraGura
{
	/// <summary>
	/// Local cache untuk offline support
	/// </summary>
	public static class LocalDataCache
	{
		private static readonly string CacheDir = 
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
						"KostSiguraGura", "LocalCache");

		static LocalDataCache()
		{
			if (!Directory.Exists(CacheDir))
				Directory.CreateDirectory(CacheDir);
		}

		// Cache payments locally
		public static void CachePayments(List<Pembayaran> payments)
		{
			try
			{
				string json = JsonConvert.SerializeObject(payments, Formatting.Indented);
				File.WriteAllText(Path.Combine(CacheDir, "payments.json"), json);
			}
			catch { }
		}

		public static List<Pembayaran> GetCachedPayments()
		{
			try
			{
				string path = Path.Combine(CacheDir, "payments.json");
				if (File.Exists(path))
				{
					string json = File.ReadAllText(path);
					return JsonConvert.DeserializeObject<List<Pembayaran>>(json) ?? new();
				}
			}
			catch { }
			return new List<Pembayaran>();
		}

		// Cache rooms locally
		public static void CacheRooms(List<Kamar> rooms)
		{
			try
			{
				string json = JsonConvert.SerializeObject(rooms, Formatting.Indented);
				File.WriteAllText(Path.Combine(CacheDir, "rooms.json"), json);
			}
			catch { }
		}

		public static List<Kamar> GetCachedRooms()
		{
			try
			{
				string path = Path.Combine(CacheDir, "rooms.json");
				if (File.Exists(path))
				{
					string json = File.ReadAllText(path);
					return JsonConvert.DeserializeObject<List<Kamar>>(json) ?? new();
				}
			}
			catch { }
			return new List<Kamar>();
		}

		// Action queue untuk sync later
		public static void QueueAction(string action, object data)
		{
			try
			{
				var queue = GetActionQueue();
				queue.Add(new { action, data, timestamp = DateTime.Now });

				string json = JsonConvert.SerializeObject(queue, Formatting.Indented);
				File.WriteAllText(Path.Combine(CacheDir, "action_queue.json"), json);
			}
			catch { }
		}

		public static List<dynamic> GetActionQueue()
		{
			try
			{
				string path = Path.Combine(CacheDir, "action_queue.json");
				if (File.Exists(path))
				{
					string json = File.ReadAllText(path);
					return JsonConvert.DeserializeObject<List<dynamic>>(json) ?? new();
				}
			}
			catch { }
			return new List<dynamic>();
		}

		public static void ClearActionQueue()
		{
			try
			{
				string path = Path.Combine(CacheDir, "action_queue.json");
				if (File.Exists(path))
					File.Delete(path);
			}
			catch { }
		}
	}
}
```

**Create ConnectionMonitor.cs**:
```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kost_SiguraGura
{
	/// <summary>
	/// Monitor internet connectivity
	/// </summary>
	public static class ConnectionMonitor
	{
		public static event Action OnConnectionRestored;
		public static event Action OnConnectionLost;

		private static bool isOnline = true;
		private static System.Threading.Timer connectionCheckTimer;

		public static void Start()
		{
			// Check connection every 30 seconds
			connectionCheckTimer = new System.Threading.Timer(
				async _ => await CheckConnection(),
				null,
				TimeSpan.Zero,
				TimeSpan.FromSeconds(30)
			);
		}

		public static void Stop()
		{
			connectionCheckTimer?.Dispose();
		}

		private static async Task CheckConnection()
		{
			try
			{
				using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
				{
					var response = await client.GetAsync("https://rahmatzaw.elarisnoir.my.id/api/health");

					bool wasOnline = isOnline;
					isOnline = response.IsSuccessStatusCode;

					if (!wasOnline && isOnline)
					{
						OnConnectionRestored?.Invoke();
						System.Diagnostics.Debug.WriteLine("✅ Connection restored!");
					}
					else if (wasOnline && !isOnline)
					{
						OnConnectionLost?.Invoke();
						System.Diagnostics.Debug.WriteLine("❌ Connection lost!");
					}
				}
			}
			catch
			{
				isOnline = false;
			}
		}

		public static bool IsOnline => isOnline;
	}
}
```

**Update forms to use offline support**:
```csharp
private async void LoadData()
{
	try
	{
		if (ConnectionMonitor.IsOnline)
		{
			// Fetch from API
			var payments = await ApiClient.GetAllPayments();

			// Cache locally
			LocalDataCache.CachePayments(payments);

			allPayments = payments;
		}
		else
		{
			// Use cached data
			allPayments = LocalDataCache.GetCachedPayments();
			ShowOfflineWarning();
		}

		UpdateUI();
	}
	catch (Exception ex)
	{
		// Fallback to cache
		allPayments = LocalDataCache.GetCachedPayments();
		ShowOfflineWarning();
	}
}

private void ShowOfflineWarning()
{
	MessageBox.Show("Working offline / Bekerja offline. Data mungkin tidak terbaru.",
		"Offline Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
```

---

## Summary: Minor Issues

| Issue | Effort | Impact |
|-------|--------|--------|
| #16 - Bilingual Messages | 2 hours | UX Improvement |
| #17 - Image Caching | 3 hours | Performance |
| #18 - Offline Mode | 4 hours | Resilience |

**Total**: 9 hours untuk semua minor improvements.

---

## 🎯 OVERALL PROJECT STATUS

### Completed Analysis ✅
- 18 issues identified (8 Critical, 7 Major, 3 Minor)
- Detailed implementation guides provided
- Code examples for each fix
- Effort estimation

### Next Steps 🚀
1. **Week 1**: Fix all 8 Critical issues (~40 hours)
2. **Week 2-3**: Fix all 7 Major issues (~11 hours)
3. **Week 4**: Implement 3 Minor improvements (~9 hours)
4. **Week 5**: Testing & Integration (~20 hours)

**Total Project Time**: ~80 hours for complete fixes and optimization

---

Generated: 2024
Auditor: GitHub Copilot
Target: Production deployment
Priority: HIGH
