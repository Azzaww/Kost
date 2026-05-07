# 🌳 TROUBLESHOOTING DECISION TREE

## START HERE: Diagnose TotalRevenueHtmlLabel12 Issue

```
┌─────────────────────────────────────────────────────────────┐
│ PROBLEM: TotalRevenueHtmlLabel12 tidak menampilkan data     │
│ dari API - tetap blank atau menunjukkan "Rp 0"             │
└────────────┬────────────────────────────────────────────────┘
			 │
			 ├─→ 🔵 Q1: Report page bisa dibuka?
			 │    ├─→ TIDAK → Error di UI load → See Branch A
			 │    └─→ YA → Continue ke Q2
			 │
			 ├─→ 🟢 Q2: Buka Output window, apa yang muncul?
			 │    ├─→ "[LoadDashboardStatsAsync] API returned error status: 500"
			 │    │   → See Branch B1: Backend Error 500
			 │    │
			 │    ├─→ "[LoadDashboardStatsAsync] ✗ Deserialization error:"
			 │    │   → See Branch B2: JSON Parsing Error
			 │    │
			 │    ├─→ "[LoadDashboardStatsAsync] Error loading stats:"
			 │    │   → See Branch B3: Network Error
			 │    │
			 │    ├─→ "[UpdateReportStatCards] Using pre-calculated stats from API"
			 │    │   BUT label still blank
			 │    │   → See Branch C1: UI Update Failed
			 │    │
			 │    ├─→ "[UpdateReportStatCards] ⚠️ WARNING: Dashboard stats not loaded yet!"
			 │    │   → See Branch D: Stats Null, Check Fallback
			 │    │
			 │    └─→ NO RELEVANT LOGS (atau tidak ada output)
			 │        → See Branch E: Logging Disabled
```

---

## 🔴 BRANCH A: Report Page Error

```
Q: Report page error saat dibuka?

A1: Error UI initialization
────────────────────────────
Symptom: Exception when opening Report page
Action:
  1. Check Output window untuk error message
  2. Look for: "System.NullReferenceException" atau "Designer error"
  3. Klik di error line untuk lihat code
Fix:
  - Ensure all UI controls initialized in Designer
  - Check if TotalRevenueHtmlLabel12 ada di Designer

A2: Designer error
────────────────────────────
Symptom: "Report.Designer.cs" error at load
Action:
  1. Open Report.Designer.cs
  2. Search for "TotalRevenueHtmlLabel12"
  3. Check if control initialization ada
Fix:
  - Regenerate Designer code if corrupted
  - Ensure no syntax error in Designer file

A3: Report control not registered
────────────────────────────
Symptom: "Type 'Report' not found" atau similar
Action:
  1. Check if Report class defined in Report.cs
  2. Check namespace match
Fix:
  - Ensure Report class inherit UserControl
  - namespace Kost_SiguraGura
```

---

## 🔴 BRANCH B1: Backend Error 500

```
Log Message:
[LoadDashboardStatsAsync] API returned error status: 500

Problem: Backend `/api/dashboard/stats` return server error

Diagnosis Steps:
┌───────────────────────────────────────────────────┐
│ 1. Test endpoint directly dengan Postman         │
│    GET https://...../api/dashboard/stats         │
│                                                    │
│    Status 500? → Backend crash                    │
│    Status 404? → Endpoint tidak ada               │
│    Status 403? → Authentication fail              │
│    Status 200 but parse error? → JSON format     │
└───────────────────────────────────────────────────┘

Action Items:
☐ Check backend logs untuk error message
☐ Verify database connection di backend
☐ Verify endpoint implementation exist
☐ Check if stats calculation logic error

Questions untuk Backend Team:
1. Apakah `/api/dashboard/stats` implemented?
2. Ada error di server logs?
3. Database bisa di-access?
4. Apakah endpoint require authentication?

Temporary Fix (sampai backend diperbaiki):
- Disable stats loading: Comment out LoadDashboardStatsAsync()
- Rely on manual fallback: /api/payments data
- Ensure /api/payments endpoint working

Status: 🟠 WAITING FOR BACKEND TEAM FIX
```

---

## 🔴 BRANCH B2: JSON Parsing Error

```
Log Message:
[LoadDashboardStatsAsync] ✗ Deserialization error:
  Cannot deserialize the current JSON object into type 'DashboardStats'

Problem: Backend JSON response format tidak match model

Possible Causes:
┌──────────────────────────────────────────────────┐
│ CAUSE A: Field names tidak match                 │
│ Expected: "total_revenue"                        │
│ Received: "totalRevenue" (camelCase?)            │
│                                                   │
│ CAUSE B: Data type mismatch                      │
│ Expected: decimal                                │
│ Received: string atau null                       │
│                                                   │
│ CAUSE C: Extra/missing fields                    │
│ Backend sent different structure                 │
│                                                   │
│ CAUSE D: Null values                             │
│ Some required fields null                        │
└──────────────────────────────────────────────────┘

Debug Steps:
1. Buka Desktop file: "API_Response_Debug.json"
2. Copy JSON content
3. Compare dengan DashboardStats model:

   // MODEL EXPECTS:
   {
	 "total_revenue": 25000000,         // decimal
	 "pending_revenue": 5000000,        // decimal
	 "pending_payments": 3,             // int
	 "active_tenants": 12,              // int
	 "occupied_rooms": 8,               // int
	 "available_rooms": 4,              // int
	 "type_breakdown": [...],           // array
	 "demographics": [...],             // array
	 "monthly_trend": [...]             // array
   }

4. Lihat jika backend JSON match

Fix:
┌──────────────────────────────────────────────────┐
│ Option 1: Update DashboardStats model            │
│ - Modify JsonProperty attributes                 │
│ - Adjust field names sesuai backend             │
│ - Update field types jika perlu                  │
│                                                   │
│ Option 2: Update backend JSON response          │
│ - Backend align response dengan expected model  │
│ - Ensure all fields present                      │
│ - Ensure proper data types                       │
└──────────────────────────────────────────────────┘

Status: 🟠 NEED BACKEND RESPONSE SAMPLE
```

---

## 🔴 BRANCH B3: Network Error

```
Log Message:
[LoadDashboardStatsAsync] Error loading stats:
  The underlying connection was closed:
  The connection was closed unexpectedly.

Problem: Network connectivity issue

Possible Causes:
┌──────────────────────────────────────────────────┐
│ CAUSE A: Server down/offline                     │
│ - Backend service tidak running                  │
│ - Network unreachable                            │
│                                                   │
│ CAUSE B: Request timeout                         │
│ - Backend slow response                          │
│ - Network lag                                    │
│                                                   │
│ CAUSE C: SSL/Certificate error                   │
│ - Self-signed certificate                        │
│ - Certificate validation fail                    │
│                                                   │
│ CAUSE D: Proxy/Firewall block                    │
│ - Corporate firewall blocking endpoint           │
│ - Proxy configuration issue                      │
└──────────────────────────────────────────────────┘

Debug Steps:
1. Test basic connectivity:
   ping rahmatzaw.elarisnoir.my.id

2. Test endpoint di browser:
   https://rahmatzaw.elarisnoir.my.id/api/dashboard/stats

3. Check if can reach at all:
   - Ping OK? → Server online
   - Ping fail? → Network issue

4. Test dari command line (Powershell):
   $url = "https://rahmatzaw.elarisnoir.my.id/api/dashboard/stats"
   $result = Invoke-WebRequest -Uri $url
   $result.StatusCode

Fix:
☐ Check if server running
☐ Check network connectivity
☐ Check firewall rules
☐ Check if need VPN

Status: 🟠 NETWORK/INFRASTRUCTURE ISSUE
```

---

## 🟠 BRANCH C1: UI Update Failed

```
Log Shows:
[UpdateReportStatCards] Using pre-calculated stats from API
  - Total Revenue: 25000000
BUT: Label still shows "Rp 0" or blank

Problem: Value loaded correctly but UI not updated

Possible Causes:
┌──────────────────────────────────────────────────┐
│ CAUSE A: Control reference wrong                 │
│ - TotalRevenueHtmlLabel12 reference mismatch     │
│ - Updating wrong control                         │
│                                                   │
│ CAUSE B: InvokeRequired handling                 │
│ - Thread safety issue                            │
│ - Invoke() not executing properly                │
│                                                   │
│ CAUSE C: Control disabled/hidden                 │
│ - Control Enabled = false                        │
│ - Control Visible = false                        │
│                                                   │
│ CAUSE D: Thread timing issue                     │
│ - UI update before control initialized           │
│ - Race condition                                 │
└──────────────────────────────────────────────────┘

Debug Steps:
1. Verify control name di Designer:
   Open Report.Designer.cs
   Search: "TotalRevenueHtmlLabel12"

2. Verify control visibility:
   Check if Control.Visible = true
   Check if Control.Enabled = true

3. Check if text actually set:
   Add logging:
   ```csharp
   System.Diagnostics.Debug.WriteLine(
	 $"[DEBUG] Setting TotalRevenueHtmlLabel12.Text = {formattedValue}"
   );
   TotalRevenueHtmlLabel12.Text = formattedValue;
   System.Diagnostics.Debug.WriteLine(
	 $"[DEBUG] After set, label shows: {TotalRevenueHtmlLabel12.Text}"
   );
   ```

Fix Options:
┌──────────────────────────────────────────────────┐
│ Option A: Verify control initialization         │
│ - Ensure TotalRevenueHtmlLabel12 initialized    │
│ - Check in Designer: this.TotalRevenueHtml...   │
│                                                   │
│ Option B: Force UI refresh                      │
│ Add after setting text:                         │
│   TotalRevenueHtmlLabel12.Refresh();            │
│   TotalRevenueHtmlLabel12.Invalidate();         │
│                                                   │
│ Option C: Check thread safety                   │
│ Ensure Invoke() used if needed:                 │
│   if (InvokeRequired)                           │
│     Invoke(new Action(() => { ... }));          │
└──────────────────────────────────────────────────┘

Status: 🟠 FRONTEND UI BINDING ISSUE
```

---

## 🟠 BRANCH D: Stats Null, Check Fallback

```
Log Shows:
[UpdateReportStatCards] ⚠️ WARNING: Dashboard stats not loaded yet!

Problem: currentStats == null, using fallback path

Possible Cases:
┌──────────────────────────────────────────────────┐
│ CASE D1: Fallback working (allPayments loaded)  │
│ Expected: Label shows fallback value            │
│ Example: "Rp 16.000.000" (from manual calc)    │
│ → GOOD! Fallback is protecting you              │
│                                                   │
│ CASE D2: Fallback empty (allPayments == 0)     │
│ Expected: Label blank/unchanged                 │
│ → PROBLEM! No data at all                        │
└──────────────────────────────────────────────────┘

Diagnosis for D1 (Fallback Working):
───────────────────────────────────────
Check Debug Output for:
[LoadPaymentsAsync] *** RESULT: Successfully loaded X payments from API ***

If X > 0:
  ✓ GOOD! /api/payments working
  ✓ Fallback calculation working

Action:
  - Why is stats not loading? → See Branch B1, B2, B3
  - Backend stats endpoint error?

Status: 🟡 ACCEPTABLE FOR NOW
		(Fallback protecting the system)

Diagnosis for D2 (Fallback Empty):
──────────────────────────────────
Check Debug Output for:
[LoadPaymentsAsync] *** RESULT: Successfully loaded 0 payments from API ***

If count = 0:
  ✗ /api/payments also not working!
  ✗ Both data sources failed!

Action:
  1. Check if /api/payments endpoint exist
  2. Test endpoint dengan Postman
  3. Verify JSON format

Status: 🔴 CRITICAL! Both data sources failed
		Need to fix /api/payments endpoint

Debug Tree for D2:
Q: Does /api/payments return data?
  A: Test with Postman
	 GET https://...../api/payments

  If Status 200 but empty array []:
	→ Endpoint OK but no data in database
	→ Check backend database

  If Status 500:
	→ Endpoint error → See Branch B1

  If Status 404:
	→ Endpoint not exist → See Branch B2

  If Timeout:
	→ Network/server issue → See Branch B3
```

---

## 🟠 BRANCH E: Logging Not Appearing

```
Problem: Output window empty, no logs showing

Possible Causes:
┌──────────────────────────────────────────────────┐
│ CAUSE A: Debug output not captured               │
│ - Output window not in Debug pane                │
│ - Console output goes elsewhere                  │
│                                                   │
│ CAUSE B: Code not executing                      │
│ - Report_Load not called                         │
│ - Report control not loaded                      │
│                                                   │
│ CAUSE C: Logs happened too fast                  │
│ - Output scrolled off screen                     │
│ - Logs already cleared                           │
└──────────────────────────────────────────────────┘

Fix Steps:
1. ✓ Ensure Output window OPEN:
   Debug > Windows > Output

2. ✓ Ensure DEBUG mode (not Release):
   Build dropdown: Select "Debug"

3. ✓ Ensure correct output pane:
   Output window dropdown: Select "Debug"

4. ✓ Clear existing logs:
   Ctrl+A in Output window, Delete

5. ✓ Re-open Report page:
   This should trigger new logs

6. ✓ Add breakpoint for verification:
   Set breakpoint in Report_Load()
   If breakpoint hit → Code executing
   If not → Report not loading

Alternative: Save logs to file
────────────────────────────────
Modify Report.cs LoadAllReportData():

System.Diagnostics.Debug.WriteLine("...");

// ADD THIS:
string logPath = Path.Combine(
  Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
  "Report_Execution_Log.txt"
);
File.AppendAllText(logPath, 
  "[TEST] LoadAllReportData called\n");

Then check Desktop for "Report_Execution_Log.txt"

Status: 🟡 DIAGNOSTIC TOOL ISSUE
		(Not actual code problem)
```

---

## 🎯 DECISION MATRIX

Quick reference untuk determine problem:

```
┌─────────────────────┬──────────────────────┬────────────────┐
│ Symptom             │ Likely Cause         │ Refer To       │
├─────────────────────┼──────────────────────┼────────────────┤
│ Report page error   │ UI initialization    │ Branch A       │
│                     │                      │                │
│ API error 500       │ Backend crash        │ Branch B1      │
│                     │                      │                │
│ Parse error         │ JSON format          │ Branch B2      │
│                     │                      │                │
│ Network error       │ Connection fail      │ Branch B3      │
│                     │                      │                │
│ Stats OK, label     │ UI update fail       │ Branch C1      │
│ still blank         │                      │                │
│                     │                      │                │
│ Stats null, no      │ Fallback empty       │ Branch D       │
│ fallback data       │                      │                │
│                     │                      │                │
│ No output logs      │ Logging disabled     │ Branch E       │
│                     │                      │                │
│ Label shows         │ Fallback working     │ Branch D1      │
│ "Rp 16.000.000"     │ (acceptable)         │                │
│                     │                      │                │
│ Label shows "Rp 0"  │ Both sources fail    │ Branch D2      │
│ or blank            │ (critical)           │                │
└─────────────────────┴──────────────────────┴────────────────┘
```

---

## ✅ RECOMMENDED ACTION PLAN

```
STEP 1: Immediate Debug
─────────────────────────
☐ Open Output window
☐ Open Report page
☐ Copy ALL output logs
☐ Check Desktop debug files

STEP 2: Identify Problem Type
─────────────────────────────
☐ Is it Backend Error? (B1)
☐ Is it JSON Format? (B2)
☐ Is it Network? (B3)
☐ Is it UI Update? (C1)
☐ Is it Both APIs Down? (D2)

STEP 3: Narrow Down
─────────────────────────
☐ Test /api/dashboard/stats with Postman
☐ Test /api/payments with Postman
☐ Check if either endpoint working
☐ Compare JSON vs model

STEP 4: Report Findings
─────────────────────────
To Backend Team:
☐ Which endpoints working/failing
☐ Response status codes
☐ Error messages received
☐ JSON format issues

To DevOps Team:
☐ If network/infrastructure issue
☐ If server connectivity problem

STEP 5: Implement Fix
─────────────────────────
Depending on problem:
☐ Fix backend endpoint
☐ Fix frontend model mapping
☐ Fix network/firewall
☐ Update code based on findings

STEP 6: Verify
─────────────────────────
☐ Retest with actual data
☐ Check Output logs again
☐ Verify label shows correct value
☐ Test with different scenarios
```

---

## 📞 ASKING FOR HELP

Jika butuh ask di forum/chat, include:

```
PROBLEM REPORT TEMPLATE
═══════════════════════

[ISSUE TITLE]
TotalRevenueHtmlLabel12 tidak menampilkan data dari API

[DESCRIPTION]
Label tetap blank/showing placeholder value meskipun 
data loading code sudah ada.

[ENVIRONMENT]
- OS: Windows 10/11
- VS: Visual Studio 2026
- .NET: Framework 4.8
- Backend: [URL]

[ERROR MESSAGE]
[Paste exact error dari Output window atau dialog]

[DEBUG INFO]
- From Output window:
[Paste relevant logs]

- From Desktop files:
[Paste content dari Report_Debug_Log.txt]

[WHAT YOU'VE TRIED]
- Checked Output window: [YES/NO]
- Tested endpoint dengan Postman: [YES/NO]
- Verified model mapping: [YES/NO]

[QUESTION]
[Specific question untuk the maintainer/backend team]

[ATTACHMENT]
- Screenshot Output window
- Desktop debug files
- Postman response sample
```

---

**Happy debugging! 🎯**

Jika follow tree ini step-by-step, Anda pasti ketemu akar masalahnya!

