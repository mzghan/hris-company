using HRIS.Api.Common;
using HRIS.Api.DTOs.Attendance;
using HRIS.Api.DTOs.Auth;
using HRIS.Api.DTOs.Employee;
using HRIS.Api.DTOs.Kpi;
using HRIS.Api.DTOs.Leave;
using HRIS.Api.DTOs.Organization;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;
using HRIS.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Data;

// Urutan seed saat aplikasi pertama kali dijalankan:
//   1. Data REF_* (seed wajib dari dokumen desain DB bab 3.5) dan SYS_Role.
//      Idempotent: tiap tabel hanya diisi kalau masih kosong.
//   2. Akun Support awal (tanpa Employee), supaya tidak ada masalah "ayam-telur"
//      (butuh HR/Support untuk membuat Employee & User, tapi belum ada akun sama sekali).
//   3. Data dummy satu perusahaan kecil (organisasi bertingkat, karyawan, atasan,
//      akun tiap role, absensi, cuti multi-level, KPI) kalau belum ada Employee,
//      supaya semua fitur langsung bisa dicoba tanpa input manual.
public static class DbSeeder
{
    // Foto placeholder (4x4 px abu-abu) + koordinat kantor dummy di Jakarta,
    // dipakai supaya seed data absensi tetap lolos validasi "foto & lokasi
    // wajib" tanpa perlu kamera/GPS sungguhan saat seeding.
    private const string SeedAttendancePhotoBase64 =
        "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL/wAARCAAEAAQDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD0iiiigD//2Q==";

    private const double SeedAttendanceLatitude = -6.2088;
    private const double SeedAttendanceLongitude = 106.8456;

    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var userRepository = services.GetRequiredService<IUserRepository>();
        var authService = services.GetRequiredService<IAuthService>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // --- 1. Data referensi & role ---
        await SeedReferenceDataAsync(db);

        // --- 2. Akun Support awal ---
        // Dicek terpisah dari data dummy di bawah, supaya kalau percobaan seed
        // sebelumnya gagal di tengah jalan, akun ini tidak mengunci data dummy
        // supaya tidak pernah dicoba ulang.
        var supportUsername = config["SeedSupport:Username"] ?? "support";
        var supportPassword = config["SeedSupport:Password"] ?? "Support123!";

        if (!await userRepository.AnyUserExistsAsync())
        {
            await authService.RegisterAsync(new RegisterDto
            {
                Username = supportUsername,
                Password = supportPassword,
                Roles = new List<string> { RoleNames.Support },
                EmployeeId = null
            }, actorIsSupport: true);

            logger.LogWarning(
                "Akun Support awal dibuat: username='{Username}', password default dari appsettings. " +
                "GANTI PASSWORD INI setelah login pertama kali.", supportUsername);
        }

        var support = await userRepository.GetByUsernameAsync(supportUsername);
        if (support is null)
        {
            logger.LogWarning("Akun Support '{Username}' tidak ditemukan, seed data dummy company dilewati.", supportUsername);
            return;
        }

        // --- 3. Data dummy perusahaan ---
        // Guard-nya "apakah sudah ada Employee", bukan "apakah ada User" — supaya
        // kalau sebelumnya hanya akun Support yang berhasil dibuat, run berikutnya
        // tetap mencoba seed ulang data dummy-nya.
        if (await db.Employees.AnyAsync())
            return;

        try
        {
            // Satu transaction eksplisit: semua Service/Repository di bawah berjalan
            // di atas AppDbContext yang sama (satu scope DI), jadi kalau ada yang
            // gagal di tengah, semua baris yang sempat ter-insert ikut di-rollback.
            await using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                await SeedCompanyDataAsync(services, db, support, logger);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            // Data dummy sengaja tidak menggagalkan startup aplikasi: akun Support
            // tetap bisa login dan input data manual. Akan dicoba lagi otomatis
            // saat aplikasi di-restart.
            logger.LogError(ex, "Gagal seed data dummy perusahaan. Support tetap bisa login & input data manual. " +
                "Akan dicoba lagi otomatis saat aplikasi di-restart.");
        }
    }

    // ================= REF_* & SYS_Role =================

    private static async Task SeedReferenceDataAsync(AppDbContext db)
    {
        if (!await db.Roles.AnyAsync())
        {
            db.Roles.AddRange(
                new Role { RoleName = RoleNames.Employee },
                new Role { RoleName = RoleNames.HR },
                new Role { RoleName = RoleNames.Support });
        }

        if (!await db.HierarchyTypes.AnyAsync())
            db.HierarchyTypes.AddRange(
                new HierarchyType { HierarchyTypeName = RefNames.DirectManager },
                new HierarchyType { HierarchyTypeName = RefNames.DottedManager });

        if (!await db.IdentityTypes.AnyAsync())
            db.IdentityTypes.AddRange(new[] { "KTP", "Passport", "KITAS", "KITAP", "NPWP", "BPJS Kesehatan", "BPJS Ketenagakerjaan" }
                .Select(n => new IdentityType { IdentityTypeName = n }));

        if (!await db.ContactTypes.AnyAsync())
            db.ContactTypes.AddRange(new[] { RefNames.WorkEmail, "Personal Email", "Mobile" }
                .Select(n => new ContactType { ContactTypeName = n }));

        if (!await db.Relationships.AnyAsync())
            db.Relationships.AddRange(new[] { "Spouse", "Child", "Parent", "Sibling" }
                .Select(n => new Relationship { RelationshipName = n }));

        if (!await db.EmploymentStatuses.AnyAsync())
            db.EmploymentStatuses.AddRange(new[] { "Active", "Probation", "Resigned", "Terminated" }
                .Select(n => new EmploymentStatus { EmploymentStatusName = n }));

        if (!await db.EmploymentTypes.AnyAsync())
            db.EmploymentTypes.AddRange(
                new EmploymentType { EmploymentTypeName = "Permanent", EmploymentTypeDescription = "Karyawan tetap" },
                new EmploymentType { EmploymentTypeName = "Contract", EmploymentTypeDescription = "Karyawan kontrak" },
                new EmploymentType { EmploymentTypeName = RefNames.Outsource, EmploymentTypeDescription = "Karyawan dari vendor (wajib vendor)" });

        if (!await db.EndReasons.AnyAsync())
            db.EndReasons.AddRange(new[] { "Resigned - better position", "Terminated - not perform", "Contract ended" }
                .Select(n => new EndReason { EndReasonName = n }));

        if (!await db.Genders.AnyAsync())
            db.Genders.AddRange(new[] { "Male", "Female" }.Select(n => new Gender { GenderName = n }));

        if (!await db.MaritalStatuses.AnyAsync())
            db.MaritalStatuses.AddRange(new[] { "Single", "Married", "Divorced", "Widowed" }
                .Select(n => new MaritalStatus { MaritalStatusName = n }));

        if (!await db.Religions.AnyAsync())
            db.Religions.AddRange(new[] { "Islam", "Kristen", "Katolik", "Hindu", "Buddha", "Konghucu" }
                .Select(n => new Religion { ReligionName = n }));

        if (!await db.AddressTypes.AnyAsync())
            db.AddressTypes.AddRange(new[] { "KTP", "Domicile" }.Select(n => new AddressType { AddressTypeName = n }));

        if (!await db.AccountTypes.AnyAsync())
            db.AccountTypes.AddRange(new[] { "Bank Account", "E-Wallet", "Credit Card" }
                .Select(n => new AccountType { AccountTypeName = n }));

        if (!await db.EducationDegrees.AnyAsync())
            db.EducationDegrees.AddRange(new[] { "SMA/SMK", "D3", "S1", "S2", "S3" }
                .Select(n => new EducationDegree { DegreeName = n }));

        if (!await db.Industries.AnyAsync())
            db.Industries.AddRange(new[] { "Information Technology", "Manufacturing", "Finance", "Retail" }
                .Select(n => new Industry { IndustryName = n }));

        // Grade angka 1-8 (makin besar makin tinggi).
        if (!await db.Grades.AnyAsync())
            db.Grades.AddRange(Enumerable.Range(1, 8)
                .Select(i => new Grade { GradeLevel = i, GradeDescription = $"Grade {i}" }));

        // Job level: urutan perkiraan dari dokumen desain, bisa dikoreksi kapan saja.
        if (!await db.JobLevels.AnyAsync())
        {
            var levels = new[] { "Staff", "Senior Staff", "Supervisor", "Assistant Manager", "Manager", "Senior Manager", "Head", "Group Head" };
            db.JobLevels.AddRange(levels.Select((n, i) => new JobLevel { JobLevelName = n, LevelOrder = i + 1 }));
        }

        // ID = kode ISO. Ekspatriat ditentukan dari nationality_country_id bukan Indonesia.
        if (!await db.Countries.AnyAsync())
            db.Countries.AddRange(
                new Country { CountryName = "Indonesia", CountryCode = "ID" },
                new Country { CountryName = "Japan", CountryCode = "JP" },
                new Country { CountryName = "Singapore", CountryCode = "SG" },
                new Country { CountryName = "Malaysia", CountryCode = "MY" });

        if (!await db.Locations.AnyAsync())
            db.Locations.AddRange(new[] { "Head Office Jakarta", "Branch Surabaya" }
                .Select(n => new Location { LocationName = n }));

        if (!await db.JobTitles.AnyAsync())
            db.JobTitles.AddRange(new[]
            {
                "Head of Operations", "Engineering Manager", "Software Engineer",
                "Finance Manager", "Finance Staff", "HR Staff"
            }.Select(n => new JobTitle { JobTitleName = n }));

        if (!await db.Vendors.AnyAsync())
            db.Vendors.Add(new Vendor { VendorName = "PT Vendor Contoh" });

        await db.SaveChangesAsync();

        // Provinsi & kota butuh Id negara, jadi disimpan setelah Country.
        if (!await db.Provinces.AnyAsync())
        {
            var indonesia = await db.Countries.FirstAsync(c => c.CountryCode == "ID");
            var jakarta = new Province { ProvinceName = "DKI Jakarta", CountryId = indonesia.Id };
            db.Provinces.Add(jakarta);
            await db.SaveChangesAsync();

            db.Cities.AddRange(
                new City { CityName = "Jakarta Selatan", ProvinceId = jakarta.Id },
                new City { CityName = "Jakarta Pusat", ProvinceId = jakarta.Id });
            await db.SaveChangesAsync();
        }

        await SeedDocumentCategoriesAsync(db);
        await SeedApprovalFlowAsync(db);
        await SeedBatchBContentAsync(db);
    }

    // Kategori awal dokumen (modul 1 Learning, 15 HR Forms, 16 Regulation, dokumen pribadi).
    private static async Task SeedDocumentCategoriesAsync(AppDbContext db)
    {
        if (await db.DocumentCategories.AnyAsync()) return;

        var forms = new DocumentCategory { Name = "Forms" };
        db.DocumentCategories.AddRange(
            forms,
            new DocumentCategory { Name = "Learning" },
            new DocumentCategory { Name = "Regulations" },
            new DocumentCategory { Name = "Personal Documents" });
        await db.SaveChangesAsync();

        db.DocumentCategories.AddRange(
            new DocumentCategory { Name = "Medical", ParentId = forms.Id },
            new DocumentCategory { Name = "General", ParentId = forms.Id });
        await db.SaveChangesAsync();
    }

    // Alur approval awal (bab 4.2 dokumen desain). Semua bisa diubah HR lewat
    // PUT /api/approvals/flows/{id} tanpa deploy. PersonalAction sengaja belum diseed:
    // alurnya (manager lama -> manager baru -> HR) dikerjakan di Batch E.
    private static async Task SeedApprovalFlowAsync(AppDbContext db)
    {
        if (await db.ApprovalFlowSteps.AnyAsync()) return;

        var hrRoleId = (await db.Roles.FirstAsync(r => r.RoleName == RoleNames.HR)).Id;

        ApprovalFlowStep Chain(string type, int level, int depth, int? minDays = null) => new()
        {
            RequestType = type, Level = level, ApproverType = ApproverType.ManagerChain,
            ChainDepth = depth, MinRequestedDays = minDays
        };
        ApprovalFlowStep HrRole(string type, int level) => new()
        {
            RequestType = type, Level = level, ApproverType = ApproverType.Role, RoleId = hrRoleId
        };

        db.ApprovalFlowSteps.AddRange(
            // Leave: atasan langsung; cuti panjang (>= 6 hari kerja, angka perkiraan) ditambah Head.
            Chain(ApprovalRequestTypes.Leave, 1, 1),
            Chain(ApprovalRequestTypes.Leave, 2, 2, minDays: 6),

            // Manpower: atasan -> Head -> HR.
            Chain(ApprovalRequestTypes.Manpower, 1, 1),
            Chain(ApprovalRequestTypes.Manpower, 2, 2),
            HrRole(ApprovalRequestTypes.Manpower, 3),

            // Benefit, Letter, Parking: HR. Laptop: HR, lalu status lanjutan oleh Support (Batch C).
            HrRole(ApprovalRequestTypes.FamilyChange, 1),
            HrRole(ApprovalRequestTypes.LeaveEncashment, 1),
            HrRole(ApprovalRequestTypes.HealthClaim, 1),
            HrRole(ApprovalRequestTypes.Letter, 1),
            HrRole(ApprovalRequestTypes.Parking, 1),
            HrRole(ApprovalRequestTypes.Laptop, 1));

        await db.SaveChangesAsync();
    }


    // Seed awal Batch B untuk memberi contoh konten regulasi saat database development direset.
    // Konten ini hanya data dummy dan boleh diubah/dihapus HR.
    private static async Task SeedBatchBContentAsync(AppDbContext db)
    {
        if (await db.Regulations.AnyAsync()) return;

        db.Regulations.AddRange(
            new Regulation
            {
                Title = "Jam Kerja & Kehadiran",
                Content = "Contoh regulasi development. Ganti dengan kebijakan perusahaan yang sebenarnya.",
                SortOrder = 1
            },
            new Regulation
            {
                Title = "Penggunaan Fasilitas Perusahaan",
                Content = "Contoh regulasi development. Ganti dengan kebijakan perusahaan yang sebenarnya.",
                SortOrder = 2
            });
        await db.SaveChangesAsync();
    }

    // ================= Data dummy perusahaan =================

    private static async Task SeedCompanyDataAsync(IServiceProvider services, AppDbContext db, User support, ILogger logger)
    {
        const string defaultPassword = "Password123!";

        var organizationService = services.GetRequiredService<IOrganizationService>();
        var employeeService = services.GetRequiredService<IEmployeeService>();
        var authService = services.GetRequiredService<IAuthService>();
        var attendanceService = services.GetRequiredService<IAttendanceService>();
        var leaveService = services.GetRequiredService<ILeaveRequestService>();
        var approvalService = services.GetRequiredService<IApprovalService>();
        var kpiService = services.GetRequiredService<IKpiService>();

        // --- Organisasi bertingkat ---
        var company = await organizationService.CreateAsync(new OrganizationCreateDto { OrganizationName = "PT Contoh Nusantara" });
        var engineering = await organizationService.CreateAsync(new OrganizationCreateDto { OrganizationName = "Engineering", ParentId = company.Id });
        var finance = await organizationService.CreateAsync(new OrganizationCreateDto { OrganizationName = "Finance", ParentId = company.Id });
        var humanResources = await organizationService.CreateAsync(new OrganizationCreateDto { OrganizationName = "Human Resources", ParentId = company.Id });

        // --- Lookup Id dari seed REF_* (berdasarkan nama) ---
        var permanentId = (await db.EmploymentTypes.FirstAsync(t => t.EmploymentTypeName == "Permanent")).Id;
        var activeId = (await db.EmploymentStatuses.FirstAsync(s => s.EmploymentStatusName == "Active")).Id;
        var locationId = (await db.Locations.FirstAsync()).Id;
        var indonesiaId = (await db.Countries.FirstAsync(c => c.CountryCode == "ID")).Id;
        var levels = await db.JobLevels.ToDictionaryAsync(l => l.JobLevelName, l => l.Id);
        var titles = await db.JobTitles.ToDictionaryAsync(t => t.JobTitleName, t => t.Id);
        var grades = await db.Grades.ToDictionaryAsync(g => g.GradeLevel, g => g.Id);

        async Task<EmployeeResponseDto> CreateEmployeeAsync(
            string number, string name, string email, DateOnly joinDate, int organizationId,
            string title, string level, int gradeLevel, int? managerId) =>
            await employeeService.CreateAsync(new EmployeeCreateDto
            {
                EmployeeNumber = number,
                FullName = name,
                WorkEmail = email,
                JoinDate = joinDate,
                NationalityCountryId = indonesiaId,
                EmploymentTypeId = permanentId,
                EmploymentStatusId = activeId,
                OrganizationId = organizationId,
                LocationId = locationId,
                JobTitleId = titles[title],
                JobLevelId = levels[level],
                GradeId = grades[gradeLevel],
                DirectManagerId = managerId
            });

        // --- Karyawan berjenjang (atasan lewat MST_Employee_Hierarchy) ---
        // Budi (Head) -> Siti (Eng Manager) -> Dewi, Rudi
        //             -> Andi (Finance Manager) -> Maya
        //             -> Hana (HR)
        var budi = await CreateEmployeeAsync("EMP-0001", "Budi Santoso", "budi@contoh.co.id", new DateOnly(2020, 1, 5), company.Id, "Head of Operations", "Head", 7, null);
        var siti = await CreateEmployeeAsync("EMP-0002", "Siti Rahayu", "siti@contoh.co.id", new DateOnly(2021, 3, 10), engineering.Id, "Engineering Manager", "Manager", 5, budi.Id);
        var andi = await CreateEmployeeAsync("EMP-0003", "Andi Wijaya", "andi@contoh.co.id", new DateOnly(2021, 4, 1), finance.Id, "Finance Manager", "Manager", 5, budi.Id);
        var dewi = await CreateEmployeeAsync("EMP-0004", "Dewi Lestari", "dewi@contoh.co.id", new DateOnly(2022, 6, 15), engineering.Id, "Software Engineer", "Staff", 2, siti.Id);
        var rudi = await CreateEmployeeAsync("EMP-0005", "Rudi Hartono", "rudi@contoh.co.id", new DateOnly(2022, 8, 1), engineering.Id, "Software Engineer", "Staff", 2, siti.Id);
        var maya = await CreateEmployeeAsync("EMP-0006", "Maya Putri", "maya@contoh.co.id", new DateOnly(2023, 2, 20), finance.Id, "Finance Staff", "Staff", 1, andi.Id);
        var hana = await CreateEmployeeAsync("EMP-0007", "Hana Kusuma", "hana@contoh.co.id", new DateOnly(2021, 9, 1), humanResources.Id, "HR Staff", "Senior Staff", 3, budi.Id);

        // --- Akun login. Semua dapat role Employee otomatis; Hana tambahan HR.
        // Budi/Siti/Andi otomatis dapat claim IsManager karena punya bawahan aktif. ---
        async Task<AuthResponseDto> CreateUserAsync(string username, int employeeId, params string[] extraRoles) =>
            await authService.RegisterAsync(new RegisterDto
            {
                Username = username,
                Password = defaultPassword,
                Roles = extraRoles.ToList(),
                EmployeeId = employeeId
            }, actorIsSupport: true);

        var budiUser = await CreateUserAsync("budi", budi.Id);
        var sitiUser = await CreateUserAsync("siti", siti.Id);
        await CreateUserAsync("andi", andi.Id);
        await CreateUserAsync("dewi", dewi.Id);
        await CreateUserAsync("rudi", rudi.Id);
        await CreateUserAsync("maya", maya.Id);
        await CreateUserAsync("hana", hana.Id, RoleNames.HR);

        // --- Absensi contoh (Dewi & Rudi) ---
        foreach (var empId in new[] { dewi.Id, rudi.Id })
        {
            await attendanceService.CheckInAsync(empId, new AttendanceCheckInDto
            {
                PhotoBase64 = SeedAttendancePhotoBase64,
                Latitude = SeedAttendanceLatitude,
                Longitude = SeedAttendanceLongitude
            });
            await attendanceService.CheckOutAsync(empId, new AttendanceCheckOutDto
            {
                PhotoBase64 = SeedAttendancePhotoBase64,
                Latitude = SeedAttendanceLatitude,
                Longitude = SeedAttendanceLongitude
            });
        }

        // --- Leave request contoh, lewat approval engine. Tanggal dihitung dari hari Senin
        // supaya jumlah hari kerjanya pasti (tidak bergantung hari saat seeder jalan). ---
        // 1) Dewi: 3 hari kerja, Pending di langkah 1 (Siti).
        // 2) Rudi: 10 hari kerja (>= 6) sehingga butuh 2 langkah: Siti lalu Budi (Head). Sudah Approved.
        var today = JakartaTime.Today();
        var nextMonday = MondayOnOrBefore(today.AddDays(14));
        await leaveService.CreateAsync(dewi.Id, new LeaveRequestCreateDto
        {
            StartDate = nextMonday,
            EndDate = nextMonday.AddDays(2),
            Reason = "Acara keluarga"
        });

        var pastMonday = MondayOnOrBefore(today.AddDays(-28));
        var rudiLeave = await leaveService.CreateAsync(rudi.Id, new LeaveRequestCreateDto
        {
            StartDate = pastMonday,
            EndDate = pastMonday.AddDays(11),
            Reason = "Cuti tahunan"
        });

        var employeeRoles = new[] { RoleNames.Employee };
        await approvalService.ApproveAsync(rudiLeave.ApprovalId!.Value,
            new UserContext(sitiUser.UserId, siti.Id, employeeRoles), "Disetujui, jadwal tim aman.");
        await approvalService.ApproveAsync(rudiLeave.ApprovalId!.Value,
            new UserContext(budiUser.UserId, budi.Id, employeeRoles), "OK.");

        // --- KPI: kriteria + satu periode terisi + satu contoh override Manager ---
        await kpiService.CreateCriteriaAsync(new KpiCriteriaCreateDto { Name = "Kedisiplinan", Weight = 30 });
        await kpiService.CreateCriteriaAsync(new KpiCriteriaCreateDto { Name = "Produktivitas", Weight = 40 });
        await kpiService.CreateCriteriaAsync(new KpiCriteriaCreateDto { Name = "Kerja Sama Tim", Weight = 30 });
        var criteria = await kpiService.GetAllCriteriaAsync();

        var quarter = (DateTime.Today.Month - 1) / 3 + 1;
        var kpiPeriod = await kpiService.CreatePeriodAsync(new KpiPeriodCreateDto { Name = $"Q{quarter} {DateTime.Today.Year}", Year = DateTime.Today.Year });

        var dewiScores = await kpiService.FillScoresAsync(kpiPeriod.Id, support.Id, new EmployeeKpiScoreFillDto
        {
            EmployeeId = dewi.Id,
            Scores = criteria.Select(c => new KpiScoreItemDto { CriteriaId = c.Id, Score = 80 }).ToList()
        });
        await kpiService.FillScoresAsync(kpiPeriod.Id, support.Id, new EmployeeKpiScoreFillDto
        {
            EmployeeId = rudi.Id,
            Scores = criteria.Select(c => new KpiScoreItemDto { CriteriaId = c.Id, Score = 75 }).ToList()
        });
        await kpiService.FillScoresAsync(kpiPeriod.Id, support.Id, new EmployeeKpiScoreFillDto
        {
            EmployeeId = maya.Id,
            Scores = criteria.Select(c => new KpiScoreItemDto { CriteriaId = c.Id, Score = 85 }).ToList()
        });

        // Siti me-review lalu override satu nilai Dewi, supaya audit trail
        // (KpiScoreRevision) langsung ada datanya.
        var produktivitasScore = dewiScores.First(s => s.CriteriaName == "Produktivitas");
        await kpiService.OverrideScoreAsync(produktivitasScore.Id, siti.Id, sitiUser.UserId, new KpiScoreOverrideDto
        {
            NewScore = 90,
            Note = "Menyelesaikan migrasi database lebih cepat dari target, dinaikkan dari nilai awal."
        });

        logger.LogInformation(
            "Data dummy perusahaan berhasil di-seed: 4 organisasi, 7 employee, 1 kpi period.");
    }

    // Senin terdekat pada atau sebelum tanggal ini.
    private static DateOnly MondayOnOrBefore(DateOnly date)
    {
        var offset = ((int)date.DayOfWeek + 6) % 7; // Senin = 0
        return date.AddDays(-offset);
    }
}
