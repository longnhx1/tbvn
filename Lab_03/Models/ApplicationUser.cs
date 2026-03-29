using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Lab_03.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Age { get; set; }
        public List<Tour>? Tours { get; set; }
        public List<Booking>? Bookings { get; set; }
    }
}
