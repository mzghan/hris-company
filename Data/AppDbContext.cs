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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Enum disimpan sebagai string di SQLite, biar gampang dibaca manual saat debug ---
        modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();
        modelBuilder.Entity<LeaveRequest>().Property(l => l.Status).HasConversion<string>();
        modelBuilder.Entity<LeaveApproval>().Property(a => a.Status).HasConversion<string>();

        // --- Unique constraints ---
        modelBuilder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

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
    }
}
