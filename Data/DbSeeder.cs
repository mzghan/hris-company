using HRIS.Api.Models;
using HRIS.Api.Models.Enums;
using HRIS.Api.Repositories;

namespace HRIS.Api.Data;

// Seed satu akun Admin awal saat aplikasi pertama kali dijalankan,
// supaya tidak ada masalah "ayam-telur" (butuh Admin untuk membuat
// Employee & User, tapi belum ada akun sama sekali untuk login).
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        var userRepository = services.GetRequiredService<IUserRepository>();

        if (await userRepository.AnyUserExistsAsync())
            return;

        var username = config["SeedAdmin:Username"] ?? "admin";
        var password = config["SeedAdmin:Password"] ?? "Admin123!";

        var admin = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.Admin,
            EmployeeId = null
        };

        await userRepository.AddAsync(admin);

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(
            "Akun Admin awal dibuat: username='{Username}', password default dari appsettings. " +
            "GANTI PASSWORD INI setelah login pertama kali.", username);
    }
}
