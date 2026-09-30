# HRIS API (MVP 3, Batch 0: Rombak DB)

.NET 8 + EF Core + SQLite, pola `Controller → Service → Repository → DbContext`.
Payroll dihapus. Database dirombak mengikuti ERD (`MST_*`, `REF_*`, `SYS_*`, `TRX_*`)
dan dokumen `HRIS-MVP3-DB-Design.md`.

## Yang sudah ada di Batch 0
- **Auth** — JWT + cookie. Role tersimpan hanya `Employee`, `HR`, `Support`.
  Manager/Head/Group Head bukan role: claim `IsManager` diisi saat login kalau punya bawahan aktif.
- **Employee** — data pribadi + Employment + Hierarchy. Perubahan jabatan/grade/organisasi
  (`PUT /api/employees/{id}/employment`) dan atasan (`PUT /api/employees/{id}/manager`)
  menutup baris lama lalu membuat baris baru, jadi riwayat terjaga.
- **Organization** — CRUD bertingkat (`REF_Organization`), menggantikan Department.
- **References** — `GET /api/references/{type}` untuk dropdown REF_*.
- **Attendance, Leave, KPI** — dibawa dari versi lama, sudah diarahkan ke model baru.
  Leave masih memakai `LeaveApproval` lama sampai approval engine (Batch A).

## Menjalankan (database direset, bukan migrasi data)
```bash
dotnet restore
rm -f hris.db hris.db-shm hris.db-wal
dotnet ef migrations add InitialCreate
dotnet run
```
Migration otomatis di-apply saat startup (`db.Database.Migrate()`), lalu seeder mengisi REF_*, role,
akun Support, dan data dummy perusahaan.

## Login awal (dari seed)
| Username | Password | Keterangan |
|---|---|---|
| `support` | `Support123!` | Akun Support tanpa Employee (`appsettings.json` → `SeedSupport`) |
| `hana` | `Password123!` | HR |
| `budi`, `siti`, `andi` | `Password123!` | Otomatis `IsManager` (punya bawahan) |
| `dewi`, `rudi`, `maya` | `Password123!` | Employee |

**Ganti semua password ini** sebelum dipakai serius.

## Catatan desain
- Kolom DB snake_case lewat `UseSnakeCaseNamingConvention()`; nama tabel dan kolom PK diatur eksplisit di `AppDbContext`.
- Kolom audit (`created_at/by`, `updated_at/by`) diisi otomatis di `AppDbContext.SaveChangesAsync`.
- Employment terkini = `end_date IS NULL`, dijaga unique index parsial (satu per karyawan).
- Outsource wajib `vendor_id`; tipe lain harus kosong (divalidasi di `EmployeeService`).
- Akun Support tidak bisa dibuat oleh HR; hanya Support yang boleh memberi role Support.
