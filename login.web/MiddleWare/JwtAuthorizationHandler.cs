using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace login.web.MiddleWare;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;

    public JwtAuthorizationHandler(IJSRuntime run)
    {
        _jsRuntime = run;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //Grab token from local storage
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", cancellationToken, "jwt_token");

        //if a token exists attach it to request headers
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        //pass the request to the network engine
        return await base.SendAsync(request,cancellationToken);
    }
}