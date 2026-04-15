# Backend API Query - Payment Receipt Image (bukti_transfer) Field

## Pertanyaan tentang Field `bukti_transfer` pada Endpoint Pembayaran

Kami sedang mengimplementasikan fitur untuk menampilkan gambar bukti transfer pembayaran di aplikasi desktop. Mendapat error saat mencoba load gambar dari field `bukti_transfer`.

### Error yang Didapat:
```
Error loading image: InvalidOperationException - An invalid request URI was provided. 
The request URI must either be an absolute URI or BaseAddress must be set.
```

Ini menunjukkan bahwa URL yang disimpan di field `bukti_transfer` adalah **RELATIVE URL**, bukan **ABSOLUTE URL**.

---

## Pertanyaan Spesifik:

### 1. Format Field `bukti_transfer` di Database
- **Apakah field `bukti_transfer` menyimpan:**
  - ❓ Full absolute URL? (contoh: `https://rahmatzaw.elarisnoir.my.id/uploads/bukti/image123.jpg`)
  - ❓ Relative path? (contoh: `uploads/bukti/image123.jpg`)
  - ❓ Hanya nama file? (contoh: `image123.jpg`)
  - ❓ Format lain?

### 2. Base URL untuk File Storage
- **Jika menggunakan relative path, apa base URL-nya?**
  - Contoh: `https://rahmatzaw.elarisnoir.my.id/`
  - Atau: `https://rahmatzaw.elarisnoir.my.id/storage/`
  - Atau URL lain?

### 3. Contoh Response API
**Mohon berikan contoh response dari endpoint pembayaran dengan field `bukti_transfer`:**

```json
{
  "id": 1,
  "nama_pembayar": "Budi Santoso",
  "jumlah_bayar": 1500000,
  "metode_pembayaran": "transfer bank",
  "bukti_transfer": "???" ← Apa format/value ini?
  "status_pembayaran": "pending",
  "created_at": "2024-04-15T10:00:00Z"
}
```

### 4. Public Storage Path
- **Apakah file image disimpan di folder public/accessible?**
  - Kalau ya, apa path lengkapnya?
  - Contoh: `/storage/bukti/` atau `/uploads/` atau `/public/images/`?

### 5. Alternate Solution
- **Apakah ada endpoint terpisah untuk download/view bukti transfer?**
  - Contoh: `GET /api/payments/{id}/bukti-transfer` 
  - Atau: `GET /api/bukti-transfer/{file_id}`

---

## Current Implementation Issue:

Di aplikasi desktop kami mencoba:
```csharp
var response = await client.GetAsync(PaymentData.BuktiTransfer);
```

Jika `BuktiTransfer` adalah relative path (ex: `uploads/bukti/image.jpg`), maka akan error karena HttpClient butuh absolute URL.

---

## Yang Kami Butuhkan:

1. **Konfirmasi format field `bukti_transfer` di response API**
2. **Base URL lengkap untuk file storage** (jika relative)
3. **Contoh value real dari database**
4. **Path lengkap di server** (untuk validasi)

---

## Contoh Solusi yang Bisa Kami Implementasikan:

**Opsi A:** Jika `bukti_transfer` adalah relative path
```csharp
string baseUrl = "https://rahmatzaw.elarisnoir.my.id/";
string fullImageUrl = baseUrl + PaymentData.BuktiTransfer;
var response = await client.GetAsync(fullImageUrl);
```

**Opsi B:** Jika ada dedicated API endpoint
```csharp
string imageUrl = $"https://rahmatzaw.elarisnoir.my.id/api/payments/{PaymentData.Id}/bukti-transfer";
var response = await client.GetAsync(imageUrl);
```

**Opsi C:** Jika `bukti_transfer` sudah absolute URL
```csharp
var response = await client.GetAsync(PaymentData.BuktiTransfer);
```

---

## Contact:
Mohon balas dengan informasi di atas agar kami bisa selesaikan implementasi fitur receipt image dengan benar.

---

## Additional Info:
- **Desktop App Type:** Windows Forms (.NET Framework 4.8)
- **API Client:** HttpClient with Cookie-based authentication
- **Error Context:** Attempting to load image for bank transfer payment receipts
- **Current Endpoint Used:** Already have working `/api/payments` endpoint
