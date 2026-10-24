using System.Net.Http.Json;
using System.Text.Json.Nodes;
using login.web.dto;

namespace login.web.services;

public interface IDashboardClientService
{
    Task<(List<ReadUserDto>? users, string? error)> GetAllUsersAsync();
    Task<(ReadUserDto? user, string? error)> GetUserAsync(int userId);
    Task<(ReadGameDto? game, string? error)> PostGameAsync(CreateGameDto request);
    Task<(List<ReadGameDto>? games, string? error)> GetAllGamesAsync();
    Task<(ReadGameDto? game, string? error)> GetGameAsync(int gameId);
}

public class DashboardClientService : IDashboardClientService
{
    private readonly HttpClient _http;

    public DashboardClientService(HttpClient client)
    {
        _http = client;
    }

    private readonly string genericError = "Operation returned success but no valid object was returned";

    public async Task<(List<ReadUserDto>? users, string? error)> GetAllUsersAsync()
    {
        try
        {
            var response = await _http.GetAsync("users");

            if (response.IsSuccessStatusCode)
            {
                SuccesfulUserListDto? userListObject = await response.Content.ReadFromJsonAsync<SuccesfulUserListDto>();

                if (userListObject == null)
                {
                    return (null, genericError);
                }

                return (userListObject.UserList, null);
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


    public async Task<(ReadUserDto? user, string? error)> GetUserAsync(int userId)
    {
        try
        {
            var response = await _http.GetAsync($"user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                ReadUserDto? user = await response.Content.ReadFromJsonAsync<ReadUserDto>();

                if (user == null)
                {
                    return (null, genericError);
                }

                return (user, null);
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



    public async Task<(ReadGameDto? game, string? error)> PostGameAsync(CreateGameDto request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("games", request);

            if (response.IsSuccessStatusCode)
            {
                ReadGameDto? game = await response.Content.ReadFromJsonAsync<ReadGameDto>();

                if (game == null)
                {
                    return (null, genericError);
                }

                return (game, null);
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





    public async Task<(List<ReadGameDto>? games, string? error)> GetAllGamesAsync()
    {
        try
        {
            var response = await _http.GetAsync("games");

            if (response.IsSuccessStatusCode)
            {
                SuccesfulGameListDto? gameListObject = await response.Content.ReadFromJsonAsync<SuccesfulGameListDto>();

                if (gameListObject == null)
                {
                    return (null, genericError);
                }

                return (gameListObject.GameList, null);
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




    public async Task<(ReadGameDto? game, string? error)> GetGameAsync(int gameId)
    {
        try
        {
            var response = await _http.GetAsync($"game/{gameId}");

            if (response.IsSuccessStatusCode)
            {
                ReadGameDto? game = await response.Content.ReadFromJsonAsync<ReadGameDto>();

                if (game == null)
                {
                    return (null, genericError);
                }

                return (game, null);
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