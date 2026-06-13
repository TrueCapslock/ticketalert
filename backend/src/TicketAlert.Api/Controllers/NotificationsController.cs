using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Features.Notifications;

namespace TicketAlert.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(NotificationListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyNotifications(
        [FromServices] IGetUserNotificationsHandler handler,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var result = await handler.HandleAsync(Guid.Parse(userId), page, pageSize);
        return Ok(result);
    }

    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAsRead(
        Guid id,
        [FromServices] IMarkNotificationReadHandler handler)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        await handler.HandleAsync(Guid.Parse(userId), id);
        return NoContent();
    }
}
