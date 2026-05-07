# 🧪 DIAGNOSTIC TESTING GUIDE - TotalRevenueHtmlLabel12

## 📋 Pre-Testing Checklist

- [ ] Visual Studio sudah terbuka
- [ ] Project `Kost_SiguraGura` bisa di-build (compile successful)
- [ ] Backend API server RUNNING (atau minimal accessible)
- [ ] Network connection OK

---

## 🔴 TEST 1: Output Window Logging

**Tujuan**: Lihat apakah data sedang di-load dari API

**Cara**:
1. Buka **Debug > Windows > Output** (atau Ctrl+Alt+O)
2. Di dropdown pane, pilih "Debug"
3. Clear existing logs (Ctrl+A, Delete)
4. Run aplikasi
5. Buka halaman **Report** admin
6. Lihat Output window

**Apa yang dicari**:

### ✓ JIKA BERHASIL (NORMAL CASE):
```
[LoadDashboardStatsAsync] Starting dashboard stats fetch...
[LoadDashboardStatsAsync] Raw JSON response length: 1245 chars
[LoadDashboardStatsAsync] First 500 chars:
{"total_revenue":25000000,"pending_revenue":5000000,"pending_payments":3,...}
[LoadDashboardStatsAsync] ✓ SUCCESS! Stats loaded:
  - Total Revenue: 25000000
  - Pending Revenue: 5000000
  - Active Tenants: 12
  - Occupied Rooms: 8/12
  - Type Breakdown items: 2
  - Demographics items: 4
[LoadPaymentsAsync] *** RESULT: Successfully loaded 28 payments from API ***
[UpdateReportStatCards] ========== START UPDATE STAT CARDS ==========
[UpdateReportStatCards] Using pre-calculated stats from API
  - Total Revenue: 25000000
```

**Indikator**: ✓✓✓ LABEL SHOULD SHOW "Rp 25.000.000"

---

### ✗ JIKA GAGAL - CASE 1: API ERROR

```
[LoadDashboardStatsAsync] Starting dashboard stats fetch...
[LoadDashboardStatsAsync] API returned error status: 500

[LoadPaymentsAsync] *** RESULT: Successfully loaded 28 payments from API ***
[UpdateReportStatCards] ========== START UPDATE STAT CARDS ==========
[UpdateReportStatCards] ⚠️ WARNING: Dashboard stats not loaded yet!
[UpdateReportStatCardsFromManualData] Fallback being used...
  - Total Revenue (Confirmed): Rp 16.000.000
```

**Indikator**: Fallback digunakan, LABEL SHOWS "Rp 16.000.000"

**Diagnosis**: Backend `/api/dashboard/stats` return error 500

---

### ✗ JIKA GAGAL - CASE 2: JSON PARSING ERROR

```
[LoadDashboardStatsAsync] Starting dashboard stats fetch...
[LoadDashboardStatsAsync] Raw JSON response length: 1245 chars
[LoadDashboardStatsAsync] First 500 chars:
{UNEXPECTED FORMAT HERE...}
[LoadDashboardStatsAsync] ✗ Deserialization error: 
  Cannot deserialize the current JSON object into type 'DashboardStats'
[LoadDashboardStatsAsync] Exception: JsonSerializationException

[UpdateReportStatCards] ⚠️ WARNING: Dashboard stats not loaded yet!
```

**Indikator**: JSON format tidak match model

**Diagnosis**: Backend response format berbeda dengan expected

---

### ✗ JIKA GAGAL - CASE 3: NETWORK ERROR

```
[LoadDashboardStatsAsync] Starting dashboard stats fetch...
[LoadDashboardStatsAsync] Error loading stats: 
  The underlying connection was closed: 
  The connection was closed unexpectedly.
```

**Indikator**: Network/connection error

**Diagnosis**: Backend tidak bisa di-reach, atau timeout

---

## 🔵 TEST 2: Desktop Debug Files

**Tujuan**: Inspect raw data yang di-load

**Cara**:
1. Buka Report page (seperti Test 1)
2. Check **Desktop** folder untuk:
   - `Report_Debug_Log.txt`
   - `API_Response_Debug.json`

### File: Report_Debug_Log.txt

Isi file akan terlihat seperti:

```
========== REPORT DEBUG LOG ==========
Timestamp: 2024-01-15 14:30:45.123

========== LOADED DATA ==========
Total Payments: 28
Total Rooms: 12

========== PAYMENT BREAKDOWN ==========
Confirmed: 22
Pending: 6
Rejected: 0

========== REVENUE BREAKDOWN ==========
Confirmed Total: Rp 16.000.000 (Expected: 16000000) ✓
Pending Total: Rp 5.000.000 (Expected: 5000000) ✓
ALL Total: Rp 21.000.000 (Expected: 21000000) ✓

========== FIRST 10 PAYMENTS ==========
ID: 1 | Amount: 1000000 | Status: Confirmed | Date: 2024-01-10
ID: 2 | Amount: 500000 | Status: Pending | Date: 2024-01-12
...
```

**Apa yang dicari**:
- Payments loaded? (> 0)
- Revenues match expected? (16M confirmed, 5M pending)
- First payment parse correctly?

---

### File: API_Response_Debug.json

Raw JSON dari `/api/payments` endpoint:

```json
[
  {
	"id": 1,
	"jumlah_bayar": 1000000,
	"status_pembayaran": "Confirmed",
	"tanggal_bayar": "2024-01-10T10:30:00"
  },
  {
	"id": 2,
	"jumlah_bayar": 500000,
	"status_pembayaran": "Pending",
	"tanggal_bayar": "2024-01-12T15:45:00"
  }
  ...
]
```

**Apa yang dicari**:
- JSON structure valid? (proper brackets, quotes)
- Field names match? (`jumlah_bayar`, `status_pembayaran`)
- Data ada? (not empty array)

---

## 🟢 TEST 3: Click Diagnostic

**Tujuan**: Trigger real-time diagnostic dialog

**Cara**:
1. Buka Report page
2. **Klik langsung pada label "Rp 0"** (TotalRevenueHtmlLabel12)
3. Dialog akan pop-up dengan diagnostic info

**Dialog akan show 3 tabs:**

### Tab 1: "Raw Response"
Menampilkan raw JSON dari `/api/payments`

**Apa yang dicari**:
- JSON ada? (or "Error: Cannot fetch"?)
- Format valid?
- Data ada? (not empty)

---

### Tab 2: "Parsing Info"

Menampilkan parsing diagnostics:

```
========== PARSING DIAGNOSTIC ==========

Response Length: 2458 characters
Response Status: 200

--- Attempt 1: Direct Array ---
✓ SUCCESS: Parsed 28 items
  First item: ID=1, Amount=1000000, Status=Confirmed

--- Attempt 2: Wrapper Object ---
Root token type: Array
Root is array - direct array parsing should have worked
```

**Apa yang dicari**:
- ✓ SUCCESS atau ✗ FAILED?
- Item count? (28 items?)
- First item correct? (Amount, Status)

---

### Tab 3: "Current Data"

Menampilkan data yang sudah loaded:

```
========== CURRENT IN-MEMORY DATA ==========

Total Payments Loaded: 28
Total Rooms Loaded: 12

--- Status Breakdown ---
Confirmed: 22
Pending: 6
Rejected: 0

--- Revenue Breakdown ---
Confirmed Total: Rp 16.000.000
Pending Total: Rp 5.000.000
ALL Total: Rp 21.000.000

--- Expected from Database ---
Confirmed Total: Rp 16.000.000 (22 items) ✓
Pending Total: Rp 5.000.000 (6 items) ✓
ALL Total: Rp 21.000.000 (31 items) ✓

Current data MATCHES expected values ✓✓✓
```

**Apa yang dicari**:
- Total Payments > 0?
- Status breakdown reasonable?
- Revenue matches expected?
- Any MISMATCH detected?

---

## 🟡 TEST 4: Postman API Test

**Tujuan**: Test backend endpoint secara direct

**Tools**: Postman, Insomnia, atau curl

### Test Endpoint 1: Dashboard Stats

**Request**:
```
GET https://rahmatzaw.elarisnoir.my.id/api/dashboard/stats
```

**Cara di Postman**:
1. Buka Postman
2. Create NEW request
3. Method: **GET**
4. URL: `https://rahmatzaw.elarisnoir.my.id/api/dashboard/stats`
5. Click **Send**

**Apa yang dicari**:

#### ✓ JIKA BERHASIL (Status 200):
```json
{
  "total_revenue": 25000000,
  "pending_revenue": 5000000,
  "occupied_rooms": 8,
  "available_rooms": 4,
  "pending_payments": 3,
  "active_tenants": 12,
  "type_breakdown": [
	{
	  "type": "Premium",
	  "revenue": 15000000,
	  "occupied": 5,
	  "count": 6
	}
  ],
  "monthly_trend": [...],
  "demographics": [...]
}
```

**Diagnosis**: ✓ Endpoint working, format OK

---

#### ✗ JIKA GAGAL (Status 500, 404, dll):
```json
{
  "error": "Internal Server Error",
  "message": "Database connection failed"
}
```

**Diagnosis**: ✗ Backend error, tidak bisa access

---

### Test Endpoint 2: Payments

**Request**:
```
GET https://rahmatzaw.elarisnoir.my.id/api/payments
```

**Cara di Postman**: (sama seperti di atas)

**Apa yang dicari**:

#### ✓ JIKA BERHASIL (Status 200):
```json
[
  {
	"id": 1,
	"jumlah_bayar": 1000000,
	"status_pembayaran": "Confirmed",
	"tanggal_bayar": "2024-01-10T10:30:00",
	"pemesanan_id": 1
  },
  {
	"id": 2,
	"jumlah_bayar": 500000,
	"status_pembayaran": "Pending",
	"tanggal_bayar": "2024-01-12T15:45:00",
	"pemesanan_id": 2
  }
  ...
]
```

**Count**: Lihat berapa items returned

---

## 🔶 TEST 5: Check Model Mapping

**Tujuan**: Pastikan JSON field match dengan C# model

**Cara di Visual Studio**:

### Check DashboardStats Model

Buka file: `Kost_SiguraGura/DashboardStats.cs`

Lihat field definitions:

```csharp
[JsonProperty("total_revenue")]
public decimal TotalRevenue { get; set; }

[JsonProperty("pending_revenue")]
public decimal PendingRevenue { get; set; }

[JsonProperty("pending_payments")]
public int PendingPayments { get; set; }
```

**Important**: 
- `[JsonProperty("total_revenue")]` ← Backend JSON field harus SAMA persis!
- `public decimal TotalRevenue` ← C# property name

**Apa yang dicari**:
- Apakah backend JSON field names match `JsonProperty` attributes?
- Apakah type cocok? (decimal, int, string?)

---

### Check Pembayaran Model

Buka file: `Kost_SiguraGura/PaymentResponse.cs`

```csharp
[JsonProperty("jumlah_bayar")]
public decimal JumlahBayar { get; set; }

[JsonProperty("status_pembayaran")]
public string StatusPembayaran { get; set; }

[JsonProperty("tanggal_bayar")]
public DateTime? TanggalBayar { get; set; }
```

**Apa yang dicari**:
- Backend `/api/payments` return field dengan nama: `jumlah_bayar`, `status_pembayaran`, `tanggal_bayar`?

---

## 🟣 TEST 6: Network Capture

**Tujuan**: Lihat exact request/response antara frontend dan backend

**Tools**: Fiddler, Charles, atau Chrome DevTools (untuk web)

**Cara**:
1. Buka Fiddler
2. Configure WinForms app to use Fiddler proxy
3. Buka Report page di aplikasi
4. Lihat network traffic di Fiddler

**Apa yang dicari**:

### Request 1: GET /api/dashboard/stats
```
GET https://rahmatzaw.elarisnoir.my.id/api/dashboard/stats
Status: 200 (atau error?)
Response Body: JSON structure OK?
```

### Request 2: GET /api/payments
```
GET https://rahmatzaw.elarisnoir.my.id/api/payments
Status: 200 (atau error?)
Response Body: Array of payments?
```

---

## 📝 TEST EXECUTION CHECKLIST

Jalankan tests dalam urutan ini:

1. **TEST 1: Output Window**
   - [ ] Buka Output window
   - [ ] Open Report page
   - [ ] Screenshot output log
   - [ ] Cari error messages

2. **TEST 2: Desktop Debug Files**
   - [ ] Check `Report_Debug_Log.txt`
   - [ ] Check `API_Response_Debug.json`
   - [ ] Verify counts match

3. **TEST 3: Click Diagnostic**
   - [ ] Click label TotalRevenueHtmlLabel12
   - [ ] Screenshot diagnostic dialog
   - [ ] Check all 3 tabs

4. **TEST 4: Postman**
   - [ ] Test `/api/dashboard/stats` endpoint
   - [ ] Test `/api/payments` endpoint
   - [ ] Verify status codes
   - [ ] Verify JSON format

5. **TEST 5: Model Mapping**
   - [ ] Open DashboardStats.cs
   - [ ] Open PaymentResponse.cs
   - [ ] Verify JsonProperty attributes

6. **TEST 6: (Optional) Network Capture**
   - [ ] Setup Fiddler
   - [ ] Capture requests
   - [ ] Verify request/response

---

## 📊 TEST RESULTS SUMMARY TEMPLATE

Untuk lapor hasil testing, copy-paste dan fill template ini:

```
=== TEST RESULTS ===
Date: [DATE]
Time: [TIME]

TEST 1 - Output Window:
✓ / ✗ Stats loaded? [YES/NO]
Status: [COPY LOG HERE]

TEST 2 - Debug Files:
✓ / ✗ Files created? [YES/NO]
Payments count: [NUMBER]
Revenue (Confirmed): [AMOUNT]

TEST 3 - Click Diagnostic:
✓ / ✗ Dialog opened? [YES/NO]
Raw Response tab: [OK/ERROR]
Parsing Info tab: [OK/ERROR]
Current Data tab: [OK/ERROR]

TEST 4 - Postman:
✓ / ✗ /api/dashboard/stats: [STATUS CODE]
✓ / ✗ /api/payments: [STATUS CODE]

TEST 5 - Model Mapping:
✓ / ✗ DashboardStats fields: [MATCH/MISMATCH]
✓ / ✗ Pembayaran fields: [MATCH/MISMATCH]

FINAL DIAGNOSIS:
[WHAT'S THE PROBLEM?]
[SUGGESTED FIX?]
```

---

## 🚨 CRITICAL TESTS IF LABEL STILL BLANK

Jika setelah semua test label masih blank, jalankan ini:

### Critical Test A: Force Manual Calculation
Temporary modify `Report.cs`:

```csharp
private void UpdateReportStatCards()
{
	// TEMPORARY: Force manual calculation
	System.Diagnostics.Debug.WriteLine("[CRITICAL TEST] Forcing manual calculation...");
	UpdateReportStatCardsFromManualData();
	return;  // Skip stats check
}
```

Recompile dan test. 
- Jika label sekarang OK → Problem adalah stats
- Jika label masih blank → Problem adalah allPayments

---

### Critical Test B: Hard-code Value
Temporary modify `Report.cs`:

```csharp
private void UpdateReportStatCards()
{
	// TEMPORARY: Hard-code test value
	TotalRevenueHtmlLabel12.Text = "Rp 999.999.999";  // Test value
}
```

Recompile dan test.
- Jika label shows "Rp 999.999.999" → UI update OK
- Jika label still blank → UI control issue

---

## ✅ NEXT STEPS

Setelah complete testing:

1. **Collect all test results**
2. **Send to backend team** jika ada API error
3. **Share output logs** untuk diagnosis lebih detail
4. **Implement fix** based on test findings

