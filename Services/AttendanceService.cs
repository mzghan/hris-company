using HRIS.Api.Common;
using HRIS.Api.DTOs.Attendance;
using HRIS.Api.Exceptions;
using HRIS.Api.Models;
using HRIS.Api.Repositories;
using Microsoft.AspNetCore.Hosting;

namespace HRIS.Api.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<AttendanceService> _logger;
    private readonly IReferenceRepository _referenceRepository;
    private readonly IWebHostEnvironment _env;

    // Folder relatif di dalam wwwroot tempat foto absen disimpan.
    private const string UploadFolder = "uploads/attendance";

    public AttendanceService(
        IAttendanceRepository repository,
        IEmployeeRepository employeeRepository,
        ILogger<AttendanceService> logger,
        IWebHostEnvironment env,
        IReferenceRepository referenceRepository)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _logger = logger;
        _env = env;
        _referenceRepository = referenceRepository;
    }

    public async Task<AttendanceResponseDto> CheckInAsync(int employeeId, AttendanceCheckInDto dto)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException("Employee tidak ditemukan.");

        ValidatePhotoAndLocation(dto.PhotoBase64, dto.Latitude, dto.Longitude);

        var workTypeId = dto.WorkTypeId;
        if (workTypeId <= 0)
            workTypeId = await _referenceRepository.GetWorkTypeIdAsync("WFO") ?? 0;
        var workType = await _referenceRepository.GetWorkTypeAsync(workTypeId);
        if (workType is null)
            throw new BadRequestException("Work Type tidak ditemukan.");
        if (string.Equals(workType.Name, "WFH with Note", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(dto.Note))
            throw new BadRequestException("Catatan wajib diisi untuk WFH with Note.");

        var today = JakartaTime.Today();
        var existing = await _repository.GetByEmployeeAndDateAsync(employeeId, today);

        if (existing is not null)
            throw new BadRequestException("Sudah check-in hari ini.");

        var attendance = new Attendance
        {
            EmployeeId = employeeId,
            Date = today,
            CheckIn = JakartaTime.Now().TimeOfDay,
            WorkTypeId = workType.Id,
            Note = dto.Note?.Trim(),
            CheckInPhotoPath = SavePhoto(dto.PhotoBase64!, employeeId, "in"),
            CheckInLatitude = dto.Latitude,
            CheckInLongitude = dto.Longitude
        };

        await _repository.AddAsync(attendance);
        _logger.LogInformation("Employee {Id} check-in pada {Date} (WIB)", employeeId, today);

        attendance.Employee = employee;
        return ToDto(attendance);
    }

    public async Task<AttendanceResponseDto> CheckOutAsync(int employeeId, AttendanceCheckOutDto dto)
    {
        ValidatePhotoAndLocation(dto.PhotoBase64, dto.Latitude, dto.Longitude);

        var today = JakartaTime.Today();
        var attendance = await _repository.GetByEmployeeAndDateAsync(employeeId, today)
            ?? throw new BadRequestException("Belum check-in hari ini.");

        if (attendance.CheckOut is not null)
            throw new BadRequestException("Sudah check-out hari ini.");

        attendance.CheckOut = JakartaTime.Now().TimeOfDay;
        attendance.CheckOutPhotoPath = SavePhoto(dto.PhotoBase64!, employeeId, "out");
        attendance.CheckOutLatitude = dto.Latitude;
        attendance.CheckOutLongitude = dto.Longitude;

        await _repository.UpdateAsync(attendance);

        return ToDto(attendance);
    }

    public async Task<List<AttendanceResponseDto>> GetHistoryAsync(int employeeId)
    {
        var history = await _repository.GetByEmployeeAsync(employeeId);
        return history.Select(ToDto).ToList();
    }

    // Foto & lokasi wajib ada — ini yang menegakkan aturan "harus buka kamera
    // & lokasi" di level backend, bukan cuma di UI (supaya tidak bisa
    // dilewati lewat panggilan API langsung).
    private static void ValidatePhotoAndLocation(string? photoBase64, double? latitude, double? longitude)
    {
        if (string.IsNullOrWhiteSpace(photoBase64))
            throw new BadRequestException("Foto dari kamera wajib diambil saat absen.");

        if (latitude is null || longitude is null)
            throw new BadRequestException("Lokasi (GPS) wajib diaktifkan saat absen.");
    }

    // Decode data URL base64 hasil capture kamera browser, simpan sebagai
    // file JPEG di wwwroot/uploads/attendance, dan kembalikan path relatif
    // yang bisa langsung dipakai sebagai <img src="...">.
    private string SavePhoto(string photoBase64, int employeeId, string suffix)
    {
        var commaIndex = photoBase64.IndexOf(',');
        var base64Data = commaIndex >= 0 ? photoBase64[(commaIndex + 1)..] : photoBase64;

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(base64Data);
        }
        catch (FormatException)
        {
            throw new BadRequestException("Format foto tidak valid.");
        }

        var folderPath = Path.Combine(_env.WebRootPath, UploadFolder);
        Directory.CreateDirectory(folderPath);

        var fileName = $"{employeeId}_{JakartaTime.Now():yyyyMMdd_HHmmss}_{suffix}.jpg";
        var fullPath = Path.Combine(folderPath, fileName);
        File.WriteAllBytes(fullPath, bytes);

        return $"/{UploadFolder}/{fileName}";
    }

    private static AttendanceResponseDto ToDto(Attendance a) => new()
    {
        Id = a.Id,
        EmployeeId = a.EmployeeId,
        EmployeeName = a.Employee?.FullName ?? string.Empty,
        Date = a.Date,
        CheckIn = a.CheckIn,
        CheckOut = a.CheckOut,
        WorkTypeId = a.WorkTypeId,
        WorkTypeName = a.WorkType?.Name ?? string.Empty,
        Note = a.Note,
        CheckInPhotoUrl = a.CheckInPhotoPath,
        CheckInLatitude = a.CheckInLatitude,
        CheckInLongitude = a.CheckInLongitude,
        CheckOutPhotoUrl = a.CheckOutPhotoPath,
        CheckOutLatitude = a.CheckOutLatitude,
        CheckOutLongitude = a.CheckOutLongitude
    };
}
