# Backend API Integration - Debugging & Validation Checklist

## Quick Reference - Apa yang Perlu Di-Tanyakan

### Ringkas untuk Chat/Email ke Backend:

```
Hi Backend Team,

Untuk halaman Admin Report, kami perlu klarifikasi:

1. **Total Revenue Card** - Kami sum payments dengan status "Confirmed". 
   - Benar tidak approach ini?
   - Ada field khusus atau harus sum dari array?

2. **Date Filter** - Apakah /api/payments support query params:
   - ?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD
   - Atau harus fetch semua lalu filter di frontend?

3. **Response Format** - Apakah selalu:
   ```json
   [
	 {"id": 1, "jumlah_bayar": 2000000, "status_pembayaran": "Confirmed", "tanggal_bayar": "2024-01-15"}
   ]
   ```
   Atau ada wrapper? Contoh actual response?

4. **Status Values** - Apakah "Confirmed", "Pending", "Rejected"?
   - Case sensitive?
   - Atau ada status lain?

5. **Refresh Interval** - Kami auto-sync setiap 15 detik. Terlalu sering?

Thanks!
```

---

## Debugging Steps - Untuk Di-Coba Sendiri

### Step 1: Test Raw API Response
```powershell
# Di PowerShell, test endpoint langsung
$response = Invoke-WebRequest -Uri "https://rahmatzaw.elarisnoir.my.id/api/payments" -Method GET
$response.Content | ConvertFrom-Json | ConvertTo-Json | Out-File "C:\Users\Arkan\Desktop\API_Response.json"
```

### Step 2: Check Response Structure
Buka file `API_Response.json` dan lihat:
- Apakah langsung array `[...]` atau object dengan property `{"data": [...]}`?
- Berapa items di response?
- Apakah semua `status_pembayaran` values?

### Step 3: Validate JSON Parsing
```csharp
// Di code, uncomment ini untuk debug:
System.Diagnostics.Debug.WriteLine($"[DEBUG] Raw JSON:\n{content}");
System.Diagnostics.Debug.WriteLine($"[DEBUG] Parsed count: {allPayments.Count}");
System.Diagnostics.Debug.WriteLine($"[DEBUG] First item: {JsonConvert.SerializeObject(allPayments.FirstOrDefault())}");
```

### Step 4: Check Case Sensitivity
```csharp
// Add di SafeDeserializePayments setelah parsing:
var distinctStatuses = allPayments.Select(p => p.StatusPembayaran).Distinct();
System.Diagnostics.Debug.WriteLine($"[DEBUG] Distinct statuses: {string.Join(", ", distinctStatuses)}");
```

### Step 5: Verify Date Range Filter
```csharp
// Di UpdateReportStatCards, log date range:
System.Diagnostics.Debug.WriteLine($"[DEBUG] Filter range: {selectedStartDate:yyyy-MM-dd} to {selectedEndDate:yyyy-MM-dd}");
System.Diagnostics.Debug.WriteLine($"[DEBUG] Payments in range: {filteredPayments.Count}");

// Check if any payment terlewat karena date null
var nullDatePayments = allPayments.Where(p => !p.TanggalBayar.HasValue).Count();
System.Diagnostics.Debug.WriteLine($"[DEBUG] Payments dengan null date: {nullDatePayments}");
```

---

## Data Validation Matrix

| Card | Field | Expected Type | Expected Range | Validation |
|------|-------|---|---|---|
| Total Revenue | jumlah_bayar | decimal | > 0 | Sum confirmed payments |
| Pending Revenue | jumlah_bayar | decimal | >= 0 | Sum pending payments |
| Avg Rate | PRICE | decimal | > 0 | Average of all room prices |
| Occupancy | STATUS | string | "Full"/"Available"/"Maintenance" | Count "Full" / Total |

---

## Common Issues & Solutions

### Issue 1: Hanya 1 Payment Di-Parse
**Kemungkinan Penyebab:**
- Response format tidak sesuai ekspektasi
- JSON structure adalah `{data: [...]}` tapi kode expect array langsung
- API response berubah format

**Solusi:**
```csharp
// Check actual response format
var json = await response.Content.ReadAsStringAsync();
if (json.StartsWith("{") && !json.StartsWith("["))
{
	// Ada wrapper object, extract dulu
	var wrapped = JsonConvert.DeserializeObject<dynamic>(json);
	json = JsonConvert.SerializeObject(wrapped.data); // atau wrapped.payments
}
```

---

### Issue 2: Status Tidak Match ("Confirmed" vs "confirmed")
**Solusi:**
```csharp
// Di Pembayaran model, normalize status:
private string _statusPembayaran;

[JsonProperty("status_pembayaran")]
public string StatusPembayaran 
{
	get => _statusPembayaran;
	set => _statusPembayaran = value?.Trim(); // normalize
}

// Di filter:
var confirmedPayments = filteredPayments.Where(p => 
	string.Equals(p.StatusPembayaran, "Confirmed", StringComparison.OrdinalIgnoreCase)
).ToList();
```

---

### Issue 3: Pagination/Large Dataset
**Jika lebih dari 100 items:**
```csharp
// Modify LoadPaymentsAsync untuk handle pagination
private async Task LoadPaymentsAsync()
{
	allPayments.Clear();
	int page = 1;
	bool hasMore = true;

	while (hasMore)
	{
		var response = await ApiClient.Client.GetAsync(
			$"https://rahmatzaw.elarisnoir.my.id/api/payments?page={page}&limit=100"
		);

		var pageData = JsonConvert.DeserializeObject<List<Pembayaran>>(
			await response.Content.ReadAsStringAsync()
		);

		if (pageData.Count < 100) hasMore = false;
		allPayments.AddRange(pageData);
		page++;
	}
}
```

---

### Issue 4: Null Date Values Causing Filter to Fail
**Solusi:**
```csharp
var filteredPayments = allPayments
	.Where(p => 
	{
		// Handle null dates - treat as all-time
		var paymentDate = p.TanggalBayar?.Date ?? DateTime.MinValue;
		return paymentDate >= selectedStartDate.Date && 
			   paymentDate <= selectedEndDate.Date;
	})
	.ToList();
```

---

## Questions to Ask Backend - Prioritized by Urgency

### 🔴 CRITICAL (Ask First)
1. Apakah format response `/api/payments` berubah?
2. Bisa dikirim sample response terbaru?
3. Berapa total payments yang seharusnya di-return?

### 🟡 IMPORTANT (Ask Second)
4. Apakah endpoint support query params untuk date range?
5. Apa status values yang valid?
6. Apakah ada pagination?

### 🟢 NICE-TO-HAVE (Ask Later)
7. Rekomendasi refresh interval?
8. Ada webhook untuk real-time updates?
9. Bisa exclude certain fields untuk performance?

---

## Sample Email Template

```
Subject: [URGENT] Clarification needed: Admin Report API Integration

Hi [Backend Team Leader],

We're implementing the Admin Report page and need urgent clarification on API responses.

**Issue**: 
- We're fetching from `/api/payments` but only 1 payment is being parsed instead of 31 expected
- Expected: 22 Confirmed + 6 Pending = 28 total (some may have other statuses)
- Actual: Only 1 payment loaded

**Questions**:

1. Can you provide the exact JSON response from `/api/payments`? 
   - Is it a direct array `[...]` or wrapped in an object?
   - What's the typical payload size for the full dataset?

2. We're looking for payments with `status_pembayaran` = "Confirmed" and "Pending"
   - Are these the correct values?
   - Are they case-sensitive?
   - Any other statuses we should handle?

3. For the date range filter, does the endpoint support:
   ```
   GET /api/payments?startDate=2024-01-01&endDate=2024-06-30
   ```
   Or should we fetch all and filter client-side?

4. Is there pagination? If so, what's the default/max limit?

5. What's the recommended refresh interval for auto-sync?

This is blocking our Admin feature completion. 🙏

Thanks,
[Your Name]
```

---

## After Getting Backend Response - Validation Checklist

- [ ] Save actual JSON response to `docs/API_Response_Sample.json`
- [ ] Update `Pembayaran` model if structure changed
- [ ] Test parsing with actual response
- [ ] Verify all expected fields are present
- [ ] Check date format (ISO 8601 vs other?)
- [ ] Verify decimal precision
- [ ] Test date range filtering
- [ ] Test pagination if applicable
- [ ] Document any edge cases
- [ ] Update this file with actual API specs
