using login.web.dto;
using login.web.Models;
using login.web.Pages.UserViewer;
using login.web.services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace login.web.Pages.GameViewer;


public partial class Game
{
    //Dependencies
    [Inject] private IDashboardClientService _dashService {get; set;} = default!;
    [Inject] private ICheckoutClientService _checkService {get; set;} = default!;
    [Inject] private NavigationManager NavManager {get; set;} = default!;


    //State Variables
    [Parameter]
    public int gameId {get; set;}
    private string statusMessage = "";
    private int userId = 0;
    private string userName = "";
    private GameModel gameModel = new();
    private bool UserOwnsGame => !string.IsNullOrWhiteSpace(userName)
        && gameModel.UserNames?.Any(name => string.Equals(name, userName, StringComparison.OrdinalIgnoreCase)) == true;
        
    [CascadingParameter]
    private Task<AuthenticationState>? AuthStateTask {get; set;}



    //Methods
    protected override async Task OnParametersSetAsync()
    {
        var (gameInfo, error) = await _dashService.GetGameAsync(gameId);

        if(error != null)
        {
            statusMessage = error;
        }
        else if (gameInfo != null)
        {
            gameModel.Title = gameInfo.Title;
            gameModel.UserNames = gameInfo.UserNames;
            gameModel.CoverArt = gameInfo.CoverArt;
        }

        if(AuthStateTask != null)
        {
            var authState = await AuthStateTask;

            if (authState.User.Identity?.IsAuthenticated == true)
            {
                if(int.TryParse(authState.User.FindFirst("nameid")?.Value, out int result))
                {
                    userId = result;
                }

                if(authState.User.Identity?.Name != null)
                {
                    userName = authState.User.Identity.Name;
                }
            }
        }
    }


    private async Task CreateCheckoutAsync()
    {
        if (!string.IsNullOrEmpty(gameModel.Title) && userId != 0)
        {
            var metadata = new CreateCheckoutDto
            {
                UserId = userId,
                GameTitle = gameModel.Title
            };

            var (URI, error) = await _checkService.CreateCheckoutAsync(metadata);

            if(error != null)
            {
                statusMessage = error;
            }
            else if(URI != null)
            {
                NavManager.NavigateTo(URI);
            }
        }
    }
}