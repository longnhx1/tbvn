using Lab_03.Models;
using Lab_03.Models.ViewModels;
using Lab_03.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Lab_03.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITourRepository _tourRepository;
        private readonly IDestinationRepository _destinationRepository;

        public HomeController(ITourRepository tourRepository, IDestinationRepository destinationRepository)
        {
            _tourRepository = tourRepository;
            _destinationRepository = destinationRepository;
        }

        public async Task<IActionResult> Index()
        {
            var tours = (await _tourRepository.GetAllAsync()).Take(6).ToList();
            var destinations = (await _destinationRepository.GetAllWithToursAsync()).ToList();
            return View(new HomeIndexViewModel
            {
                FeaturedTours = tours,
                Destinations = destinations
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
