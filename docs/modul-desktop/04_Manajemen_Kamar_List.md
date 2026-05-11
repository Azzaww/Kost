# 04 - TUTORIAL MODUL MANAJEMEN KAMAR (LIST VIEW)

**File Terkait:** `DataKamar.cs`, `Kamar.cs`, `AddKamar.cs`, `EditKamar.cs`

---

## 📖 Pendahuluan

Modul Manajemen Kamar merupakan pusat kontrol daftar ruangan di sistem Kos-kosan. Dalam tutorial ini, kita akan membuat tampilan grid interaktif yang memuat fitur-fitur penting seperti: Search (Pencarian), Filter multi-kondisi (Bilingual Support), dan Navigasi untuk operasi CRUD (Create, Update, Delete).

### Fungsi Utama
1. **List & DataGrid** - Menampilkan tabel `DataGridView` yang rapi.
2. **Search & Debounce** - Pencarian dengan mekanisme *Debounce* agar tidak men-*spam* hit API.
3. **Multi-Filter & Bilingual** - Filter Tipe kamar dan Status (yang mendukung label Bahasa Inggris & Indonesia).
4. **Hapus Data (Delete)** - Eksekusi penghapusan dengan konfirmasi keamanan.

---

## Langkah 1: Memahami Alur (Flow Diagram)

```text
┌─────────────────────────────────────────────────────────┐
│     USER BUKA HALAMAN KAMAR                            │
│  Fetch Data API -> Isi ComboBox Filter -> Tampil Tabel │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│     USER MELAKUKAN INTERAKSI (SEARCH/FILTER)           │
│  - Jika mengetik di Search, trigger Debounce timer     │
│  - Jika rubah status/tipe, langsung apply filter       │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│     EVALUASI FILTER MULTI-KONDISI                      │
│  Kamar dipilah sesuai kriteria -> Tampilkan ke Tabel   │
│  Update keterangan di Status Bar                       │
└─────────────────────────────────────────────────────────┘
```

---

## Langkah 2: Pembuatan Struktur UI

Buat `UserControl` bernama `DataKamar.cs` lalu siapkan kontrol UI berikut:
- 1 x `TextBox` untuk kotak pencarian (search box).
- 2 x `ComboBox` untuk filter Status (`guna2ComboBox1`) dan Tipe (`guna2ComboBox2`).
- 1 x `DataGridView` (`dataGridKamar`) untuk menampilkan data tabel.
- Beberapa `Button` untuk Tambah, Edit, Hapus.

---

## Langkah 3: Setup Filter & Bilingual Mapping

Pada saat modul di inisialisasi, siapkan item filter pada *combobox*. Karena status di sistem dapat memiliki dua bahasa, buat *helper* normalisasi.

```csharp
private void SetupComboBox()
{
    // Filter Status
    guna2ComboBox1.Items.Clear();
    guna2ComboBox1.Items.Add("Semua Status");
    guna2ComboBox1.Items.Add("Tersedia / Available");
    guna2ComboBox1.Items.Add("Penuh / Full");
    guna2ComboBox1.Items.Add("Perbaikan / Maintenance");
    guna2ComboBox1.SelectedIndex = 0;
    
    // Binding event on-change
    guna2ComboBox1.SelectedIndexChanged += (sender, e) => ApplyFilters();
    // (Lakukan hal sama untuk combobox Tipe Kamar)
}

// Normalisasi String Bilingual
private string NormalizeStatus(string status)
{
    if (string.IsNullOrEmpty(status)) return "";
    status = status.Trim().ToLower();
    
    if (status.Contains("/")) status = status.Split('/')[0].Trim(); // Ambil teks bahasa pertamanya

    if (status == "tersedia" || status == "available") return "tersedia_available";
    if (status == "penuh" || status == "full") return "penuh_full";
    if (status == "perbaikan" || status == "maintenance") return "perbaikan_maintenance";

    return status;
}
```

---

## Langkah 4: Load Data API

Panggil data dari backend dan simpan hasilnya ke *Memory Cache* (list internal) agar pencarian di sisi client sangat cepat.

```csharp
private List<Kamar> fullListKamar = new List<Kamar>();
private BindingList<Kamar> bindingListKamar;

private async Task LoadAllKamarData()
{
    try
    {
        string url = "https://rahmatzaw.elarisnoir.my.id/api/kamar";
        var response = await ApiClient.Client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var result = Newtonsoft.Json.Linq.JToken.Parse(jsonResponse);
            
            var listRaw = result is Newtonsoft.Json.Linq.JArray ? result : (result["kamars"] ?? result["data"] ?? result);
            fullListKamar = listRaw.ToObject<List<Kamar>>() ?? new List<Kamar>();
        }
        
        ApplyFilters(); // Eksekusi filter default ("Semua")
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error loading: {ex.Message}");
    }
}
```

---

## Langkah 5: Logika Pencarian & Multi-Filter

Kita gabungkan input `TextBox` dan dua `ComboBox` menjadi satu metode evaluasi berjenjang menggunakan **LINQ**.

```csharp
private void ApplyFilters()
{
    var filtered = fullListKamar.AsEnumerable();

    // 1. Filter Search (Text)
    string searchText = txtSearchKamar.Text.ToLower();
    if (!string.IsNullOrWhiteSpace(searchText))
    {
        filtered = filtered.Where(k => 
            (k.ROOM?.ToLower().Contains(searchText) == true) ||
            (k.TYPE?.ToLower().Contains(searchText) == true)
        );
    }

    // 2. Filter Status
    string selStatus = guna2ComboBox1.SelectedItem?.ToString();
    if (selStatus != "Semua Status" && !string.IsNullOrEmpty(selStatus))
    {
        string statusFilter = selStatus.Split('/')[0].Trim().ToLower();
        filtered = filtered.Where(k => NormalizeStatus(k.STATUS) == NormalizeStatus(statusFilter));
    }

    // Bind kembali hasil filter ke Datagrid
    bindingListKamar = new BindingList<Kamar>(filtered.ToList());
    dataGridKamar.DataSource = bindingListKamar;
}
```

> [!TIP]
> **Debouncing Search:** Tambahkan `Timer` interval `300ms` saat *user* mengetik di TextBox (event `TextChanged`) agar aplikasi tidak men-*trigger* pemilahan data setiap 1 huruf diketik. Ini sangat meringankan kinerja CPU.

---

## Langkah 6: Implementasi Hapus Data (Delete)

Jika admin menekan tombol hapus, eksekusi API Delete setelah memberikan prompt interaktif.

```csharp
private async void btnDeleteKamar_Click(object sender, EventArgs e)
{
    if (dataGridKamar.SelectedRows.Count == 0) return;

    int kamarId = (int)dataGridKamar.SelectedRows[0].Cells["NO"].Value;
    
    if (MessageBox.Show($"Yakin ingin menghapus kamar?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
    {
        try
        {
            string url = $"https://rahmatzaw.elarisnoir.my.id/api/kamar/{kamarId}";
            var response = await ApiClient.Client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Sukses dihapus.");
                await LoadAllKamarData(); // Refresh Data
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }
}
```

---

## Langkah 7: Skenario Pengujian (Testing)

- [ ] **Test Pencarian Ketat**: Ketik nomor kamar (misal: `101`), pastikan tabel langsung menyusut hanya menampilkan kamar yang relevan saja.
- [ ] **Test Filter Tipe & Status**: Pilih Status `Tersedia / Available`, pastikan kamar dengan status Penuh otomatis hilang dari tabel.
- [ ] **Test Aksi Hapus**: Klik sembarang kamar di tabel, tekan hapus, jawab *Yes* pada dialog. Pastikan data terhapus.

---

## 📚 Navigasi Dokumentasi

**← Sebelumnya:** [03_Dashboard_Beranda](03_Dashboard_Beranda) - Modul Dashboard / Beranda  
**Berikutnya →** [05_Edit_Kamar](05_Edit_Kamar) - Modul Edit Kamar

**Daftar Lengkap:** [README_DOKUMENTASI](README_DOKUMENTASI)