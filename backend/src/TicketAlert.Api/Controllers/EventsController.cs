using Microsoft.AspNetCore.Mvc;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Features.Events;

namespace TicketAlert.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    [HttpGet("search")]
    [ProducesResponseType(typeof(EventSearchResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] EventSearchRequest request,
        [FromServices] ISearchEventsHandler handler)
    {
        var result = await handler.HandleAsync(request);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromServices] IGetEventHandler handler)
    {
        var result = await handler.HandleAsync(id);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpGet("ticketmaster/{ticketmasterEventId}")]
    [ProducesResponseType(typeof(EventDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByTicketmasterId(
        string ticketmasterEventId,
        [FromServices] IGetEventHandler handler)
    {
        var result = await handler.HandleByTicketmasterIdAsync(ticketmasterEventId);
        return result is not null ? Ok(result) : NotFound();
    }
}
