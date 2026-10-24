using login.api.dto;
using Stripe;
using Stripe.Checkout;
using Stripe.Forwarding;

namespace login.api.services;


public interface ICheckoutService
{
    Task<(string? URI, string? error)> CreateCheckoutAsync(CreateCheckoutDto metadata);
}


public class ImplementedCheckoutService : ICheckoutService
{
    public async Task<(string? URI, string? error)> CreateCheckoutAsync(CreateCheckoutDto metadata)
    {
        try
        {
            // 1. Define the checkout session rules
            var options = new SessionCreateOptions
            {
                // 2. Define the product and price inline
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = 5999, // $59.99
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = metadata.GameTitle //Title user sees at checkout
                        }
                    },

                    Quantity = 1,
                }
            },

                Mode = "payment", //"payment" is for one time purchases and "subscription" for recurring

                // 3. Where Stripe sends the browser after completion
                // Note: In production, you will swap localhost for your real domain
                SuccessUrl = "http://localhost:5087/dashboard/success",
                CancelUrl = "http://localhost:5087/dashboard/cancel",

                // 4. THE METADATA: This is invisible to the user but critical for your Webhook.
                // Both the Key and the Value MUST be strings.
                Metadata = new Dictionary<string, string>
            {
                {"UserId", metadata.UserId.ToString()},
                {"GameTitle", metadata.GameTitle}
            }
            };

            // 5. Connect to Stripe's servers to mint the session
            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            // 6. Return the temporary URL
            if (string.IsNullOrWhiteSpace(session?.Url))
            {
                return (null, "Stripe did not return a valid checkout URL.");
            }
            
            return (session.Url, null);
        }
        catch (StripeException ex)
        {
            return (null, $"Stripe Error: {ex.StripeError?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
            return (null, $"Unexpected Error: {ex.Message}");
        }
    }
}