using login.web.dto;
using login.web.Models;
using login.web.services;
using Microsoft.AspNetCore.Components;

namespace login.web.Pages.Dashboard;

public partial class Games
{
    //Dependency Injections
    [Inject] IDashboardClientService _dashService {get; set;} = default!;
    [Inject] public NavigationManager NavManager {get; set;} = default!;


    //State Variables
    private string statusMessage = "";
    private string adminStatusMessage = "";
    private List<ReadGameDto> currentGames = new List<ReadGameDto>();
    private bool isVisible = false;
    private GameModelAdmin gameModelAdmin = new();


    //Methods
    protected override async Task OnInitializedAsync()
    {
        var (gameList, error) = await _dashService.GetAllGamesAsync();

        if(error != null)
        {
            statusMessage = error;
        }
        else if (gameList != null)
        {
            currentGames = gameList;
        }
    }

    private void GoToGame(int gameId)
    {
        NavManager.NavigateTo($"/dashboard/game/{gameId}");
    }

    private void ToggleAdminButton()
    {
        isVisible = !isVisible;
    }

    private async Task CreateGameAsync()
    {
        var newGame = new CreateGameDto
        {
            Title = gameModelAdmin.GameTitle
        };

        var (game, error) = await _dashService.PostGameAsync(newGame);

        if(error != null)
        {
            adminStatusMessage = error;
        }
        else if (game != null)
        {
            currentGames.Add(game); //Add game to list to re-paint UI since it only does OnInitialize by default
            adminStatusMessage = "Game Posted Succesfully";
        }
    }
}