using MediatR;
using Microsoft.AspNetCore.Mvc;
using rezolvam.Application.Commands.CreateReport;
using rezolvam.Application.Commands.DeleteReport;
using rezolvam.Application.Commands.UpdateReport;
using rezolvam.Application.Commands.UpdateReportStatus;
using rezolvam.Application.Queries;

namespace rezolvam.Api.Controllers
{
    [ApiController]
    [Route("api/reports")]  // Fixed route name
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _mediator.Send(new ReportQuery());
            return Ok(reports);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReportCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id = result }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReportCommand command)
        {
            var commandWithId = command with { Id = id };
            var result = await _mediator.Send(commandWithId);
            return Ok(result);
        }

        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateReportStatusCommand command)
        {
            var commandWithId = command with { Id = id };
            var result = await _mediator.Send(commandWithId);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteReportCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}