using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kost_SiguraGura
{
    /// <summary>
    /// Pembayaran - Main payment model
    /// Represents a payment transaction in the system
    /// </summary>
    public class Pembayaran
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
        public string BuktiTransfer { get; set; } // CDN URL

        [JsonProperty("status_pembayaran")]
        public string StatusPembayaran { get; set; } // Pending, Confirmed, Rejected

        [JsonProperty("metode_pembayaran")]
        public string MetodePembayaran { get; set; } // transfer, cash, manual

        [JsonProperty("tipe_pembayaran")]
        public string TipePembayaran { get; set; } // full, dp

        [JsonProperty("jumlah_dp")]
        public decimal JumlahDP { get; set; }

        [JsonProperty("tanggal_jatuh_tempo")]
        public DateTime? TanggalJatuhTempo { get; set; }

        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Nested relations
        [JsonProperty("pemesanan")]
        public Pemesanan Pemesanan { get; set; }
    }

    /// <summary>
    /// Pemesanan - Booking/Reservation model
    /// Contains tenant, room and booking information
    /// </summary>
    public class Pemesanan
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("durasi_sewa")]
        public int DurasiSewa { get; set; } // in months

        [JsonProperty("status_pemesanan")]
        public string StatusPemesanan { get; set; } // Active, Cancelled, Expired

        [JsonProperty("penyewa_id")]
        public int PenyewaId { get; set; }

        [JsonProperty("kamar_id")]
        public int KamarId { get; set; }

        [JsonProperty("penyewa")]
        public PenyewaData Penyewa { get; set; }

        [JsonProperty("kamar")]
        public Kamar Kamar { get; set; }
    }

    /// <summary>
    /// PenyewaData - Tenant/Guest model (nested in Pemesanan)
    /// Note: Different from main Penyewa class to avoid conflicts
    /// </summary>
    public class PenyewaData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nama_penyewa")]
        public string NamaPenyewa { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("nomor_telepon")]
        public string NomorTelepon { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; } // guest, tenant, admin

        [JsonProperty("status_penyewa")]
        public string StatusPenyewa { get; set; } // Active, Inactive
    }

    /// <summary>
    /// Wrapper response for payment confirmations
    /// </summary>
    public class PaymentActionResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public Pembayaran Data { get; set; }
    }
}
