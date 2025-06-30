using Microsoft.AspNetCore.Mvc;
using MediatR;
using FluentValidation;
using HikruCodeChallenge.Application.Commands;
using HikruCodeChallenge.Application.Queries;
using HikruCodeChallenge.Application.DTOs;

namespace HikruCodeChallenge.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AssessmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateAssessmentDto> _createValidator;
    private readonly IValidator<UpdateAssessmentDto> _updateValidator;
    private readonly IValidator<AssessmentFilterDto> _filterValidator;

    public AssessmentsController(
        IMediator mediator,
        IValidator<CreateAssessmentDto> createValidator,
        IValidator<UpdateAssessmentDto> updateValidator,
        IValidator<AssessmentFilterDto> filterValidator)
    {
        _mediator = mediator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _filterValidator = filterValidator;
    }

    /// <summary>
    /// Get all assessments with optional filtering and pagination
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <returns>Paged list of assessments</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AssessmentDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<PagedResult<AssessmentDto>>> GetAssessments([FromQuery] AssessmentFilterDto filter)
    {
        var validationResult = await _filterValidator.ValidateAsync(filter);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { 
                message = "Validation failed", 
                errors = validationResult.Errors.Select(e => e.ErrorMessage) 
            });
        }

        var query = new GetAssessmentsQuery { Filter = filter };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get assessment by ID
    /// </summary>
    /// <param name="id">Assessment ID</param>
    /// <returns>Assessment details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AssessmentDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AssessmentDto>> GetAssessment(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid assessment ID");
        }

        var query = new GetAssessmentByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound($"Assessment with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new assessment
    /// </summary>
    /// <param name="createDto">Assessment data</param>
    /// <returns>Created assessment</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AssessmentDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AssessmentDto>> CreateAssessment([FromBody] CreateAssessmentDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { 
                message = "Validation failed", 
                errors = validationResult.Errors.Select(e => e.ErrorMessage) 
            });
        }

        var command = new CreateAssessmentCommand { Assessment = createDto };
        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetAssessment), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing assessment
    /// </summary>
    /// <param name="id">Assessment ID</param>
    /// <param name="updateDto">Updated assessment data</param>
    /// <returns>Updated assessment</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AssessmentDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AssessmentDto>> UpdateAssessment(int id, [FromBody] UpdateAssessmentDto updateDto)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid assessment ID");
        }

        var validationResult = await _updateValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { 
                message = "Validation failed", 
                errors = validationResult.Errors.Select(e => e.ErrorMessage) 
            });
        }

        var command = new UpdateAssessmentCommand { Id = id, Assessment = updateDto };
        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound($"Assessment with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete an assessment
    /// </summary>
    /// <param name="id">Assessment ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> DeleteAssessment(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid assessment ID");
        }

        var command = new DeleteAssessmentCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result)
        {
            return NotFound($"Assessment with ID {id} not found");
        }

        return Ok(new { message = "Assessment deleted successfully" });
    }
}
