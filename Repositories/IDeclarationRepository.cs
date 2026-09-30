using HRIS.Api.Models;
namespace HRIS.Api.Repositories;
public interface IDeclarationRepository { Task<List<DeclarationTemplate>> GetTemplatesAsync(bool includeInactive=false); Task<DeclarationTemplate?> GetTemplateByIdAsync(int id); Task<List<DeclarationSubmission>> GetSubmissionsAsync(int? employeeId=null); Task<DeclarationSubmission?> GetSubmissionByIdAsync(int id); Task<DeclarationTemplate> AddTemplateAsync(DeclarationTemplate x); Task UpdateTemplateAsync(DeclarationTemplate x); Task<DeclarationSubmission> AddSubmissionAsync(DeclarationSubmission x); }
