using AutoMapper;
using HikruCodeChallenge.Application.DTOs;
using HikruCodeChallenge.Application.Interfaces;
using HikruCodeChallenge.Domain.Entities;
using MediatR;

namespace HikruCodeChallenge.Application.Commands
{
    public class DeleteAssessmentCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
    public class DeleteAssessmentHandler : IRequestHandler<DeleteAssessmentCommand, bool>
    {
        private readonly IAssessmentRepository _repository;

        public DeleteAssessmentHandler(IAssessmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteAssessmentCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteAsync(request.Id);
        }
    }
}
