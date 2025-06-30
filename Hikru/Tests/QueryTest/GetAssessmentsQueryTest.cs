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
    public class GetAssessmentByIdQueryTest
    {
        private Mock<IAssessmentRepository> _repositoryMock;
        private GetAssessmentsHandler _handler;
        private IFixture _fixture; // Fixed type declaration for '_fixture'

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IAssessmentRepository>();
            _fixture = new Fixture();
            _handler = new GetAssessmentsHandler(_repositoryMock.Object, MapperBuilder.Create());
            _repositoryMock.Setup(repo => repo.GetPagedAsync(It.IsAny<AssessmentFilterDto>()))
                .ReturnsAsync(It.IsAny<PagedResult<Assessment>>());
        }

        [Test]
        public async Task GetAssessments_ShouldReturnPagedResult_WhenValidFilterProvided()
        {
            // Arrange
            var assessment = _fixture.Create<GetAssessmentsQuery>();
            var filter = _fixture.Create<AssessmentFilterDto>();
            assessment.Filter = filter;
            var expectedPagedResult = _fixture.Create<PagedResult<Assessment>>();
            _repositoryMock.Setup(repo => repo.GetPagedAsync(filter))
                .ReturnsAsync(expectedPagedResult);
            // Act  
            var result = await _handler.Handle(assessment, CancellationToken.None);
            // Assert  
            Assert.IsNotNull(result);
            _repositoryMock.Verify(x => x.GetPagedAsync(filter), Times.Once);
        }
    }
}
