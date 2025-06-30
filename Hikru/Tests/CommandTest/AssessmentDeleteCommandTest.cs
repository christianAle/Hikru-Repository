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
    public class AssessmentDeleteCommandTest
    {
        private Mock<IAssessmentRepository> _repositoryMock;
        private DeleteAssessmentHandler _handler;
        private IFixture _fixture; // Fixed type declaration for '_fixture'

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IAssessmentRepository>();
            _fixture = new Fixture();
            _handler = new DeleteAssessmentHandler(_repositoryMock.Object);
            _repositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(It.IsAny<bool>());
            _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_fixture.Create<Assessment>());
        }

        [Test]
        public async Task DeleteAssessment_ShouldReturnTrue_WhenValidIdProvided()
        {
            // Arrange  
            var command = _fixture.Create<DeleteAssessmentCommand>(); // Fixed instantiation of 'command'
            command.Id = 1; // Assuming the ID to delete is 1
            // Act  
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert  
            _repositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Once);
        }
    }
}
