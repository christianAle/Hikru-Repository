using AutoFixture;
using AutoMapper;
using HikruCodeChallenge.Application.Commands;
using HikruCodeChallenge.Application.DTOs;
using HikruCodeChallenge.Application.Interfaces;
using HikruCodeChallenge.Application.Mappings;
using HikruCodeChallenge.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.CommandTest
{
   [TestFixture]
    public class AssessmentCreateCommandTest
    {
        private Mock<IAssessmentRepository> _repositoryMock;
        private CreateAssessmentHandler _handler;
        private IFixture _fixture; // Fixed type declaration for '_fixture'

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IAssessmentRepository>();
            _fixture = new Fixture();
            _handler = new CreateAssessmentHandler(_repositoryMock.Object, MapperBuilder.Create());
            _repositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Assessment>()))
                .ReturnsAsync(It.IsAny<Assessment>());
        }

        [Test]
        public async Task CreateAssessment_ShouldReturnCreatedAssessment_WhenValidDataProvided()
        {
            // Arrange  
            var assessmentDto = _fixture.Create<CreateAssessmentDto>();
            var command = _fixture.Create<CreateAssessmentCommand>(); // Fixed instantiation of 'command'
            command.Assessment = assessmentDto;
            
            // Act  
            await _handler.Handle(command, CancellationToken.None);

            // Assert  
            _repositoryMock.Verify(x=> x.CreateAsync(It.IsAny<Assessment>()));
        }
    }
}
