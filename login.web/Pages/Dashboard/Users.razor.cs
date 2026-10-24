using login.web.dto;
using login.web.services;
using Microsoft.AspNetCore.Components;

namespace login.web.Pages.Dashboard;


public partial class Users
{
    //Dependency
    [Inject] public IDashboardClientService _dashService {get; set;} = default!;
    [Inject] public NavigationManager NavManager {get; set;} = default!;


    //State Variable
    private List<ReadUserDto> currentUsers = new List<ReadUserDto>();
    private string statusMessage = "";



    //Methods
    protected override async Task OnInitializedAsync()
    {
        var (userList, error) = await _dashService.GetAllUsersAsync();

        if(error != null)
        {
            statusMessage = error;
        }
        else if (userList != null)
        {
            currentUsers = userList;
        }
    }


    private void GoToUser(int userId)
    {
        NavManager.NavigateTo($"/dashboard/user/{userId}");
    }
}