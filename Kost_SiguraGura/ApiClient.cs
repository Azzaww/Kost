using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kost_SiguraGura
{
    public static class ApiClient
    {
        // Container ini yang menyimpan 'izin' login dari server
        private static readonly CookieContainer cookieContainer = new CookieContainer();
        private static readonly HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer };

        // Gunakan satu Client ini untuk SEMUA Form dan UserControl
        public static readonly HttpClient Client = new HttpClient(handler);

        // ✅ NEW: Multi-URL support dengan failover
        private const string ProductionUrl = "https://rahmatzaw.elarisnoir.my.id/api";
        private const string LocalhostUrl = "http://localhost:8081/api";

        // Active URL yang digunakan (dapat berubah sesuai hasil connectivity check)
        private static string _activeBaseUrl = ProductionUrl;
        public static string ActiveBaseUrl
        {
            get { return _activeBaseUrl; }
            private set { _activeBaseUrl = value; }
        }

        // Lock untuk thread-safety saat update ActiveBaseUrl
        private static readonly object _urlLock = new object();

        /// <summary>
        /// Static constructor to initialize HttpClient timeout
        /// ✅ FIX Issue #2: Set default timeout untuk prevent hanging requests
        /// </summary>
        static ApiClient()
        {
            // Set timeout ke 30 detik (consistent dengan GalleryForm)
            // Jika request lebih lama dari ini, akan throw TaskCanceledException
            Client.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// ✅ NEW: Initialize connection dengan smart failover system
        /// Cek koneksi ke Production server, jika gagal fallback ke Localhost
        /// Harus dipanggil sekali saat aplikasi startup
        /// </summary>
        public static async Task InitializeConnection()
        {
            try
            {
                // Pertama coba Production URL
                using (var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(5)))
                using (var response = await Client.GetAsync(ProductionUrl + "/health", System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cts.Token))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        lock (_urlLock)
                        {
                            ActiveBaseUrl = ProductionUrl;
                        }
                        System.Diagnostics.Debug.WriteLine("✅ Connected to Production server");
                        return;
                    }
                }
            }
            catch
            {
                // Production gagal, log dan lanjut ke fallback
                System.Diagnostics.Debug.WriteLine("⚠️ Production server not accessible, trying localhost...");
            }

            // Fallback ke Localhost
            try
            {
                using (var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(5)))
                using (var response = await Client.GetAsync(LocalhostUrl + "/health", System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cts.Token))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        lock (_urlLock)
                        {
                            ActiveBaseUrl = LocalhostUrl;
                        }
                        System.Diagnostics.Debug.WriteLine("✅ Connected to Localhost server");
                        return;
                    }
                }
            }
            catch
            {
                // Localhost juga gagal
                System.Diagnostics.Debug.WriteLine("❌ Both servers unreachable, using Production as default");
            }

            // Default ke Production jika kedua gagal (akan error di actual request nanti)
            lock (_urlLock)
            {
                ActiveBaseUrl = ProductionUrl;
            }
        }

        /// <summary>
        /// ✅ NEW: Add Bearer token to request header
        /// </summary>
        private static void AddBearerTokenHeader()
        {
            // Clear existing Authorization headers
            Client.DefaultRequestHeaders.Remove("Authorization");

            // Add new Bearer token jika ada
            if (!string.IsNullOrEmpty(Session.Token))
            {
                Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session.Token}");
            }
        }

        /// <summary>
        /// ✅ NEW: Auto-refresh token when receiving 401 Unauthorized
        /// Menggunakan RefreshToken untuk mendapatkan AccessToken baru
        /// Returns true jika refresh berhasil, false jika gagal
        /// </summary>
        private static async Task<bool> RefreshTokenAsync()
        {
            try
            {
                // Jika tidak ada RefreshToken, tidak bisa refresh
                if (string.IsNullOrEmpty(Session.RefreshToken))
                {
                    System.Diagnostics.Debug.WriteLine("❌ RefreshToken tidak tersedia, tidak dapat refresh");
                    return false;
                }

                // Buat request body
                var refreshRequest = new { refreshToken = Session.RefreshToken };
                string jsonString = JsonConvert.SerializeObject(refreshRequest);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Hubungi /auth/refresh endpoint (tanpa Bearer token)
                Client.DefaultRequestHeaders.Remove("Authorization");
                string url = $"{ActiveBaseUrl}/auth/refresh";
                HttpResponseMessage response = await Client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(responseBody);

                    // Extract accessToken dari response
                    string newAccessToken = result["accessToken"]?.ToString() ?? result["access_token"]?.ToString();

                    if (string.IsNullOrEmpty(newAccessToken))
                    {
                        System.Diagnostics.Debug.WriteLine("❌ Response tidak mengandung accessToken");
                        return false;
                    }

                    // Update Session.Token dengan token baru
                    Session.Token = newAccessToken;
                    System.Diagnostics.Debug.WriteLine("✅ Token berhasil di-refresh");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Token refresh gagal dengan status: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error saat refresh token: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ✅ NEW: Wrapper untuk GET request dengan auto-retry on 401
        /// PUBLIC sehingga bisa digunakan oleh Form/UserControl lain
        /// </summary>
        public static async Task<HttpResponseMessage> GetWithRetry(string url)
        {
            AddBearerTokenHeader();
            HttpResponseMessage response = await Client.GetAsync(url);

            // Jika 401, coba refresh token dan ulangi request
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshSuccess = await RefreshTokenAsync();
                if (refreshSuccess)
                {
                    AddBearerTokenHeader();
                    response = await Client.GetAsync(url);
                }
            }

            return response;
        }

        /// <summary>
        /// ✅ NEW: Wrapper untuk PUT request dengan auto-retry on 401
        /// PUBLIC sehingga bisa digunakan oleh Form/UserControl lain
        /// </summary>
        public static async Task<HttpResponseMessage> PutWithRetry(string url, HttpContent content = null)
        {
            AddBearerTokenHeader();
            HttpResponseMessage response = await Client.PutAsync(url, content);

            // Jika 401, coba refresh token dan ulangi request
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshSuccess = await RefreshTokenAsync();
                if (refreshSuccess)
                {
                    AddBearerTokenHeader();
                    response = await Client.PutAsync(url, content);
                }
            }

            return response;
        }

        /// <summary>
        /// ✅ NEW: Wrapper untuk POST request dengan auto-retry on 401
        /// PUBLIC sehingga bisa digunakan oleh Form/UserControl lain
        /// </summary>
        public static async Task<HttpResponseMessage> PostWithRetry(string url, HttpContent content)
        {
            AddBearerTokenHeader();
            HttpResponseMessage response = await Client.PostAsync(url, content);

            // Jika 401, coba refresh token dan ulangi request
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshSuccess = await RefreshTokenAsync();
                if (refreshSuccess)
                {
                    AddBearerTokenHeader();
                    response = await Client.PostAsync(url, content);
                }
            }

            return response;
        }

        /// <summary>
        /// Get all payments from API
        /// Includes nested relations: Pemesanan (with Penyewa and Kamar)
        /// </summary>
        public static async Task<List<Pembayaran>> GetAllPayments()
        {
            try
            {
                string url = $"{ActiveBaseUrl}/payments";
                HttpResponseMessage response = await GetWithRetry(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<Pembayaran>>(jsonResponse);
                    return result ?? new List<Pembayaran>();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Sesi login habis. Silakan login ulang.");
                }
                else
                {
                    throw new Exception($"Gagal mengambil data pembayaran. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saat mengambil data pembayaran: {ex.Message}");
            }
        }

        /// <summary>
        /// Confirm a payment (update status to Confirmed)
        /// </summary>
        public static async Task<bool> ConfirmPayment(int paymentId)
        {
            try
            {
                string url = $"{ActiveBaseUrl}/payments/{paymentId}/confirm";
                HttpResponseMessage response = await PutWithRetry(url, null);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Sesi login habis. Silakan login ulang.");
                }
                else
                {
                    throw new Exception($"Gagal mengkonfirmasi pembayaran. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saat mengkonfirmasi pembayaran: {ex.Message}");
            }
        }

        /// <summary>
        /// Reject a payment (update status to Rejected)
        /// </summary>
        public static async Task<bool> RejectPayment(int paymentId)
        {
            try
            {
                string url = $"{ActiveBaseUrl}/payments/{paymentId}/reject";
                HttpResponseMessage response = await PutWithRetry(url, null);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Sesi login habis. Silakan login ulang.");
                }
                else
                {
                    throw new Exception($"Gagal menolak pembayaran. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saat menolak pembayaran: {ex.Message}");
            }
        }

        /// <summary>
        /// Get bilingual status display (Indonesian & English)
        /// </summary>
        public static string GetBilingualStatus(string statusEnglish)
        {
            if (string.IsNullOrEmpty(statusEnglish)) return "Unknown";

            string lower = statusEnglish.ToLower();
            if (lower == "pending") return "Pending / Menunggu Konfirmasi";
            if (lower == "confirmed") return "Confirmed / Terkonfirmasi";
            if (lower == "rejected") return "Rejected / Ditolak";

            return statusEnglish;
        }

        /// <summary>
        /// Get payment category bilingual display
        /// </summary>
        public static string GetBilingualCategory(string categoryEnglish)
        {
            if (string.IsNullOrEmpty(categoryEnglish)) return "Unknown";

            string lower = categoryEnglish.ToLower();
            if (lower == "full") return "Full Payment / Pembayaran Penuh";
            if (lower == "dp") return "Down Payment / Uang Muka";
            if (lower == "booking") return "Booking / Pemesanan";

            return categoryEnglish;
        }

        /// <summary>
        /// Get payment method bilingual display
        /// </summary>
        public static string GetBilingualMethod(string methodEnglish)
        {
            if (string.IsNullOrEmpty(methodEnglish)) return "Unknown";

            string lower = methodEnglish.ToLower();
            if (lower == "transfer") return "Bank Transfer / Transfer Bank";
            if (lower == "cash") return "Cash / Tunai";
            if (lower == "manual") return "Manual / Manual";

            return methodEnglish;
        }

        /// <summary>
        /// Get list of tenants with pagination and optional search/filter
        /// </summary>
        public static async Task<TenantListResponse> GetTenants(int page = 1, int limit = 20, string search = "", string role = "")
        {
            try
            {
                string url = $"{ActiveBaseUrl}/tenants?page={page}&limit={limit}";

                if (!string.IsNullOrEmpty(search))
                    url += $"&search={Uri.EscapeDataString(search)}";

                if (!string.IsNullOrEmpty(role))
                    url += $"&role={role}";

                HttpResponseMessage response = await GetWithRetry(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TenantListResponse>(jsonResponse);
                    return result ?? new TenantListResponse { Data = new List<Penyewa>(), Meta = new PaginationMeta() };
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Sesi login habis. Silakan login ulang.");
                }
                else
                {
                    throw new Exception($"Gagal mengambil data tenant. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saat mengambil data tenant: {ex.Message}");
            }
        }

        /// <summary>
        /// Suspend a tenant (change role to non_active)
        /// </summary>
        public static async Task<bool> SuspendTenant(int tenantId)
        {
            try
            {
                string url = $"{ActiveBaseUrl}/tenants/{tenantId}/deactivate";
                HttpResponseMessage response = await PutWithRetry(url, null);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Sesi login habis. Silakan login ulang.");
                }
                else
                {
                    throw new Exception($"Gagal suspend tenant. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saat suspend tenant: {ex.Message}");
            }
        }

        /// <summary>
        /// Get payment history for a specific tenant
        /// </summary>
        public static async Task<List<TenantPaymentHistory>> GetTenantPaymentHistory(int tenantId)
        {
            try
            {
                string url = $"{ActiveBaseUrl}/tenant-payments/{tenantId}";
                HttpResponseMessage response = await GetWithRetry(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Try to parse as array first, then as object with data property
                    var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    List<TenantPaymentHistory> paymentHistory = new List<TenantPaymentHistory>();

                    if (result is Newtonsoft.Json.Linq.JArray)
                    {
                        paymentHistory = JsonConvert.DeserializeObject<List<TenantPaymentHistory>>(jsonResponse);
                    }
                    else if (result["data"] != null)
                    {
                        paymentHistory = JsonConvert.DeserializeObject<List<TenantPaymentHistory>>(result["data"].ToString());
                    }

                    return paymentHistory ?? new List<TenantPaymentHistory>();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Sesi login habis. Silakan login ulang.");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new List<TenantPaymentHistory>();
                }
                else
                {
                    throw new Exception($"Gagal mengambil riwayat pembayaran. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saat mengambil riwayat pembayaran: {ex.Message}");
            }
        }
    }
}
