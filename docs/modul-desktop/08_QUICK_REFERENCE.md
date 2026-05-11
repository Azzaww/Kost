# 08 - CHEATSHEET & QUICK REFERENCE

**File Terkait:** Keseluruhan File dan Dokumentasi Proyek.

---

## 📖 Pendahuluan

Modul ini adalah buku panduan cepat (Cheat Sheet) yang merangkum *Best Practices*, *Snippets* Kode, dan Troubleshooting Umum yang sering dialami oleh Developer/Maintainer sistem Modul Desktop Kos ini. 

Gunakan halaman ini sebagai referensi darurat sebelum Anda bertanya atau membedah logika mendalam pada file Dokumentasi 01 hingga 07.

---

## Langkah 1: Setup Lingkungan (Development Checklist)

Sebelum merombak atau menjalankan *build* produksi untuk Windows Desktop (WinForms):
1. [ ] **Framework Requirement**: Pastikan PC Developer memiliki .NET Framework v4.8 terinstall.
2. [ ] **Library Nuget Wajib**:
   - `Newtonsoft.Json` (v13+) untuk parsing API.
   - `Guna.UI2.WinForms` (v2+) untuk tema dan komponen visual antarmuka elegan.
3. [ ] **Server Validation**: Pastikan Domain API `https://rahmatzaw.elarisnoir.my.id/api` dalam keadaan Aktif (bisa dites via browser atau Postman).

---

## Langkah 2: Kode Pintas Umum (Snippets)

### Memanggil API GET (Sederhana)
Gunakan kode instan ini untuk melakukan ekstraksi API list:
```csharp
string url = $"{ApiClient.BaseUrl}/nama_endpoint";
var response = await ApiClient.Client.GetAsync(url);
if(response.IsSuccessStatusCode) {
    var rawJson = await response.Content.ReadAsStringAsync();
    var result = Newtonsoft.Json.Linq.JToken.Parse(rawJson);
    var list = (result["data"] ?? result).ToObject<List<ModelAnda>>();
}
```

### Navigasi Berpindah Form Modal
Untuk membuka popup *Editor* (seperti Edit Kamar) dan menunggu konfirmasi tersimpan (*Refresh* list):
```csharp
EditKamar frm = new EditKamar(dataTerpilih);
if (frm.ShowDialog() == DialogResult.OK)
{
    await LoadSemuaData(); // Fungsi merefresh tabel List setelah ditutup
}
```

### Normalisasi Status Translasi (Bilingual)
Saat mengecek apakah data adalah `Tersedia`, gunakan fungsi pemotong teks bawaan ini:
```csharp
public string NormalizeStatus(string status) {
    if (string.IsNullOrEmpty(status)) return "";
    return status.Contains("/") ? status.Split('/')[0].Trim().ToLower() : status.ToLower();
}
```

---

## Langkah 3: Debugging Error Umum (Troubleshooting)

**Masalah**: UI Freeze/Ngehang / Macet saat tombol ditekan.
- **Penyebab**: Memanggil method Async dengan `.Result` (Contoh: `ApiClient.Client.GetAsync(url).Result;`).
- **Solusi**: Jadikan tombol bertipe `async void` dan panggil `await ApiClient.Client.GetAsync(url);`.

**Masalah**: Datagrid tidak bisa di-filter. Error *Memory Source Bindings*.
- **Penyebab**: Filter memodifikasi langsung Data List sumber tanpa melakukan Rebind.
- **Solusi**: Filter harus diekstrak ke List terpisah, dan dimasukkan menggunakan `BindingList<T>` pada `dataGridView.DataSource`.

**Masalah**: Sesi tiba-tiba habis, harus login kembali.
- **Penyebab**: Cookie dari backend API telah expired, atau Domain Backend API terputus sehingga Cookie Container mereset.
- **Solusi**: Tangkap (Catch) respons berstatus Http `401 Unauthorized` lalu keluarkan Popup MessageBox agar pengguna masuk ke `Form1.cs` kembali.

---

## 📚 Navigasi Dokumentasi

**← Sebelumnya:** [07_Model_Data](07_Model_Data) - Model Data & Struktur

**🏁 Selesai!** Ini adalah halaman terakhir. Untuk kembali ke awal, buka [00_INDEX](00_INDEX) atau lihat [README_DOKUMENTASI](README_DOKUMENTASI) untuk membaca Overview Lengkap.
