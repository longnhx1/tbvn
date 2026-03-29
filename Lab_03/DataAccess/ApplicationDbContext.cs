
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Lab_03.Models;


// Thêm dòng namespace này để khớp với cấu trúc thư mục
namespace Lab_03.DataAccess 
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<TourImage> TourImages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}