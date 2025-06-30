using AutoMapper;
using HikruCodeChallenge.Application.DTOs;
using HikruCodeChallenge.Application.Interfaces;
using HikruCodeChallenge.Domain.Entities;
using MediatR;

namespace HikruCodeChallenge.Application.Commands
{
    public class CreateAssessmentCommand : IRequest<AssessmentDto>
    {
        public CreateAssessmentDto Assessment { get; set; } = new();
    }

    public class CreateAssessmentHandler : IRequestHandler<CreateAssessmentCommand, AssessmentDto>
    {
        private readonly IAssessmentRepository _repository;
        private readonly IMapper _mapper;

        public CreateAssessmentHandler(IAssessmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AssessmentDto> Handle(CreateAssessmentCommand request, CancellationToken cancellationToken)
        {
            var assessment = _mapper.Map<Assessment>(request.Assessment);
            assessment.CreatedDate = DateTime.UtcNow;
            assessment.UpdatedDate = DateTime.UtcNow;

            var createdAssessment = await _repository.CreateAsync(assessment);
            return _mapper.Map<AssessmentDto>(createdAssessment);
        }
    }
}
