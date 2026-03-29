using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab_03.Models.ViewModels;

public class AdminUserListRowViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}

public class AdminCreateUserViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu từ {2} đến {1} ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Xác nhận mật khẩu")]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Chọn loại tài khoản.")]
    [Display(Name = "Loại tài khoản")]
    public string Role { get; set; } = SD.Role_Customer;

    public IEnumerable<SelectListItem> RoleOptions { get; set; } = Array.Empty<SelectListItem>();
}

public class AdminEditUserViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Chọn loại tài khoản.")]
    [Display(Name = "Vai trò")]
    public string Role { get; set; } = SD.Role_Customer;

    public IEnumerable<SelectListItem> RoleOptions { get; set; } = Array.Empty<SelectListItem>();
}
