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

        [JsonProperty("nama_lengkap")]
        public string NAMA_LENGKAP { get; set; }

        [JsonProperty("nik")]
        public string NIK { get; set; }

        [JsonProperty("email")] 
        public string KONTAK { get; set; }

        [JsonProperty("role")]
        public string PERAN { get; set; }
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