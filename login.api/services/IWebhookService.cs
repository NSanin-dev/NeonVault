using Microsoft.IdentityModel.Tokens;
using Stripe;

namespace login.api.services;

public interface IWebhookService
{
    Task<string?> ProcessWebhookAsync(string json, string? signatureHeader);
}


public class ImplementedWebookService : IWebhookService
{
    private readonly string _endpointSecret;
    private readonly IDashboardService _service;

    public ImplementedWebookService(IConfiguration config, IDashboardService service)
    {
        _endpointSecret = config["Stripe:WebhookSecret"] ?? string.Empty;
        _service = service;
    }


    public async Task<string?> ProcessWebhookAsync(string json, string? signatureHeader)
    {
        //Check if there's signature if not reject
        if (string.IsNullOrEmpty(signatureHeader))
        {
            return "No signatue could be verified";
        }

        try
        {
            // 1. VERIFICATION: The Stripe SDK cryptographically ensures this request is authentic
            var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _endpointSecret);

            // 2. ROUTING: Check if this specific event means they successfully paid
            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                // 3. EXTRACTION: Safely pull your metadata dictionary
                if(session?.Metadata != null)
                {
                    if(int.TryParse(session.Metadata["UserId"], out int userId) && 
                    session.Metadata.TryGetValue("GameTitle", out string? gameTitle))
                    {
                        // 4. EXECUTION: Call your existing database function!
                        string? error = await _service.AddGameToAccountAsync(userId, gameTitle);

                        if(error != null)
                        {
                            return error;
                        }
                    }
                }
            }

            return null;
        }
        catch(StripeException ex)
        {
            return $"Stripe Error: {ex.StripeError?.Message ?? ex.Message}";
        }
        catch(Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}