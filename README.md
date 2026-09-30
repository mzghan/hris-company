# HRIS API (MVP 3, Batch D: Aturan Rumit)

.NET 8 + EF Core + SQLite, pola `Controller → Service → Repository → DbContext`.
Payroll dihapus. Database dirombak mengikuti ERD (`MST_*`, `REF_*`, `SYS_*`, `TRX_*`)
dan dokumen `HRIS-MVP3-DB-Design.md`.

## Fondasi Batch 0 yang sudah ada
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


## Batch C: Approval sederhana (modul 3, 5, 7, 8, 9, 10)

- **Eligibility** — `REF_Module_Eligibility` mengikuti aturan DB Design: Permanent semua modul; Contract/Outsource menolak modul 3, 5, 6, 10. Service Family, Service Award, dan Laptop memeriksa eligibility sebelum menerima/menampilkan fitur.
- **Benefit / Family Change (3)** — `TRX_Family_Change_Request` menyimpan snapshot perubahan. Approval HR menerapkan Add/Update/Delete ke `MST_Employee_Family` hanya setelah Approved.
- **HR Letter (7)** — `REF_Letter_Type` + `TRX_Letter_Request`; approval HR, lalu HR/Support dapat mengaitkan dokumen hasil sebagai `issued_document_id`.
- **Parking (8)** — `REF_Vehicle_Type` + `TRX_Parking_Registration`; approval HR lalu status `CardReady` → `Collected`. Nomor polisi unik selama registrasi tidak Rejected/Cancelled.
- **Declaration Letter (9)** — `TRX_Declaration_Template` + `TRX_Declaration_Submission`; employee mencatat persetujuan terhadap template aktif. Tidak dibuat approval karena desain DB tidak menetapkan approval untuk modul ini.
- **Laptop Ownership (10)** — `TRX_Laptop_Request` + `TRX_Laptop_Status_Log`; approval HR, lalu status operasional `ForwardedToIT` → `Purchased` → `ResetDone`.
- **Amenities / Service Award (5)** — `TRX_Service_Award`; milestone dibuat untuk kelipatan 5 tahun dari `MST_Employee.join_date`, dengan status `Pending` → `Reminded` → `Given`.

### Migration Batch C

```powershell
Remove-Item hris.db* -Force -ErrorAction SilentlyContinue
dotnet ef database update
dotnet run
```


## Batch D: Aturan rumit (modul 2, 4, 6, 18)

- **Attendance (2)** — `TRX_Attendance` sekarang menyimpan `work_type_id` dan `note`, dengan `REF_Work_Type` (WFO, WFH, WFH with Note). Bukti foto + GPS tetap wajib. `WFH with Note` mewajibkan catatan. Riwayat hari kerja tetap unik per employee/tanggal.
- **Leave (2)** — `REF_Leave_Type`, `TRX_Leave_Balance`, dan `REF_Public_Holiday` ditambahkan. Hari cuti dihitung Senin-Jumat dikurangi public holiday. Saldo dicek saat submit dan dipotong pada approval final. Leave tetap memakai approval engine Batch A; cuti >= 6 hari kerja memakai langkah Head dari konfigurasi flow.
- **Booking Meeting (4)** — `REF_Meeting_Room` + `TRX_Room_Booking`. Sistem menolak overlap waktu pada ruang yang sama. HR/Support dapat membuat/nonaktifkan ruang dan menandai booking prioritas; pembuat dapat membatalkan booking.
- **Flexible Benefit (6)** — `TRX_Flex_Period`, `TRX_Leave_Encashment`, dan `TRX_Health_Claim`. Penjualan cuti dan klaim kesehatan masuk approval engine. Health claim menyimpan nominal integer rupiah dan tidak memiliki plafon otomatis; status akhir `Approved`/`Paid` dapat diproses manual oleh HR. Saldo cuti yang dijual dicatat pada `sold`.
- **Manpower Request (18)** — `TRX_Manpower_Request` memakai flow ManagerChain(1) → ManagerChain(2) → HR. Pengaju hanya Manager, HR, atau Support. Approval final mengubah status menjadi `Approved`.
- **UI** — Navbar atas diganti menjadi sidebar responsif di `Pages/Shared/_Layout.cshtml`; menu Batch D ditambahkan dan dikelompokkan mengikuti modul.

### Migration Batch D
```powershell
Remove-Item hris.db* -Force -ErrorAction SilentlyContinue
dotnet ef database update
dotnet run
```

