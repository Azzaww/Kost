using System;

namespace Kost_SiguraGura
{
    /// <summary>
    /// Configuration settings for data synchronization
    /// Allows customization of auto-sync behavior
    /// </summary>
    public static class SyncConfiguration
    {
        /// <summary>
        /// Default refresh interval in milliseconds (15 seconds)
        /// </summary>
        public const int DefaultRefreshIntervalMs = 15000;

        /// <summary>
        /// Minimum allowed refresh interval (5 seconds)
        /// </summary>
        public const int MinRefreshIntervalMs = 5000;

        /// <summary>
        /// Maximum allowed refresh interval (5 minutes)
        /// </summary>
        public const int MaxRefreshIntervalMs = 300000;

        /// <summary>
        /// Enable/disable automatic synchronization globally
        /// </summary>
        public static bool EnableAutoSync = true;

        /// <summary>
        /// Current refresh interval in milliseconds
        /// </summary>
        private static int _refreshIntervalMs = DefaultRefreshIntervalMs;

        public static int RefreshIntervalMs
        {
            get { return _refreshIntervalMs; }
            set
            {
                // Clamp value between min and max
                _refreshIntervalMs = Math.Max(MinRefreshIntervalMs, Math.Min(MaxRefreshIntervalMs, value));
            }
        }

        /// <summary>
        /// Initialize configuration from environment or use defaults
        /// </summary>
        public static void Initialize()
        {
            try
            {
                // Try to read from environment variables
                string intervalEnv = Environment.GetEnvironmentVariable("SYNC_REFRESH_INTERVAL");
                if (!string.IsNullOrEmpty(intervalEnv) && int.TryParse(intervalEnv, out int configuredInterval))
                {
                    RefreshIntervalMs = configuredInterval;
                }

                string autoSyncEnv = Environment.GetEnvironmentVariable("ENABLE_AUTO_SYNC");
                if (!string.IsNullOrEmpty(autoSyncEnv) && bool.TryParse(autoSyncEnv, out bool configuredAutoSync))
                {
                    EnableAutoSync = configuredAutoSync;
                }

                System.Diagnostics.Debug.WriteLine($"[SyncConfig] Initialized - Interval: {_refreshIntervalMs}ms, AutoSync: {EnableAutoSync}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SyncConfig] Error initializing: {ex.Message}. Using defaults.");
            }
        }

        /// <summary>
        /// Reset configuration to defaults
        /// </summary>
        public static void ResetToDefaults()
        {
            _refreshIntervalMs = DefaultRefreshIntervalMs;
            EnableAutoSync = true;
        }
    }
}
