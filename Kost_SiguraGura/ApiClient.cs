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

        private const string BaseUrl = "https://rahmatzaw.elarisnoir.my.id/api";

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
        /// Get all payments from API
        /// Includes nested relations: Pemesanan (with Penyewa and Kamar)
        /// </summary>
        public static async Task<List<Pembayaran>> GetAllPayments()
        {
            try
            {
                string url = $"{BaseUrl}/payments";
                HttpResponseMessage response = await Client.GetAsync(url);

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
                string url = $"{BaseUrl}/payments/{paymentId}/confirm";
                HttpResponseMessage response = await Client.PutAsync(url, null);

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
                string url = $"{BaseUrl}/payments/{paymentId}/reject";
                HttpResponseMessage response = await Client.PutAsync(url, null);

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
                string url = $"{BaseUrl}/tenants?page={page}&limit={limit}";

                if (!string.IsNullOrEmpty(search))
                    url += $"&search={Uri.EscapeDataString(search)}";

                if (!string.IsNullOrEmpty(role))
                    url += $"&role={role}";

                HttpResponseMessage response = await Client.GetAsync(url);

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
                string url = $"{BaseUrl}/tenants/{tenantId}/deactivate";
                HttpResponseMessage response = await Client.PutAsync(url, null);

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
                string url = $"{BaseUrl}/tenant-payments/{tenantId}";
                HttpResponseMessage response = await Client.GetAsync(url);

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
