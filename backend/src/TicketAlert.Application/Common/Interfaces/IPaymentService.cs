namespace TicketAlert.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(Guid userId, Guid watchId, decimal amount, string currency, string successUrl, string cancelUrl);
    Task<bool> ProcessWebhookAsync(string json, string signature);
    Task<decimal> GetPriceInNokAsync();
}
