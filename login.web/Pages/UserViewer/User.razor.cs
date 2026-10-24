using login.web.Models;
using login.web.services;
using Microsoft.AspNetCore.Components;

namespace login.web.Pages.UserViewer;

public partial class User
{
    //Dependencies
    [Inject] IDashboardClientService _dashService {get; set;} = default!;


    //State Variables
    [Parameter]
    public int userId {get; set;}
    private string statusMessage = "";
    private ProfileModel profileModel = new();


    //Methods
    protected override async Task OnParametersSetAsync()
    {
        var (userInfo, error) = await _dashService.GetUserAsync(userId);

        if(error != null)
        {
            statusMessage = error;
        }
        else if(userInfo != null)
        {
            profileModel.UserName = userInfo.UserName;
            profileModel.Role = userInfo.Role;
            profileModel.ProfilePicture = userInfo.ProfilePicture;
            profileModel.Games = userInfo.Games;
            profileModel.GamesCoverArt = userInfo.GamesCoverArt;
        }
    }
}