using HRIS.Api.DTOs.Document;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _service;

    public DocumentsController(IDocumentService service)
    {
        _service = service;
    }

    // Dokumen umum + dokumen pribadi milik sendiri. HR/Support melihat semuanya.
    [HttpGet]
    public async Task<ActionResult<List<DocumentResponseDto>>> GetList([FromQuery] int? categoryId, [FromQuery] int? ownerEmployeeId) =>
        Ok(await _service.GetListAsync(User.ToUserContext(), categoryId, ownerEmployeeId));

    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var download = await _service.OpenAsync(id, User.ToUserContext());
        return File(download.Content, download.ContentType, download.FileName);
    }

    // multipart/form-data: file, categoryId, title, ownerEmployeeId (opsional).
    [HttpPost]
    [Authorize(Roles = "HR,Support")]
    [RequestSizeLimit(26_214_400)]
    public async Task<ActionResult<DocumentResponseDto>> Upload([FromForm] DocumentUploadForm form)
    {
        if (form.File is null)
            throw new BadRequestException("File wajib diunggah.");

        await using var stream = form.File.OpenReadStream();
        var created = await _service.UploadAsync(
            User.ToUserContext(), form.CategoryId, form.Title, form.OwnerEmployeeId,
            form.File.FileName, form.File.ContentType, form.File.Length, stream);
        return Ok(created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<DocumentResponseDto>> Update(int id, DocumentUpdateDto dto) =>
        Ok(await _service.UpdateAsync(id, User.ToUserContext(), dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, User.ToUserContext());
        return NoContent();
    }
}
