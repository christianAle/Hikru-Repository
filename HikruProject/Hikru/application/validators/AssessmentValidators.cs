using FluentValidation;
using HikruCodeChallenge.Application.DTOs;
using HikruCodeChallenge.Domain.Enums;

namespace HikruCodeChallenge.Application.Validators;

public class CreateAssessmentValidator : AbstractValidator<CreateAssessmentDto>
{
    public CreateAssessmentValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value");

        RuleFor(x => x.RecruiterId)
            .GreaterThan(0).WithMessage("RecruiterId must be greater than 0");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId must be greater than 0");

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than 0");

        RuleFor(x => x.ClosingDate)
            .GreaterThan(DateTime.Now).When(x => x.ClosingDate.HasValue)
            .WithMessage("Closing date must be in the future");
    }
}

public class UpdateAssessmentValidator : AbstractValidator<UpdateAssessmentDto>
{
    public UpdateAssessmentValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value");

        RuleFor(x => x.RecruiterId)
            .GreaterThan(0).WithMessage("RecruiterId must be greater than 0");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId must be greater than 0");

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than 0");

        RuleFor(x => x.ClosingDate)
            .GreaterThan(DateTime.Now).When(x => x.ClosingDate.HasValue)
            .WithMessage("Closing date must be in the future");
    }
}

public class AssessmentFilterValidator : AbstractValidator<AssessmentFilterDto>
{
    public AssessmentFilterValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.MinBudget)
            .GreaterThanOrEqualTo(0).When(x => x.MinBudget.HasValue)
            .WithMessage("Minimum budget must be greater than or equal to 0");

        RuleFor(x => x.MaxBudget)
            .GreaterThanOrEqualTo(x => x.MinBudget).When(x => x.MinBudget.HasValue && x.MaxBudget.HasValue)
            .WithMessage("Maximum budget must be greater than or equal to minimum budget");

        RuleFor(x => x.Status)
            .IsInEnum().When(x => x.Status.HasValue)
            .WithMessage("Invalid status value");
    }
}
