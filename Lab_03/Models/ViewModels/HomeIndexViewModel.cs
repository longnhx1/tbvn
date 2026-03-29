using Lab_03.Models;

namespace Lab_03.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Tour> FeaturedTours { get; set; } = [];
        public List<Destination> Destinations { get; set; } = [];
    }
}
