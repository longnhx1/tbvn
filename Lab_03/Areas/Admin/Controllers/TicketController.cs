using Lab_03.Models;
using Lab_03.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab_03.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class TicketController : Controller
    {
        private readonly ITourRepository _tourRepository;
        private readonly IDestinationRepository _destinationRepository;
        public TicketController(
            ITourRepository tourRepository,
            IDestinationRepository destinationRepository)
        {
            _tourRepository = tourRepository;
            _destinationRepository = destinationRepository;
        }

        // Hiển thị danh sách tour
        public async Task<IActionResult> Index()
        {
            var tours = await _tourRepository.GetAllAsync();
            return View(tours);
        }

        // Hiển thị form thêm tour mới
        public async Task<IActionResult> Add()
        {
            var destinations = await _destinationRepository.GetAllAsync();
            ViewBag.Destinations = new SelectList(destinations, "Id", "Name");

            return View();
        }

        // Xử lý thêm tour mới
        [HttpPost]
        public async Task<IActionResult> Add(Tour tour, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null)
                {
                    tour.ImageUrl = await SaveImage(ImageUrl);
                }

                await _tourRepository.AddAsync(tour);
                return RedirectToAction(nameof(Index));
            }

            var destinations = await _destinationRepository.GetAllAsync();
            ViewBag.Destinations = new SelectList(destinations, "Id", "Name");

            return View(tour);
        }

        // Hàm lưu hình ảnh
        private async Task<string> SaveImage(IFormFile image)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, image.FileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/" + image.FileName;
        }

        // Hiển thị thông tin chi tiết tour
        public async Task<IActionResult> Display(int id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // Hiển thị form cập nhật tour
        public async Task<IActionResult> Update(int id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);

            if (tour == null)
            {
                return NotFound();
            }

            var destinations = await _destinationRepository.GetAllAsync();
            ViewBag.Destinations = new SelectList(
                destinations,
                "Id",
                "Name",
                tour.DestinationId
            );

            return View(tour);
        }

        // Xử lý cập nhật tour
        [HttpPost]
        public async Task<IActionResult> Update(int id, Tour tour, IFormFile ImageUrl)
        {
            ModelState.Remove("ImageUrl");

            if (id != tour.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingTour = await _tourRepository.GetByIdAsync(id);
                if (existingTour == null)
                    return NotFound();

                if (ImageUrl == null)
                {
                    tour.ImageUrl = existingTour.ImageUrl;
                }
                else
                {
                    tour.ImageUrl = await SaveImage(ImageUrl);
                }

                existingTour.Name = tour.Name;
                existingTour.Price = tour.Price;
                existingTour.Description = tour.Description;
                existingTour.DestinationId = tour.DestinationId;
                existingTour.ImageUrl = tour.ImageUrl;

                await _tourRepository.UpdateAsync(existingTour);

                return RedirectToAction(nameof(Index));
            }

            var destinations = await _destinationRepository.GetAllAsync();
            ViewBag.Destinations = new SelectList(destinations, "Id", "Name");

            return View(tour);
        }

        // Hiển thị form xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);

            if (tour == null)
            {
                return NotFound();
            }

            return View(tour);
        }

        // Xử lý xóa
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _tourRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}