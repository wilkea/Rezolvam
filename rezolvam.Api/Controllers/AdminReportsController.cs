using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rezolvam.Application.Commands.Report;
using rezolvam.Application.DTOs.Common;
using rezolvam.Application.DTOs;
using rezolvam.Application.Queries.Report;

namespace rezolvam.Api.Controllers;

[ApiController]
[Route("api/admin/reports")]
[Authorize(Roles = "Admin", Policy = "JwtPolicy")]
public class AdminReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ReportDto>>> Get([FromQuery] PaginationRequest pagination)
    {
        var query = new GetReportsQuery { Pagination = pagination };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReportDetailDto?>> Get(Guid id)
    {
        var query = new GetReportByIdQuery { ReportId = id, UserId = Guid.Empty, IsAdmin = true };
        var result = await _mediator.Send(query);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ReportDto>> Create([FromBody] CreateReportCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }
}
