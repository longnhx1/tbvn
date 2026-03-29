using Lab_03.Models;
using Lab_03.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab_03.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class DestinationController : Controller
    {
        private readonly IDestinationRepository _destinationRepository;

        public DestinationController(IDestinationRepository destinationRepository)
        {
            _destinationRepository = destinationRepository;
        }

        public async Task<IActionResult> Index() =>
            View("~/Areas/Admin/Views/Categories/Index.cshtml", await _destinationRepository.GetAllAsync());

        public IActionResult Add() => View("~/Areas/Admin/Views/Categories/Add.cshtml");

        [HttpPost]
        public async Task<IActionResult> Add(Destination destination)
        {
            if (!ModelState.IsValid) return View("~/Areas/Admin/Views/Categories/Add.cshtml", destination);
            await _destinationRepository.AddAsync(destination);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Display(int id)
        {
            var destination = await _destinationRepository.GetByIdWithToursAsync(id);
            if (destination == null) return NotFound();
            return View("~/Areas/Admin/Views/Categories/Display.cshtml", destination);
        }

        public async Task<IActionResult> Update(int id)
        {
            var destination = await _destinationRepository.GetByIdAsync(id);
            if (destination == null) return NotFound();
            return View("~/Areas/Admin/Views/Categories/Update.cshtml", destination);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Destination destination)
        {
            if (id != destination.Id) return NotFound();
            if (!ModelState.IsValid) return View("~/Areas/Admin/Views/Categories/Update.cshtml", destination);
            await _destinationRepository.UpdateAsync(destination);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var destination = await _destinationRepository.GetByIdAsync(id);
            if (destination == null) return NotFound();
            return View("~/Areas/Admin/Views/Categories/Delete.cshtml", destination);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _destinationRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
