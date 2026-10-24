using login.web.Models;
using login.web.Pages.Dashboard;
using login.web.services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace login.web.Pages.ProfileOptions;

public partial class Profile
{
    //Injections
    [Inject] private IDashboardClientService _dashService {get; set;} = default!;




    //State Variables
    [CascadingParameter]
    private Task<AuthenticationState>? AuthStateTask {get; set;}
    private int userId = 0;
    private ProfileModel profileModel = new();
    private string statusMessage = "";



    //Methods
    protected override async Task OnInitializedAsync()
    {
        if (AuthStateTask != null)
        {
            var authState = await AuthStateTask;

            if (authState.User.Identity?.IsAuthenticated == true)
            {
                if (int.TryParse(authState.User.FindFirst("nameid")?.Value, out int value))
                {
                    userId = value;

                    var (user, error) = await _dashService.GetUserAsync(userId);

                    if (error != null)
                    {
                        statusMessage = error;
                    }
                    else if (user != null)
                    {
                        profileModel.ProfilePicture = user.ProfilePicture;
                        profileModel.UserName = user.UserName;
                        profileModel.Role = user.Role;
                        profileModel.Games = user.Games;
                        profileModel.GamesCoverArt = user.GamesCoverArt;
                    }
                }
                else
                {
                    statusMessage = "Failed to fetch user ID";
                }
            }
        }
    }
}