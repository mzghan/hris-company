using HRIS.Api.Data;
using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Repositories;

public class ReferenceRepository : IReferenceRepository
{
    private readonly AppDbContext _context;

    public ReferenceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync<T>(int id) where T : class =>
        (await _context.Set<T>().FindAsync(id)) is not null;

    public async Task<EmploymentType?> GetEmploymentTypeAsync(int id) =>
        await _context.EmploymentTypes.FirstOrDefaultAsync(t => t.Id == id);

    public async Task<int?> GetHierarchyTypeIdAsync(string name) =>
        await _context.HierarchyTypes
            .Where(t => t.HierarchyTypeName == name)
            .Select(t => (int?)t.Id)
            .FirstOrDefaultAsync();

    public async Task<int?> GetContactTypeIdAsync(string name) =>
        await _context.ContactTypes
            .Where(t => t.ContactTypeName == name)
            .Select(t => (int?)t.Id)
            .FirstOrDefaultAsync();

    public async Task<List<ReferenceOption>?> GetOptionsAsync(string type)
    {
        List<ReferenceOption>? rows = type.ToLowerInvariant() switch
        {
            "country" => await _context.Countries.OrderBy(x => x.CountryName).Select(x => new ReferenceOption(x.Id, x.CountryName)).ToListAsync(),
            "province" => await _context.Provinces.OrderBy(x => x.ProvinceName).Select(x => new ReferenceOption(x.Id, x.ProvinceName)).ToListAsync(),
            "city" => await _context.Cities.OrderBy(x => x.CityName).Select(x => new ReferenceOption(x.Id, x.CityName)).ToListAsync(),
            "addresstype" => await _context.AddressTypes.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.AddressTypeName)).ToListAsync(),
            "accounttype" => await _context.AccountTypes.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.AccountTypeName)).ToListAsync(),
            "hierarchytype" => await _context.HierarchyTypes.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.HierarchyTypeName)).ToListAsync(),
            "identitytype" => await _context.IdentityTypes.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.IdentityTypeName)).ToListAsync(),
            "contacttype" => await _context.ContactTypes.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.ContactTypeName)).ToListAsync(),
            "religion" => await _context.Religions.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.ReligionName)).ToListAsync(),
            "gender" => await _context.Genders.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.GenderName)).ToListAsync(),
            "maritalstatus" => await _context.MaritalStatuses.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.MaritalStatusName)).ToListAsync(),
            "relationship" => await _context.Relationships.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.RelationshipName)).ToListAsync(),
            "vendor" => await _context.Vendors.OrderBy(x => x.VendorName).Select(x => new ReferenceOption(x.Id, x.VendorName)).ToListAsync(),
            "employmenttype" => await _context.EmploymentTypes.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.EmploymentTypeName)).ToListAsync(),
            "employmentstatus" => await _context.EmploymentStatuses.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.EmploymentStatusName)).ToListAsync(),
            "endreason" => await _context.EndReasons.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.EndReasonName)).ToListAsync(),
            "location" => await _context.Locations.OrderBy(x => x.LocationName).Select(x => new ReferenceOption(x.Id, x.LocationName)).ToListAsync(),
            "jobtitle" => await _context.JobTitles.OrderBy(x => x.JobTitleName).Select(x => new ReferenceOption(x.Id, x.JobTitleName)).ToListAsync(),
            "joblevel" => await _context.JobLevels.OrderBy(x => x.LevelOrder).Select(x => new ReferenceOption(x.Id, x.JobLevelName)).ToListAsync(),
            "educationdegree" => await _context.EducationDegrees.OrderBy(x => x.Id).Select(x => new ReferenceOption(x.Id, x.DegreeName)).ToListAsync(),
            "educationtitle" => await _context.EducationTitles.OrderBy(x => x.TitleName).Select(x => new ReferenceOption(x.Id, x.TitleName)).ToListAsync(),
            "university" => await _context.Universities.OrderBy(x => x.UniversityName).Select(x => new ReferenceOption(x.Id, x.UniversityName)).ToListAsync(),
            "industry" => await _context.Industries.OrderBy(x => x.IndustryName).Select(x => new ReferenceOption(x.Id, x.IndustryName)).ToListAsync(),
            "grade" => (await _context.Grades.OrderBy(x => x.GradeLevel).ToListAsync())
                .Select(x => new ReferenceOption(x.Id, $"Grade {x.GradeLevel}")).ToList(),
            _ => null
        };

        return rows;
    }
}
