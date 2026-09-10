using HRIS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveApproval> LeaveApprovals => Set<LeaveApproval>();
    public DbSet<EmployeeSalary> EmployeeSalaries => Set<EmployeeSalary>();
    public DbSet<PayrollPeriod> PayrollPeriods => Set<PayrollPeriod>();
    public DbSet<PayrollItem> PayrollItems => Set<PayrollItem>();
    public DbSet<PayrollApproval> PayrollApprovals => Set<PayrollApproval>();
    public DbSet<KpiCriteria> KpiCriteria => Set<KpiCriteria>();
    public DbSet<KpiPeriod> KpiPeriods => Set<KpiPeriod>();
    public DbSet<EmployeeKpiScore> EmployeeKpiScores => Set<EmployeeKpiScore>();
    public DbSet<KpiScoreRevision> KpiScoreRevisions => Set<KpiScoreRevision>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Enum disimpan sebagai string di SQLite, biar gampang dibaca manual saat debug ---
        modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();
        modelBuilder.Entity<LeaveRequest>().Property(l => l.Status).HasConversion<string>();
        modelBuilder.Entity<LeaveApproval>().Property(a => a.Status).HasConversion<string>();
        modelBuilder.Entity<PayrollPeriod>().Property(p => p.Status).HasConversion<string>();
        modelBuilder.Entity<PayrollApproval>().Property(a => a.Status).HasConversion<string>();
        modelBuilder.Entity<KpiPeriod>().Property(p => p.Status).HasConversion<string>();

        // --- Unique constraints ---
        modelBuilder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        // Satu PayrollPeriod per kombinasi bulan+tahun.
        modelBuilder.Entity<PayrollPeriod>().HasIndex(p => new { p.Month, p.Year }).IsUnique();

        // Satu KpiPeriod per kombinasi nama+tahun (mis. tidak ada dua
        // "Q1" di tahun yang sama), pola sama seperti PayrollPeriod di atas.
        modelBuilder.Entity<KpiPeriod>().HasIndex(p => new { p.Name, p.Year }).IsUnique();

        // Satu employee hanya boleh punya satu EmployeeKpiScore per
        // kombinasi periode+kriteria — mencegah baris duplikat saat
        // FillScoresAsync dipanggil berkali-kali untuk kriteria yang sama.
        modelBuilder.Entity<EmployeeKpiScore>()
            .HasIndex(s => new { s.KpiPeriodId, s.EmployeeId, s.CriteriaId })
            .IsUnique();

        // --- Employee self-reference (Manager) ---
        // Restrict: mencegah "multiple cascade paths" error dan mencegah
        // penghapusan manager otomatis menghapus/merusak data bawahannya.
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Manager)
            .WithMany(e => e.Subordinates)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Employee)
            .WithMany()
            .HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // LeaveApproval -> Employee (sebagai Approver) juga Restrict, supaya
        // tidak ada dua jalur cascade berbeda menuju Employee yang sama
        // (satu lewat ApproverId, satu lagi lewat LeaveRequest -> EmployeeId).
        modelBuilder.Entity<LeaveApproval>()
            .HasOne(a => a.Approver)
            .WithMany()
            .HasForeignKey(a => a.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Satu-satunya jalur cascade menuju Employee: lewat LeaveRequest.
        // Hapus Employee -> ikut hapus LeaveRequest miliknya -> ikut hapus LeaveApproval terkait.
        modelBuilder.Entity<LeaveRequest>()
            .HasOne(l => l.Employee)
            .WithMany()
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LeaveApproval>()
            .HasOne(a => a.LeaveRequest)
            .WithMany(l => l.Approvals)
            .HasForeignKey(a => a.LeaveRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Payroll (Fase 2) ---
        // Sama seperti Attendance dan LeaveApproval.Approver: Restrict,
        // supaya tetap hanya LeaveRequest yang jadi satu-satunya jalur
        // cascade menuju Employee.
        modelBuilder.Entity<EmployeeSalary>()
            .HasOne(s => s.Employee)
            .WithMany()
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PayrollItem>()
            .HasOne(i => i.Employee)
            .WithMany()
            .HasForeignKey(i => i.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PayrollApproval>()
            .HasOne(a => a.Approver)
            .WithMany()
            .HasForeignKey(a => a.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);

        // PayrollItem & PayrollApproval ikut terhapus kalau PayrollPeriod
        // induknya dihapus (jalur cascade menuju PayrollPeriod, bukan Employee).
        modelBuilder.Entity<PayrollItem>()
            .HasOne(i => i.PayrollPeriod)
            .WithMany(p => p.Items)
            .HasForeignKey(i => i.PayrollPeriodId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PayrollApproval>()
            .HasOne(a => a.PayrollPeriod)
            .WithMany(p => p.Approvals)
            .HasForeignKey(a => a.PayrollPeriodId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- KPI (Fase 3) ---
        // Sama seperti PayrollItem/PayrollApproval: satu-satunya jalur
        // cascade menuju Employee tetap lewat LeaveRequest. EmployeeKpiScore
        // -> Employee dan -> KpiCriteria di-Restrict; EmployeeKpiScore
        // -> KpiPeriod di-Cascade (mengikuti induk periode-nya).
        modelBuilder.Entity<EmployeeKpiScore>()
            .HasOne(s => s.Employee)
            .WithMany()
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeKpiScore>()
            .HasOne(s => s.Criteria)
            .WithMany()
            .HasForeignKey(s => s.CriteriaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeKpiScore>()
            .HasOne(s => s.FilledByUser)
            .WithMany()
            .HasForeignKey(s => s.FilledByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeKpiScore>()
            .HasOne(s => s.KpiPeriod)
            .WithMany(p => p.Scores)
            .HasForeignKey(s => s.KpiPeriodId)
            .OnDelete(DeleteBehavior.Cascade);

        // KpiScoreRevision -> EmployeeKpiScore adalah satu-satunya jalur
        // cascade untuk KpiScoreRevision (hapus skor -> ikut hapus riwayat
        // revisinya). KpiScoreRevision -> User (RevisedByUserId) di-Restrict.
        modelBuilder.Entity<KpiScoreRevision>()
            .HasOne(r => r.EmployeeKpiScore)
            .WithMany(s => s.Revisions)
            .HasForeignKey(r => r.EmployeeKpiScoreId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<KpiScoreRevision>()
            .HasOne(r => r.RevisedByUser)
            .WithMany()
            .HasForeignKey(r => r.RevisedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
