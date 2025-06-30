using Microsoft.EntityFrameworkCore;
using HikruCodeChallenge.Application.Interfaces;
using HikruCodeChallenge.Domain.Entities;
using HikruCodeChallenge.Application.DTOs;
using HikruCodeChallenge.Infrastructure.Data;

namespace HikruCodeChallenge.Infrastructure.Repositories;

public class AssessmentRepository : IAssessmentRepository
{
    private readonly ApplicationDbContext _context;

    public AssessmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Assessment?> GetByIdAsync(int id)
    {
        return await _context.Assessments.FindAsync(id);
    }

    public async Task<PagedResult<Assessment>> GetPagedAsync(AssessmentFilterDto filter)
    {
        var query = _context.Assessments.AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(filter.Title))
        {
            query = query.Where(a => a.Title.Contains(filter.Title));
        }

        if (!string.IsNullOrEmpty(filter.Location))
        {
            query = query.Where(a => a.Location.Contains(filter.Location));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(a => a.Status == filter.Status.Value);
        }

        if (filter.RecruiterId.HasValue)
        {
            query = query.Where(a => a.RecruiterId == filter.RecruiterId.Value);
        }

        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(a => a.DepartmentId == filter.DepartmentId.Value);
        }

        if (filter.MinBudget.HasValue)
        {
            query = query.Where(a => a.Budget >= filter.MinBudget.Value);
        }

        if (filter.MaxBudget.HasValue)
        {
            query = query.Where(a => a.Budget <= filter.MaxBudget.Value);
        }

        if (filter.ClosingDateFrom.HasValue)
        {
            query = query.Where(a => a.ClosingDate >= filter.ClosingDateFrom.Value);
        }

        if (filter.ClosingDateTo.HasValue)
        {
            query = query.Where(a => a.ClosingDate <= filter.ClosingDateTo.Value);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        var items = await query
            .OrderByDescending(a => a.CreatedDate)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<Assessment>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalPages = totalPages,
            HasPreviousPage = filter.PageNumber > 1,
            HasNextPage = filter.PageNumber < totalPages
        };
    }

    public async Task<Assessment> CreateAsync(Assessment assessment)
    {
        _context.Assessments.Add(assessment);
        await _context.SaveChangesAsync();
        return assessment;
    }

    public async Task<Assessment> UpdateAsync(Assessment assessment)
    {
        _context.Entry(assessment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return assessment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var assessment = await _context.Assessments.FindAsync(id);
        if (assessment == null)
            return false;

        _context.Assessments.Remove(assessment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Assessments.AnyAsync(a => a.Id == id);
    }
}
