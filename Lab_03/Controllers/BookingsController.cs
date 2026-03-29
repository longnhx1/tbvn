using System.Security.Cryptography;
using Lab_03.Infrastructure;
using Lab_03.Models;
using Lab_03.Models.ViewModels;
using Lab_03.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lab_03.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ITourRepository _tourRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BookingsController> _logger;
        private readonly BookingPaymentCalculator _paymentCalculator;
        private readonly BookingPaymentOptions _paymentOptions;

        public BookingsController(
            ITourRepository tourRepository,
            IBookingRepository bookingRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<BookingsController> logger,
            BookingPaymentCalculator paymentCalculator,
            IOptions<BookingPaymentOptions> paymentOptions)
        {
            _tourRepository = tourRepository;
            _bookingRepository = bookingRepository;
            _userManager = userManager;
            _logger = logger;
            _paymentCalculator = paymentCalculator;
            _paymentOptions = paymentOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int tourId)
        {
            var tour = await _tourRepository.GetByIdAsync(tourId);
            if (tour == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var vm = new BookingCreateViewModel
            {
                TourId = tour.Id,
                TourName = tour.Name,
                UnitPrice = tour.Price,
                DefaultTravelDate = tour.DepartureDate,
                TravelDate = tour.DepartureDate,
                ContactName = user?.FullName ?? string.Empty,
                ContactPhone = user?.PhoneNumber ?? string.Empty,
                ContactEmail = user?.Email ?? string.Empty,
                PaymentMode = BookingPaymentMode.Full,
                InstallmentMonths = _paymentOptions.AllowedInstallmentMonths.Length > 0
                    ? _paymentOptions.AllowedInstallmentMonths[0]
                    : 3
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            var tour = await _tourRepository.GetByIdAsync(model.TourId);
            if (tour == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.TourName = tour.Name;
                model.UnitPrice = tour.Price;
                model.DefaultTravelDate = tour.DepartureDate;
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var guests = model.Adults + model.Children;
            if (tour.AvailableSeats < guests)
            {
                ModelState.AddModelError(string.Empty, "So cho trong khong du cho so luong khach da chon.");
                model.TourName = tour.Name;
                model.UnitPrice = tour.Price;
                model.DefaultTravelDate = tour.DepartureDate;
                return View(model);
            }

            var totalPrice = model.UnitPrice * model.Adults + (model.UnitPrice * 0.7m * model.Children);

            var paymentMode = model.PaymentMode;
            int? installmentMonths = null;

            // Điều kiện áp dụng trả góp: từ 3 người trở lên.
            if (guests < _paymentOptions.MinGuestsForInstallment)
            {
                paymentMode = BookingPaymentMode.Full;
            }
            else if (paymentMode == BookingPaymentMode.Installment)
            {
                if (!_paymentOptions.AllowedInstallmentMonths.Contains(model.InstallmentMonths))
                {
                    ModelState.AddModelError(string.Empty,
                        $"Số tháng trả góp không hợp lệ. Hãy chọn một trong: {string.Join(", ", _paymentOptions.AllowedInstallmentMonths)}.");
                    model.TourName = tour.Name;
                    model.UnitPrice = tour.Price;
                    model.DefaultTravelDate = tour.DepartureDate;
                    return View(model);
                }

                installmentMonths = model.InstallmentMonths;
            }
            else
            {
                paymentMode = BookingPaymentMode.Full;
            }

            var booking = new Booking
            {
                TourId = tour.Id,
                UserId = user.Id,
                ContactName = model.ContactName,
                ContactPhone = model.ContactPhone,
                ContactEmail = model.ContactEmail,
                Adults = model.Adults,
                Children = model.Children,
                TravelDate = model.TravelDate,
                UnitPrice = model.UnitPrice,
                TotalPrice = totalPrice,
                Note = model.Note,
                Status = BookingStatus.Pending,
                PaymentMode = paymentMode,
                InstallmentMonths = installmentMonths,
                MonthlyInterestPercent = paymentMode == BookingPaymentMode.Installment ? _paymentOptions.MonthlyInterestPercent : 0m,
                DownPaymentAmount = paymentMode == BookingPaymentMode.Installment ? _paymentCalculator.GetDownPaymentAmount(totalPrice) : 0m,
                TotalPaid = 0m,
                PaidInstallmentCount = paymentMode == BookingPaymentMode.Installment ? -1 : 0,
                DemoPayToken = GenerateDemoPayToken(),
                DemoPayTokenExpiresAt = DateTime.UtcNow.AddHours(48)
            };

            await _bookingRepository.AddAsync(booking);

            tour.AvailableSeats -= guests;
            await _tourRepository.UpdateAsync(tour);

            return RedirectToAction(nameof(Confirmation), new { id = booking.Id });
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || booking.UserId != user.Id)
            {
                return Forbid();
            }

            if (!booking.IsDemoPaid && string.IsNullOrEmpty(booking.DemoPayToken))
            {
                booking.DemoPayToken = GenerateDemoPayToken();
                booking.DemoPayTokenExpiresAt = DateTime.UtcNow.AddHours(48);
                await _bookingRepository.UpdateAsync(booking);
            }

            if (booking.IsDemoPaid)
                ViewData["TicketCard"] = BuildTicketCard(booking);
            else
            {
                var payUrl = Url.Action(nameof(PayDemo), "Bookings", new { token = booking.DemoPayToken }, Request.Scheme, Request.Host.ToString()) ?? "";
                ViewData["DemoPayQrDataUrl"] = string.IsNullOrEmpty(booking.DemoPayToken) || string.IsNullOrEmpty(payUrl)
                    ? null
                    : TicketQrPng.ToDataUrlPng(payUrl);

                ViewData["CurrentDueAmount"] = _paymentCalculator.GetCurrentDueAmount(booking);
                ViewData["CurrentDueLabel"] = _paymentCalculator.GetCurrentDueLabel(booking);
            }

            return View(booking);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PayDemo(string? token)
        {
            var booking = await FindBookingForPayDemoAsync(token);
            if (booking == null)
                return View("PayDemoInvalid");

            ViewData["DueAmount"] = _paymentCalculator.GetCurrentDueAmount(booking);
            ViewData["DueLabel"] = _paymentCalculator.GetCurrentDueLabel(booking);
            return View("PayDemo", booking);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(PayDemo))]
        public async Task<IActionResult> PayDemoPost(string? token)
        {
            var booking = await FindBookingForPayDemoAsync(token);
            if (booking == null)
                return View("PayDemoInvalid");

            var dueAmount = _paymentCalculator.GetCurrentDueAmount(booking);
            if (dueAmount <= 0m)
                return View("PayDemoInvalid");

            booking.TotalPaid += dueAmount;
            booking.DemoPaidAt = DateTime.UtcNow;
            booking.DemoPaymentRef = "DEMO-" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();

            if (booking.PaymentMode == BookingPaymentMode.Full)
            {
                booking.PaidInstallmentCount = 0;
                booking.IsDemoPaid = true;
                booking.DemoPayToken = null;
                booking.DemoPayTokenExpiresAt = null;
            }
            else
            {
                if (booking.PaidInstallmentCount == -1)
                {
                    // Vừa trả cọc 30% xong.
                    booking.PaidInstallmentCount = 0;
                }
                else
                {
                    // Vừa trả xong 1 kỳ trả góp.
                    booking.PaidInstallmentCount += 1;
                }

                if (_paymentCalculator.IsFullyPaid(booking))
                {
                    booking.IsDemoPaid = true;
                    booking.DemoPayToken = null;
                    booking.DemoPayTokenExpiresAt = null;
                }
                else
                {
                    // Tạo token/QR cho kỳ tiếp theo.
                    booking.DemoPayToken = GenerateDemoPayToken();
                    booking.DemoPayTokenExpiresAt = DateTime.UtcNow.AddHours(48);
                }
            }

            await _bookingRepository.UpdateAsync(booking);
            _logger.LogInformation(
                "Thanh toán demo (QR điện thoại) — BookingId {BookingId}, Ref {Ref}",
                booking.Id, booking.DemoPaymentRef);

            return RedirectToAction(nameof(PayDemoSuccess), new { id = booking.Id });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PayDemoSuccess(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null || !booking.IsDemoPaid)
                return NotFound();

            return View("PayDemoSuccess", booking);
        }

        [HttpGet]
        public async Task<IActionResult> DemoPaymentStatus(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || booking.UserId != user.Id)
                return NotFound();

            return Json(new { isDemoPaid = booking.IsDemoPaid });
        }

        [HttpGet]
        public async Task<IActionResult> MockPayment(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || booking.UserId != user.Id)
                return Forbid();

            if (booking.IsDemoPaid)
            {
                TempData["InfoMessage"] = "Đặt chỗ này đã được ghi nhận thanh toán demo trước đó.";
                return RedirectToAction(nameof(Confirmation), new { id });
            }

            ViewData["DueAmount"] = _paymentCalculator.GetCurrentDueAmount(booking);
            ViewData["DueLabel"] = _paymentCalculator.GetCurrentDueLabel(booking);

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MockPayment(int id, bool success)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || booking.UserId != user.Id)
                return Forbid();

            if (booking.IsDemoPaid)
            {
                TempData["InfoMessage"] = "Thanh toán demo đã hoàn tất trước đó.";
                return RedirectToAction(nameof(Confirmation), new { id });
            }

            if (success)
            {
                var dueAmount = _paymentCalculator.GetCurrentDueAmount(booking);
                if (dueAmount <= 0m)
                    return RedirectToAction(nameof(Confirmation), new { id });

                booking.TotalPaid += dueAmount;
                booking.DemoPaidAt = DateTime.UtcNow;
                booking.DemoPaymentRef = "DEMO-" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();

                if (booking.PaymentMode == BookingPaymentMode.Full)
                {
                    booking.PaidInstallmentCount = 0;
                    booking.IsDemoPaid = true;
                    booking.DemoPayToken = null;
                    booking.DemoPayTokenExpiresAt = null;
                }
                else
                {
                    if (booking.PaidInstallmentCount == -1)
                    {
                        booking.PaidInstallmentCount = 0;
                    }
                    else
                    {
                        booking.PaidInstallmentCount += 1;
                    }

                    if (_paymentCalculator.IsFullyPaid(booking))
                    {
                        booking.IsDemoPaid = true;
                        booking.DemoPayToken = null;
                        booking.DemoPayTokenExpiresAt = null;
                    }
                    else
                    {
                        booking.DemoPayToken = GenerateDemoPayToken();
                        booking.DemoPayTokenExpiresAt = DateTime.UtcNow.AddHours(48);
                    }
                }

                await _bookingRepository.UpdateAsync(booking);
                _logger.LogInformation(
                    "Thanh toán demo thành công — BookingId {BookingId}, Email {Email}, Ref {Ref}",
                    booking.Id, booking.ContactEmail, booking.DemoPaymentRef);
                TempData["SuccessMessage"] = "Đã mô phỏng thanh toán thành công. Đây không phải giao dịch tiền thật.";
            }
            else
            {
                _logger.LogInformation("Thanh toán demo bị hủy (giả lập) — BookingId {BookingId}", booking.Id);
                TempData["ErrorMessage"] = "Bạn đã hủy bước thanh toán mô phỏng. Đặt chỗ vẫn được giữ; bạn có thể thử lại.";
            }

            return RedirectToAction(nameof(Confirmation), new { id });
        }

        public async Task<IActionResult> Ticket(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || booking.UserId != user.Id)
                return Forbid();

            if (!booking.IsDemoPaid)
            {
                TempData["InfoMessage"] = "Hoàn tất thanh toán demo để xem vé điện tử.";
                return RedirectToAction(nameof(Confirmation), new { id });
            }

            ViewData["TicketCard"] = BuildTicketCard(booking, showEmailBlock: false);
            return View(booking);
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var bookings = await _bookingRepository.GetByUserIdAsync(user.Id);
            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || booking.UserId != user.Id)
                return Forbid();

            if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Completed)
                return RedirectToAction(nameof(MyOrders));

            // Hoàn tiền dựa theo số tiền đã nộp (demo).
            var paidForRefund = booking.TotalPaid > 0m
                ? booking.TotalPaid
                : (booking.IsDemoPaid ? booking.TotalPrice : 0m);

            var refundPercent = _paymentCalculator.GetRefundPercentFromCancellationPolicy(booking.Tour?.CancellationPolicy);
            booking.RefundAmount = decimal.Round(paidForRefund * refundPercent, 2);
            booking.CancelledAt = DateTime.UtcNow;
            booking.Status = BookingStatus.Cancelled;
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

            await _bookingRepository.UpdateAsync(booking);
            TempData["SuccessMessage"] = $"Đơn #{booking.Id} đã được hủy. Hoàn {booking.RefundAmount?.ToString("N0")} đ (theo demo).";
            return RedirectToAction(nameof(MyOrders));
        }

        public async Task<IActionResult> Details(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || booking.UserId != user.Id)
            {
                return Forbid();
            }

            if (!booking.IsDemoPaid && string.IsNullOrEmpty(booking.DemoPayToken))
            {
                booking.DemoPayToken = GenerateDemoPayToken();
                booking.DemoPayTokenExpiresAt = DateTime.UtcNow.AddHours(48);
                await _bookingRepository.UpdateAsync(booking);
            }

            if (booking.IsDemoPaid)
                ViewData["TicketCard"] = BuildTicketCard(booking);
            else
            {
                var payUrl = Url.Action(nameof(PayDemo), "Bookings", new { token = booking.DemoPayToken }, Request.Scheme, Request.Host.ToString()) ?? "";
                ViewData["DemoPayQrDataUrl"] = string.IsNullOrEmpty(booking.DemoPayToken) || string.IsNullOrEmpty(payUrl)
                    ? null
                    : TicketQrPng.ToDataUrlPng(payUrl);

                ViewData["CurrentDueAmount"] = _paymentCalculator.GetCurrentDueAmount(booking);
                ViewData["CurrentDueLabel"] = _paymentCalculator.GetCurrentDueLabel(booking);
            }

            return View(booking);
        }

        private async Task<Booking?> FindBookingForPayDemoAsync(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var booking = await _bookingRepository.GetByDemoPayTokenAsync(token);
            if (booking == null || booking.IsDemoPaid)
                return null;

            if (booking.DemoPayTokenExpiresAt == null || booking.DemoPayTokenExpiresAt < DateTime.UtcNow)
                return null;

            return booking;
        }

        private static string GenerateDemoPayToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        }

        private BookingTicketCardModel BuildTicketCard(Booking booking, bool showEmailBlock = true)
        {
            var ticketUrl = Url.Action(nameof(Ticket), "Bookings", new { id = booking.Id }, Request.Scheme, Request.Host.ToString()) ?? "";
            var payload = string.Join("|", "TRAVEL-DEMO", booking.Id, booking.ContactEmail, booking.Tour?.Name ?? "");
            return new BookingTicketCardModel
            {
                Booking = booking,
                TicketUrl = ticketUrl,
                QrDataUrl = TicketQrPng.ToDataUrlPng(payload),
                MockEmailBody = BookingMockEmailText.Build(booking),
                ShowEmailBlock = showEmailBlock
            };
        }
    }
}
