using HikruCodeChallenge.Domain.Enums;

namespace HikruCodeChallenge.Application.DTOs;

public class AssessmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public AssessmentStatus Status { get; set; }
    public int RecruiterId { get; set; }
    public int DepartmentId { get; set; }
    public decimal Budget { get; set; }
    public DateTime? ClosingDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

public class CreateAssessmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public AssessmentStatus Status { get; set; } = AssessmentStatus.Draft;
    public int RecruiterId { get; set; }
    public int DepartmentId { get; set; }
    public decimal Budget { get; set; }
    public DateTime? ClosingDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

public class UpdateAssessmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public AssessmentStatus Status { get; set; }
    public int RecruiterId { get; set; }
    public int DepartmentId { get; set; }
    public decimal Budget { get; set; }
    public DateTime? ClosingDate { get; set; }
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

public class AssessmentFilterDto
{
    public string? Title { get; set; }
    public string? Location { get; set; }
    public AssessmentStatus? Status { get; set; }
    public int? RecruiterId { get; set; }
    public int? DepartmentId { get; set; }
    public decimal? MinBudget { get; set; }
    public decimal? MaxBudget { get; set; }
    public DateTime? ClosingDateFrom { get; set; }
    public DateTime? ClosingDateTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
