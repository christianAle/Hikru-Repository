using HikruCodeChallenge.Application.DTOs;
using HikruCodeChallenge.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace HikruCodeChallenge.WebAPITesting
{

    [TestFixture]
    public class AssessmentControllerIntegrationTests
    {

        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
            // Si tu API requiere API Key:
            //_client.DefaultRequestHeaders.Add("X-API-Key", "hikru-api-key-2025");
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Post_CreateAssessment_ReturnsCreated()
        {
            var dto = new CreateAssessmentDto
            {
                Title = "Integración",
                Description = "Prueba integración",
                Location = "Remoto",
                Status = AssessmentStatus.Draft,
                RecruiterId = 1,
                DepartmentId = 1,
                Budget = 1000,
                ClosingDate = null
            };

            var response = await _client.PostAsJsonAsync("/api/assessments", dto);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<AssessmentDto>();
            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Title, result.Title);
        }

        [Test]
        public async Task Get_ById_ReturnsAssessment()
        {
            // Primero crea un assessment
            var dto = new CreateAssessmentDto
            {
                Title = "Consulta",
                Description = "Consulta por ID",
                Location = "Remoto",
                Status = AssessmentStatus.Draft,
                RecruiterId = 1,
                DepartmentId = 1,
                Budget = 500,
                ClosingDate = null
            };
            var postResponse = await _client.PostAsJsonAsync("/api/assessments", dto);
            var created = await postResponse.Content.ReadFromJsonAsync<AssessmentDto>();

            var getResponse = await _client.GetAsync($"/api/assessments/{created.Id}");

            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            var result = await getResponse.Content.ReadFromJsonAsync<AssessmentDto>();
            Assert.IsNotNull(result);
            Assert.AreEqual(created.Id, result.Id);
        }

        [Test]
        public async Task Put_UpdateAssessment_ReturnsNoContent()
        {
            // Crea un assessment
            var dto = new CreateAssessmentDto
            {
                Title = "Actualizar",
                Description = "Antes de actualizar",
                Location = "Remoto",
                Status = AssessmentStatus.Draft,
                RecruiterId = 1,
                DepartmentId = 1,
                Budget = 800,
                ClosingDate = null
            };
            var postResponse = await _client.PostAsJsonAsync("/api/assessments", dto);
            var created = await postResponse.Content.ReadFromJsonAsync<AssessmentDto>();

            // Actualiza el assessment
            var updateDto = new UpdateAssessmentDto
            {
                Title = "Actualizado",
                Description = "Después de actualizar",
                Location = "Presencial",
                Status = AssessmentStatus.Open,
                RecruiterId = 1,
                DepartmentId = 1,
                Budget = 900,
                ClosingDate = null
            };

            var putResponse = await _client.PutAsJsonAsync($"/api/assessments/{created.Id}", updateDto);

            Assert.AreEqual(HttpStatusCode.NoContent, putResponse.StatusCode);
        }

        [Test]
        public async Task Delete_Assessment_ReturnsNoContent()
        {
            // Crea un assessment
            var dto = new CreateAssessmentDto
            {
                Title = "Eliminar",
                Description = "Para eliminar",
                Location = "Remoto",
                Status = AssessmentStatus.Draft,
                RecruiterId = 1,
                DepartmentId = 1,
                Budget = 700,
                ClosingDate = null
            };
            var postResponse = await _client.PostAsJsonAsync("/api/assessments", dto);
            var created = await postResponse.Content.ReadFromJsonAsync<AssessmentDto>();

            // Elimina el assessment
            var deleteResponse = await _client.DeleteAsync($"/api/assessments/{created.Id}");

            Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}

