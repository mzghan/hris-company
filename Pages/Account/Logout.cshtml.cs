using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Account;

// [IgnoreAntiforgeryToken]: Logout sering diakses setelah cookie auth/antiforgery
// sudah lama (sliding expiration 8 jam) atau setelah aplikasi baru saja
// direstart saat development (Data Protection key ring berubah, jadi token
// antiforgery lama tidak bisa divalidasi lagi). Kalau validasi ini gagal,
// Razor Pages langsung mengembalikan HTTP 400 kosong SEBELUM request sampai
// ke ExceptionHandlingMiddleware kita (makanya muncul sebagai error browser
// polos, bukan JSON error yang biasa). Logout adalah operasi aman &
// idempoten (cuma clear cookie), jadi wajar dikecualikan dari validasi ini.
[AllowAnonymous]
[IgnoreAntiforgeryToken]
public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToPage("/Account/Login");
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToPage("/Account/Login");
    }
}
