using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface ILetterRepository { Task<List<LetterType>> GetTypesAsync(bool includeInactive=false); Task<LetterType?> GetTypeByIdAsync(int id); Task<List<LetterRequest>> GetRequestsAsync(int? employeeId=null); Task<LetterRequest?> GetRequestByIdAsync(int id); Task<LetterType> AddTypeAsync(LetterType x); Task UpdateTypeAsync(LetterType x); Task<LetterRequest> AddRequestAsync(LetterRequest x); Task UpdateRequestAsync(LetterRequest x); }
