using HikruCodeChallenge.Domain.Entities;
using HikruCodeChallenge.Application.DTOs;

namespace HikruCodeChallenge.Application.Interfaces;

public interface IAssessmentRepository
{
    Task<Assessment?> GetByIdAsync(int id);
    Task<PagedResult<Assessment>> GetPagedAsync(AssessmentFilterDto filter);
    Task<Assessment> CreateAsync(Assessment assessment);
    Task<Assessment> UpdateAsync(Assessment assessment);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
