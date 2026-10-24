using login.web.dto;
using login.web.MiddleWare;
using login.web.Models;
using login.web.services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace login.web.Pages.Auth;

public partial class Register
{
    //Dependencies
    [Inject] private NavigationManager NavManager {get; set;} = default!;
    [Inject] private IAuthClientService _authService {get; set;} = default!;


    //State Variables
    private RegisterModel registerModel = new();
    private bool hasClicked = false;
    private string statusMessage = "";



    //Methods
    private void ChangeBool()
    {
        hasClicked = !hasClicked;
    }

    
    private async Task RegisterAsync()
    {
        var newUser = new CreateUserDto
        {
            UserName = registerModel.UserName,
            PassWord = registerModel.PassWord,
            ProfilePicture = registerModel.ProfilePicture
        };

        var (user, error, URI) = await _authService.AppRegisterAsync(newUser);

        if (error != null)
        {
            statusMessage = error;
        }
        else
        {
            NavManager.NavigateTo("/login", replace: true);
        }
    }

}