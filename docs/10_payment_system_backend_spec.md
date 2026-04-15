# 💳 Spesifikasi Sistem Pembayaran Backend - Integration Guide

## 📋 Ringkasan Eksekutif

Sistem pembayaran Kost dibangun dengan arsitektur **Pull Everything** (fetch semua data tanpa paginasi) dan operasi status yang **ketat dan role-based**. Backend dirancang untuk efisiensi maksimal di sisi server dengan mengandalkan client-side logic untuk filtering dan reporting.

---

## 1️⃣ API ENDPOINTS PEMBAYARAN

### A. GET /api/payments — Daftar Semua Pembayaran

| Atribut | Detail |
|---------|--------|
| **Method** | GET |
| **Otorisasi** | ✅ Admin ONLY (Role Check: "admin") |
| **Query Parameters** | ❌ TIDAK ADA (No pagination, no filter, no sort) |
| **Response Type** | Raw JSON Array `[...]` (bukan wrapped object) |
| **HTTP Status** | 200 OK |
| **Error Status** | 401 Unauthorized, 403 Forbidden |

#### Request
```bash
curl -X GET "https://rahmatzaw.elarisnoir.my.id/api/payments" \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json"
```

#### Response (200 OK)
```json
[
  {
    "id": 1,
    "pemesanan_id": 10,
    "jumlah_bayar": 600000,
    "tanggal_bayar": "2026-04-14T00:00:00Z",
    "bukti_transfer": "https://res.cloudinary.com/example/image/upload/v1234567890/proof.jpg",
    "status_pembayaran": "Confirmed",
    "metode_pembayaran": "transfer",
    "tipe_pembayaran": "full",
    "jumlah_dp": 0,
    "tanggal_jatuh_tempo": "2026-05-14T00:00:00Z",
    "order_id": "INV-2026-001",
    "pemesanan": {
      "id": 10,
      "durasi_sewa": 3,
      "status_pemesanan": "Active",
      "penyewa_id": 5,
      "kamar_id": 8,
      "penyewa": {
        "id": 5,
        "nama_penyewa": "John Doe",
        "email": "john@example.com",
        "nomor_telepon": "+6281234567890"
      },
      "kamar": {
        "id": 8,
        "nomor_kamar": "A-101",
        "harga_sewa": 600000
      }
    }
  },
  {
    "id": 2,
    "pemesanan_id": 11,
    "jumlah_bayar": 300000,
    "tanggal_bayar": null,
    "bukti_transfer": "https://res.cloudinary.com/example/image/upload/v1234567891/proof2.jpg",
    "status_pembayaran": "Pending",
    "metode_pembayaran": "transfer",
    "tipe_pembayaran": "dp",
    "jumlah_dp": 300000,
    "tanggal_jatuh_tempo": "2026-05-20T00:00:00Z",
    "order_id": "INV-2026-002",
    "pemesanan": { ... }
  }
]
```

#### Response (401 Unauthorized)
```json
{
  "error": "Unauthorized: Missing or invalid token"
}
```

#### Response (403 Forbidden)
```json
{
  "error": "Forbidden: Admin role required"
}
```

**Key Points:**
- ⚠️ **Ambil SEMUA pembayaran** — Tidak ada limit/offset/paginasi
- 📊 **Data besar** — Untuk ribuan record, perhatikan RAM usage
- 🔗 **Nested objects** — Relasi pemesanan, penyewa, kamar sudah di-join
- 🎯 **Client-side filtering** — Filtering dilakukan di C# menggunakan LINQ

---

### B. GET /api/payments/{id} — Detail Pembayaran Spesifik

| Atribut | Detail |
|---------|--------|
| **Status** | ❌ **TIDAK TERSEDIA** |
| **Alternatif** | Gunakan GET /api/payments, kemudian filter via C# LINQ |

```csharp
// C# Alternative:
var payment = allPayments.FirstOrDefault(p => p.Id == id);
if (payment == null)
    MessageBox.Show("Pembayaran tidak ditemukan", "Error");
```

---

### C. PUT /api/payments/{id}/confirm — Konfirmasi Pembayaran

| Atribut | Detail |
|---------|--------|
| **Method** | PUT |
| **Otorisasi** | ✅ Admin ONLY |
| **Path Parameter** | `{id}` = Payment ID |
| **Request Body** | ❌ KOSONG (tidak diperlukan) |
| **Response Type** | Success message atau pembayaran object |
| **HTTP Status** | 200 OK, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 500 Server Error |

#### Request
```bash
curl -X PUT "https://rahmatzaw.elarisnoir.my.id/api/payments/1/confirm" \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json"
```

#### Response (200 OK)
```json
{
  "message": "Payment confirmed successfully",
  "data": {
    "id": 1,
    "status_pembayaran": "Confirmed",
    "tanggal_bayar": "2026-04-15T10:30:00Z"
  }
}
```

#### Automatic Backend Actions (Transactional):
1. ✅ Set `tanggal_bayar` = current timestamp (Asia/Jakarta timezone)
2. ✅ Update status kamar menjadi `"Penuh"` (Full)
3. ✅ Jika mode `Extend`, tambah durasi sewa penyewa
4. ✅ Jika penyewa baru, upgrade role dari `"guest"` → `"tenant"`
5. ✅ Send email receipt ke penyewa
6. ✅ Send WhatsApp notification (async)

#### Error Response (400 Bad Request)
```json
{
  "error": "Invalid payment ID",
  "details": "Payment not found or already confirmed"
}
```

#### Error Response (403 Forbidden - Non-Admin User)
```json
{
  "error": "Forbidden: Only admin can confirm payments"
}
```

---

### D. PUT /api/payments/{id}/reject — Tolak Pembayaran

| Atribut | Detail |
|---------|--------|
| **Method** | PUT |
| **Otorisasi** | ✅ Admin ONLY |
| **Path Parameter** | `{id}` = Payment ID |
| **Request Body** | ❌ KOSONG (tidak diperlukan) |
| **Response Type** | Success message |
| **HTTP Status** | 200 OK, 400 Bad Request, 401 Unauthorized, 403 Forbidden |

#### Request
```bash
curl -X PUT "https://rahmatzaw.elarisnoir.my.id/api/payments/1/reject" \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json"
```

#### Response (200 OK)
```json
{
  "message": "Payment rejected successfully",
  "data": {
    "id": 1,
    "status_pembayaran": "Rejected",
    "tanggal_bayar": null
  }
}
```

#### Backend Actions:
- ✅ Set status → `"Rejected"`
- ✅ Reset `tanggal_bayar` → `null`
- ✅ Send rejection notification ke penyewa (WhatsApp/Email)
- ℹ️ Penyewa bisa re-upload bukti → status otomatis reset ke `"Pending"`

---

### E. POST /api/payments/confirm-cash/{id} — Konfirmasi Pembayaran Cash/Tunai

| Atribut | Detail |
|---------|--------|
| **Method** | POST |
| **Otorisasi** | ✅ Admin ONLY |
| **Path Parameter** | `{id}` = Payment ID |
| **Request Body** | ✅ **WAJIB**: JSON dengan field `bukti_transfer` (string deskripsi) |
| **Response Type** | Success message |
| **HTTP Status** | 200 OK, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 500 Server Error |

#### Request
```bash
curl -X POST "https://rahmatzaw.elarisnoir.my.id/api/payments/confirm-cash/1" \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "bukti_transfer": "Pembayaran tunai diterima langsung. Nominal: Rp 600.000. Tanggal: 15 April 2026"
  }'
```

#### Response (200 OK)
```json
{
  "message": "Cash payment confirmed",
  "data": {
    "id": 1,
    "status_pembayaran": "Confirmed",
    "metode_pembayaran": "cash",
    "bukti_transfer": "Pembayaran tunai diterima langsung...",
    "tanggal_bayar": "2026-04-15T10:30:00Z"
  }
}
```

#### Error Response (400 - Missing bukti_transfer)
```json
{
  "error": "bukti_transfer field is required",
  "details": "Please provide payment proof description"
}
```

---

### F. DELETE /api/payments/{id} — Hapus Pembayaran

| Atribut | Detail |
|---------|--------|
| **Status** | ❌ **TIDAK TERSEDIA** |
| **Alasan** | Untuk menjaga integritas buku kas (accounting audit trail) |
| **Alternatif** | Hanya bisa terhapus jika Pemesanan (Booking) di-cancel |

**⚠️ Critical Note:** Backend sengaja tidak menyediakan endpoint delete untuk mencegah manipulasi riwayat keuangan. Penghapusan data pembayaran hanya otomatis terjadi sebagai cascade effect ketika Pemesanan parent di-cancel.

---

### G. POST /api/payments/{id}/proof — Upload Bukti Pembayaran

| Atribut | Detail |
|---------|--------|
| **Method** | POST |
| **Path Parameter** | `{id}` = Payment ID |
| **Otorisasi** | ✅ Penyewa yang sesuai atau Admin |
| **Content-Type** | `multipart/form-data` |
| **Form Key** | `proof` (nama field wajib) |
| **File Types** | PNG, JPG, JPEG (image only) |
| **Max Size** | Tergantung Cloudinary config (biasanya 5MB) |
| **Response Type** | Success message dengan URL CDN |
| **HTTP Status** | 200 OK, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 500 Server Error |

#### Request (cURL)
```bash
curl -X POST "https://rahmatzaw.elarisnoir.my.id/api/payments/1/proof" \
  -H "Authorization: Bearer {TOKEN}" \
  -F "proof=@/path/to/proof.jpg"
```

#### Request (C# WinForms)
```csharp
// Pseudo-code untuk C# WinForms
var handler = new HttpClientHandler();
using (var client = new HttpClient(handler))
{
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

    var form = new MultipartFormDataContent();
    var fileStream = File.OpenRead("C:\\path\\to\\proof.jpg");
    form.Add(new StreamContent(fileStream), "proof", "proof.jpg");

    var response = await client.PostAsync(
        "https://rahmatzaw.elarisnoir.my.id/api/payments/1/proof",
        form
    );

    if (response.IsSuccessStatusCode)
    {
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(json);
        MessageBox.Show($"Upload sukses: {result.data.bukti_transfer}");
    }
}
```

#### Response (200 OK)
```json
{
  "message": "Proof uploaded successfully",
  "data": {
    "id": 1,
    "bukti_transfer": "https://res.cloudinary.com/example/image/upload/v1234567890/proof.jpg",
    "status_pembayaran": "Pending"
  }
}
```

#### Error Response (400 - Invalid File)
```json
{
  "error": "Invalid file type",
  "details": "Only PNG and JPG are allowed"
}
```

#### Error Response (500 - Upload Failure)
```json
{
  "error": "Cloudinary upload failed",
  "details": "Internal server error"
}
```

---

## 2️⃣ DATA MODEL PEMBAYARAN

### Field Structure

| Field | Type | Required | Default | Notes |
|-------|------|----------|---------|-------|
| `id` | int | ✅ | Auto-increment | Primary key |
| `pemesanan_id` | int | ✅ | - | Foreign key → Pemesanan |
| `jumlah_bayar` | float64 | ✅ | - | Nominal dalam Rupiah |
| `tanggal_bayar` | DateTime | ❌ | null | Set otomatis saat di-confirm |
| `bukti_transfer` | string | ❌ | null | URL CDN Cloudinary image |
| `status_pembayaran` | string | ✅ | "Pending" | Enum: Pending, Confirmed, Rejected |
| `metode_pembayaran` | string | ✅ | "transfer" | Enum: transfer, cash, manual |
| `tipe_pembayaran` | string | ✅ | "full" | Enum: full, dp (down payment) |
| `jumlah_dp` | float64 | ❌ | 0 | Nominal DP (jika tipe=dp) |
| `tanggal_jatuh_tempo` | DateTime | ❌ | null | Deadline tagihan cicilan |
| `order_id` | string | ❌ | null | Reference ID (invoice number) |
| `created_at` | DateTime | ✅ | now() | Created timestamp |
| `updated_at` | DateTime | ✅ | now() | Updated timestamp |

### Nested Relations (Auto-included di Response)

```
Payment
├── pemesanan (Booking/Pemesanan object)
│   ├── id
│   ├── durasi_sewa (bulan)
│   ├── status_pemesanan (Active, Cancelled, Expired)
│   ├── penyewa_id
│   ├── kamar_id
│   ├── penyewa (Tenant object)
│   │   ├── id
│   │   ├── nama_penyewa
│   │   ├── email
│   │   ├── nomor_telepon
│   │   ├── role (guest, tenant, admin)
│   │   └── status_penyewa (Active, Inactive)
│   └── kamar (Room object)
│       ├── id
│       ├── nomor_kamar
│       ├── harga_sewa
│       ├── status_kamar (Tersedia, Penuh, Perbaikan)
│       └── type (Standard, Premium)
```

### C# DTO Model

```csharp
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
    public string BuktiTransfer { get; set; }

    [JsonProperty("status_pembayaran")]
    public string StatusPembayaran { get; set; }

    [JsonProperty("metode_pembayaran")]
    public string MetodePembayaran { get; set; }

    [JsonProperty("tipe_pembayaran")]
    public string TipePembayaran { get; set; }

    [JsonProperty("jumlah_dp")]
    public decimal JumlahDP { get; set; }

    [JsonProperty("tanggal_jatuh_tempo")]
    public DateTime? TanggalJatuhTempo { get; set; }

    [JsonProperty("order_id")]
    public string OrderId { get; set; }

    [JsonProperty("pemesanan")]
    public Pemesanan Pemesanan { get; set; }
}

public class Pemesanan
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("durasi_sewa")]
    public int DurasiSewa { get; set; }

    [JsonProperty("status_pemesanan")]
    public string StatusPemesanan { get; set; }

    [JsonProperty("penyewa_id")]
    public int PenyewaId { get; set; }

    [JsonProperty("kamar_id")]
    public int KamarId { get; set; }

    [JsonProperty("penyewa")]
    public Penyewa Penyewa { get; set; }

    [JsonProperty("kamar")]
    public Kamar Kamar { get; set; }
}

public class Penyewa
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
    public string Role { get; set; }

    [JsonProperty("status_penyewa")]
    public string StatusPenyewa { get; set; }
}

public class Kamar
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("nomor_kamar")]
    public string NomorKamar { get; set; }

    [JsonProperty("harga_sewa")]
    public decimal HargaSewa { get; set; }

    [JsonProperty("status_kamar")]
    public string StatusKamar { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
}
```

---

## 3️⃣ STATUS PEMBAYARAN & WORKFLOW

### Valid Status Values

| Status | Code | Description |
|--------|------|-------------|
| Pending | `"Pending"` | Menunggu konfirmasi admin (initial state) |
| Confirmed | `"Confirmed"` | Sudah dikonfirmasi, pembayaran diterima |
| Rejected | `"Rejected"` | Ditolak karena bukti tidak valid, dll |

### State Transitions

```
┌─────────────────────────────────────────┐
│    Initial State: PENDING               │
│ (Payment created, proof uploaded)       │
└────────────┬────────────────────────────┘
             │
             ├─── [Admin: /confirm] ──→ CONFIRMED
             │                          ↓
             │                    [Final State]
             │
             └─── [Admin: /reject] ──→ REJECTED
                                      ↓
                          [Penyewa re-upload]
                                      ↓
                                   PENDING
```

### Workflow Detail

#### 1. Penyewa Membuat Pembayaran (Booking)
```
Status: PENDING
- Payment record created
- bukti_transfer: null (jika hanya nominal, belum ada proof)
- tanggal_bayar: null
```

#### 2. Penyewa Upload Bukti Pembayaran
```
POST /api/payments/{id}/proof
Status remains: PENDING
- bukti_transfer: https://res.cloudinary.com/... (updated)
```

#### 3. Admin Confirmasi Pembayaran
```
PUT /api/payments/{id}/confirm
Status: PENDING → CONFIRMED
- tanggal_bayar: NOW (Asia/Jakarta)
- Kamar status → "Penuh"
- Penyewa role: "guest" → "tenant" (jika baru)
- Durasi sewa extend (jika perpanjangan)
- Email + WhatsApp notification sent (async)
```

#### 4. Admin Tolak Pembayaran
```
PUT /api/payments/{id}/reject
Status: PENDING → REJECTED
- tanggal_bayar: null
- Notification sent ke penyewa
- Penyewa bisa re-upload bukti →  status reset ke PENDING
```

### Role-Based Access Control

| Role | GET | Confirm | Reject | Upload Proof |
|------|-----|---------|--------|--------------|
| **Admin** | ✅ (all) | ✅ | ✅ | ✅ |
| **Tenant/Guest** | ✅ (own only) | ❌ | ❌ | ✅ (own) |
| **Public** | ❌ | ❌ | ❌ | ❌ |

**Validation:**
- Admin: No restriction on ANY payment
- Tenant: Can only view/upload for `pemesanan.penyewa_id == current_user_id`
- Attempt violation → HTTP 403 Forbidden

---

## 4️⃣ BUKTI PEMBAYARAN (PROOF OF PAYMENT)

### Storage & Format

| Atribut | Detail |
|---------|--------|
| **Platform** | Cloudinary CDN (cloud storage) |
| **Access** | Public-read (no auth required to view URL) |
| **Formats** | PNG, JPG, JPEG |
| **Max Size** | ~5MB (Cloudinary default limit) |
| **Compression** | Auto-optimized by Cloudinary |
| **Retention** | Permanent (no auto-delete policy) |
| **URL Format** | `https://res.cloudinary.com/{cloud_name}/image/upload/v{version}/{filename}.jpg` |

### Accessing Proof Image

**Option 1: Direct URL (No API Call Needed)**
```csharp
// C# WinForms - Display di PictureBox
var payment = /* ... fetched pembayaran ... */;

if (!string.IsNullOrEmpty(payment.BuktiTransfer))
{
    pictureBox.ImageLocation = payment.BuktiTransfer;
    // atau
    pictureBox.Load(payment.BuktiTransfer);
}
else
{
    pictureBox.Image = null; // or placeholder
}
```

**Option 2: Caching Image Locally**
```csharp
using (var client = new HttpClient())
{
    var imageBytes = await client.GetByteArrayAsync(payment.BuktiTransfer);
    using (var ms = new MemoryStream(imageBytes))
    {
        var bitmap = new Bitmap(ms);
        pictureBox.Image = bitmap;
    }
}
```

### Uploading Proof

**Backend Endpoint:** `POST /api/payments/{id}/proof`

**Request Type:** `multipart/form-data`

**Form Field Name:** `proof` (WAJIB)

**C# Implementation:**
```csharp
private async Task UploadPaymentProof(int paymentId, string filePath)
{
    try
    {
        var handler = new HttpClientHandler();
        using (var client = new HttpClient(handler))
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var form = new MultipartFormDataContent();
            var fileStream = File.OpenRead(filePath);
            var filename = Path.GetFileName(filePath);

            form.Add(new StreamContent(fileStream), "proof", filename);

            var response = await client.PostAsync(
                $"https://rahmatzaw.elarisnoir.my.id/api/payments/{paymentId}/proof",
                form
            );

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(json);
                MessageBox.Show("Upload berhasil!", "Sukses");

                // Refresh data
                await LoadPayments();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Upload gagal: {error}", "Error");
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Exception");
    }
}
```

---

## 5️⃣ PAGINATION & FILTERING

### Backend Limitation: No Native Filtering

Backend endpoint `GET /api/payments` **TIDAK SUPPORT** query parameters untuk:
- ❌ Pagination (`?page`, `?limit`, `?offset`)
- ❌ Filtering (`?status=Pending`, `?penyewa_id=5`)
- ❌ Sorting (`?sort=tanggal_bayar`, `?order=desc`)

**Design Choice:** "Pull Everything" strategy untuk kesederhanaan backend, logic filtering dilakukan client-side (C#).

### Client-Side Filtering (C# LINQ)

```csharp
// Fetch semua pembayaran
var allPayments = await FetchAllPayments(); // GET /api/payments

// Filter by Status
var pendingPayments = allPayments
    .Where(p => p.StatusPembayaran == "Pending")
    .ToList();

var confirmedPayments = allPayments
    .Where(p => p.StatusPembayaran == "Confirmed")
    .ToList();

// Filter by Penyewa
var paymentsByTenant = allPayments
    .Where(p => p.Pemesanan.PenyewaId == userId)
    .ToList();

// Filter by Date Range
var aprilPayments = allPayments
    .Where(p => p.TanggalBayar >= DateTime(2026, 4, 1) 
             && p.TanggalBayar < DateTime(2026, 5, 1))
    .ToList();

// Combined filters
var query = allPayments
    .Where(p => p.StatusPembayaran == "Confirmed"
             && p.Pemesanan.PenyewaId == userId
             && p.TanggalBayar.HasValue)
    .OrderByDescending(p => p.TanggalBayar)
    .ToList();

// Pagination
var pageSize = 10;
var pageNumber = 1;
var paginatedList = query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToList();
```

### DataGridView Binding dengan Filtering

```csharp
private BindingList<Pembayaran> bindingList = new BindingList<Pembayaran>();

private void ApplyFilters()
{
    var filtered = allPayments.AsEnumerable();

    // Filter by status
    if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "Semua Status")
    {
        filtered = filtered.Where(p => p.StatusPembayaran == selectedStatus);
    }

    // Filter by date range
    if (startDate != null && endDate != null)
    {
        filtered = filtered.Where(p => p.TanggalBayar >= startDate 
                                    && p.TanggalBayar <= endDate);
    }

    // Bind to DataGridView
    bindingList = new BindingList<Pembayaran>(filtered.ToList());
    dataGridView1.DataSource = bindingList;
}

private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
{
    selectedStatus = cmbStatus.SelectedItem?.ToString();
    ApplyFilters();
}
```

---

## 6️⃣ REPORTING & CALCULATION

### Backend Limitation: No Pre-Calculated Reports

Backend **TIDAK MENYEDIAKAN** endpoint untuk:
- ❌ Total confirmed payments (sum)
- ❌ Count by status
- ❌ Revenue reports
- ❌ Export to Excel/PDF

**Design Choice:** Client-side reporting untuk reduce server load.

### Client-Side Calculations (C#)

```csharp
public class PaymentReporting
{
    private List<Pembayaran> allPayments;

    public PaymentReporting(List<Pembayaran> payments)
    {
        allPayments = payments;
    }

    // Total dari status tertentu
    public decimal GetTotalByStatus(string status)
    {
        return allPayments
            .Where(p => p.StatusPembayaran == status)
            .Sum(p => p.JumlahBayar);
    }

    // Count pembayaran
    public int GetCountByStatus(string status)
    {
        return allPayments
            .Where(p => p.StatusPembayaran == status)
            .Count();
    }

    // Revenue bulan ini
    public decimal GetMonthlyRevenue(int year, int month)
    {
        return allPayments
            .Where(p => p.StatusPembayaran == "Confirmed"
                     && p.TanggalBayar.HasValue
                     && p.TanggalBayar.Value.Year == year
                     && p.TanggalBayar.Value.Month == month)
            .Sum(p => p.JumlahBayar);
    }

    // Top penyewa by total payment
    public var GetTopTenants(int limit = 10)
    {
        return allPayments
            .GroupBy(p => p.Pemesanan.Penyewa.NamaPenyewa)
            .Select(g => new
            {
                TenantName = g.Key,
                TotalPayment = g.Sum(p => p.JumlahBayar),
                PaymentCount = g.Count(),
                Status = g.First().StatusPembayaran
            })
            .OrderByDescending(x => x.TotalPayment)
            .Take(limit)
            .ToList();
    }

    // Generate CSV export
    public string ExportToCSV()
    {
        var sb = new StringBuilder();
        sb.AppendLine("ID,Penyewa,Kamar,Jumlah,Status,Tanggal Bayar,Metode");

        foreach (var p in allPayments)
        {
            sb.AppendLine($"{p.Id}," +
                $"\"{p.Pemesanan.Penyewa.NamaPenyewa}\"," +
                $"\"{p.Pemesanan.Kamar.NomorKamar}\"," +
                $"{p.JumlahBayar}," +
                $"{p.StatusPembayaran}," +
                $"{p.TanggalBayar:yyyy-MM-dd}," +
                $"{p.MetodePembayaran}");
        }

        return sb.ToString();
    }
}
```

### Dashboard Display

```csharp
private void UpdateDashboard()
{
    var reporting = new PaymentReporting(allPayments);

    // Update labels
    lblTotalConfirmed.Text = $"Rp {reporting.GetTotalByStatus("Confirmed"):N0}";
    lblCountPending.Text = reporting.GetCountByStatus("Pending").ToString();
    lblCountRejected.Text = reporting.GetCountByStatus("Rejected").ToString();

    // Monthly revenue
    var thisMonth = DateTime.Now;
    var monthlyRevenue = reporting.GetMonthlyRevenue(thisMonth.Year, thisMonth.Month);
    lblMonthlyRevenue.Text = $"Rp {monthlyRevenue:N0}";

    // Top tenants chart
    var topTenants = reporting.GetTopTenants(5);
    chartTopTenants.Series[0].Points.Clear();
    foreach (var tenant in topTenants)
    {
        chartTopTenants.Series[0].Points.AddXY(tenant.TenantName, tenant.TotalPayment);
    }
}
```

---

## 7️⃣ ERROR HANDLING & VALIDATION

### HTTP Status Codes

| Status | Meaning | Common Causes |
|--------|---------|---------------|
| **200 OK** | Success | Pembayaran berhasil diproses |
| **400 Bad Request** | Invalid input | Payment ID tidak valid, file terlalu besar, field missing |
| **401 Unauthorized** | No/Invalid token | Token expired, token tidak dikirim |
| **403 Forbidden** | Permission denied | Non-admin mencoba confirm, cross-user access |
| **404 Not Found** | Resource not found | Payment ID tidak ada |
| **422 Unprocessable Entity** | Validation failed | Status invalid, file format tidak support |
| **500 Internal Server Error** | Server error | Cloudinary upload failed, DB error |
| **503 Service Unavailable** | Service down | Backend sedang maintenance |

### Error Response Format

```json
{
  "error": "Error title/code",
  "details": "Error explanation",
  "timestamp": "2026-04-15T10:30:00Z",
  "path": "/api/payments/1/confirm"
}
```

### C# Error Handling

```csharp
private async Task HandlePaymentOperation(string operation, int paymentId)
{
    try
    {
        HttpResponseMessage response = null;

        switch (operation)
        {
            case "confirm":
                response = await client.PutAsync(
                    $"https://rahmatzaw.elarisnoir.my.id/api/payments/{paymentId}/confirm",
                    null
                );
                break;
            case "reject":
                response = await client.PutAsync(
                    $"https://rahmatzaw.elarisnoir.my.id/api/payments/{paymentId}/reject",
                    null
                );
                break;
        }

        if (response.IsSuccessStatusCode)
        {
            MessageBox.Show("Operasi berhasil", "Success", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadPayments();
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.BadRequest: // 400
                    MessageBox.Show($"Input tidak valid: {content}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case System.Net.HttpStatusCode.Unauthorized: // 401
                    MessageBox.Show("Session expired. Please login again", "Auth Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    // Navigate to login
                    break;

                case System.Net.HttpStatusCode.Forbidden: // 403
                    MessageBox.Show("Anda tidak punya akses untuk operasi ini", "Permission Denied",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    break;

                case System.Net.HttpStatusCode.NotFound: // 404
                    MessageBox.Show("Pembayaran tidak ditemukan", "Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case System.Net.HttpStatusCode.InternalServerError: // 500
                    MessageBox.Show("Server error. Please try again later", "Server Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                default:
                    MessageBox.Show($"Unexpected error: {response.StatusCode}\n{content}", 
                        "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
    catch (HttpRequestException ex)
    {
        MessageBox.Show($"Network error: {ex.Message}", "Connection Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    catch (TaskCanceledException)
    {
        MessageBox.Show("Request timeout. Please try again", "Timeout",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Unexpected error: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

---

## 8️⃣ AUTHENTICATION & PERMISSION

### Token Requirements

| Endpoint | Token Required | Role Restriction |
|----------|----------------|------------------|
| GET /api/payments | ✅ Yes | Admin only |
| PUT /payments/{id}/confirm | ✅ Yes | Admin only |
| PUT /payments/{id}/reject | ✅ Yes | Admin only |
| POST /payments/confirm-cash/{id} | ✅ Yes | Admin only |
| POST /payments/{id}/proof | ✅ Yes | Penyewa (own) or Admin |
| DELETE /payments/{id} | ❌ N/A | Not available |

### Token Format

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Checking User Role (C#)

```csharp
public class AuthHelper
{
    private string _token;
    private string _userRole; // stored from login response

    public bool IsAdmin()
    {
        return _userRole == "admin";
    }

    public bool CanConfirmPayment()
    {
        return IsAdmin();
    }

    public bool CanUploadProof(int paymentId, int currentUserId)
    {
        // Admin can upload for anyone
        if (IsAdmin()) return true;

        // Tenant can only upload for own payment
        var payment = /* fetch from cache */;
        return payment.Pemesanan.PenyewaId == currentUserId;
    }
}
```

### Role-Based UI Visibility

```csharp
private void ShowPaymentActions()
{
    bool isAdmin = Session.UserRole == "admin";

    btnConfirm.Visible = isAdmin;
    btnReject.Visible = isAdmin;
    btnUploadProof.Visible = !isAdmin; // Penyewa
    btnExport.Visible = isAdmin;

    if (isAdmin)
    {
        // Admin view: full list
        LoadAllPayments();
    }
    else
    {
        // Tenant view: own payments only
        LoadMyPayments(Session.UserId);
    }
}
```

---

## 9️⃣ INTEGRATION NOTES - C# BEST PRACTICES

### 1. Caching Strategy

**Problem:** Fetching seluruh payment list setiap kali buka form akan menambah server load.

**Solution:**
```csharp
private List<Pembayaran> cachedPayments;
private DateTime lastFetchTime = DateTime.MinValue;
private TimeSpan cacheExpiry = TimeSpan.FromMinutes(5);

private async Task<List<Pembayaran>> GetPayments(bool forceRefresh = false)
{
    if (!forceRefresh && 
        cachedPayments != null && 
        (DateTime.Now - lastFetchTime) < cacheExpiry)
    {
        return cachedPayments;
    }

    try
    {
        var response = await client.GetAsync("https://rahmatzaw.elarisnoir.my.id/api/payments");
        var json = await response.Content.ReadAsStringAsync();
        cachedPayments = JsonConvert.DeserializeObject<List<Pembayaran>>(json);
        lastFetchTime = DateTime.Now;

        return cachedPayments;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error fetching payments: {ex.Message}");
        return new List<Pembayaran>();
    }
}
```

### 2. Background Loading (Prevent UI Freeze)

```csharp
private async void LoadPaymentsAsync()
{
    try
    {
        // Show loading indicator
        progressBar.Visible = true;
        progressBar.Style = ProgressBarStyle.Marquee;

        // Fetch in background thread
        var payments = await Task.Run(() => GetPayments(forceRefresh: true));

        // Update UI on main thread
        this.Invoke((Action)(() =>
        {
            bindingSource.DataSource = payments;
            dataGridView1.DataSource = bindingSource;
            progressBar.Visible = false;
        }));
    }
    catch (Exception ex)
    {
        this.Invoke((Action)(() =>
        {
            MessageBox.Show($"Error: {ex.Message}");
            progressBar.Visible = false;
        }));
    }
}
```

### 3. Refresh Button dengan Confirmation

```csharp
private async void btnRefresh_Click(object sender, EventArgs e)
{
    if (MessageBox.Show("Refresh data from server?", "Confirm",
        MessageBoxButtons.YesNo) == DialogResult.Yes)
    {
        await LoadPaymentsAsync(); // Clear cache & reload
    }
}
```

### 4. Handle Large Datasets Efficiently

```csharp
// DON'T: Bind all 10000 records to DataGridView
// dataGridView.DataSource = allPayments;

// DO: Implement paging
private int pageSize = 100;
private int currentPage = 1;

private void DisplayPage(List<Pembayaran> allData)
{
    var pageData = allData
        .Skip((currentPage - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    bindingSource.DataSource = pageData;
    lblPageInfo.Text = $"Page {currentPage} of {Math.Ceiling((double)allData.Count / pageSize)}";
}
```

### 5. WebSocket (Optional - Real-time Updates)

Backend support WebSocket tapi **TIDAK emit payment status changes** secara real-time.

**Recommendation:** Gunakan polling dengan interval 30-60 detik untuk payment status.

```csharp
private async void InitializeAutoRefresh()
{
    while (true)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            await LoadPaymentsAsync();
        }
        catch { /* silently fail */ }
    }
}

// Call di Form_Load
private void PembayaranForm_Load(object sender, EventArgs e)
{
    _ = InitializeAutoRefresh(); // Fire and forget
}
```

### 6. Validation Before Submit

```csharp
private bool ValidatePaymentAction(Pembayaran payment, string action)
{
    if (payment == null)
    {
        MessageBox.Show("Please select a payment");
        return false;
    }

    if (action == "confirm" && payment.BuktiTransfer == null)
    {
        MessageBox.Show("Bukti transfer must be uploaded first");
        return false;
    }

    if (payment.StatusPembayaran == "Confirmed" && action == "confirm")
    {
        MessageBox.Show("Payment is already confirmed");
        return false;
    }

    return true;
}
```

---

## 🔟 QUICK START - IMPLEMENTATION CHECKLIST

### Phase 1: Data Fetching
- [ ] Create Pembayaran DTO models
- [ ] Implement GET /api/payments client method
- [ ] Implement caching logic
- [ ] Test API connectivity

### Phase 2: Display
- [ ] Create DataGridView columns (ID, Penyewa, Kamar, Amount, Status, Date)
- [ ] Bind payment data to grid
- [ ] Implement status color-coding (Pending=Yellow, Confirmed=Green, Rejected=Red)
- [ ] Add filter ComboBox (Status)

### Phase 3: Admin Actions
- [ ] Implement Confirm button (PUT /confirm)
- [ ] Implement Reject button (PUT /reject)
- [ ] Add confirmation dialog before action
- [ ] Handle API responses & errors

### Phase 4: Proof Management
- [ ] Add PictureBox for displaying proof image
- [ ] Implement image URL loading
- [ ] Add upload button with file dialog
- [ ] Implement POST /proof upload

### Phase 5: Reporting
- [ ] Calculate & display totals (Confirmed, Pending, Rejected)
- [ ] Implement monthly revenue calculation
- [ ] Add export to CSV
- [ ] Add date range filter

### Phase 6: Polish
- [ ] Add loading indicators
- [ ] Implement auto-refresh timer
- [ ] Add error handling & logging
- [ ] User testing & bug fixes

---

## 📚 References

- Backend Source: `payment_handler.go`, `payment_service.go`, `payment_repository.go`
- Routes: `routes.go` (lines payment-related)
- Models: `models.go` (Payment struct definition)
- Frontend: `PembayaranForm.cs`, `PaymentResponse.cs`

---

**Last Updated:** April 2026
**Backend Version:** Go 1.21+
**Frontend:** .NET Framework 4.8 WinForms
**Status:** Active Integration

