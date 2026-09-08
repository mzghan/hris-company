# HRIS API — Fase 1 (MVP)

Web API (bukan MVC UI) untuk HRIS, dibangun dengan .NET 8 + EF Core + SQLite,
mengikuti pola arsitektur `Controller → Service → Repository → DbContext`
yang sama dengan project referensi `product-management-api-dotnet`.

Modul yang sudah dibuat di Fase 1:
- **Auth** — JWT login, register (Admin only)
- **Department** — CRUD
- **Employee** — CRUD, termasuk relasi Manager (self-referencing) & Department
- **Attendance** — check-in / check-out harian
- **LeaveRequest** — pengajuan cuti dengan approval **multi-level**, mengikuti rantai `Employee.ManagerId`

Payroll dan Penilaian KPI **belum** ada di sini — itu Fase 2, sesuai dokumen
technical alignment yang sudah kita sepakati.

## 1. Persiapan (sekali saja)

Pastikan sudah terinstall:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- VS Code + extension **C# Dev Kit**
- (opsional tapi disarankan) `dotnet-ef` tool:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## 2. Restore & jalankan

```bash
cd HRIS.Api
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

> **Catatan:** Migration belum di-generate di dalam zip ini (project ini dibuat
> tanpa akses internet ke NuGet, jadi belum sempat di-build/di-restore).
> Jalankan `dotnet restore` dulu sebelum `dotnet ef migrations add`.

File database `hris.db` akan otomatis terbuat di folder project saat
`dotnet run` dijalankan (migration di-apply otomatis lewat `db.Database.Migrate()`
di `Program.cs`).

Swagger UI tersedia di `https://localhost:{port}/swagger` untuk test manual.

## 3. Login pertama kali

Saat pertama kali dijalankan dan belum ada User sama sekali di database,
sistem otomatis membuat satu akun Admin (lihat `DbSeeder.cs`):

- Username: `admin`
- Password: `Admin123!`

Kredensial ini diambil dari `appsettings.json` (`SeedAdmin` section) —
**ganti password ini** kalau sudah lanjut ke tahap yang lebih serius.

Alur pemakaian dari nol:
1. Login sebagai `admin` → dapat JWT token.
2. Pakai token itu (`Authorization: Bearer {token}`) untuk:
   - Buat `Department` (`POST /api/departments`)
   - Buat `Employee` (`POST /api/employees`) — misal buat Manager dulu (tanpa `ManagerId`), lalu buat Employee biasa dengan `ManagerId` mengarah ke Manager tadi.
   - Buat akun `User` untuk masing-masing Employee (`POST /api/auth/register`, isi `EmployeeId` dan `Role`).
3. Login sebagai Employee/Manager yang baru dibuat untuk test fitur Attendance & LeaveRequest.

## 4. Poin penting soal desain

- **Employee butuh Manager untuk bisa mengajukan cuti.** Kalau `Employee.ManagerId`
  kosong, `POST /api/leave-requests` akan menolak dengan `BadRequestException`.
  Ini konsekuensi dari desain approval multi-level yang menelusuri rantai
  `ManagerId` ke atas (lihat `EmployeeRepository.GetManagerChainAsync`).
- Jumlah level approval maksimum diatur di `appsettings.json` → `LeaveApproval:MaxLevels`
  (default `2`). Rantai approval berhenti lebih awal kalau manager teratas
  sudah tidak punya manager lagi.
- `employeeId` yang dipakai di endpoint Attendance & LeaveRequest **selalu**
  diambil dari JWT claim milik user yang sedang login (lihat
  `ClaimsPrincipalExtensions.GetEmployeeId`), bukan dari body request — supaya
  seseorang tidak bisa absen atau mengajukan cuti atas nama orang lain.
- Relasi ke `Employee` (Manager, User, Attendance, LeaveApproval.Approver) sengaja
  di-set `DeleteBehavior.Restrict`, kecuali `LeaveRequest → Employee` yang `Cascade`.
  Ini untuk menghindari error "multiple cascade paths" dari EF Core saat generate
  migration (satu Employee dirujuk dari banyak tempat).

## 5. Langkah selanjutnya (belum termasuk di Fase 1 ini)

- Unit test (xUnit + Moq) di layer Service, mengikuti pola `ProductServiceTests.cs`
  di project referensi.
- Fase 2: Payroll & Penilaian KPI (lihat technical alignment document).
