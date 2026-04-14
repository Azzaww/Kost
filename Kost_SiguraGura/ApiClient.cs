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
    }
}
