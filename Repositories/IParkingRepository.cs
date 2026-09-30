using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface IParkingRepository { Task<List<VehicleType>> GetVehicleTypesAsync(); Task<List<ParkingRegistration>> GetAllAsync(int? employeeId=null); Task<ParkingRegistration?> GetByIdAsync(int id); Task<bool> ActivePlateExistsAsync(string plate,int? exceptId=null); Task<ParkingRegistration> AddAsync(ParkingRegistration x); Task UpdateAsync(ParkingRegistration x); }
