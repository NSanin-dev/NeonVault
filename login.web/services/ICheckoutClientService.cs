using System.Net.Http.Json;
using login.web.dto;

namespace login.web.services;



public interface ICheckoutClientService
{
    Task<(string? URI, string? error)> CreateCheckoutAsync(CreateCheckoutDto metadata);
}


public class ImplementedCheckoutClientService : ICheckoutClientService
{
    private readonly HttpClient _http;
    private readonly string genericError = "Operation returned success but no valid object was returned";

    public ImplementedCheckoutClientService(HttpClient client)
    {
        _http = client;
    }


    public async Task<(string? URI, string? error)> CreateCheckoutAsync(CreateCheckoutDto metadata)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("checkout/create-checkout", metadata);

            if (response.IsSuccessStatusCode)
            {
                SuccesfulStringDto? succesfulStringObject = await response.Content.ReadFromJsonAsync<SuccesfulStringDto>();

                if(succesfulStringObject == null)
                {
                    return (null, genericError);
                }

                return (succesfulStringObject.Message, null);
            }
            else
            {
                ErrorReturnerDto? errorObject = await response.Content.ReadFromJsonAsync<ErrorReturnerDto>();

                return(null, errorObject?.Message ?? $"Unexpected Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }
    }
}