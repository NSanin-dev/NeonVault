namespace login.api.services;

using System.Net.Http.Headers;
using login.api.dto;
using Microsoft.Extensions.Caching.Memory;
using Stripe;

public interface IigdbService
{
    Task<(string? coverURL, string? error)> GetGameCoverUrlAsync(string gameTitle);
}

public class ImplementedIgdbService : IigdbService
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly string _clientId;
    private readonly string _secretKey;

    public ImplementedIgdbService(HttpClient http, IConfiguration config, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
        _clientId = config["IGDB:ClientId"] ?? string.Empty;
        _secretKey = config["IGDB:SecretKey"] ?? string.Empty;
    }


    public async Task<(string? coverURL, string? error)> GetGameCoverUrlAsync(string gameTitle)
    {
        try
        {
            var (twitchToken, error) = await GetTwitchTokenAsync(); //Look up for the cache or send a new request

            if(error != null)
            {
                return (null, error);
            }

            // STEP 2: Ask IGDB for the game cover
            // IGDB uses a custom text syntax called "Apicalypse" inside the request body
            // Escape quotes so "Assassin's Creed" becomes "Assassin\'s Creed"
            var safeTitle = gameTitle.Replace("\\", "\\\\").Replace("\"", "\\\"");

            var query = $"search \"{safeTitle}\"; fields name, cover.url; limit 1;";
            
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.igdb.com/v4/games");
            request.Headers.Add("Client-ID", _clientId);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", twitchToken);
            request.Content = new StringContent(query);

            var igdbRes = await _http.SendAsync(request); //Send request as a "send" since you already passed the options another way

            if (igdbRes.IsSuccessStatusCode)
            {
                var games = await igdbRes.Content.ReadFromJsonAsync<List<IgdbGameResponse>>();
                var coverUrl = games?.FirstOrDefault()?.cover?.url;

                if (string.IsNullOrEmpty(coverUrl))
                {
                    return (null, "IGDB returned an empty cover URL");
                }

                // STEP 3: Format the URL
                // IGDB returns URLs like: "//images.igdb.com/igdb/image/upload/t_thumb/co1r8o.jpg"
                // We need to add "https:" and change "t_thumb" to "t_cover_big" for a high-res image
                coverUrl = "https:" + coverUrl.Replace("t_thumb", "t_cover_big"); 

                return (coverUrl, null);
            }
            else
            {
                return (null, $"IGDB Post request responded with: {igdbRes.StatusCode}");
            }

        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }
    
    }


    private async Task<(string? token, string? error)> GetTwitchTokenAsync()
    {
        try
        {
            // STEP A: Look in the RAM bucket first
            if (_cache.TryGetValue("TwitchToken", out string? cachedToken))
            {
                return (cachedToken, null); // Cache hit! Return instantly.
            }

            // STEP B: If empty, execute the HTTP request to Twitch
            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _secretKey),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var tokenRes = await _http.PostAsync("https://id.twitch.tv/oauth2/token", tokenRequest);

            if (!tokenRes.IsSuccessStatusCode)
            {
                return (null, $"Twitch Post request responded with {tokenRes.StatusCode}");
            }


            TwitchTokenResponse? tokenData = await tokenRes.Content.ReadFromJsonAsync<TwitchTokenResponse>();

            if (string.IsNullOrEmpty(tokenData?.access_token))
            {
                return (null, "Twitch returned empty token");
            }

            // Validate the expiration time. If it is greater than 5 minutes (300 seconds), 
            // safely subtract the buffer. Otherwise, default to caching the token for 1 hour.
            var safeExpirationSeconds = tokenData.expires_in > 300 ? tokenData.expires_in - 300 : 3600;

            // STEP C: Create a self-destruct timer, set an absolute expiration taking into account the tokendata expiry property
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(safeExpirationSeconds));


            // STEP D: Save the new token to the RAM bucket with the timer
            _cache.Set("TwitchToken", tokenData.access_token, cacheOptions);

            return (tokenData.access_token, null);
        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }

    }
}