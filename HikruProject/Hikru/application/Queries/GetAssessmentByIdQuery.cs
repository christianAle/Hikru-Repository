using AutoMapper;
using HikruCodeChallenge.Application.DTOs;
using HikruCodeChallenge.Application.Interfaces;
using MediatR;

namespace HikruCodeChallenge.Application.Queries;

public class GetAssessmentByIdQuery : IRequest<AssessmentDto?>
{
    public int Id { get; set; }
}

public class GetAssessmentByIdHandler : IRequestHandler<GetAssessmentByIdQuery, AssessmentDto?>
{
    private readonly IAssessmentRepository _repository;
    private readonly IMapper _mapper;

    public GetAssessmentByIdHandler(IAssessmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AssessmentDto?> Handle(GetAssessmentByIdQuery request, CancellationToken cancellationToken)
    {
        var assessment = await _repository.GetByIdAsync(request.Id);
        return assessment != null ? _mapper.Map<AssessmentDto>(assessment) : null;
    }
}
