using HRIS.Api.DTOs.Document;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.Api.Controllers;

[ApiController]
[Route("api/document-categories")]
[Authorize]
public class DocumentCategoriesController : ControllerBase
{
    private readonly IDocumentService _service;

    public DocumentCategoriesController(IDocumentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<DocumentCategoryResponseDto>>> GetAll() =>
        Ok(await _service.GetCategoriesAsync());

    [HttpPost]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<DocumentCategoryResponseDto>> Create(DocumentCategoryCreateDto dto) =>
        Ok(await _service.CreateCategoryAsync(dto));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<ActionResult<DocumentCategoryResponseDto>> Update(int id, DocumentCategoryCreateDto dto) =>
        Ok(await _service.UpdateCategoryAsync(id, dto));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "HR,Support")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteCategoryAsync(id);
        return NoContent();
    }
}
