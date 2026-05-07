# Pertanyaan Spesifikasi API - Halaman Report Admin

## Konteks
Kami sedang mengintegrasikan halaman Report (Laporan) di aplikasi admin. Saat ini ada issue dengan data yang tidak ter-load dengan benar, khususnya pada stat cards. Kami perlu memahami dengan detail bagaimana API endpoints seharusnya mengirim data.

---

## 1. STAT CARDS (Cards di bagian atas)

### Card 1: Total Revenue (Rp 100.000)
**Pertanyaan:**
- Endpoint mana yang digunakan untuk fetch total revenue?
- Apakah data bisa di-filter berdasarkan date range (Last 6 Months, Last 30 Days, 1 Year, All Time)?
- Apakah hanya menghitung status `Confirmed` atau juga `Pending`?
- Format response apa yang diharapkan? (contoh: total langsung atau array payments yang perlu di-sum?)
- Apakah ada max data limit atau pagination?

**Contoh response yang kami harapkan:**
```json
// Option 1: Direct calculation
{
  "totalRevenue": 16000000,
  "currency": "IDR",
  "period": "6_months"
}

// Option 2: Array of payments (kami sum di frontend)
[
  {
	"id": 1,
	"jumlah_bayar": 2000000,
	"status_pembayaran": "Confirmed",
	"tanggal_bayar": "2024-01-15"
  },
  ...
]
```

---

### Card 2: Pending Revenue (Rp 0)
**Pertanyaan:**
- Apakah menggunakan endpoint yang sama seperti Total Revenue?
- Bagaimana filter untuk status `Pending` saja?
- Apakah pending revenue dihitung dari `JumlahBayar` atau ada field terpisah seperti `JumlahDP`?

---

### Card 3: Average Rate (Rp 1,4 jt)
**Pertanyaan:**
- Endpoint mana yang digunakan untuk fetch average room price?
- Apakah ini rata-rata dari semua kamar atau hanya kamar yang terisi?
- Format response:
```json
// Option 1: Direct calculation
{
  "averagePrice": 1400000,
  "totalRooms": 2,
  "availableRooms": 1
}

// Option 2: Array of rooms
[
  {
	"id": 1,
	"tipe_kamar": "Premium",
	"price": 1500000
  },
  {
	"id": 2,
	"tipe_kamar": "Standard",
	"price": 1300000
  }
]
```

---

### Card 4: Occupancy Rate (50%)
**Pertanyaan:**
- Endpoint mana untuk fetch room status?
- Apa saja status yang mungkin? (`Available`, `Full`, `Maintenance`, atau dalam bahasa Indonesia?)
- Format response:
```json
// Expected response
{
  "totalRooms": 2,
  "occupiedRooms": 1,
  "availableRooms": 1,
  "maintenanceRooms": 0,
  "occupancyPercentage": 50
}

// Atau array?
[
  {
	"id": 1,
	"tipe_kamar": "Premium",
	"status": "Full",
	"occupiedBeds": 2,
	"totalBeds": 2
  }
]
```

---

## 2. GRAFIK - REVENUE BY ROOM TYPE
**Pertanyaan:**
- Endpoint: `/api/...?` untuk mendapat revenue breakdown per room type?
- Apakah data sudah aggregated di backend atau array yang perlu di-group frontend?
- Period filter supported?

**Expected response:**
```json
[
  {
	"type": "Premium",
	"revenue": 8000000,
	"occupiedRooms": 2,
	"totalRooms": 2
  },
  {
	"type": "Standard",
	"revenue": 8000000,
	"occupiedRooms": 0,
	"totalRooms": 1
  }
]
```

---

## 3. GRAFIK - TENANT DEMOGRAPHICS (Age Distribution)
**Pertanyaan:**
- Endpoint mana untuk fetch tenant data?
- Apakah ada field `age` atau `birthDate` di tenant model?
- Bagaimana age grouping? (18-25, 26-35, 36-45, dst?)
- Format response apa?

**Expected response:**
```json
[
  {
	"ageGroup": "18-25",
	"tenantCount": 5
  },
  {
	"ageGroup": "26-35",
	"tenantCount": 3
  }
]
```

---

## 4. PERUBAHAN FILTER DATE RANGE

### Pertanyaan:
- Ketika user ganti filter dari "Last 6 Months" ke "Last 30 Days", apakah harus:
  - **A)** Fetch semua data baru dari API?
  - **B)** Hanya filter data yang sudah loaded di client-side?

- Apakah API support query parameter untuk date range?
```
GET /api/payments?startDate=2024-01-01&endDate=2024-12-31
GET /api/payments?period=last_6_months
GET /api/payments?dateRange=6_months
```

---

## 5. DATA CONSISTENCY & CACHING

### Pertanyaan:
- Berapa lama data seharusnya di-cache di client?
- Apakah ada webhook atau real-time update mechanism?
- Apakah data berubah frequently? (setiap berapa detik/menit?)
- Apakah safe untuk cache dan update setiap 15 detik seperti yang ada sekarang?

---

## 6. ERROR HANDLING & PAGINATION

### Pertanyaan:
- Jika ada error di API (timeout, 500, dll), apakah ada fallback data?
- Untuk large dataset (misal 1000+ payments), apakah ada pagination?
```
GET /api/payments?page=1&limit=100
GET /api/payments?offset=0&limit=100
```

---

## 7. CURRENT ISSUE - DEBUG INFO

Saat ini kami mengalami issue:
- **Problem**: Total Revenue label stuck di "Rp 100.000" padahal seharusnya menampilkan total revenue sebenarnya
- **Debug Output**: Hanya 1 payment ter-parse dari API padahal seharusnya 31 items
- **Expected**: Confirmed: Rp 16.000.000 (22 payments), Pending: Rp 5.000.000 (6 payments)

**Pertanyaan untuk Backend:**
- Apakah format JSON response dari `/api/payments` berubah?
- Apakah struktur response sesuai dengan `Pembayaran` model?
```csharp
public class Pembayaran
{
	public int id
	public int pemesanan_id
	public decimal jumlah_bayar
	public DateTime tanggal_bayar
	public string status_pembayaran // "Confirmed", "Pending", "Rejected"
	public string metode_pembayaran
	// ... fields lainnya
}
```

- Apakah ada case sensitivity issue? (API mengirim "confirmed" vs frontend expect "Confirmed"?)

---

## 8. BANDWIDTH & PERFORMANCE

### Pertanyaan:
- Berapa typical response size untuk:
  - `/api/payments` → full dataset?
  - `/api/kamar` → full room list?
  - `/api/penyewa` → full tenant list?

- Apakah disarankan untuk exclude certain fields untuk mengurangi payload?
```
GET /api/payments?fields=id,jumlah_bayar,status_pembayaran,tanggal_bayar
```

---

## Template Prompt untuk Backend Team

```
Halo Backend Team,

Kami sedang membuat integrasi Report page di admin dashboard. Ada beberapa pertanyaan tentang API responses:

1. Untuk Total Revenue card, kami call `/api/payments` dan sum berdasarkan status "Confirmed". 
   - Apakah ini approach yang benar?
   - Atau ada endpoint khusus `/api/reports/revenue`?

2. Response format dari `/api/payments` - apakah selalu langsung array atau ada wrapper object?
   Contoh yang kami expect:
   ```
   [
	 { "id": 1, "jumlah_bayar": 2000000, "status_pembayaran": "Confirmed", ... }
   ]
   ```
   Tapi kadang kami terima format berbeda. Bisa di-standardkan?

3. Untuk filter date range, apakah endpoint support query params seperti:
   ```
   GET /api/payments?startDate=2024-01-01&endDate=2024-06-30
   ```

4. Berapa rekomendasi interval untuk auto-sync/refresh data? (kami sedang pakai 15 detik)

5. Untuk performance, apakah ada saran tentang pagination atau field filtering?

Terima kasih!
```

---

## Checklist untuk Validasi Response

- [ ] Response format konsisten (array vs object wrapper)
- [ ] Status values case-sensitive consistency (Confirmed vs confirmed)
- [ ] DateTime format konsisten
- [ ] Decimal precision untuk currency values
- [ ] Null handling untuk optional fields
- [ ] Empty array vs null untuk empty responses
- [ ] HTTP status codes dokumentasi
- [ ] Rate limiting info
