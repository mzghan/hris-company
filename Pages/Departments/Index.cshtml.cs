using System.ComponentModel.DataAnnotations;
using HRIS.Api.DTOs.Department;
using HRIS.Api.Exceptions;
using HRIS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HRIS.Api.Pages.Departments;

[Authorize(AuthenticationSchemes = "Cookies", Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IDepartmentService _service;

    public IndexModel(IDepartmentService service)
    {
        _service = service;
    }

    public List<DepartmentResponseDto> Departments { get; set; } = new();

    [BindProperty]
    public NewDepartmentInput NewDepartment { get; set; } = new();

    public class NewDepartmentInput
    {
        [Required(ErrorMessage = "Nama departemen wajib diisi.")]
        public string Name { get; set; } = string.Empty;
    }

    public async Task OnGetAsync()
    {
        Departments = await _service.GetAllAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            Departments = await _service.GetAllAsync();
            return Page();
        }

        await _service.CreateAsync(new DepartmentCreateDto { Name = NewDepartment.Name });
        TempData["Success"] = $"Departemen '{NewDepartment.Name}' berhasil ditambahkan.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRenameAsync(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Nama departemen tidak boleh kosong.";
            return RedirectToPage();
        }

        try
        {
            await _service.UpdateAsync(id, new DepartmentCreateDto { Name = name });
            TempData["Success"] = "Nama departemen berhasil diperbarui.";
        }
        catch (NotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            TempData["Success"] = "Departemen berhasil dihapus.";
        }
        catch (NotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception)
        {
            TempData["Error"] = "Departemen ini tidak bisa dihapus karena masih punya karyawan di dalamnya.";
        }
        return RedirectToPage();
    }
}
