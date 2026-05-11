# 05 - TUTORIAL MODUL EDIT KAMAR

**File Terkait:** `EditKamar.cs`, `EditKamar.Designer.cs`, `Kamar.cs`

---

## 📖 Pendahuluan

Modul Edit Kamar berupa jendela dialog (Modal Form) yang dipanggil ketika admin menekan tombol "Edit" pada List Kamar. Di tutorial ini, kita akan membangun form yang secara otomatis menampilkan data lama (Pre-fill), melakukan validasi ketika user mengubah nilai, lalu menyimpannya kembali ke server menggunakan API PUT Request.

### Fungsi Utama
1. **Pre-fill Form** - Mengisi kolom *textbox/dropdown* dengan data kamar yang dipilih.
2. **Input Validation** - Mengecek data sisi klien sebelum API dipanggil (contoh: harga tidak boleh minus).
3. **Bilingual Status** - Mengelola Dropdown Status menggunakan bahasa ganda.
4. **Update to API** - Membungkus ulang data yang diubah dan menembakkan fungsi *PUT*.

---

## Langkah 1: Memahami Alur (Flow Diagram)

```text
┌─────────────────────────────────────────────────────────┐
│     KLIK EDIT PADA LIST KAMAR                          │
│  Buka Form EditKamar & Bawa Data Kamar Tersebut        │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│     FORM LOAD (PRE-FILL DATA)                          │
│  Isi textboxes & Dropdown dengan data bawaan           │
└─────────────────────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────────────────────┐
│     USER MENGEDIT DATA & KLIK "SIMPAN"                 │
│  Validasi Input (Harga, Nomor, Kapasitas)              │
└─────────────────────────────────────────────────────────┘
        /                 \
  Lolos Validasi      Tidak Valid
      /                     \
┌─────────────┐       ┌─────────────────┐
│  API PUT    │       │ TAMPILKAN ERROR │
│  REQUEST    │       │  LABEL MERAH    │
└─────────────┘       └─────────────────┘
```

---

## Langkah 2: Pembuatan Struktur UI Form

Buat sebuah `Form` baru bernama `EditKamar.cs`. Tambahkan *controls* berikut:
- **TextBox**: `txtNomorKamar`, `txtHargaPerBulan`, `txtFasilitas`, `txtDeskripsi`.
- **ComboBox**: `cmbTipeKamar`, `cmbStatus`.
- **NumericUpDown**: `nudFloor`, `nudKapasitas`.
- **Label (Untuk Error)**: Siapkan beberapa Label dengan teks merah yang disembunyikan (Visible = false) di bawah textbox.
- **Button**: `btnSimpan`, `btnBatal`.

---

## Langkah 3: Konstruktor & Pre-fill Data

Kita harus mem-*passing* (mengoper) objek `Kamar` dari Form sebelumnya ke form ini lewat *constructor*.

```csharp
public partial class EditKamar : Form
{
    private Kamar kamarData;
    
    // Konstruktor menerima kamar object yang akan diedit
    public EditKamar(Kamar kamar)
    {
        InitializeComponent();
        this.kamarData = kamar; // Simpan ke variabel global class
    }

    private void EditKamar_Load(object sender, EventArgs e)
    {
        this.Text = $"Edit Kamar - {kamarData.ROOM}";
        
        // 1. Load Dropdown Item
        LoadComboBoxes(); 
        
        // 2. Pre-fill data ke UI
        PreFillFormData();
    }
}
```

Implementasi metode `PreFillFormData`:
```csharp
private void PreFillFormData()
{
    txtNomorKamar.Text = kamarData.ROOM ?? "";
    txtHargaPerBulan.Text = kamarData.PRICE.ToString();
    txtFasilitas.Text = kamarData.FACILITIES ?? "";
    txtDeskripsi.Text = kamarData.DESCRIPTION ?? "";
    
    nudFloor.Value = kamarData.FLOOR;
    nudKapasitas.Value = kamarData.KAPASITAS;

    // Set tipe kamar
    if (!string.IsNullOrEmpty(kamarData.TYPE))
        cmbTipeKamar.SelectedIndex = cmbTipeKamar.Items.IndexOf(kamarData.TYPE);

    // Set Status Bilingual (Gunakan Normalisasi)
    string normalizedStatus = NormalizeStatus(kamarData.STATUS);
    for (int i = 0; i < cmbStatus.Items.Count; i++)
    {
        if (NormalizeStatus(cmbStatus.Items[i].ToString()) == normalizedStatus)
        {
            cmbStatus.SelectedIndex = i;
            break;
        }
    }
}
```

---

## Langkah 4: Validasi Input Sisi Klien

Buat fungsi `ValidateFormInputs()` untuk dipanggil sesaat sebelum submit.

```csharp
private bool ValidateFormInputs()
{
    bool isValid = true;
    
    // Validasi Harga
    if (string.IsNullOrWhiteSpace(txtHargaPerBulan.Text) || !decimal.TryParse(txtHargaPerBulan.Text, out decimal harga) || harga <= 0)
    {
        lblErrorHarga.Text = "⚠️ Harga tidak valid (harus angka > 0)";
        lblErrorHarga.Visible = true;
        isValid = false;
    }

    // Validasi Kapasitas
    if (nudKapasitas.Value < 1)
    {
        MessageBox.Show("Kapasitas minimal 1 orang.");
        isValid = false;
    }
    
    return isValid;
}
```

---

## Langkah 5: Submit API (PUT Request)

Jika validasi lolos, buat objek Kamar baru dan kirim via HTTP `PUT`.

```csharp
private async void btnSimpan_Click(object sender, EventArgs e)
{
    if (!ValidateFormInputs()) return;

    // Disable tombol agar tidak double klik
    btnSimpan.Enabled = false;
    btnSimpan.Text = "Loading...";

    try
    {
        // 1. Build Data
        Kamar updatedKamar = new Kamar
        {
            NO = kamarData.NO, // ID tetap
            ROOM = txtNomorKamar.Text,
            TYPE = cmbTipeKamar.SelectedItem.ToString(),
            PRICE = decimal.Parse(txtHargaPerBulan.Text),
            STATUS = cmbStatus.SelectedItem.ToString(),
            FLOOR = (int)nudFloor.Value,
            KAPASITAS = (int)nudKapasitas.Value,
            FACILITIES = txtFasilitas.Text,
            DESCRIPTION = txtDeskripsi.Text
        };

        // 2. Serialize ke JSON
        string jsonString = JsonConvert.SerializeObject(updatedKamar);
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        // 3. Panggil API
        string url = $"https://rahmatzaw.elarisnoir.my.id/api/kamar/{kamarData.NO}";
        var response = await ApiClient.Client.PutAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            MessageBox.Show("Berhasil diupdate!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK; // Sinyal tutup form
            this.Close();
        }
        else
        {
            MessageBox.Show($"Error Update: {response.StatusCode}");
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}");
    }
    finally
    {
        btnSimpan.Enabled = true;
        btnSimpan.Text = "Simpan";
    }
}
```

---

## Langkah 6: Skenario Pengujian (Testing)

- [ ] **Test Pre-fill Data**: Buka form edit untuk "Kamar 101". Perhatikan apakah semua textbox, checkbox, dan combobox langsung terisi oleh data Kamar 101.
- [ ] **Test Gagal Validasi**: Hapus nominal dari kolom Harga Per Bulan. Klik Simpan. Harusnya muncul label merah dan form tidak tertutup.
- [ ] **Test API PUT**: Ganti nama kamar, klik Simpan. Form akan tertutup, dan data di grid `DataKamar` di balik form harusnya ter-*refresh* dengan nama kamar baru.

---

## 📚 Navigasi Dokumentasi

**← Sebelumnya:** [04_Manajemen_Kamar_List](04_Manajemen_Kamar_List) - Modul Manajemen Kamar List  
**Berikutnya →** [06_Integrasi_API](06_Integrasi_API) - Integrasi API REST

**Daftar Lengkap:** [README_DOKUMENTASI](README_DOKUMENTASI)
