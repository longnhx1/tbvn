using Lab_03.DataAccess;
using Lab_03.Models;
using Lab_03.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab_03.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.UtcNow.Date;
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var totalBookings = await _context.Bookings.CountAsync();
            var pendingBookings = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Pending);
            var totalRevenue = await _context.Bookings
                .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                .SumAsync(b => (decimal?)b.TotalPrice) ?? 0m;
            var totalToursInCatalog = await _context.Tours.CountAsync();
            var toursOnSale = await _context.Tours.CountAsync(t =>
                t.DepartureDate.Date >= today && t.AvailableSeats > 0);

            var distinctBookersLast30 = await _context.Bookings
                .Where(b => b.CreatedAt >= thirtyDaysAgo)
                .Select(b => b.UserId)
                .Distinct()
                .CountAsync();

            var totalUsers = await _context.Users.CountAsync();

            var recent = await _context.Bookings
                .AsNoTracking()
                .OrderByDescending(b => b.CreatedAt)
                .Take(8)
                .Select(b => new AdminRecentBookingRow
                {
                    BookingId = b.Id,
                    TourName = b.Tour != null ? b.Tour.Name : "",
                    ContactName = b.ContactName,
                    Adults = b.Adults,
                    Children = b.Children,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            var vm = new AdminDashboardViewModel
            {
                TotalBookings = totalBookings,
                TotalToursInCatalog = totalToursInCatalog,
                PendingBookings = pendingBookings,
                TotalRevenue = totalRevenue,
                TotalRevenueRounded = (long)Math.Round(totalRevenue, MidpointRounding.AwayFromZero),
                ToursOnSale = toursOnSale,
                DistinctBookersLast30Days = distinctBookersLast30,
                TotalUsers = totalUsers,
                RecentBookings = recent
            };

            return View(vm);
        }
    }
}
