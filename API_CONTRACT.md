# Backend API Contract - Dashboard Stats Endpoint

## Expected Endpoint Response

The frontend now expects the following JSON structure from `GET /api/dashboard/stats`:

```json
{
  "total_revenue": 25000000,
  "pending_revenue": 5000000,
  "potential_revenue": 30000000,
  "pending_payments": 3,
  "active_tenants": 12,
  "occupied_rooms": 8,
  "available_rooms": 4,
  "total_rooms": 12,
  "type_breakdown": [
	{
	  "type": "Premium",
	  "revenue": 15000000,
	  "occupied": 5,
	  "count": 6
	},
	{
	  "type": "Standard",
	  "revenue": 10000000,
	  "occupied": 3,
	  "count": 6
	}
  ],
  "monthly_trend": [
	{
	  "month": "Aug",
	  "revenue": 4500000
	},
	{
	  "month": "Sep",
	  "revenue": 5200000
	},
	{
	  "month": "Oct",
	  "revenue": 6300000
	},
	{
	  "month": "Nov",
	  "revenue": 4800000
	},
	{
	  "month": "Dec",
	  "revenue": 2400000
	},
	{
	  "month": "Jan",
	  "revenue": 1800000
	}
  ],
  "demographics": [
	{
	  "name": "18-25",
	  "value": 4
	},
	{
	  "name": "26-35",
	  "value": 5
	},
	{
	  "name": "36-45",
	  "value": 2
	},
	{
	  "name": "46+",
	  "value": 1
	}
  ],
  "recent_checkouts": [
	{
	  "guest_name": "John Doe",
	  "room": "Premium #1",
	  "checkout_date": "2024-01-15"
	}
  ]
}
```

---

## C# Model Mapping

The frontend deserializes this JSON into the following C# model:

```csharp
[DataContract]
public class DashboardStats
{
	[DataMember(Name = "total_revenue")]
	public decimal TotalRevenue { get; set; }

	[DataMember(Name = "pending_revenue")]
	public decimal PendingRevenue { get; set; }

	[DataMember(Name = "potential_revenue")]
	public decimal PotentialRevenue { get; set; }

	[DataMember(Name = "pending_payments")]
	public int PendingPayments { get; set; }

	[DataMember(Name = "active_tenants")]
	public int ActiveTenants { get; set; }

	[DataMember(Name = "occupied_rooms")]
	public int OccupiedRooms { get; set; }

	[DataMember(Name = "available_rooms")]
	public int AvailableRooms { get; set; }

	[DataMember(Name = "total_rooms")]
	public int TotalRooms { get; set; }

	[DataMember(Name = "type_breakdown")]
	public List<TypeBreakdown> TypeBreakdown { get; set; }

	[DataMember(Name = "monthly_trend")]
	public List<MonthlyTrendItem> MonthlyTrend { get; set; }

	[DataMember(Name = "demographics")]
	public List<DemographicsItem> Demographics { get; set; }

	[DataMember(Name = "recent_checkouts")]
	public List<RecentCheckout> RecentCheckouts { get; set; }
}

[DataContract]
public class TypeBreakdown
{
	[DataMember(Name = "type")]
	public string Type { get; set; }

	[DataMember(Name = "revenue")]
	public decimal Revenue { get; set; }

	[DataMember(Name = "occupied")]
	public int Occupied { get; set; }

	[DataMember(Name = "count")]
	public int Count { get; set; }
}

[DataContract]
public class MonthlyTrendItem
{
	[DataMember(Name = "month")]
	public string Month { get; set; }

	[DataMember(Name = "revenue")]
	public decimal Revenue { get; set; }
}

[DataContract]
public class DemographicsItem
{
	[DataMember(Name = "name")]
	public string Name { get; set; }

	[DataMember(Name = "value")]
	public int Value { get; set; }
}

[DataContract]
public class RecentCheckout
{
	[DataMember(Name = "guest_name")]
	public string GuestName { get; set; }

	[DataMember(Name = "room")]
	public string Room { get; set; }

	[DataMember(Name = "checkout_date")]
	public string CheckoutDate { get; set; }
}
```

---

## Frontend Usage

### Stat Cards (Top Section)
```csharp
// Card 1: Total Revenue
TotalRevenueHtmlLabel12.Text = FormatCurrency(currentStats.TotalRevenue);

// Card 2: Pending Payments
guna2HtmlLabel13.Text = currentStats.PendingPayments.ToString();

// Card 3: Occupancy Rate
int totalRoomCount = currentStats.OccupiedRooms + currentStats.AvailableRooms;
double occupancyRate = (currentStats.OccupiedRooms / (double)totalRoomCount) * 100;
guna2HtmlLabel15.Text = $"{occupancyRate:F1} %";

// Card 4: Average Rate (from type breakdown)
int totalRooms = currentStats.TypeBreakdown.Sum(t => t.Count);
decimal avgRate = currentStats.TypeBreakdown.Sum(t => t.Revenue) / totalRooms;
guna2HtmlLabel16.Text = FormatCurrency(avgRate);
```

### Charts (Middle Section)
```csharp
// Chart 1: Revenue by Type
foreach (var item in currentStats.TypeBreakdown)
{
	series.Points.AddXY(item.Type, item.Revenue);
}

// Chart 2: Room Demographics (Pie Chart)
foreach (var item in currentStats.Demographics)
{
	series.Points.AddXY(item.Name, item.Value);
}

// Chart 3: Monthly Trend (Line Chart)
foreach (var item in currentStats.MonthlyTrend)
{
	series.Points.AddXY(item.Month, item.Revenue);
}
```

### Revenue Breakdown Cards (Lower Section)
```csharp
// Cards for each room type
foreach (var item in currentStats.TypeBreakdown)
{
	// Type: Premium, Standard, etc.
	// Revenue: Total revenue for this type
	// Occupied: Number of occupied rooms
	// Total: Total rooms of this type
	// OccupancyText: "{Occupied}/{Total} occupied"
}
```

---

## Performance Considerations

- ✅ **Single Endpoint Call**: Frontend makes 1 call to `/api/dashboard/stats` instead of multiple calls
- ✅ **Pre-Calculated**: Backend calculates all aggregates; frontend only reads/displays
- ✅ **Reduced Payload**: No need to fetch full payment/room details for dashboard cards
- ✅ **Caching**: Backend can implement caching for frequently-accessed stats

---

## Error Scenarios

If the `/api/dashboard/stats` endpoint fails or times out:

```csharp
// Frontend falls back to manual calculation
if (currentStats == null)
{
	UpdateReportStatCardsFromManualData();  // Uses allPayments + allRooms
	SetupRevenueByTypeChartManual();         // Manual grouping
	SetupRoomDemographicsChartManual();      // Manual grouping
	SetupMonthlyTrendChartManual();          // Manual calculation
}
```

This ensures the Report page remains functional even if backend stats are temporarily unavailable.

---

## Testing the Integration

### 1. Verify Endpoint Response
```powershell
# PowerShell
$response = Invoke-RestMethod -Uri "https://rahmatzaw.elarisnoir.my.id/api/dashboard/stats" -Method Get
$response | ConvertTo-Json | Out-File "stats-response.json"

# Verify structure matches expected model
$response | Select-Object -Property total_revenue, pending_payments, type_breakdown | Format-List
```

### 2. Debug Frontend Loading
Open Visual Studio Output window and look for:
```
[LoadDashboardStatsAsync] Starting stats load...
[LoadDashboardStatsAsync] Stats loaded successfully: TotalRevenue=25000000
[UpdateReportStatCards] Using pre-calculated stats from API
```

### 3. Verify Chart Data
After page loads, inspect each chart in Visual Studio designer or check debug output:
```
[SetupRevenueByTypeChart] Using stats data - 2 types
[SetupRevenueByTypeChart] Adding: Premium = Rp 15,000,000
[SetupRevenueByTypeChart] Adding: Standard = Rp 10,000,000
```

---

## Future Enhancements

1. **Date Range Filtering**: Add query parameters to `/api/dashboard/stats`
   ```
   GET /api/dashboard/stats?start_date=2024-01-01&end_date=2024-01-31
   ```

2. **Real-Time Updates**: Implement WebSocket connection for live dashboard
   ```
   WS: ws://rahmatzaw.elarisnoir.my.id/api/dashboard/live
   ```

3. **Room Type Filtering**: Get stats for specific room types only
   ```
   GET /api/dashboard/stats?room_type=Premium
   ```

4. **Tenant Filtering**: Get stats for specific tenants
   ```
   GET /api/dashboard/stats?tenant_id=123
   ```

---

**Contract Version**: 1.0  
**Last Updated**: 2024-01-15  
**Status**: ✅ Active
