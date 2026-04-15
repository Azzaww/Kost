using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kost_SiguraGura
{
    public class Penyewa
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("user_id")]
        public int? USER_ID { get; set; }

        [JsonProperty("user")]
        public User USER { get; set; }

        [JsonProperty("nama_lengkap")]
        public string NAMA_LENGKAP { get; set; }

        [JsonProperty("nik")]
        public string NIK { get; set; }

        [JsonProperty("email")] 
        public string KONTAK { get; set; }

        [JsonProperty("nomor_hp")]
        public string NOMOR_HP { get; set; }

        [JsonProperty("tanggal_lahir")]
        public DateTime? TANGGAL_LAHIR { get; set; }

        [JsonProperty("alamat_asal")]
        public string ALAMAT_ASAL { get; set; }

        [JsonProperty("jenis_kelamin")]
        public string JENIS_KELAMIN { get; set; }

        [JsonProperty("foto_profil")]
        public string FOTO_PROFIL { get; set; }

        [JsonProperty("role")]
        public string PERAN { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CREATED_AT { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UPDATED_AT { get; set; }
    }

    /// <summary>
    /// Nested User object dari API response
    /// </summary>
    public class User
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("username")]
        public string USERNAME { get; set; }

        [JsonProperty("role")]
        public string ROLE { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CREATED_AT { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UPDATED_AT { get; set; }
    }

    /// <summary>
    /// Pagination metadata for tenant list response
    /// </summary>
    public class PaginationMeta
    {
        [JsonProperty("total_rows")]
        public int TotalRows { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("current_page")]
        public int CurrentPage { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }
    }

    /// <summary>
    /// Tenant list response with pagination
    /// </summary>
    public class TenantListResponse
    {
        [JsonProperty("data")]
        public List<Penyewa> Data { get; set; }

        [JsonProperty("meta")]
        public PaginationMeta Meta { get; set; }
    }

    /// <summary>
    /// Payment history for a tenant
    /// </summary>
    public class TenantPaymentHistory
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("jumlah_bayar")]
        public decimal JumlahBayar { get; set; }

        [JsonProperty("tanggal_bayar")]
        public DateTime? TanggalBayar { get; set; }

        [JsonProperty("status_pembayaran")]
        public string StatusPembayaran { get; set; }

        [JsonProperty("metode_pembayaran")]
        public string MetodePembayaran { get; set; }

        [JsonProperty("tipe_pembayaran")]
        public string TipePembayaran { get; set; }

        [JsonProperty("nomor_kamar")]
        public string NomorKamar { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class PenyewaResponse
    {
        // Coba tambahkan "tenants" karena URL-nya /api/tenants
        // Biasanya API Laravel/Golang menggunakan nama jamak dari URL-nya
        [JsonProperty("tenants")]
        public List<Penyewa> Tenants { get; set; }

        [JsonProperty("penyewas")]
        public List<Penyewa> Penyewas { get; set; }
    }
}