using System.Net.Http.Json;
using login.web.dto;
using Microsoft.JSInterop;

namespace login.web.services;

public interface IAuthClientService
{
    Task<(ReadUserDto? user, string? errorMessage, string? URI)> AppRegisterAsync(CreateUserDto request);
    Task<(string? token, string? error)> AppLoginAsync(LoginUserDto request);
}


public class AuthClientService : IAuthClientService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _jsRunTime;
    
    public AuthClientService(HttpClient client, IJSRuntime run)
    {
        _http = client;
        _jsRunTime = run;
    }

    private readonly string genericError = "Operation returned success but no valid object was returned";


    public async Task<(ReadUserDto? user, string? errorMessage, string? URI)> AppRegisterAsync(CreateUserDto request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("auth/register", request);

            if (response.IsSuccessStatusCode)
            {
                ReadUserDto? user = await response.Content.ReadFromJsonAsync<ReadUserDto>();
                string? newURI = response.Headers.Location?.ToString();

                if (user == null)
                {
                    return (null, genericError, null);
                }

                return (user, null, newURI);
            }
            else
            {
                ErrorReturnerDto? errorObject = await response.Content.ReadFromJsonAsync<ErrorReturnerDto>();

                return (null, errorObject?.Message ?? $"Unexpected Server Error: {response.StatusCode}", null);
            }

        }
        catch (Exception ex)
        {
            return (null, ex.Message, null);
        }
    }


    public async Task<(string? token, string? error)> AppLoginAsync(LoginUserDto request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                SuccesfulStringDto? tokenObject = await response.Content.ReadFromJsonAsync<SuccesfulStringDto>();

                if (tokenObject == null)
                {
                    return (null, genericError);
                }

                //Tell the browser to add token to local storage by asking JS to do so
                if (!string.IsNullOrEmpty(tokenObject.Message))
                {
                    await _jsRunTime.InvokeVoidAsync("localStorage.setItem", "jwt_token", tokenObject.Message);
                }

                return (tokenObject.Message, null);
            }
            else
            {
                ErrorReturnerDto? errorObject = await response.Content.ReadFromJsonAsync<ErrorReturnerDto>();

                return (null, errorObject?.Message ?? $"Unexpected Server Error: {response.StatusCode}");
            }

        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }
    }
}