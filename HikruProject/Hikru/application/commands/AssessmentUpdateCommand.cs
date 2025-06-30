using AutoMapper;
using HikruCodeChallenge.Application.DTOs;
using HikruCodeChallenge.Application.Interfaces;
using MediatR;

namespace HikruCodeChallenge.Application.Commands
{
    public class UpdateAssessmentCommand : IRequest<AssessmentDto?>
    {
        public int Id { get; set; }
        public UpdateAssessmentDto Assessment { get; set; } = new();
    }

    public class UpdateAssessmentHandler : IRequestHandler<UpdateAssessmentCommand, AssessmentDto?>
    {
        private readonly IAssessmentRepository _repository;
        private readonly IMapper _mapper;

        public UpdateAssessmentHandler(IAssessmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AssessmentDto?> Handle(UpdateAssessmentCommand request, CancellationToken cancellationToken)
        {
            var existingAssessment = await _repository.GetByIdAsync(request.Id);
            if (existingAssessment == null)
                return null;

            _mapper.Map(request.Assessment, existingAssessment);
            existingAssessment.UpdatedDate = DateTime.UtcNow;

            var updatedAssessment = await _repository.UpdateAsync(existingAssessment);
            return _mapper.Map<AssessmentDto>(updatedAssessment);
        }
    }

}
