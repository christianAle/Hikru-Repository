using AutoFixture;
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
    public class AssessmentUpdateCommandTest
    {
        private Mock<IAssessmentRepository> _repositoryMock;
        private UpdateAssessmentHandler _handler;
        private IFixture _fixture; // Fixed type declaration for '_fixture'

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IAssessmentRepository>();
            _fixture = new Fixture();
            _handler = new UpdateAssessmentHandler(_repositoryMock.Object, MapperBuilder.Create());
            _repositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Assessment>()))
                .ReturnsAsync(It.IsAny<Assessment>());
            _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Create<Assessment>());
        }

        [Test]
        public async Task UpdateAssessment_ShouldReturnUpdatedAssessment_WhenValidDataProvided()
        {
            // Arrange  
            var assessmentDto = _fixture.Build<UpdateAssessmentDto>().With(x=> x.Id, 1).Create();
            var command = _fixture.Create<UpdateAssessmentCommand>(); // Fixed instantiation of 'command'
            command.Assessment = assessmentDto;
            // Act  
            await _handler.Handle(command, CancellationToken.None);
            // Assert  
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Assessment>()));
        }
    }
}
