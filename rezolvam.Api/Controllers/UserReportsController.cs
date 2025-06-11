using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rezolvam.Application.Commands.Report;
using rezolvam.Application.DTOs;
using rezolvam.Application.DTOs.Common;
using rezolvam.Application.Queries.Report;

namespace rezolvam.Api.Controllers;

[ApiController]
[Route("api/user/reports")]
[Authorize(Policy = "JwtPolicy")]
public class UserReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ReportDto>>> Get([FromQuery] PaginationRequest pagination)
    {
        var query = new GetPublicReportsQuery { Request = pagination };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ReportDto>> Create([FromBody] CreateReportCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }
}
