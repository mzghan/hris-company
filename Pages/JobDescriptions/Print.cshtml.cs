using HRIS.Api.Controllers;
using HRIS.Api.Common;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace HRIS.Api.Pages.JobDescriptions;
[Authorize(AuthenticationSchemes="Cookies")]
public class PrintModel:PageModel
{
    private readonly IJobDescriptionService _service;
    public PrintModel(IJobDescriptionService service)=>_service=service;
    public DTOs.JobDescription.JobDescriptionResponseDto Item {get;set;}=new();
    public async Task<IActionResult> OnGetAsync(int id){try{Item=await _service.GetByIdAsync(id,User.ToUserContext());return Page();}catch(NotFoundException){return NotFound();}}
}
