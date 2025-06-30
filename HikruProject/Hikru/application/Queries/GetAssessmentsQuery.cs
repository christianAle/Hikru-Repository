using AutoMapper;
using HikruCodeChallenge.Application.DTOs;
using HikruCodeChallenge.Application.Interfaces;
using MediatR;

namespace HikruCodeChallenge.Application.Queries;

public class GetAssessmentsQuery : IRequest<PagedResult<AssessmentDto>>
{
    public AssessmentFilterDto Filter { get; set; } = new();
}

public class GetAssessmentsHandler : IRequestHandler<GetAssessmentsQuery, PagedResult<AssessmentDto>>
{
    private readonly IAssessmentRepository _repository;
    private readonly IMapper _mapper;

    public GetAssessmentsHandler(IAssessmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<AssessmentDto>> Handle(GetAssessmentsQuery request, CancellationToken cancellationToken)
    {
        var pagedResult = await _repository.GetPagedAsync(request.Filter);

        return new PagedResult<AssessmentDto>
        {
            Items = _mapper.Map<IEnumerable<AssessmentDto>>(pagedResult.Items),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages,
            HasPreviousPage = pagedResult.HasPreviousPage,
            HasNextPage = pagedResult.HasNextPage
        };
    }
}
