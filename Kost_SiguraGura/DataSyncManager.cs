using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kost_SiguraGura
{
    /// <summary>
    /// Manages automatic data synchronization with API
    /// Handles periodic data refresh for reports and other views
    /// </summary>
    public class DataSyncManager : IDisposable
    {
        // Events for data refresh notifications
        public event EventHandler<DataRefreshEventArgs> PaymentsRefreshed;
        public event EventHandler<DataRefreshEventArgs> RoomsRefreshed;
        public event EventHandler<SyncStatusChangedEventArgs> SyncStatusChanged;
        public event EventHandler<SyncErrorEventArgs> SyncErrorOccurred;

        private Timer _refreshTimer;
        private bool _isRefreshing = false;
        private bool _isDisposed = false;
        private DateTime _lastPaymentRefresh = DateTime.MinValue;
        private DateTime _lastRoomRefresh = DateTime.MinValue;

        // Configuration
        private int _refreshIntervalMs = SyncConfiguration.DefaultRefreshIntervalMs;
        private bool _isAutoSyncEnabled = SyncConfiguration.EnableAutoSync;

        public bool IsAutoSyncEnabled
        {
            get { return _isAutoSyncEnabled; }
            set
            {
                _isAutoSyncEnabled = value;
                if (value)
                    StartAutoSync();
                else
                    StopAutoSync();
            }
        }

        public int RefreshIntervalMs
        {
            get { return _refreshIntervalMs; }
            set
            {
                if (value >= 5000) // Minimum 5 seconds
                {
                    _refreshIntervalMs = value;
                    if (_refreshTimer != null)
                        _refreshTimer.Interval = value;
                }
            }
        }

        public DataSyncManager(int refreshIntervalMs = -1)
        {
            // Use configuration value if not specified
            if (refreshIntervalMs <= 0)
                refreshIntervalMs = SyncConfiguration.RefreshIntervalMs;

            _refreshIntervalMs = Math.Max(SyncConfiguration.MinRefreshIntervalMs, refreshIntervalMs);
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _refreshTimer = new Timer();
            _refreshTimer.Interval = _refreshIntervalMs;
            _refreshTimer.Tick += RefreshTimer_Tick;
        }

        public void StartAutoSync()
        {
            if (_isDisposed) return;
            if (_refreshTimer != null && !_refreshTimer.Enabled)
            {
                _refreshTimer.Start();
                OnSyncStatusChanged(SyncStatus.Started, "Auto-sync dimulai");
            }
        }

        public void StopAutoSync()
        {
            if (_refreshTimer != null && _refreshTimer.Enabled)
            {
                _refreshTimer.Stop();
                OnSyncStatusChanged(SyncStatus.Stopped, "Auto-sync dihentikan");
            }
        }

        public async Task ManualRefresh()
        {
            if (_isRefreshing) return;

            _isRefreshing = true;
            OnSyncStatusChanged(SyncStatus.Syncing, "Mengambil data dari API...");

            try
            {
                // Parallel refresh untuk payments dan rooms
                await Task.WhenAll(
                    RefreshPaymentsAsync(),
                    RefreshRoomsAsync()
                );

                OnSyncStatusChanged(SyncStatus.Completed, "Sinkronisasi berhasil");
            }
            catch (Exception ex)
            {
                OnSyncErrorOccurred(ex, "Gagal sinkronisasi data");
                OnSyncStatusChanged(SyncStatus.Error, $"Error: {ex.Message}");
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (_isRefreshing) return;

            // Refresh with minimal UI blocking (async on background)
            Task.Run(async () => await ManualRefresh());
        }

        private async Task RefreshPaymentsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[DataSyncManager] Starting RefreshPaymentsAsync...");
                var response = await ApiClient.Client.GetAsync("https://rahmatzaw.elarisnoir.my.id/api/payments");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var payments = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Pembayaran>>(content);

                    System.Diagnostics.Debug.WriteLine($"[DataSyncManager] API returned {payments?.Count ?? 0} payments");

                    // Log payment details for debugging
                    if (payments != null && payments.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DataSyncManager] First payment: ID={payments[0].Id}, Status={payments[0].StatusPembayaran}, Amount={payments[0].JumlahBayar}, Date={payments[0].TanggalBayar}");

                        // Log status distribution
                        var statusGroups = payments.GroupBy(p => p.StatusPembayaran);
                        foreach (var group in statusGroups)
                        {
                            var count = group.Count();
                            var total = group.Sum(p => p.JumlahBayar);
                            System.Diagnostics.Debug.WriteLine($"[DataSyncManager] Status '{group.Key}': {count} payments, Total: {total}");
                        }
                    }

                    _lastPaymentRefresh = DateTime.Now;
                    OnPaymentsRefreshed(new DataRefreshEventArgs 
                    { 
                        Timestamp = _lastPaymentRefresh,
                        DataCount = payments?.Count ?? 0,
                        Success = true 
                    });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[DataSyncManager] API error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DataSyncManager] Error in RefreshPaymentsAsync: {ex.Message}");
                throw new Exception($"Error refreshing payments: {ex.Message}", ex);
            }
        }

        private async Task RefreshRoomsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[DataSyncManager] Starting RefreshRoomsAsync...");
                var response = await ApiClient.Client.GetAsync("https://rahmatzaw.elarisnoir.my.id/api/kamar");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var rooms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Kamar>>(content);

                    System.Diagnostics.Debug.WriteLine($"[DataSyncManager] API returned {rooms?.Count ?? 0} rooms");

                    if (rooms != null && rooms.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"[DataSyncManager] First room: ROOM={rooms[0].ROOM}, STATUS={rooms[0].STATUS}, PRICE={rooms[0].PRICE}");
                    }

                    _lastRoomRefresh = DateTime.Now;
                    OnRoomsRefreshed(new DataRefreshEventArgs 
                    { 
                        Timestamp = _lastRoomRefresh,
                        DataCount = rooms?.Count ?? 0,
                        Success = true 
                    });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[DataSyncManager] API error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DataSyncManager] Error in RefreshRoomsAsync: {ex.Message}");
                throw new Exception($"Error refreshing rooms: {ex.Message}", ex);
            }
        }

        public DateTime GetLastPaymentRefreshTime() => _lastPaymentRefresh;
        public DateTime GetLastRoomRefreshTime() => _lastRoomRefresh;

        protected virtual void OnPaymentsRefreshed(DataRefreshEventArgs e)
        {
            PaymentsRefreshed?.Invoke(this, e);
        }

        protected virtual void OnRoomsRefreshed(DataRefreshEventArgs e)
        {
            RoomsRefreshed?.Invoke(this, e);
        }

        protected virtual void OnSyncStatusChanged(SyncStatus status, string message)
        {
            SyncStatusChanged?.Invoke(this, new SyncStatusChangedEventArgs { Status = status, Message = message });
        }

        protected virtual void OnSyncErrorOccurred(Exception exception, string message)
        {
            SyncErrorOccurred?.Invoke(this, new SyncErrorEventArgs { Exception = exception, Message = message });
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            StopAutoSync();
            _refreshTimer?.Dispose();
            _isDisposed = true;
        }
    }

    // Event argument classes
    public class DataRefreshEventArgs : EventArgs
    {
        public DateTime Timestamp { get; set; }
        public int DataCount { get; set; }
        public bool Success { get; set; }
    }

    public class SyncStatusChangedEventArgs : EventArgs
    {
        public SyncStatus Status { get; set; }
        public string Message { get; set; }
    }

    public class SyncErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }

    public enum SyncStatus
    {
        Started,
        Syncing,
        Completed,
        Stopped,
        Error
    }
}
