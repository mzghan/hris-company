using HRIS.Api.DTOs.Attendance;
using HRIS.Api.DTOs.Kpi;
using HRIS.Api.DTOs.Leave;
using HRIS.Api.DTOs.Payroll;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;
using HRIS.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Data;

// Seed satu akun Admin awal saat aplikasi pertama kali dijalankan,
// supaya tidak ada masalah "ayam-telur" (butuh Admin untuk membuat
// Employee & User, tapi belum ada akun sama sekali untuk login).
//
// Sekaligus (kalau belum ada user sama sekali, artinya DB baru) di-seed
// data dummy satu perusahaan kecil lengkap dengan Department, Employee
// berjenjang, akun login tiap role, riwayat gaji, absensi, pengajuan
// cuti multi-level, satu PayrollPeriod, dan satu KpiPeriod terisi —
// supaya semua fitur (termasuk Payroll & KPI) langsung bisa dicoba
// tanpa harus input manual satu-satu dulu.
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
        var userRepository = services.GetRequiredService<IUserRepository>();
        var employeeRepository = services.GetRequiredService<IEmployeeRepository>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // --- 1. Akun Admin awal (seperti sebelumnya) ---
        // Dicek terpisah dari data dummy perusahaan di bawah, supaya kalau
        // percobaan seed sebelumnya sempat gagal di tengah jalan (mis. karena
        // migration yang belum lengkap) dan Admin sudah kadung ke-create,
        // itu tidak mengunci data dummy company supaya tidak pernah dicoba
        // ulang lagi. "Sudah ada User" dan "sudah ada data dummy company"
        // sekarang jadi dua pertanyaan yang terpisah.
        User admin;
        if (!await userRepository.AnyUserExistsAsync())
        {
            var adminUsername = config["SeedAdmin:Username"] ?? "admin";
            var adminPassword = config["SeedAdmin:Password"] ?? "Admin123!";

            admin = new User
            {
                Username = adminUsername,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                Role = UserRole.Admin,
                EmployeeId = null
            };
            await userRepository.AddAsync(admin);

            logger.LogWarning(
                "Akun Admin awal dibuat: username='{Username}', password default dari appsettings. " +
                "GANTI PASSWORD INI setelah login pertama kali.", adminUsername);
        }
        else
        {
            // Admin (atau user lain) sudah ada dari percobaan sebelumnya.
            // Ambil akun admin yang sudah ada supaya tetap bisa dipakai
            // sebagai FilledByUserId dkk kalau data dummy company di bawah
            // belum sempat ke-seed.
            var adminUsername = config["SeedAdmin:Username"] ?? "admin";
            admin = await userRepository.GetByUsernameAsync(adminUsername)
                ?? new User { Id = 0, Username = adminUsername, Role = UserRole.Admin };
        }

        // --- 2. Data dummy perusahaan (Department + Employee berjenjang + akun) ---
        // Guard-nya sengaja BUKAN "apakah ada User", tapi "apakah sudah ada
        // Employee" — supaya kalau sebelumnya cuma Admin yang berhasil dibuat
        // (mis. run pertama gagal saat bikin data KPI karena tabelnya belum
        // ada), run berikutnya tetap mencoba seed ulang data dummy-nya,
        // bukan langsung skip selamanya.
        var existingEmployees = await employeeRepository.GetAllAsync();
        if (existingEmployees.Count > 0)
            return;

        if (admin.Id == 0)
        {
            // Tidak ada akun admin sama sekali yang bisa dipakai (kasus yang
            // seharusnya tidak terjadi lagi, tapi dijaga supaya tidak NRE).
            logger.LogWarning("Tidak ada akun Admin ditemukan, seed data dummy company dilewati.");
            return;
        }

        try
        {
            // Dibungkus satu transaction eksplisit: SeedCompanyDataAsync memanggil
            // banyak repository yang masing-masing SaveChangesAsync sendiri-sendiri
            // (per baris), tapi karena semuanya jalan di atas AppDbContext yang
            // SAMA (satu scope DI), membungkusnya dalam transaction bikin semua
            // SaveChanges itu ikut satu transaction yang sama juga. Kalau ada
            // exception di tengah (mis. tabel KPI belum ke-migrate), transaction
            // di-rollback dan SEMUA baris yang sempat ke-insert (Department,
            // Employee, dst) ikut hilang lagi — bukan nyangkut di tengah kayak
            // sebelumnya. Employees.Count di run berikutnya jadi selalu akurat:
            // 0 kalau belum pernah sukses penuh, atau 6 kalau sudah sukses penuh.
            var db = services.GetRequiredService<AppDbContext>();
            await using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                await SeedCompanyDataAsync(services, admin, logger);
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
            // Data dummy sengaja tidak menggagalkan startup aplikasi kalau
            // ada yang meleset (mis. konfigurasi PayrollApproval tidak cocok
            // dengan Id employee yang ke-generate) — Admin tetap bisa login
            // dan input data manual seperti biasa. Karena rollback di atas,
            // kegagalan di sini tidak menyisakan data setengah jadi — akan
            // dicoba ulang lagi dari nol otomatis saat aplikasi di-restart.
            logger.LogError(ex, "Gagal seed data dummy perusahaan. Admin tetap bisa login & input data manual. " +
                "Akan dicoba lagi otomatis saat aplikasi di-restart.");
        }
    }

    private static async Task SeedCompanyDataAsync(IServiceProvider services, User admin, ILogger logger)
    {
        const string defaultPassword = "Password123!";

        var departmentRepository = services.GetRequiredService<IDepartmentRepository>();
        var employeeRepository = services.GetRequiredService<IEmployeeRepository>();
        var userRepository = services.GetRequiredService<IUserRepository>();
        var salaryService = services.GetRequiredService<IEmployeeSalaryService>();
        var attendanceService = services.GetRequiredService<IAttendanceService>();
        var leaveService = services.GetRequiredService<ILeaveRequestService>();
        var payrollService = services.GetRequiredService<IPayrollService>();
        var kpiService = services.GetRequiredService<IKpiService>();

        // --- Department ---
        var engineering = await departmentRepository.AddAsync(new Department { Name = "Engineering" });
        var finance = await departmentRepository.AddAsync(new Department { Name = "Finance" });

        // --- Employee berjenjang ---
        // Id yang dihasilkan (DB baru, urutan insert ini) akan jadi 1..6,
        // sengaja disusun supaya Budi=Id1 & Siti=Id2 cocok dengan konfigurasi
        // "PayrollApproval:ApproverEmployeeIds": "2,1" di appsettings.json
        // (level 1 = Manager Engineering, level 2 = Direktur).
        var budi = await employeeRepository.AddAsync(new Employee
        {
            FullName = "Budi Santoso",
            Email = "budi.santoso@hris.local",
            Position = "Direktur Utama",
            HireDate = new DateTime(2020, 1, 5),
            DepartmentId = null,
            ManagerId = null
        });

        var siti = await employeeRepository.AddAsync(new Employee
        {
            FullName = "Siti Aminah",
            Email = "siti.aminah@hris.local",
            Position = "Engineering Manager",
            HireDate = new DateTime(2021, 3, 10),
            DepartmentId = engineering.Id,
            ManagerId = budi.Id
        });

        var andi = await employeeRepository.AddAsync(new Employee
        {
            FullName = "Andi Wijaya",
            Email = "andi.wijaya@hris.local",
            Position = "Finance Manager",
            HireDate = new DateTime(2021, 4, 1),
            DepartmentId = finance.Id,
            ManagerId = budi.Id
        });

        var dewi = await employeeRepository.AddAsync(new Employee
        {
            FullName = "Dewi Lestari",
            Email = "dewi.lestari@hris.local",
            Position = "Backend Engineer",
            HireDate = new DateTime(2022, 6, 15),
            DepartmentId = engineering.Id,
            ManagerId = siti.Id
        });

        var rudi = await employeeRepository.AddAsync(new Employee
        {
            FullName = "Rudi Hartono",
            Email = "rudi.hartono@hris.local",
            Position = "Frontend Engineer",
            HireDate = new DateTime(2022, 8, 1),
            DepartmentId = engineering.Id,
            ManagerId = siti.Id
        });

        var maya = await employeeRepository.AddAsync(new Employee
        {
            FullName = "Maya Putri",
            Email = "maya.putri@hris.local",
            Position = "Finance Staff",
            HireDate = new DateTime(2023, 2, 20),
            DepartmentId = finance.Id,
            ManagerId = andi.Id
        });

        // --- Akun login untuk tiap employee (password sama semua untuk demo) ---
        async Task<User> CreateUserAsync(string username, UserRole role, int employeeId)
        {
            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                Role = role,
                EmployeeId = employeeId
            };
            return await userRepository.AddAsync(user);
        }

        var budiUser = await CreateUserAsync("budi", UserRole.Manager, budi.Id);
        var sitiUser = await CreateUserAsync("siti", UserRole.Manager, siti.Id);
        await CreateUserAsync("andi", UserRole.Manager, andi.Id);
        await CreateUserAsync("dewi", UserRole.Employee, dewi.Id);
        var rudiUser = await CreateUserAsync("rudi", UserRole.Employee, rudi.Id);
        await CreateUserAsync("maya", UserRole.Employee, maya.Id);

        logger.LogWarning(
            "Akun dummy dibuat (password semua: '{Password}'): admin, budi (Manager/Direktur), " +
            "siti (Manager Engineering), andi (Manager Finance), dewi, rudi, maya (Employee).",
            defaultPassword);

        // --- Riwayat gaji (dibutuhkan sebelum bikin PayrollPeriod) ---
        var effectiveDate = new DateOnly(DateTime.Today.Year, 1, 1);
        await salaryService.CreateAsync(new EmployeeSalaryCreateDto { EmployeeId = budi.Id, BaseSalary = 25_000_000, AllowanceTotal = 5_000_000, EffectiveDate = effectiveDate });
        await salaryService.CreateAsync(new EmployeeSalaryCreateDto { EmployeeId = siti.Id, BaseSalary = 15_000_000, AllowanceTotal = 3_000_000, EffectiveDate = effectiveDate });
        await salaryService.CreateAsync(new EmployeeSalaryCreateDto { EmployeeId = andi.Id, BaseSalary = 15_000_000, AllowanceTotal = 3_000_000, EffectiveDate = effectiveDate });
        await salaryService.CreateAsync(new EmployeeSalaryCreateDto { EmployeeId = dewi.Id, BaseSalary = 8_000_000, AllowanceTotal = 1_000_000, EffectiveDate = effectiveDate });
        await salaryService.CreateAsync(new EmployeeSalaryCreateDto { EmployeeId = rudi.Id, BaseSalary = 8_000_000, AllowanceTotal = 1_000_000, EffectiveDate = effectiveDate });
        await salaryService.CreateAsync(new EmployeeSalaryCreateDto { EmployeeId = maya.Id, BaseSalary = 7_500_000, AllowanceTotal = 1_000_000, EffectiveDate = effectiveDate });

        // --- Absensi contoh (beberapa hari terakhir untuk Dewi & Rudi) ---
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

        // --- Leave request contoh: satu Pending (baru diajukan Dewi, level 1
        // pending di Siti), satu lagi sampai selesai Approved (Rudi, disetujui
        // Siti lalu Budi) supaya kelihatan alur multi-level dari awal sampai akhir ---
        await leaveService.CreateAsync(dewi.Id, new LeaveRequestCreateDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(9)),
            Reason = "Acara keluarga"
        });

        var rudiLeave = await leaveService.CreateAsync(rudi.Id, new LeaveRequestCreateDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-8)),
            Reason = "Cuti tahunan"
        });
        await leaveService.ApproveAsync(rudiLeave.Id, siti.Id, new LeaveApprovalActionDto { Note = "Disetujui, jadwal tim aman." });
        await leaveService.ApproveAsync(rudiLeave.Id, budi.Id, new LeaveApprovalActionDto { Note = "OK." });

        // --- PayrollPeriod bulan berjalan (item + approval level 1 & 2 ter-generate otomatis) ---
        await payrollService.CreatePeriodAsync(new PayrollPeriodCreateDto { Month = DateTime.Today.Month, Year = DateTime.Today.Year });

        // --- KPI: kriteria + satu periode terisi penuh + satu contoh override Manager ---
        await kpiService.CreateCriteriaAsync(new KpiCriteriaCreateDto { Name = "Kedisiplinan", Weight = 30 });
        await kpiService.CreateCriteriaAsync(new KpiCriteriaCreateDto { Name = "Produktivitas", Weight = 40 });
        await kpiService.CreateCriteriaAsync(new KpiCriteriaCreateDto { Name = "Kerja Sama Tim", Weight = 30 });
        var criteria = await kpiService.GetAllCriteriaAsync();

        var quarter = (DateTime.Today.Month - 1) / 3 + 1;
        var kpiPeriod = await kpiService.CreatePeriodAsync(new KpiPeriodCreateDto { Name = $"Q{quarter} {DateTime.Today.Year}", Year = DateTime.Today.Year });

        var dewiScores = await kpiService.FillScoresAsync(kpiPeriod.Id, admin.Id, new EmployeeKpiScoreFillDto
        {
            EmployeeId = dewi.Id,
            Scores = criteria.Select(c => new KpiScoreItemDto { CriteriaId = c.Id, Score = 80 }).ToList()
        });
        await kpiService.FillScoresAsync(kpiPeriod.Id, admin.Id, new EmployeeKpiScoreFillDto
        {
            EmployeeId = rudi.Id,
            Scores = criteria.Select(c => new KpiScoreItemDto { CriteriaId = c.Id, Score = 75 }).ToList()
        });
        await kpiService.FillScoresAsync(kpiPeriod.Id, admin.Id, new EmployeeKpiScoreFillDto
        {
            EmployeeId = maya.Id,
            Scores = criteria.Select(c => new KpiScoreItemDto { CriteriaId = c.Id, Score = 85 }).ToList()
        });

        // Contoh Manager (Siti) me-review lalu override satu nilai Dewi,
        // supaya jejak audit trail (KpiScoreRevision) langsung ada datanya.
        var produktivitasScore = dewiScores.First(s => s.CriteriaName == "Produktivitas");
        await kpiService.OverrideScoreAsync(produktivitasScore.Id, siti.Id, sitiUser.Id, new KpiScoreOverrideDto
        {
            NewScore = 90,
            Note = "Menyelesaikan migrasi database lebih cepat dari target, dinaikkan dari nilai awal."
        });

        logger.LogInformation(
            "Data dummy perusahaan berhasil di-seed: 2 department, 6 employee, 1 payroll period, 1 kpi period.");

        // Hindari warning "unused variable" untuk yang cuma dipakai referensi Id.
        _ = budiUser; _ = rudiUser;
    }
}
