using System.Text;
using HRIS.Api.Common;
using HRIS.Api.Data;
using HRIS.Api.Middlewares;
using HRIS.Api.Repositories;
using HRIS.Api.Services;
using HRIS.Api.Services.Jobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Database (SQLite) ---
builder.Services.AddDbContext<AppDbContext>(options =>
    // UseSnakeCaseNamingConvention: kolom DB snake_case sesuai ERD (EmployeeId -> employee_id).
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

// Dipakai AppDbContext untuk mengisi kolom audit created_by/updated_by.
builder.Services.AddHttpContextAccessor();

// --- Repositories ---
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IReferenceRepository, ReferenceRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<IKpiCriteriaRepository, KpiCriteriaRepository>();
builder.Services.AddScoped<IKpiPeriodRepository, KpiPeriodRepository>();
builder.Services.AddScoped<IEmployeeKpiScoreRepository, EmployeeKpiScoreRepository>();
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IEmailOutboxRepository, EmailOutboxRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<ILearningMaterialRepository, LearningMaterialRepository>();
builder.Services.AddScoped<IRegulationRepository, RegulationRepository>();
builder.Services.AddScoped<IAssistanceRequestRepository, AssistanceRequestRepository>();
builder.Services.AddScoped<IFamilyChangeRepository, FamilyChangeRepository>();
builder.Services.AddScoped<ILetterRepository, LetterRepository>();
builder.Services.AddScoped<IParkingRepository, ParkingRepository>();
builder.Services.AddScoped<IDeclarationRepository, DeclarationRepository>();
builder.Services.AddScoped<ILaptopRepository, LaptopRepository>();
builder.Services.AddScoped<IServiceAwardRepository, ServiceAwardRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IFlexibleBenefitRepository, FlexibleBenefitRepository>();
builder.Services.AddScoped<IManpowerRepository, ManpowerRepository>();
builder.Services.AddScoped<ILeaveAdminRepository, LeaveAdminRepository>();
builder.Services.AddScoped<IExpatriateRepository, ExpatriateRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ITransactionRunner, TransactionRunner>();

// --- Services ---
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IReferenceService, ReferenceService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<IKpiService, KpiService>();

// --- Batch A: approval engine, notifikasi, dokumen, audit log ---
// Tiap modul yang memakai approval engine mendaftarkan satu IApprovalHandler untuk request_type-nya.
builder.Services.AddScoped<IApprovalHandler, LeaveApprovalHandler>();
builder.Services.AddScoped<IApprovalHandler, FamilyChangeApprovalHandler>();
builder.Services.AddScoped<IApprovalHandler, LetterApprovalHandler>();
builder.Services.AddScoped<IApprovalHandler, ParkingApprovalHandler>();
builder.Services.AddScoped<IApprovalHandler, LaptopApprovalHandler>();
builder.Services.AddScoped<IApprovalHandler, LeaveEncashmentApprovalHandler>();
builder.Services.AddScoped<IApprovalHandler, HealthClaimApprovalHandler>();
builder.Services.AddScoped<IApprovalHandler, ManpowerApprovalHandler>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ILearningMaterialService, LearningMaterialService>();
builder.Services.AddScoped<IRegulationService, RegulationService>();
builder.Services.AddScoped<IAssistanceRequestService, AssistanceRequestService>();
builder.Services.AddScoped<IFamilyChangeService, FamilyChangeService>();
builder.Services.AddScoped<ILetterService, LetterService>();
builder.Services.AddScoped<IParkingService, ParkingService>();
builder.Services.AddScoped<IDeclarationService, DeclarationService>();
builder.Services.AddScoped<ILaptopService, LaptopService>();
builder.Services.AddScoped<IServiceAwardService, ServiceAwardService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IFlexibleBenefitService, FlexibleBenefitService>();
builder.Services.AddScoped<IManpowerService, ManpowerService>();
builder.Services.AddScoped<ILeaveAdminService, LeaveAdminService>();
builder.Services.AddScoped<IExpatriateService, ExpatriateService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();

// --- Konfigurasi email & dokumen ---
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<DocumentOptions>(builder.Configuration.GetSection("Documents"));

// Email:Mode = Smtp -> kirim sungguhan; selain itu (Log/Off) -> hanya tulis ke log.
if (string.Equals(builder.Configuration["Email:Mode"], "Smtp", StringComparison.OrdinalIgnoreCase))
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
else
    builder.Services.AddScoped<IEmailSender, LogEmailSender>();

// --- Background job (satu hosted service menjalankan semua IRecurringJob) ---
// Batch berikutnya cukup menambah baris AddSingleton<IRecurringJob, ...> di sini.
builder.Services.AddSingleton<IRecurringJob, EmailDispatchJob>();
builder.Services.AddHostedService<RecurringJobHostedService>();

// --- Controllers (API) + Razor Pages (frontend interaktif) ---
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// --- JWT Authentication (dipakai oleh API, tetap default scheme) ---
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
})
// Scheme kedua khusus untuk halaman Razor Pages (frontend), terpisah dari JWT
// yang dipakai API. Login lewat form akan sign-in ke scheme "Cookies" ini,
// sementara /api/** tetap divalidasi lewat JWT Bearer seperti sebelumnya.
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.Name = "HRIS.Auth";
});

// Manager/Head/Group Head bukan role tersimpan: diturunkan dari claim IsManager
// (punya bawahan aktif di MST_Employee_Hierarchy). HR dan Support (akses penuh)
// otomatis lolos policy ini.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(RoleNames.ManagerOrHrPolicy, policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole(RoleNames.HR)
            || ctx.User.IsInRole(RoleNames.Support)
            || ctx.User.HasClaim(RoleNames.IsManagerClaim, "true")));
});

// --- Swagger, dengan dukungan JWT Bearer supaya bisa login-test dari UI /swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "HRIS API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Masukkan: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --- Migrate database & seed data otomatis saat startup ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await DbSeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();

// Diperlukan supaya ILogger<Program> di DbSeeder bisa resolve
public partial class Program { }
