# Real-Time Data Synchronization System for Reports

## Overview
Sistem automatic data synchronization telah diimplementasikan untuk halaman Reports agar semua data (Pembayaran & Kamar) ter-sync real-time dengan API tanpa memerlukan user interaction.

## Fitur

### 1. **Automatic Background Sync**
- Data payments dan rooms secara otomatis di-fetch dari API setiap 15 detik (default)
- Tidak memerlukan manual refresh button
- Berjalan di background thread untuk tidak memblokir UI

### 2. **Event-Driven Updates**
- Ketika data diperbarui dari API, semua charts dan stat cards otomatis update
- Real-time reflection dari perubahan transaksi, status pembayaran, status kamar
- UI update dilakukan dengan proper thread marshalling (Invoke)

### 3. **Smart Refresh Management**
- Menggunakan timer-based polling untuk efisiensi
- Concurrent request handling untuk menghindari race conditions
- Parallel refresh untuk payments dan rooms data

### 4. **Configurable Sync Intervals**
- Default interval: **15 seconds**
- Minimum interval: **5 seconds** (untuk update yang lebih cepat)
- Maximum interval: **5 minutes** (untuk load yang lebih rendah)

## Configuration

### Mengubah Refresh Interval

#### **Opsi 1: Environment Variables**
```powershell
# Set untuk 30 detik
[Environment]::SetEnvironmentVariable("SYNC_REFRESH_INTERVAL", "30000", "User")

# Disable auto-sync
[Environment]::SetEnvironmentVariable("ENABLE_AUTO_SYNC", "false", "User")
```

#### **Opsi 2: Programmatic (di code)**
```csharp
// Mengubah interval ke 10 detik
SyncConfiguration.RefreshIntervalMs = 10000;

// Disable auto-sync
SyncConfiguration.EnableAutoSync = false;
```

#### **Opsi 3: Default (tidak perlu config)**
- Gunakan default 15 seconds refresh interval
- Auto-sync langsung aktif saat Report page dibuka

## Technical Architecture

### Classes

#### **DataSyncManager**
Menangani semua operasi synchronization:
- Manages timer-based refresh cycle
- Fetches data dari API (payments & rooms)
- Raises events ketika data diperbarui
- Thread-safe operations

**Events:**
```csharp
// Fired ketika payments berhasil di-refresh
event EventHandler<DataRefreshEventArgs> PaymentsRefreshed;

// Fired ketika rooms berhasil di-refresh
event EventHandler<DataRefreshEventArgs> RoomsRefreshed;

// Status perubahan (Started, Syncing, Completed, Stopped, Error)
event EventHandler<SyncStatusChangedEventArgs> SyncStatusChanged;

// Error handling
event EventHandler<SyncErrorEventArgs> SyncErrorOccurred;
```

#### **SyncConfiguration**
Static configuration class untuk manage sync settings:
- `RefreshIntervalMs` - Custom interval configuration
- `EnableAutoSync` - Global sync enable/disable
- `Initialize()` - Load config dari environment variables
- `ResetToDefaults()` - Reset ke default values

### Flow Diagram

```
Report_Load()
    ↓
Initialize SyncConfiguration (load from environment)
    ↓
LoadAllReportData() (initial load)
    ↓
InitializeAutoSync()
    ├─ Create DataSyncManager instance
    ├─ Subscribe to PaymentsRefreshed event
    ├─ Subscribe to RoomsRefreshed event
    └─ Start Timer (every 15 seconds)
    ↓
Timer Tick (every 15 seconds)
    ├─ RefreshPaymentsAsync()
    │   ├─ Fetch from API: /api/payments
    │   └─ Raise PaymentsRefreshed event
    │
    └─ RefreshRoomsAsync()
        ├─ Fetch from API: /api/kamar
        └─ Raise RoomsRefreshed event
    ↓
Event Handlers
    ├─ DataSyncManager_PaymentsRefreshed
    │   ├─ LoadPaymentsAsync()
    │   ├─ UpdateReportStatCards()
    │   └─ SetupCharts()
    │
    └─ DataSyncManager_RoomsRefreshed
        ├─ LoadRoomsAsync()
        ├─ UpdateReportStatCards()
        └─ SetupCharts()
```

## Behavior

### Data Update Scenarios

#### **Scenario 1: Pembayaran Baru Ditambahkan**
1. API menerima POST request untuk pembayaran baru
2. Setelah 15 detik (default), DataSyncManager fetch updated payments list
3. Report page otomatis update:
   - Total Revenue meningkat
   - Transaction table menampilkan entry baru
   - Charts di-redraw dengan data terbaru

#### **Scenario 2: Status Pembayaran Berubah (Pending → Confirmed)**
1. Backend update status pembayaran di database
2. Setelah 15 detik, DataSyncManager fetch payments terbaru
3. Report page otomatis update:
   - Pending Payments count berkurang
   - Total Revenue meningkat
   - Revenue Breakdown ter-update

#### **Scenario 3: Status Kamar Berubah (Available → Full)**
1. Backend update room status
2. Setelah 15 detik, DataSyncManager fetch rooms terbaru
3. Report page otomatis update:
   - Occupancy Rate berubah
   - Room Demographics chart di-redraw
   - Revenue Breakdown ter-update

### Error Handling

- Jika API request gagal, error di-log (tidak crash)
- Sync terus berjalan meski ada error
- Status error di-report via `SyncErrorOccurred` event
- Retry automatic pada next cycle

### Resource Cleanup

- Saat Report page ditutup/unloaded:
  - Timer di-stop
  - Events di-unsubscribe
  - DataSyncManager di-dispose
  - Semua resources di-release

## Performance Considerations

### Default 15-Second Interval
- **Pros:**
  - Good balance antara freshness dan server load
  - Smooth user experience
  - Minimal resource consumption

- **Cons:**
  - Data bisa delayed max 15 detik
  - Jika butuh real-time, tingkatkan ke 5 detik

### Network Impact
```
Requests per minute: 60 / 15 = 4 requests/min
Data transferred per request: ~2-5 KB per endpoint
Total: ~10-20 KB/min (very minimal)
```

## Monitoring & Debugging

### Debug Output
Semua sync activities di-log ke Debug Output:
```
[Sync Status] Started: Auto-sync dimulai
[Sync Status] Syncing: Mengambil data dari API...
[Sync Status] Completed: Sinkronisasi berhasil
[Sync Error] Gagal sinkronisasi data: Connection timeout
```

### Check Sync Status
```csharp
// Get last refresh times
DateTime lastPaymentRefresh = _dataSyncManager.GetLastPaymentRefreshTime();
DateTime lastRoomRefresh = _dataSyncManager.GetLastRoomRefreshTime();
```

## Future Enhancements

1. **Adaptive Refresh Rate** - Increase/decrease interval berdasarkan activity
2. **WebSocket Support** - Real-time push notifications dari server
3. **Offline Sync** - Queue changes dan sync ketika online
4. **Sync Status UI** - Indicator di UI untuk show sync status
5. **Differential Sync** - Only fetch/update data yang berubah
6. **Export During Sync** - Block export saat sync ongoing

## Troubleshooting

### Data tidak ter-update
1. Check internet connection
2. Verify API endpoints accessible
3. Check Debug output untuk error messages
4. Verify SyncConfiguration.EnableAutoSync = true

### CPU/Memory usage tinggi
1. Increase RefreshIntervalMs (contoh: 30000 untuk 30 detik)
2. Check apakah ada UI bottleneck di update methods
3. Monitor chart rendering performance

### API rate limit errors
1. Increase RefreshIntervalMs untuk reduce request frequency
2. Implementasikan backend rate limiting yang lebih intelligent
3. Consider WebSocket atau Server-Sent Events

---

**Created:** November 2024
**Last Updated:** November 2024
