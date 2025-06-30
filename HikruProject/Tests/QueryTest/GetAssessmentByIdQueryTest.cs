using AutoFixture;
using HikruCodeChallenge.Application.Commands;
using HikruCodeChallenge.Application.DTOs;
using HikruCodeChallenge.Application.Interfaces;
using HikruCodeChallenge.Application.Mappings;
using HikruCodeChallenge.Application.Queries;
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
    public class GetAssessmentsQueryTest
    {
        private Mock<IAssessmentRepository> _repositoryMock;
        private GetAssessmentByIdHandler _handler;
        private IFixture _fixture; // Fixed type declaration for '_fixture'

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IAssessmentRepository>();
            _fixture = new Fixture();
            _handler = new GetAssessmentByIdHandler(_repositoryMock.Object, MapperBuilder.Create());
            _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(It.IsAny<Assessment>());
        }

        [Test]
        public async Task Handle_ValidId_ReturnsAssessmentDto()
        {
            // Arrange
            var assessment = _fixture.Create<Assessment>();
            _repositoryMock.Setup(repo => repo.GetByIdAsync(assessment.Id))
                .ReturnsAsync(assessment);
            var query = new GetAssessmentByIdQuery { Id = assessment.Id };
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(assessment.Id, result.Id);
            Assert.AreEqual(assessment.Title, result.Title);
        }
    }
}
