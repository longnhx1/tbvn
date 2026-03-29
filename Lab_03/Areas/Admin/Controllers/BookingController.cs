using Lab_03.Models;
using Lab_03.Infrastructure;
using Lab_03.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab_03.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITourRepository _tourRepository;
        private readonly BookingPaymentCalculator _paymentCalculator;

        public BookingController(
            IBookingRepository bookingRepository,
            ITourRepository tourRepository,
            BookingPaymentCalculator paymentCalculator)
        {
            _bookingRepository = bookingRepository;
            _tourRepository = tourRepository;
            _paymentCalculator = paymentCalculator;
        }

        public async Task<IActionResult> Index(string? status)
        {
            var bookings = await _bookingRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BookingStatus>(status, true, out var parsed))
            {
                bookings = bookings.Where(b => b.Status == parsed);
            }

            ViewBag.CurrentStatus = status;
            return View(bookings);
        }

        public async Task<IActionResult> Details(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, BookingStatus status, string? returnStatus)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var oldStatus = booking.Status;
            booking.Status = status;

            if (status == BookingStatus.Cancelled && oldStatus != BookingStatus.Cancelled)
            {
                var paidForRefund = booking.TotalPaid > 0m
                    ? booking.TotalPaid
                    : (booking.IsDemoPaid ? booking.TotalPrice : 0m);

                var refundPercent =
                    _paymentCalculator.GetRefundPercentFromCancellationPolicy(booking.Tour?.CancellationPolicy);

                booking.RefundAmount = decimal.Round(paidForRefund * refundPercent, 2);
                booking.CancelledAt = DateTime.UtcNow;

                // Khóa luồng thanh toán & vé điện tử.
                booking.IsDemoPaid = false;
                booking.DemoPayToken = null;
                booking.DemoPayTokenExpiresAt = null;

                // Trả lại chỗ cho tour.
                var tour = await _tourRepository.GetByIdAsync(booking.TourId);
                if (tour != null)
                {
                    var guests = booking.Adults + booking.Children;
                    tour.AvailableSeats += guests;
                    await _tourRepository.UpdateAsync(tour);
                }
            }

            await _bookingRepository.UpdateAsync(booking);

            if (string.IsNullOrWhiteSpace(returnStatus))
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), new { status = returnStatus });
        }
    }
}
