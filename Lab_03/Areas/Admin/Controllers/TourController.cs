using Lab_03.DataAccess;
using Lab_03.Models;
using Lab_03.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab_03.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class TourController : Controller
    {
        private readonly ITourRepository _tourRepository;
        private readonly IDestinationRepository _destinationRepository;
        private readonly ApplicationDbContext _db;

        public TourController(
            ITourRepository tourRepository,
            IDestinationRepository destinationRepository,
            ApplicationDbContext db)
        {
            _tourRepository = tourRepository;
            _destinationRepository = destinationRepository;
            _db = db;
        }

        public async Task<IActionResult> Index() =>
            View("~/Areas/Admin/Views/Ticket/Index.cshtml", await _tourRepository.GetAllAsync());

        public async Task<IActionResult> Add()
        {
            ViewBag.Destinations = new SelectList(await _destinationRepository.GetAllAsync(), "Id", "Name");
            return View("~/Areas/Admin/Views/Ticket/Add.cshtml", new Tour());
        }

        [HttpPost]
        public async Task<IActionResult> Add(Tour tour, IFormFile? imageUrl, List<IFormFile>? galleryImages)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Destinations = new SelectList(await _destinationRepository.GetAllAsync(), "Id", "Name", tour.DestinationId);
                return View("~/Areas/Admin/Views/Ticket/Add.cshtml", tour);
            }

            if (imageUrl != null)
                tour.ImageUrl = await SaveImage(imageUrl);

            await _tourRepository.AddAsync(tour);

            var main = tour.ImageUrl;
            await SaveGalleryAsync(tour.Id, galleryImages, main);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Display(int id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null) return NotFound();
            return View("~/Areas/Admin/Views/Ticket/Display.cshtml", tour);
        }

        public async Task<IActionResult> Update(int id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null) return NotFound();

            ViewBag.Destinations = new SelectList(await _destinationRepository.GetAllAsync(), "Id", "Name", tour.DestinationId);
            return View("~/Areas/Admin/Views/Ticket/Update.cshtml", tour);
        }

        [HttpPost]
        public async Task<IActionResult> Update(
            int id,
            Tour tour,
            IFormFile? imageUrl,
            List<IFormFile>? galleryImages,
            int[]? removeGalleryIds)
        {
            if (id != tour.Id) return NotFound();

            var existing = await _tourRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Destinations = new SelectList(await _destinationRepository.GetAllAsync(), "Id", "Name", tour.DestinationId);
                return View("~/Areas/Admin/Views/Ticket/Update.cshtml", tour);
            }

            existing.Name = tour.Name;
            existing.Price = tour.Price;
            existing.Description = tour.Description;
            existing.DestinationId = tour.DestinationId;
            existing.DepartureDate = tour.DepartureDate;
            existing.DepartureLocation = tour.DepartureLocation;
            existing.DurationDays = tour.DurationDays;
            existing.MaxGuests = tour.MaxGuests;
            existing.AvailableSeats = tour.AvailableSeats;
            existing.TransportType = tour.TransportType;
            existing.IncludedServices = tour.IncludedServices;
            existing.ExcludedServices = tour.ExcludedServices;
            existing.CancellationPolicy = tour.CancellationPolicy;

            if (imageUrl != null)
                existing.ImageUrl = await SaveImage(imageUrl);

            if (removeGalleryIds is { Length: > 0 })
            {
                var toRemove = await _db.TourImages
                    .Where(i => i.TourId == id && removeGalleryIds.Contains(i.Id))
                    .ToListAsync();
                _db.TourImages.RemoveRange(toRemove);
            }

            await SaveGalleryAsync(id, galleryImages, existing.ImageUrl);

            await _tourRepository.UpdateAsync(existing);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null) return NotFound();
            return View("~/Areas/Admin/Views/Ticket/Delete.cshtml", tour);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var extras = await _db.TourImages.Where(i => i.TourId == id).ToListAsync();
            _db.TourImages.RemoveRange(extras);
            await _db.SaveChangesAsync();
            await _tourRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task SaveGalleryAsync(int tourId, List<IFormFile>? files, string? mainUrl)
        {
            if (files == null || files.Count == 0) return;

            foreach (var f in files)
            {
                if (f.Length == 0) continue;
                var url = await SaveImage(f);
                if (!string.IsNullOrEmpty(mainUrl) &&
                    string.Equals(url, mainUrl, StringComparison.OrdinalIgnoreCase))
                    continue;
                _db.TourImages.Add(new TourImage { TourId = tourId, Url = url });
            }

            await _db.SaveChangesAsync();
        }

        private static async Task<string> SaveImage(IFormFile image)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            Directory.CreateDirectory(folderPath);
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(fileStream);
            return "/images/" + fileName;
        }
    }
}
