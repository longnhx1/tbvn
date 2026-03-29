using Lab_03.DataAccess;
using Lab_03.Models;
using Lab_03.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab_03.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class UsersController : Controller
{
    private static readonly string[] CreatableRoles =
    [
        SD.Role_Customer,
        SD.Role_Employee,
        SD.Role_Company
    ];

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync();

        var rows = new List<AdminUserListRowViewModel>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            rows.Add(new AdminUserListRowViewModel
            {
                Id = u.Id,
                Email = u.Email ?? "",
                FullName = u.FullName,
                Roles = roles.OrderBy(r => r).ToList()
            });
        }

        return View(rows);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(BuildCreateModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminCreateUserViewModel model)
    {
        model.RoleOptions = RoleSelectList(model.Role, includeAdmin: false);

        if (!CreatableRoles.Contains(model.Role))
            ModelState.AddModelError(nameof(model.Role), "Chỉ được phép gán vai trò Khách hàng, Nhân viên hoặc Đối tác.");

        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true,
            FullName = model.FullName
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, model.Role);
        TempData["SuccessMessage"] = $"Đã tạo tài khoản {model.Email} với vai trò {model.Role}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var wasAdmin = await _userManager.IsInRoleAsync(user, SD.Role_Admin);
        var roles = await _userManager.GetRolesAsync(user);
        var primaryRole = wasAdmin
            ? SD.Role_Admin
            : (roles.FirstOrDefault(r => CreatableRoles.Contains(r)) ?? SD.Role_Customer);

        var vm = new AdminEditUserViewModel
        {
            Id = user.Id,
            Email = user.Email ?? "",
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = primaryRole,
            RoleOptions = RoleSelectList(primaryRole, includeAdmin: true)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, AdminEditUserViewModel model)
    {
        if (id != model.Id) return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var wasAdmin = await _userManager.IsInRoleAsync(user, SD.Role_Admin);
        model.RoleOptions = RoleSelectList(model.Role, includeAdmin: true);

        var allowed = CreatableRoles.Contains(model.Role) || model.Role == SD.Role_Admin;
        if (!allowed)
            ModelState.AddModelError(nameof(model.Role), "Vai trò không hợp lệ.");

        if (!ModelState.IsValid)
            return View(model);

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null && currentUser.Id == user.Id && wasAdmin && model.Role != SD.Role_Admin)
        {
            ModelState.AddModelError(nameof(model.Role), "Bạn không thể tự gỡ quyền quản trị của chính mình.");
            return View(model);
        }

        user.FullName = model.FullName;
        user.PhoneNumber = model.PhoneNumber;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var e in updateResult.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            return View(model);
        }

        var currentRoles = (await _userManager.GetRolesAsync(user)).ToList();
        if (currentRoles.Count > 0)
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, model.Role);

        TempData["SuccessMessage"] = $"Đã cập nhật tài khoản {user.Email}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null && currentUser.Id == user.Id)
            return RedirectToAction(nameof(Index));

        var roles = await _userManager.GetRolesAsync(user);
        var row = new AdminUserListRowViewModel
        {
            Id = user.Id,
            Email = user.Email ?? "",
            FullName = user.FullName,
            Roles = roles.OrderBy(r => r).ToList()
        };

        var hasBookings = await _db.Bookings.AnyAsync(b => b.UserId == user.Id);
        ViewBag.HasBookings = hasBookings;

        if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
        {
            var admins = await _userManager.GetUsersInRoleAsync(SD.Role_Admin);
            ViewBag.IsOnlyAdmin = admins.Count <= 1;
        }
        else
            ViewBag.IsOnlyAdmin = false;

        return View(row);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null && currentUser.Id == user.Id)
        {
            TempData["ErrorMessage"] = "Không thể xóa tài khoản đang đăng nhập.";
            return RedirectToAction(nameof(Index));
        }

        if (await _db.Bookings.AnyAsync(b => b.UserId == user.Id))
        {
            TempData["ErrorMessage"] = "Không xóa được: tài khoản có lịch sử đặt tour.";
            return RedirectToAction(nameof(Index));
        }

        if (await _userManager.IsInRoleAsync(user, SD.Role_Admin))
        {
            var admins = await _userManager.GetUsersInRoleAsync(SD.Role_Admin);
            if (admins.Count <= 1)
            {
                TempData["ErrorMessage"] = "Không xóa được quản trị viên cuối cùng.";
                return RedirectToAction(nameof(Index));
            }
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = string.Join(" ", result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Đã xóa tài khoản.";
        return RedirectToAction(nameof(Index));
    }

    private AdminCreateUserViewModel BuildCreateModel()
    {
        return new AdminCreateUserViewModel
        {
            Role = SD.Role_Customer,
            RoleOptions = RoleSelectList(SD.Role_Customer, includeAdmin: false)
        };
    }

    private IEnumerable<SelectListItem> RoleSelectList(string? selected, bool includeAdmin)
    {
        var labels = new Dictionary<string, string>
        {
            [SD.Role_Customer] = "Khách hàng",
            [SD.Role_Employee] = "Nhân viên",
            [SD.Role_Company] = "Đối tác / Công ty",
            [SD.Role_Admin] = "Quản trị viên"
        };

        var list = new List<string>();
        if (includeAdmin)
            list.Add(SD.Role_Admin);
        list.AddRange(CreatableRoles);

        return list.Distinct().Select(r => new SelectListItem
        {
            Value = r,
            Text = labels[r],
            Selected = r == selected
        });
    }
}
