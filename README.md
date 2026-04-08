Arsitektur Global Integrasi REST API ke Desktop C#

1. Tahap Konstruksi Model (Data Blueprint)
Sebelum mengambil data, kita harus menyiapkan "wadah" di dalam aplikasi yang strukturnya sama persis dengan data dari server. Seperti "Kamar.cs"

- Logika: Jika server mengirim teks `nomor_kamar`, aplikasi tidak akan paham kecuali kita beri petunjuk (Mapping).
- Contoh Script:
  
```
public class Kamar 
{
    // Mengarahkan kunci JSON dari server ke properti di C#
    [JsonProperty("id")] 
    public int NO { get; set; }

    [JsonProperty("image_url")]
    public string ThumbnailUrl { get; set; }

    // Wadah khusus untuk menampung gambar asli (bukan teks link)
    [JsonIgnore] 
    public Image THUMBNAIL { get; set; }

    [JsonProperty("nomor_kamar")]
    public string ROOM { get; set; }
}
```

2. Tahap Komunikasi & Pengambilan Data (Network Fetching)
Tahap ini adalah proses "berbicara" dengan server di internet menggunakan protokol HTTP.

- Logika: Aplikasi mengirimkan permintaan (Request) dan menunggu jawaban (Response). Kita menggunakan `async` agar aplikasi tidak *hang* saat menunggu internet yang lambat.
- Contoh Script:
  
```
using (HttpClient client = new HttpClient()) 
{
    // 1. Mengetuk pintu server (GET Request)
    HttpResponseMessage response = await client.GetAsync("https://api.anda.com/data");

    // 2. Memastikan pintu terbuka (Status 200 OK)
    if (response.IsSuccessStatusCode) 
    {
        // 3. Menerima paket kiriman dalam bentuk teks mentah (JSON)
        string jsonRaw = await response.Content.ReadAsStringAsync();
    }
}
```

3. Tahap Transformasi & Pemrosesan Aset (Data Processing)
Ini adalah tahap "penerjemahan" dari teks mentah menjadi objek nyata, termasuk mengubah link foto menjadi gambar visual.

- Logika: Teks JSON diurai (Deserialize) menjadi daftar objek. Karena gambar di internet hanya berupa link teks, kita harus mendownload datanya kembali agar bisa tampil di tabel.
- Contoh Script:
  
```
// 1. Mengubah teks mentah menjadi daftar objek C#
var listData = JsonConvert.DeserializeObject<List<Kamar>>(jsonRaw);

foreach (var item in listData) 
{
    // 2. Download data gambar dari link yang ada di JSON
    byte[] imageBytes = await client.GetByteArrayAsync(item.ThumbnailUrl);
    
    // 3. Mengubah aliran data (Stream) menjadi objek Gambar (Image)
    using (var ms = new MemoryStream(imageBytes)) 
    {
        item.THUMBNAIL = Image.FromStream(ms);
    }
}
```

4. Tahap Visualisasi & Antarmuka (UI Binding)
Tahap terakhir adalah menyajikan data yang sudah diproses ke layar pengguna secara rapi.

- Logika: Menghubungkan daftar objek di memori ke komponen tabel (Grid). Kita juga mengatur estetika seperti menyembunyikan kolom teknis dan mengatur ukuran gambar.
- Contoh Script:
  
```
// 1. Masukkan data ke BindingList agar sinkron dengan UI
BindingList<Kamar> bl = new BindingList<Kamar>(listData);

// 2. Tempelkan ke komponen DataGridView
guna2DataGridView1.DataSource = bl;

// 3. Pengaturan Visual (Opsional namun penting)
guna2DataGridView1.Columns["ThumbnailUrl"].Visible = false; // Sembunyikan teks link
guna2DataGridView1.RowTemplate.Height = 80; // Beri ruang untuk gambar
```

Kesimpulan Alur Global:
1.  MODEL: Buat "cetakan" data sesuai format server.
2.  FETCH: Ambil teks mentah dari internet melalui API.
3.  PARSE: Ubah teks jadi objek dan download file gambarnya.
4.  BIND: Tampilkan hasilnya ke dalam tabel interaktif di layar.
