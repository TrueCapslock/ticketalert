using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using TicketAlert.Application.Common.Interfaces;

namespace TicketAlert.Infrastructure.Services.Payment;

public class StripePaymentService : IPaymentService
{
    private readonly string _webhookSecret;
    private readonly decimal _priceNok;

    public StripePaymentService(IConfiguration config)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        _webhookSecret = config["Stripe:WebhookSecret"]!;
        _priceNok = decimal.TryParse(config["Pricing:PerWatchNok"], out var p) ? p : 19;
    }

    public async Task<string> CreateCheckoutSessionAsync(
        Guid userId, Guid watchId, decimal amount, string currency,
        string successUrl, string cancelUrl)
    {
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            CustomerEmail = null,
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(amount * 100),
                        Currency = currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "TicketAlert - Konsertovervåkning",
                            Description = "Overvåkning av utsolgte billetter. Varer frem til konsertdato."
                        }
                    },
                    Quantity = 1
                }
            },
            Metadata = new Dictionary<string, string>
            {
                { "userId", userId.ToString() },
                { "watchId", watchId.ToString() }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return session.Url;
    }

    public async Task<bool> ProcessWebhookAsync(string json, string signature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, signature, _webhookSecret);

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;
                // Handle payment completion — update DB
                return await Task.FromResult(session?.PaymentStatus == "paid");
            }

            return await Task.FromResult(true);
        }
        catch
        {
            return await Task.FromResult(false);
        }
    }

    public Task<decimal> GetPriceInNokAsync() => Task.FromResult(_priceNok);
}
