using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Features.Watches;

namespace TicketAlert.Api.Controllers;

[ApiController]
[Route("api/watches")]
[Authorize]
public class WatchesController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(WatchDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateWatchRequest request,
        [FromServices] ICreateWatchHandler handler)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var result = await handler.HandleAsync(Guid.Parse(userId), request);
        return CreatedAtAction(nameof(Create), result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WatchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyWatches(
        [FromQuery] string? status,
        [FromServices] IGetUserWatchesHandler handler)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var result = await handler.HandleAsync(Guid.Parse(userId), status);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromServices] ICancelWatchHandler handler)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        await handler.HandleAsync(Guid.Parse(userId), id);
        return NoContent();
    }
}
