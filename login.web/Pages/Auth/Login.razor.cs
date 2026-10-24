namespace login.web.Pages.Auth;
using login.web.services;
using login.web.dto;
using login.web.MiddleWare;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using login.web.Models;

public partial class Login
{
    //Dependency injections
    [Inject] private IAuthClientService _authService {get; set;} = default!;
    [Inject] private AuthenticationStateProvider _authState {get; set;} = default!;
    [Inject] private NavigationManager NavManager { get; set; } = default!;



    //State variables
    private LoginModel loginModel = new();
    private string status = "";

    [CascadingParameter]
    private Task<AuthenticationState>? AuthStateTask {get; set;}



    //Methods
    protected override async Task OnParametersSetAsync()
    {
        if(AuthStateTask != null)
        {
            var authState = await AuthStateTask;
            
            if(authState.User.Identity?.IsAuthenticated == true)
            {
                NavManager.NavigateTo("/dashboard", replace: true);
            }
        }
    }

    private async Task LoginAsync()
    {
        var user = new LoginUserDto
        {
            UserName = loginModel.UserName,
            PassWord = loginModel.PassWord
        };

        var (token, error) = await _authService.AppLoginAsync(user);

        if(error != null)
        {
            status = error;
        }
        else if (token != null)
        {
            ((JwtStateProvider)_authState).NotifyUserLoggedIn(token);
        }
    }
}