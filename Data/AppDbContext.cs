using System.Security.Claims;
using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRIS.Api.Data;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    // IHttpContextAccessor dibuat opsional supaya tool dotnet-ef (design time)
    // tetap bisa membuat AppDbContext tanpa HttpContext.
    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // --- REF_* ---
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<AddressType> AddressTypes => Set<AddressType>();
    public DbSet<AccountType> AccountTypes => Set<AccountType>();
    public DbSet<HierarchyType> HierarchyTypes => Set<HierarchyType>();
    public DbSet<IdentityType> IdentityTypes => Set<IdentityType>();
    public DbSet<ContactType> ContactTypes => Set<ContactType>();
    public DbSet<Religion> Religions => Set<Religion>();
    public DbSet<Gender> Genders => Set<Gender>();
    public DbSet<MaritalStatus> MaritalStatuses => Set<MaritalStatus>();
    public DbSet<Relationship> Relationships => Set<Relationship>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<EmploymentType> EmploymentTypes => Set<EmploymentType>();
    public DbSet<EmploymentStatus> EmploymentStatuses => Set<EmploymentStatus>();
    public DbSet<EndReason> EndReasons => Set<EndReason>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<JobTitle> JobTitles => Set<JobTitle>();
    public DbSet<JobLevel> JobLevels => Set<JobLevel>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<EducationDegree> EducationDegrees => Set<EducationDegree>();
    public DbSet<EducationTitle> EducationTitles => Set<EducationTitle>();
    public DbSet<University> Universities => Set<University>();
    public DbSet<Industry> Industries => Set<Industry>();
    public DbSet<ApprovalFlowStep> ApprovalFlowSteps => Set<ApprovalFlowStep>();
    public DbSet<DocumentCategory> DocumentCategories => Set<DocumentCategory>();
    public DbSet<ModuleEligibility> ModuleEligibilities => Set<ModuleEligibility>();
    public DbSet<WorkType> WorkTypes => Set<WorkType>();
    public DbSet<PublicHoliday> PublicHolidays => Set<PublicHoliday>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<MeetingRoom> MeetingRooms => Set<MeetingRoom>();
    public DbSet<EvaluationType> EvaluationTypes => Set<EvaluationType>();
    public DbSet<Competency> Competencies => Set<Competency>();

    // --- MST_* ---
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeAddress> EmployeeAddresses => Set<EmployeeAddress>();
    public DbSet<EmployeeContact> EmployeeContacts => Set<EmployeeContact>();
    public DbSet<EmployeeIdentity> EmployeeIdentities => Set<EmployeeIdentity>();
    public DbSet<EmployeeFinancialAccount> EmployeeFinancialAccounts => Set<EmployeeFinancialAccount>();
    public DbSet<EmployeeFamily> EmployeeFamilies => Set<EmployeeFamily>();
    public DbSet<EmployeeHierarchy> EmployeeHierarchies => Set<EmployeeHierarchy>();
    public DbSet<EmployeeEmployment> EmployeeEmployments => Set<EmployeeEmployment>();
    public DbSet<EmployeeEducation> EmployeeEducations => Set<EmployeeEducation>();
    public DbSet<EmployeeWorkExperience> EmployeeWorkExperiences => Set<EmployeeWorkExperience>();

    // --- SYS_* ---
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // --- TRX_* (dibawa dari versi sebelumnya; dirombak per batch) ---
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<KpiCriteria> KpiCriteria => Set<KpiCriteria>();
    public DbSet<KpiPeriod> KpiPeriods => Set<KpiPeriod>();
    public DbSet<EmployeeKpiScore> EmployeeKpiScores => Set<EmployeeKpiScore>();
    public DbSet<KpiScoreRevision> KpiScoreRevisions => Set<KpiScoreRevision>();

    // --- TRX_* Batch A (fondasi) ---
    public DbSet<ApprovalRequest> ApprovalRequests => Set<ApprovalRequest>();
    public DbSet<ApprovalStep> ApprovalSteps => Set<ApprovalStep>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<EmailOutbox> EmailOutbox => Set<EmailOutbox>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<LearningMaterial> LearningMaterials => Set<LearningMaterial>();
    public DbSet<Regulation> Regulations => Set<Regulation>();
    public DbSet<AssistanceRequest> AssistanceRequests => Set<AssistanceRequest>();
    public DbSet<FamilyChangeRequest> FamilyChangeRequests => Set<FamilyChangeRequest>();
    public DbSet<LetterType> LetterTypes => Set<LetterType>();
    public DbSet<LetterRequest> LetterRequests => Set<LetterRequest>();
    public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();
    public DbSet<ParkingRegistration> ParkingRegistrations => Set<ParkingRegistration>();
    public DbSet<DeclarationTemplate> DeclarationTemplates => Set<DeclarationTemplate>();
    public DbSet<DeclarationSubmission> DeclarationSubmissions => Set<DeclarationSubmission>();
    public DbSet<LaptopRequest> LaptopRequests => Set<LaptopRequest>();
    public DbSet<LaptopStatusLog> LaptopStatusLogs => Set<LaptopStatusLog>();
    public DbSet<ServiceAward> ServiceAwards => Set<ServiceAward>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
    public DbSet<FlexPeriod> FlexPeriods => Set<FlexPeriod>();
    public DbSet<LeaveEncashment> LeaveEncashments => Set<LeaveEncashment>();
    public DbSet<HealthClaim> HealthClaims => Set<HealthClaim>();
    public DbSet<RoomBooking> RoomBookings => Set<RoomBooking>();
    public DbSet<ManpowerRequest> ManpowerRequests => Set<ManpowerRequest>();
    public DbSet<JobDescription> JobDescriptions => Set<JobDescription>();
    public DbSet<JobDescriptionBank> JobDescriptionBanks => Set<JobDescriptionBank>();
    public DbSet<JobDescriptionMaster> JobDescriptionMasters => Set<JobDescriptionMaster>();
    public DbSet<JobDescriptionRevision> JobDescriptionRevisions => Set<JobDescriptionRevision>();
    public DbSet<ManpowerRequestRemark> ManpowerRequestRemarks => Set<ManpowerRequestRemark>();
    public DbSet<ManpowerRequestFiling> ManpowerRequestFilings => Set<ManpowerRequestFiling>();
    public DbSet<ManpowerVacancy> ManpowerVacancies => Set<ManpowerVacancy>();
    public DbSet<EmployeeEvaluation> EmployeeEvaluations => Set<EmployeeEvaluation>();
    public DbSet<EvaluationEntry> EvaluationEntries => Set<EvaluationEntry>();
    public DbSet<EvaluationScore> EvaluationScores => Set<EvaluationScore>();
    public DbSet<PerformancePlan> PerformancePlans => Set<PerformancePlan>();
    public DbSet<PlanTask> PlanTasks => Set<PlanTask>();
    public DbSet<PlanWorkLog> PlanWorkLogs => Set<PlanWorkLog>();
    public DbSet<EmployeeCompetency> EmployeeCompetencies => Set<EmployeeCompetency>();
    public DbSet<CompetencyAssessment> CompetencyAssessments => Set<CompetencyAssessment>();
    public DbSet<PersonalAction> PersonalActions => Set<PersonalAction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Nama kolom snake_case otomatis dari UseSnakeCaseNamingConvention() di Program.cs
        // (mis. EmployeeId -> employee_id). Yang diatur manual di sini hanya nama tabel
        // (prefix MST_/REF_/TRX_/SYS_) dan nama kolom PK, karena di C# PK tetap "Id".

        // --- REF_* ---
        MapTable<Country>(modelBuilder, "REF_Country", "country_id");
        MapTable<Province>(modelBuilder, "REF_Province", "province_id");
        MapTable<City>(modelBuilder, "REF_City", "city_id");
        MapTable<AddressType>(modelBuilder, "REF_Address_Type", "address_type_id");
        MapTable<AccountType>(modelBuilder, "REF_Account_Type", "account_type_id");
        MapTable<HierarchyType>(modelBuilder, "REF_Hierarchy_Type", "hierarchy_type_id");
        MapTable<IdentityType>(modelBuilder, "REF_Identity_Type", "identity_type_id");
        MapTable<ContactType>(modelBuilder, "REF_Contact_Type", "contact_type_id");
        MapTable<Religion>(modelBuilder, "REF_Religion", "religion_id");
        MapTable<Gender>(modelBuilder, "REF_Gender", "gender_id");
        MapTable<MaritalStatus>(modelBuilder, "REF_Marital_Status", "marital_status_id");
        MapTable<Relationship>(modelBuilder, "REF_Relationship", "relationship_id");
        MapTable<Vendor>(modelBuilder, "REF_Vendor", "vendor_id");
        MapTable<EmploymentType>(modelBuilder, "REF_Employment_Type", "employment_type_id");
        MapTable<EmploymentStatus>(modelBuilder, "REF_Employment_Status", "employment_status_id");
        MapTable<EndReason>(modelBuilder, "REF_End_Reason", "end_reason_id");
        MapTable<Organization>(modelBuilder, "REF_Organization", "organization_id");
        MapTable<Location>(modelBuilder, "REF_Location", "location_id");
        MapTable<JobTitle>(modelBuilder, "REF_Job_Title", "job_title_id");
        MapTable<JobLevel>(modelBuilder, "REF_Job_Level", "job_level_id");
        MapTable<Grade>(modelBuilder, "REF_Grade", "grade_id");
        MapTable<EducationDegree>(modelBuilder, "REF_Education_Degree", "degree_id");
        MapTable<EducationTitle>(modelBuilder, "REF_Education_Title", "title_id");
        MapTable<University>(modelBuilder, "REF_University", "university_id");
        MapTable<Industry>(modelBuilder, "REF_Industry", "industry_id");
        MapTable<ApprovalFlowStep>(modelBuilder, "REF_Approval_Flow_Step", "flow_step_id");
        MapTable<DocumentCategory>(modelBuilder, "REF_Document_Category", "category_id");
        MapTable<ModuleEligibility>(modelBuilder, "REF_Module_Eligibility", "eligibility_id");
        MapTable<WorkType>(modelBuilder, "REF_Work_Type", "work_type_id");
        MapTable<PublicHoliday>(modelBuilder, "REF_Public_Holiday", "holiday_id");
        MapTable<LeaveType>(modelBuilder, "REF_Leave_Type", "leave_type_id");
        MapTable<MeetingRoom>(modelBuilder, "REF_Meeting_Room", "room_id");
        MapTable<EvaluationType>(modelBuilder, "REF_Evaluation_Type", "eval_type_id");
        MapTable<Competency>(modelBuilder, "REF_Competency", "competency_id");

        modelBuilder.Entity<JobTitle>().Property(j => j.JobTitleName).HasColumnName("job_title");

        // --- MST_* ---
        MapTable<Employee>(modelBuilder, "MST_Employee", "employee_id");
        MapTable<EmployeeAddress>(modelBuilder, "MST_Employee_Address", "address_id");
        MapTable<EmployeeContact>(modelBuilder, "MST_Employee_Contact", "contact_id");
        MapTable<EmployeeIdentity>(modelBuilder, "MST_Employee_Identity", "identity_id");
        MapTable<EmployeeFinancialAccount>(modelBuilder, "MST_Employee_FinancialAccount", "account_id");
        MapTable<EmployeeFamily>(modelBuilder, "MST_Employee_Family", "family_id");
        MapTable<EmployeeHierarchy>(modelBuilder, "MST_Employee_Hierarchy", "hierarchy_id");
        MapTable<EmployeeEmployment>(modelBuilder, "MST_Employee_Employment", "employment_id");
        MapTable<EmployeeEducation>(modelBuilder, "MST_Employee_Education", "education_id");
        MapTable<EmployeeWorkExperience>(modelBuilder, "MST_Employee_WorkExperience", "experience_id");

        // --- SYS_* ---
        MapTable<User>(modelBuilder, "SYS_User", "user_id");
        MapTable<Role>(modelBuilder, "SYS_Role", "role_id");
        MapTable<AuditLog>(modelBuilder, "SYS_Audit_Log", "log_id");
        modelBuilder.Entity<UserRole>().ToTable("SYS_User_Role");
        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

        // --- TRX_* (lama) ---
        MapTable<Attendance>(modelBuilder, "TRX_Attendance", "attendance_id");
        MapTable<LeaveRequest>(modelBuilder, "TRX_Leave_Request", "leave_id");
        MapTable<KpiCriteria>(modelBuilder, "TRX_Kpi_Criteria", "criteria_id");
        MapTable<KpiPeriod>(modelBuilder, "TRX_Kpi_Period", "kpi_period_id");
        MapTable<EmployeeKpiScore>(modelBuilder, "TRX_Employee_Kpi_Score", "score_id");
        MapTable<KpiScoreRevision>(modelBuilder, "TRX_Kpi_Score_Revision", "revision_id");

        // --- TRX_* Batch A ---
        MapTable<ApprovalRequest>(modelBuilder, "TRX_Approval_Request", "approval_id");
        MapTable<ApprovalStep>(modelBuilder, "TRX_Approval_Step", "step_id");
        MapTable<Notification>(modelBuilder, "TRX_Notification", "notification_id");
        MapTable<EmailOutbox>(modelBuilder, "TRX_Email_Outbox", "email_id");
        MapTable<Document>(modelBuilder, "TRX_Document", "document_id");
        MapTable<LearningMaterial>(modelBuilder, "TRX_Learning_Material", "material_id");
        MapTable<Regulation>(modelBuilder, "TRX_Regulation", "regulation_id");
        MapTable<AssistanceRequest>(modelBuilder, "TRX_Assistance_Request", "request_id");
        MapTable<FamilyChangeRequest>(modelBuilder, "TRX_Family_Change_Request", "change_id");
        MapTable<LetterType>(modelBuilder, "REF_Letter_Type", "letter_type_id");
        MapTable<LetterRequest>(modelBuilder, "TRX_Letter_Request", "letter_id");
        MapTable<VehicleType>(modelBuilder, "REF_Vehicle_Type", "vehicle_type_id");
        MapTable<ParkingRegistration>(modelBuilder, "TRX_Parking_Registration", "parking_id");
        MapTable<DeclarationTemplate>(modelBuilder, "TRX_Declaration_Template", "template_id");
        MapTable<DeclarationSubmission>(modelBuilder, "TRX_Declaration_Submission", "submission_id");
        MapTable<LaptopRequest>(modelBuilder, "TRX_Laptop_Request", "laptop_id");
        MapTable<LaptopStatusLog>(modelBuilder, "TRX_Laptop_Status_Log", "log_id");
        MapTable<ServiceAward>(modelBuilder, "TRX_Service_Award", "award_id");
        MapTable<LeaveBalance>(modelBuilder, "TRX_Leave_Balance", "balance_id");
        MapTable<FlexPeriod>(modelBuilder, "TRX_Flex_Period", "period_id");
        MapTable<LeaveEncashment>(modelBuilder, "TRX_Leave_Encashment", "encashment_id");
        MapTable<HealthClaim>(modelBuilder, "TRX_Health_Claim", "claim_id");
        MapTable<RoomBooking>(modelBuilder, "TRX_Room_Booking", "booking_id");
        MapTable<ManpowerRequest>(modelBuilder, "TRX_Manpower_Request", "manpower_id");
        MapTable<JobDescription>(modelBuilder, "TRN_Jobdesc", "jobdesc_id");
        MapTable<JobDescriptionBank>(modelBuilder, "BANK_JOBDESC", "bank_jobdesc_id");
        MapTable<JobDescriptionMaster>(modelBuilder, "MST_JOBDESC", "mst_jobdesc_id");
        MapTable<JobDescriptionRevision>(modelBuilder, "TRN_Revise_History_JD", "revision_id");
        MapTable<ManpowerRequestRemark>(modelBuilder, "TRX_Manpower_Request_Remarks", "remark_id");
        MapTable<ManpowerRequestFiling>(modelBuilder, "TRX_Manpower_Request_Filing", "filing_id");
        MapTable<ManpowerVacancy>(modelBuilder, "TRX_Manpower_Vacancy", "vacancy_id");
        MapTable<EmployeeEvaluation>(modelBuilder, "TRX_Employee_Evaluation", "evaluation_id");
        MapTable<EvaluationEntry>(modelBuilder, "TRX_Evaluation_Entry", "entry_id");
        MapTable<EvaluationScore>(modelBuilder, "TRX_Evaluation_Score", "score_id");
        MapTable<PerformancePlan>(modelBuilder, "TRX_Performance_Plan", "plan_id");
        MapTable<PlanTask>(modelBuilder, "TRX_Plan_Task", "task_id");
        MapTable<PlanWorkLog>(modelBuilder, "TRX_Plan_WorkLog", "log_id");
        MapTable<EmployeeCompetency>(modelBuilder, "TRX_Employee_Competency", "id");
        MapTable<CompetencyAssessment>(modelBuilder, "TRX_Competency_Assessment", "assessment_id");
        MapTable<PersonalAction>(modelBuilder, "TRX_Personal_Action", "paf_id");
        modelBuilder.Entity<RoomBooking>().Property(x => x.BookedByEmployeeId).HasColumnName("booked_by");
        modelBuilder.Entity<ManpowerRequest>().Property(x => x.RequestedByEmployeeId).HasColumnName("requested_by");
        modelBuilder.Entity<JobDescription>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<ManpowerVacancy>().HasIndex(x => x.VacancyCode).IsUnique();
        modelBuilder.Entity<ManpowerVacancy>().HasIndex(x => new { x.ManpowerRequestId, x.PositionNo }).IsUnique();

        // --- Enum disimpan sebagai string di SQLite, biar gampang dibaca manual saat debug ---
        modelBuilder.Entity<LeaveRequest>().Property(l => l.Status).HasConversion<string>();
        modelBuilder.Entity<ApprovalRequest>().Property(r => r.Status).HasConversion<string>();
        modelBuilder.Entity<ApprovalStep>().Property(x => x.Status).HasConversion<string>();
        modelBuilder.Entity<ApprovalStep>().Property(x => x.ApproverType).HasConversion<string>();
        modelBuilder.Entity<ApprovalFlowStep>().Property(x => x.ApproverType).HasConversion<string>();
        modelBuilder.Entity<EmailOutbox>().Property(e => e.Status).HasConversion<string>();
        modelBuilder.Entity<KpiPeriod>().Property(p => p.Status).HasConversion<string>();

        // --- Unique constraints ---
        modelBuilder.Entity<Employee>().HasIndex(e => e.EmployeeNumber).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        // Satu akun per Employee. SQLite membolehkan banyak NULL di unique index,
        // jadi akun Support awal (tanpa Employee) tidak bentrok.
        modelBuilder.Entity<User>().HasIndex(u => u.EmployeeId).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(r => r.RoleName).IsUnique();
        modelBuilder.Entity<Country>().HasIndex(c => c.CountryCode).IsUnique();

        // Satu pengajuan modul = satu approval request.
        modelBuilder.Entity<ApprovalRequest>().HasIndex(r => new { r.RequestType, r.RequestRefId }).IsUnique();
        modelBuilder.Entity<ApprovalStep>().HasIndex(x => new { x.ApprovalId, x.Level }).IsUnique();
        modelBuilder.Entity<ApprovalFlowStep>().HasIndex(x => new { x.RequestType, x.Level }).IsUnique();
        modelBuilder.Entity<Notification>().HasIndex(n => new { n.UserId, n.IsRead });
        modelBuilder.Entity<EmailOutbox>().HasIndex(e => new { e.Status, e.NextAttemptAt });

        modelBuilder.Entity<KpiPeriod>().HasIndex(p => new { p.Name, p.Year }).IsUnique();

        // Satu employee hanya boleh punya satu EmployeeKpiScore per kombinasi periode+kriteria.
        modelBuilder.Entity<EmployeeKpiScore>()
            .HasIndex(s => new { s.KpiPeriodId, s.EmployeeId, s.CriteriaId })
            .IsUnique();

        modelBuilder.Entity<ModuleEligibility>().HasOne(x=>x.EmploymentType).WithMany().HasForeignKey(x=>x.EmploymentTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ModuleEligibility>().HasIndex(x=>new{x.EmploymentTypeId,x.ModuleCode}).IsUnique();

        modelBuilder.Entity<FamilyChangeRequest>().HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FamilyChangeRequest>().HasOne(x => x.Family).WithMany().HasForeignKey(x => x.FamilyId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FamilyChangeRequest>().HasOne(x => x.Relationship).WithMany().HasForeignKey(x => x.RelationshipId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FamilyChangeRequest>().HasOne(x => x.Gender).WithMany().HasForeignKey(x => x.GenderId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FamilyChangeRequest>().HasIndex(x => new { x.EmployeeId, x.Status });

        modelBuilder.Entity<LetterType>().HasOne(x => x.TemplateDocument).WithMany().HasForeignKey(x => x.TemplateDocumentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LetterRequest>().HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LetterRequest>().HasOne(x => x.LetterType).WithMany().HasForeignKey(x => x.LetterTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LetterRequest>().HasOne(x => x.IssuedDocument).WithMany().HasForeignKey(x => x.IssuedDocumentId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ParkingRegistration>().HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ParkingRegistration>().HasOne(x => x.VehicleType).WithMany().HasForeignKey(x => x.VehicleTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ParkingRegistration>().HasIndex(x => x.PlateNumber).IsUnique();

        modelBuilder.Entity<DeclarationSubmission>().HasOne(x => x.Template).WithMany().HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DeclarationSubmission>().HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DeclarationSubmission>().HasOne(x => x.Document).WithMany().HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LaptopRequest>().HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LaptopStatusLog>().HasOne(x => x.Laptop).WithMany(x => x.StatusLogs).HasForeignKey(x => x.LaptopId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<LaptopStatusLog>().HasOne(x => x.ChangedByUser).WithMany().HasForeignKey(x => x.ChangedBy).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ServiceAward>().HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ServiceAward>().HasIndex(x => new { x.EmployeeId, x.ServiceYears }).IsUnique();
        modelBuilder.Entity<Attendance>().HasIndex(x => new { x.EmployeeId, x.Date }).IsUnique();
        modelBuilder.Entity<LeaveBalance>().HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year }).IsUnique();
        modelBuilder.Entity<WorkType>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<LeaveType>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<PublicHoliday>().HasIndex(x => x.Date).IsUnique();
        modelBuilder.Entity<MeetingRoom>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<RoomBooking>().HasIndex(x => new { x.RoomId, x.StartTime, x.EndTime });

        // Baris Employment terkini (end_date NULL) hanya boleh satu per karyawan.
        // Partial index didukung SQLite.
        modelBuilder.Entity<EmployeeEmployment>()
            .HasIndex(e => e.EmployeeId)
            .IsUnique()
            .HasFilter("end_date IS NULL");

        // Mencegah baris atasan aktif yang persis sama tercatat dua kali.
        // "Hanya satu Direct Manager aktif" divalidasi di Service, karena butuh
        // tahu tipe hierarchy-nya.
        modelBuilder.Entity<EmployeeHierarchy>()
            .HasIndex(h => new { h.EmployeeId, h.ManagerId, h.HierarchyTypeId })
            .IsUnique()
            .HasFilter("end_date IS NULL");

        // --- Relasi REF -> REF ---
        modelBuilder.Entity<Province>()
            .HasOne(p => p.Country).WithMany().HasForeignKey(p => p.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<City>()
            .HasOne(c => c.Province).WithMany().HasForeignKey(c => c.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Organization self-reference (bertingkat). Restrict: organisasi yang masih
        // punya anak tidak boleh terhapus diam-diam.
        modelBuilder.Entity<Organization>()
            .HasOne(o => o.Parent).WithMany(o => o.Children).HasForeignKey(o => o.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Relasi MST_Employee -> REF (semua Restrict, data referensi tidak boleh hilang) ---
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.NationalityCountry).WithMany().HasForeignKey(e => e.NationalityCountryId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Religion).WithMany().HasForeignKey(e => e.ReligionId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Gender).WithMany().HasForeignKey(e => e.GenderId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.MaritalStatus).WithMany().HasForeignKey(e => e.MaritalStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Relasi MST_Employee_* -> MST_Employee (Cascade: data pribadi ikut terhapus) ---
        modelBuilder.Entity<EmployeeAddress>()
            .HasOne(a => a.Employee).WithMany(e => e.Addresses).HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EmployeeContact>()
            .HasOne(c => c.Employee).WithMany(e => e.Contacts).HasForeignKey(c => c.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EmployeeIdentity>()
            .HasOne(i => i.Employee).WithMany(e => e.Identities).HasForeignKey(i => i.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EmployeeFinancialAccount>()
            .HasOne(a => a.Employee).WithMany(e => e.FinancialAccounts).HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EmployeeFamily>()
            .HasOne(f => f.Employee).WithMany(e => e.Families).HasForeignKey(f => f.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EmployeeEmployment>()
            .HasOne(m => m.Employee).WithMany(e => e.Employments).HasForeignKey(m => m.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EmployeeEducation>()
            .HasOne(d => d.Employee).WithMany(e => e.Educations).HasForeignKey(d => d.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EmployeeWorkExperience>()
            .HasOne(w => w.Employee).WithMany(e => e.WorkExperiences).HasForeignKey(w => w.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Hierarchy: dua FK ke Employee (bawahan & atasan) ---
        // Sisi bawahan Cascade (hapus karyawan -> riwayat atasannya ikut hilang).
        // Sisi atasan Restrict: karyawan yang masih jadi atasan orang lain tidak boleh
        // terhapus diam-diam (dicek juga di EmployeeService.DeleteAsync).
        modelBuilder.Entity<EmployeeHierarchy>()
            .HasOne(h => h.Employee).WithMany(e => e.Managers).HasForeignKey(h => h.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EmployeeHierarchy>()
            .HasOne(h => h.Manager).WithMany(e => e.Subordinates).HasForeignKey(h => h.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeHierarchy>()
            .HasOne(h => h.HierarchyType).WithMany().HasForeignKey(h => h.HierarchyTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Relasi MST_Employee_* -> REF (Restrict) ---
        modelBuilder.Entity<EmployeeAddress>().HasOne(a => a.AddressType).WithMany().HasForeignKey(a => a.AddressTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeAddress>().HasOne(a => a.City).WithMany().HasForeignKey(a => a.CityId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeContact>().HasOne(c => c.ContactType).WithMany().HasForeignKey(c => c.ContactTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeIdentity>().HasOne(i => i.IdentityType).WithMany().HasForeignKey(i => i.IdentityTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeFinancialAccount>().HasOne(a => a.AccountType).WithMany().HasForeignKey(a => a.AccountTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeFamily>().HasOne(f => f.Relationship).WithMany().HasForeignKey(f => f.RelationshipId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeFamily>().HasOne(f => f.Gender).WithMany().HasForeignKey(f => f.GenderId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeEducation>().HasOne(d => d.Degree).WithMany().HasForeignKey(d => d.DegreeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeEducation>().HasOne(d => d.Title).WithMany().HasForeignKey(d => d.TitleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeEducation>().HasOne(d => d.University).WithMany().HasForeignKey(d => d.UniversityId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeWorkExperience>().HasOne(w => w.Industry).WithMany().HasForeignKey(w => w.IndustryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeWorkExperience>().HasOne(w => w.JobLevel).WithMany().HasForeignKey(w => w.JobLevelId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeEmployment>().HasOne(m => m.EmploymentStatus).WithMany().HasForeignKey(m => m.EmploymentStatusId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeEmployment>().HasOne(m => m.EmploymentType).WithMany().HasForeignKey(m => m.EmploymentTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeEmployment>().HasOne(m => m.Vendor).WithMany().HasForeignKey(m => m.VendorId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeEmployment>().HasOne(m => m.Organization).WithMany().HasForeignKey(m => m.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeEmployment>().HasOne(m => m.Location).WithMany().HasForeignKey(m => m.LocationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeEmployment>().HasOne(m => m.JobLevel).WithMany().HasForeignKey(m => m.JobLevelId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeEmployment>().HasOne(m => m.JobTitle).WithMany().HasForeignKey(m => m.JobTitleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeEmployment>().HasOne(m => m.Grade).WithMany().HasForeignKey(m => m.GradeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeEmployment>().HasOne(m => m.EndReason).WithMany().HasForeignKey(m => m.EndReasonId).OnDelete(DeleteBehavior.Restrict);

        // --- SYS ---
        modelBuilder.Entity<User>()
            .HasOne(u => u.Employee).WithMany().HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- TRX (lama), diarahkan ke MST_Employee ---
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Employee).WithMany().HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LeaveRequest>()
            .HasOne(l => l.Employee).WithMany().HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Attendance>().HasOne(a=>a.WorkType).WithMany().HasForeignKey(a=>a.WorkTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LeaveRequest>().HasOne(l=>l.LeaveType).WithMany().HasForeignKey(l=>l.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MeetingRoom>().HasOne(r=>r.Location).WithMany().HasForeignKey(r=>r.LocationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LeaveBalance>().HasOne(x=>x.Employee).WithMany().HasForeignKey(x=>x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<LeaveBalance>().HasOne(x=>x.LeaveType).WithMany().HasForeignKey(x=>x.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LeaveEncashment>().HasOne(x=>x.Employee).WithMany().HasForeignKey(x=>x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LeaveEncashment>().HasOne(x=>x.Period).WithMany().HasForeignKey(x=>x.PeriodId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LeaveEncashment>().HasOne(x=>x.LeaveType).WithMany().HasForeignKey(x=>x.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HealthClaim>().HasOne(x=>x.Employee).WithMany().HasForeignKey(x=>x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HealthClaim>().HasOne(x=>x.Period).WithMany().HasForeignKey(x=>x.PeriodId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RoomBooking>().HasOne(x=>x.Room).WithMany().HasForeignKey(x=>x.RoomId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RoomBooking>().HasOne(x=>x.BookedByEmployee).WithMany().HasForeignKey(x=>x.BookedByEmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ManpowerRequest>().HasOne(x=>x.RequestedByEmployee).WithMany().HasForeignKey(x=>x.RequestedByEmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ManpowerRequest>().HasOne(x=>x.Organization).WithMany().HasForeignKey(x=>x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ManpowerRequest>().HasOne(x=>x.JobTitle).WithMany().HasForeignKey(x=>x.JobTitleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ManpowerRequest>().HasOne(x=>x.JobLevel).WithMany().HasForeignKey(x=>x.JobLevelId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ManpowerRequest>().HasOne(x=>x.EmploymentType).WithMany().HasForeignKey(x=>x.EmploymentTypeId).OnDelete(DeleteBehavior.Restrict);

        // --- Approval engine ---
        modelBuilder.Entity<ApprovalRequest>()
            .HasOne(r => r.Requester).WithMany().HasForeignKey(r => r.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ApprovalStep>()
            .HasOne(x => x.Approval).WithMany(r => r.Steps).HasForeignKey(x => x.ApprovalId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ApprovalStep>()
            .HasOne(x => x.ApproverRole).WithMany().HasForeignKey(x => x.ApproverRoleId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ApprovalStep>()
            .HasOne(x => x.ApproverEmployee).WithMany().HasForeignKey(x => x.ApproverEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ApprovalStep>()
            .HasOne(x => x.ActedByUser).WithMany().HasForeignKey(x => x.ActedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ApprovalFlowStep>()
            .HasOne(f => f.Role).WithMany().HasForeignKey(f => f.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ApprovalFlowStep>()
            .HasOne(f => f.ApproverEmployee).WithMany().HasForeignKey(f => f.ApproverEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Notification, Document, Audit log ---
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<DocumentCategory>()
            .HasOne(c => c.Parent).WithMany(c => c.Children).HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Category).WithMany().HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Document>()
            .HasOne(d => d.OwnerEmployee).WithMany().HasForeignKey(d => d.OwnerEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Document>()
            .HasOne(d => d.UploadedByUser).WithMany().HasForeignKey(d => d.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AuditLog>()
            .HasOne(l => l.User).WithMany().HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // --- Batch B: Learning, Regulation, EAP ---
        modelBuilder.Entity<LearningMaterial>()
            .HasOne(x => x.Document).WithMany().HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AssistanceRequest>()
            .HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<AssistanceRequest>()
            .HasOne(x => x.HandledByUser).WithMany().HasForeignKey(x => x.HandledByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // --- KPI ---
        modelBuilder.Entity<EmployeeKpiScore>()
            .HasOne(s => s.Employee).WithMany().HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeKpiScore>()
            .HasOne(s => s.Criteria).WithMany().HasForeignKey(s => s.CriteriaId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeKpiScore>()
            .HasOne(s => s.FilledByUser).WithMany().HasForeignKey(s => s.FilledByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeKpiScore>()
            .HasOne(s => s.KpiPeriod).WithMany(p => p.Scores).HasForeignKey(s => s.KpiPeriodId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<KpiScoreRevision>()
            .HasOne(r => r.EmployeeKpiScore).WithMany(s => s.Revisions).HasForeignKey(r => r.EmployeeKpiScoreId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<KpiScoreRevision>()
            .HasOne(r => r.RevisedByUser).WithMany().HasForeignKey(r => r.RevisedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    // Helper: set nama tabel + nama kolom PK sesuai ERD (di C# PK tetap "Id").
    private static void MapTable<T>(ModelBuilder modelBuilder, string tableName, string pkColumn) where T : class
    {
        modelBuilder.Entity<T>().ToTable(tableName);
        modelBuilder.Entity<T>().Property("Id").HasColumnName(pkColumn);
    }

    // --- Kolom audit otomatis (created_at/by, updated_at/by) ---
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAudit();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAudit();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAudit()
    {
        var now = DateTime.UtcNow;
        var userId = GetCurrentUserId();

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = userId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;
            }
        }
    }

    private int? GetCurrentUserId()
    {
        var value = _httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}
