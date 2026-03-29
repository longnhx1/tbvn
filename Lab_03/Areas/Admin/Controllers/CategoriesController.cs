using Lab_03.Models;
using Lab_03.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab_03.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoriesController : Controller
    {
        private readonly IDestinationRepository _destinationRepository;

        public CategoriesController(
            IDestinationRepository destinationRepository)
        {
            _destinationRepository = destinationRepository;
        }

        // Hiển thị danh sách điểm đến
        public async Task<IActionResult> Index()
        {
            var destinations = await _destinationRepository.GetAllAsync();
            return View(destinations);
        }

        // Hiển thị thông tin chi tiết một điểm đến
        public async Task<IActionResult> Display(int id)
        {
            var destination = await _destinationRepository.GetByIdAsync(id);
            if (destination == null)
            {
                return NotFound();
            }
            return View(destination);
        }

        // Hiển thị form thêm điểm đến mới
        public IActionResult Add()
        {
            return View();
        }

        // Xử lý thêm điểm đến mới
        [HttpPost]
        public async Task<IActionResult> Add(Destination destination)
        {
            if (ModelState.IsValid)
            {
                await _destinationRepository.AddAsync(destination);
                return RedirectToAction(nameof(Index));
            }
            return View(destination);
        }

        // Hiển thị form cập nhật thông tin điểm đến
        public async Task<IActionResult> Update(int id)
        {
            var destination = await _destinationRepository.GetByIdAsync(id);
            if (destination == null)
            {
                return NotFound();
            }
            return View(destination);
        }

        // Xử lý cập nhật thông tin điểm đến
        [HttpPost]
        public async Task<IActionResult> Update(int id, Destination destination)
        {
            if (id != destination.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _destinationRepository.UpdateAsync(destination);
                return RedirectToAction(nameof(Index));
            }

            return View(destination);
        }

        // Hiển thị form xác nhận xóa điểm đến
        public async Task<IActionResult> Delete(int id)
        {
            var destination = await _destinationRepository.GetByIdAsync(id);
            if (destination == null)
            {
                return NotFound();
            }
            return View(destination);
        }

        // Xử lý xóa điểm đến
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var destination = await _destinationRepository.GetByIdAsync(id);
            if (destination != null)
            {
                await _destinationRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
