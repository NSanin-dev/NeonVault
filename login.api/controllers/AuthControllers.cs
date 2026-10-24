using login.api.dto;
using login.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace login.api.controllers;


[ApiController]
[Route("api/auth")]

public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }


    [HttpPost("register")]
    public async Task<IActionResult> AppRegisterAsync(CreateUserDto request)
    {
        var (newUser, error) = await _service.AppRegisterAsync(request);

        if(error != null)
        {
            return BadRequest(new ErrorReturnerDto{Message = error});
        }

        return Created($"api/user/{newUser!.Id}", newUser);
    }


    [HttpPost("login")]
    public async Task<IActionResult> AppLoginAsync(LoginUserDto request)
    {
        var (token, error) = await _service.AppLoginAsync(request);

        if(error != null)
        {
            return Unauthorized(new ErrorReturnerDto{Message = error});
        }

        return Ok(new SuccesfulStringDto{Message = token!});
    }
}