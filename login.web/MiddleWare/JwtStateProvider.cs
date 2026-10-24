using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace login.web.MiddleWare;

public class JwtStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;

    public JwtStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    //main method that blazor calls
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt_token");

        if (string.IsNullOrWhiteSpace(token))
        {
            //no token found == anonymous user
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        //token found == decode it and check exp date
        var claims = ParseClaimsFromJwt(token);

        //get the exp claim
        var expClaim = claims.FirstOrDefault(c => c.Type == "exp");

        if(expClaim != null && long.TryParse(expClaim.Value, out var expTime)) //check if it's a type long and is not null
        {
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); //get the current time in unix format

            if(currentTime >= expTime) //check if it has expired
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "jwt_token"); //delete jwt from browser's storage
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())); //return anonymous auth state
            }
        }

        //create logged-in user
        var identity = new ClaimsIdentity(claims, "jwt", "unique_name", "role");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }


    //method to trigger UI updates logged in
    public void NotifyUserLoggedIn(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt", "unique_name", "role");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    //method to trigger UI updates logged out
    public async Task NotifyUserLoggedOut()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "jwt_token");
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
    }

    //decoder ring that converts jwt token into claims identity
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwtToken)
    {
        var payload = jwtToken.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
    }


    //base64 fixer for jwt payload
    private byte[] ParseBase64WithoutPadding(string base64Payload)
    {
        switch (base64Payload.Length % 4)
        {
            case 2: base64Payload += "=="; 
                break;
                
            case 3: base64Payload += "=";
                break;
        }
        return Convert.FromBase64String(base64Payload);
    }
}