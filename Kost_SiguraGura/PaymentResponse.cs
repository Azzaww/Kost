using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kost_SiguraGura
{
    public class PaymentResponse
    {
        [JsonProperty("pembayarans")]
        public List<Pembayaran> Pembayarans { get; set; }
    }

    public class Pembayaran
    {
        [JsonProperty("jumlah_bayar")]
        public long JumlahBayar { get; set; }

        [JsonProperty("status_pembayaran")]
        public string StatusPembayaran { get; set; }
    }
}
