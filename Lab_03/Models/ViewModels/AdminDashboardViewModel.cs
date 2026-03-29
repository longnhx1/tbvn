namespace Lab_03.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalBookings { get; set; }
    public int TotalToursInCatalog { get; set; }
    public int PendingBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public long TotalRevenueRounded { get; set; }
    public int ToursOnSale { get; set; }
    public int DistinctBookersLast30Days { get; set; }
    public int TotalUsers { get; set; }
    public IReadOnlyList<AdminRecentBookingRow> RecentBookings { get; set; } = Array.Empty<AdminRecentBookingRow>();
}

public class AdminRecentBookingRow
{
    public int BookingId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public int Adults { get; set; }
    public int Children { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
