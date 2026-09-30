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

## Batch A: Fondasi (approval engine, dokumen, notifikasi, email + job)

- **Approval engine** (`TRX_Approval_Request/Step`, `REF_Approval_Flow_Step`): approver di-resolve saat submit dan disimpan sebagai snapshot.
  Alur tiap jenis pengajuan ada di tabel konfigurasi, bisa diubah HR lewat `PUT /api/approvals/flows/{id}` tanpa deploy.
  Tipe approver: `ManagerChain` (naik N tingkat lewat Direct Manager; rantai habis = langkah dilewati), `Role`, `Employee`.
  Modul baru cukup: tambah `IApprovalHandler` (efek saat selesai), seed flow step, lalu panggil `IApprovalService.SubmitAsync`.
- **Leave** dimigrasi ke engine (`TRX_Leave_Approval` dihapus). Cuti >= 6 hari kerja otomatis butuh langkah kedua (Head).
  Endpoint approve/reject pindah ke `/api/approvals/{id}/approve|reject`. Ada `POST /api/leaverequests/{id}/cancel`.
- **Support** boleh bertindak atas langkah approval siapa pun (tidak untuk pengajuannya sendiri); tercatat di `SYS_Audit_Log`
  (`GET /api/audit-logs`, khusus Support). Pembukaan dokumen pribadi orang lain oleh Support juga dicatat.
- **Document** (`REF_Document_Category`, `TRX_Document`): file disimpan di `App_Data/documents` (bukan wwwroot), diunduh lewat endpoint
  yang mengecek kepemilikan. Dokumen pribadi hanya untuk pemilik, HR, Support. Batas ukuran dan ekstensi di `appsettings.json` -> `Documents`.
- **Notification** (`TRX_Notification`) + **email outbox** (`TRX_Email_Outbox`): notifikasi dan antrean email ditulis satu transaction dengan datanya.
  `EmailDispatchJob` mengirim dari antrean dengan retry (backoff), gagal SMTP tidak menggagalkan proses bisnis.
  `Email:Mode`: `Log` (default, hanya tulis log), `Smtp`, `Off`.
- **Background job**: implement `IRecurringJob`, daftarkan di `Program.cs`. Jadwal di memori, hanya untuk satu instance aplikasi.

### Migration Batch A
```powershell
Remove-Item hris.db* -Force -ErrorAction SilentlyContinue
dotnet ef migrations add BatchA_Foundation
dotnet run
```
Database sebaiknya direset karena `TRX_Leave_Request` berubah dan data cuti lama tidak punya approval di engine.


## Batch B: Konten (HR Forms, Regulation, Learning, YES, EAP, Expatriate)

- **HR Forms (modul 15)** memakai `TRX_Document` + kategori `Forms`/subkategori yang sudah ada. Halaman `/Forms/Index` memberi daftar formulir dan upload untuk HR/Support.
- **HR Regulation (modul 16)** memakai `TRX_Regulation`, CRUD untuk HR/Support, dan tampilan accordion untuk semua user login.
- **My Learning Portal (modul 1)** memakai `TRX_Learning_Material` yang menunjuk ke `TRX_Document` kategori `Learning`. File tetap mengikuti storage/permission Batch A.
- **YES (modul 20)** tidak membuat tabel baru. URL disimpan di `appsettings.json` pada `Yes:Url` dan ditampilkan di `/YES/Index`.
- **EAP (modul 17)** memakai `TRX_Assistance_Request`. `employee_id` nullable untuk pengajuan anonim; identitas anonim tidak ditampilkan, termasuk untuk Support. HR/Support dapat memperbarui status.
- **Expatriate Portal (modul 19)** memakai `MST_Employee_Identity` + `TRX_Document`/kategori Personal Documents. Ekspatriat ditentukan dari `nationality_country_id` yang bukan `ID`; identitas dapat dikelola oleh pemilik, HR, atau Support.

### Migration Batch B

```powershell
Remove-Item hris.db* -Force -ErrorAction SilentlyContinue
dotnet ef database update
dotnet run
```

Jika database masih mengikuti Batch A, migration `BatchB_Content` membuat `TRX_Learning_Material`, `TRX_Regulation`, dan `TRX_Assistance_Request`. Seeder menambahkan contoh regulasi development.
