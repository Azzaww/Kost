using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;

namespace Kost_SiguraGura
{
    public class Kamar
    {
        [JsonProperty("id")]
        public int NO { get; set; }

        [JsonProperty("image_url")]
        public string ThumbnailUrl { get; set; }

        [JsonIgnore]
        public Image THUMBNAIL { get; set; }

        [JsonProperty("nomor_kamar")]
        public string ROOM { get; set; }

        [JsonProperty("tipe_kamar")]
        public string TYPE { get; set; }

        [JsonProperty("harga_per_bulan")]
        public decimal PRICE { get; set; }

        [JsonProperty("floor")]
        public int FLOOR { get; set; }

        [JsonProperty("status")]
        public string STATUS { get; set; }

        [JsonProperty("capacity")]
        public int KAPASITAS { get; set; }

        [JsonProperty("size")]
        public string SIZE { get; set; }

        [JsonProperty("bedrooms")]
        public int BEDROOMS { get; set; }

        [JsonProperty("bathrooms")]
        public int BATHROOMS { get; set; }

        [JsonProperty("fasilitas")]
        public string FACILITIES { get; set; }

        [JsonProperty("description")]
        public string DESCRIPTION { get; set; }
    }

    public class KamarResponse
    {
        [JsonProperty("kamars")]
        public List<Kamar> Kamars { get; set; }
    }
}

