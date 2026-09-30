using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.YES;

[Authorize(AuthenticationSchemes = "Cookies")]
public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;
    public IndexModel(IConfiguration configuration) => _configuration = configuration;
    public string Url { get; private set; } = string.Empty;
    public void OnGet() => Url = _configuration["Yes:Url"] ?? string.Empty;
}
