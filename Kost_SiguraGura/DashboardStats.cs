using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Kost_SiguraGura
{
    /// <summary>
    /// Dashboard statistics response from /api/dashboard/stats endpoint
    /// Contains pre-calculated metrics for the admin dashboard
    /// </summary>
    public class DashboardStats
    {
        [JsonProperty("total_revenue")]
        public decimal TotalRevenue { get; set; }

        [JsonProperty("active_tenants")]
        public int ActiveTenants { get; set; }

        [JsonProperty("available_rooms")]
        public int AvailableRooms { get; set; }

        [JsonProperty("occupied_rooms")]
        public int OccupiedRooms { get; set; }

        [JsonProperty("pending_payments")]
        public int PendingPayments { get; set; }

        [JsonProperty("pending_revenue")]
        public decimal PendingRevenue { get; set; }

        [JsonProperty("rejected_payments")]
        public int RejectedPayments { get; set; }

        [JsonProperty("potential_revenue")]
        public decimal PotentialRevenue { get; set; }

        [JsonProperty("monthly_trend")]
        public List<MonthlyTrend> MonthlyTrend { get; set; } = new List<MonthlyTrend>();

        [JsonProperty("type_breakdown")]
        public List<TypeBreakdown> TypeBreakdown { get; set; } = new List<TypeBreakdown>();

        [JsonProperty("demographics")]
        public List<Demographics> Demographics { get; set; } = new List<Demographics>();

        [JsonProperty("recent_checkouts")]
        public List<RecentCheckout> RecentCheckouts { get; set; } = new List<RecentCheckout>();
    }

    /// <summary>
    /// Monthly trend data for chart display
    /// </summary>
    public class MonthlyTrend
    {
        [JsonProperty("month")]
        public string Month { get; set; }

        [JsonProperty("revenue")]
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// Revenue breakdown per room type
    /// </summary>
    public class TypeBreakdown
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("revenue")]
        public decimal Revenue { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("occupied")]
        public int Occupied { get; set; }
    }

    /// <summary>
    /// Tenant demographics (age groups)
    /// </summary>
    public class Demographics
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }
    }

    /// <summary>
    /// Recent checkout information
    /// </summary>
    public class RecentCheckout
    {
        [JsonProperty("room_name")]
        public string RoomName { get; set; }

        [JsonProperty("tenant_name")]
        public string TenantName { get; set; }

        [JsonProperty("checkout_date")]
        public DateTime CheckoutDate { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
