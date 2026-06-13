using TicketAlert.Application.Common.DTOs;

namespace TicketAlert.Application.Features.Payments;

public interface ICreateCheckoutSessionHandler
{
    Task<CheckoutResponse> HandleAsync(Guid userId, CreateCheckoutRequest request);
}

public interface IStripeWebhookHandler
{
    Task HandleAsync(string json, string signature);
}

public interface IGetPaymentHistoryHandler
{
    Task<IReadOnlyList<PaymentDto>> HandleAsync(Guid userId);
}
