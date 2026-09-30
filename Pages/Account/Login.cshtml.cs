using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HRIS.Api.DTOs.Auth;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly IAuthService _authService;

    public LoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Username wajib diisi.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Reuse persis logic Service yang sama dipakai endpoint API
            // /api/auth/login (verifikasi BCrypt), supaya tidak ada
            // duplikasi aturan autentikasi antara API dan frontend.
            AuthResponseDto result = await _authService.LoginAsync(new LoginDto
            {
                Username = Input.Username,
                Password = Input.Password
            });

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new(ClaimTypes.Name, result.Username)
            };

            claims.AddRange(result.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // Claim IsManager dibaca saat login (bukan role tersimpan); kalau atasan
            // baru ditambahkan setelahnya, berlaku setelah login ulang.
            if (result.IsManager)
            {
                claims.Add(new Claim("IsManager", "true"));
            }

            if (result.EmployeeId is not null)
            {
                claims.Add(new Claim("employeeId", result.EmployeeId.Value.ToString()));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToPage("/Index");
        }
        catch (BadRequestException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
