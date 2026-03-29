using Lab_03.Models;
using Lab_03.Models.ViewModels;
using Lab_03.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Lab_03.Controllers
{
    public class ToursController : Controller
    {
        private readonly ITourRepository _tourRepository;
        private readonly IDestinationRepository _destinationRepository;

        public ToursController(
            ITourRepository tourRepository,
            IDestinationRepository destinationRepository)
        {
            _tourRepository = tourRepository;
            _destinationRepository = destinationRepository;
        }

        public async Task<IActionResult> Index(
            string? q,
            int[]? destinationIds,
            decimal? minPrice,
            decimal? maxPrice,
            DateTime? departFrom,
            DateTime? departTo,
            string? departLoc)
        {
            var tours = (await _tourRepository.GetAllAsync()).ToList();
            var destinations = (await _destinationRepository.GetAllAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                tours = tours.Where(t =>
                    t.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || (t.Destination?.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (t.DepartureLocation?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
            }

            if (destinationIds is { Length: > 0 })
            {
                var idSet = new HashSet<int>(destinationIds!);
                tours = tours.Where(t => idSet.Contains(t.DestinationId)).ToList();
            }

            if (minPrice.HasValue)
                tours = tours.Where(t => t.Price >= minPrice.Value).ToList();

            if (maxPrice.HasValue)
                tours = tours.Where(t => t.Price <= maxPrice.Value).ToList();

            if (departFrom.HasValue)
            {
                var from = departFrom.Value.Date;
                tours = tours.Where(t => t.DepartureDate.Date >= from).ToList();
            }

            if (departTo.HasValue)
            {
                var to = departTo.Value.Date;
                tours = tours.Where(t => t.DepartureDate.Date <= to).ToList();
            }

            if (!string.IsNullOrWhiteSpace(departLoc))
            {
                var loc = departLoc.Trim();
                tours = tours.Where(t =>
                    t.DepartureLocation?.Contains(loc, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            var vm = new ToursIndexViewModel
            {
                Tours = tours,
                Destinations = destinations,
                SearchQuery = q,
                SelectedDestinationIds = destinationIds?.ToList() ?? [],
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                DepartFrom = departFrom,
                DepartTo = departTo,
                DepartureLocationFilter = departLoc
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }
    }
}
