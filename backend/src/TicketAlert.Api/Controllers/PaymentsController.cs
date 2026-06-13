using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAlert.Application.Common.DTOs;
using TicketAlert.Application.Features.Payments;

namespace TicketAlert.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    [HttpPost("checkout")]
    [Authorize]
    [ProducesResponseType(typeof(CheckoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCheckoutSession(
        [FromBody] CreateCheckoutRequest request,
        [FromServices] ICreateCheckoutSessionHandler handler)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var result = await handler.HandleAsync(Guid.Parse(userId), request);
        return Ok(result);
    }

    [HttpPost("webhook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StripeWebhook(
        [FromServices] IStripeWebhookHandler handler)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = HttpContext.Request.Headers["Stripe-Signature"].ToString();
        await handler.HandleAsync(json, signature);
        return Ok();
    }

    [HttpGet("history")]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentHistory(
        [FromServices] IGetPaymentHistoryHandler handler)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var result = await handler.HandleAsync(Guid.Parse(userId));
        return Ok(result);
    }
}
