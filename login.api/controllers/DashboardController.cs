using System.Security.Claims;
using login.api.data;
using login.api.dto;
using login.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace login.api.controllers;

[ApiController]
[Route("api")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    public DashboardController(IDashboardService service)
    {
        _service = service;
    }


    [Authorize]
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var (users, error) = await _service.GetAllUsersAsync();

        if(error != null)
        {
            return NotFound(new ErrorReturnerDto{Message = error});
        }

        return Ok(new SuccesfulUserListDto{UserList = users!});
    }


    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserAsync(int userId)
    {
        var (user, error) = await _service.GetUserAsync(userId);

        if(error != null)
        {
            return NotFound(new ErrorReturnerDto{Message = error});
        }

        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("games")]
    public async Task<IActionResult> PostGameAsync(CreateGameDto request)
    {
        var (game, error) = await _service.PostGameAsync(request);

        if(error != null)
        {
            return BadRequest(new ErrorReturnerDto{Message = error});
        }

        return Created($"api/game/{game!.Id}", game);
    }



    [Authorize]
    [HttpGet("games")]
    public async Task<IActionResult> GetAllGamesAsync()
    {
        var (games, error) = await _service.GetAllGamesAsync();

        if(error != null)
        {
            return NotFound(new ErrorReturnerDto{Message = error});
        }

        return Ok(new SuccesfulGameListDto{GameList = games!});
    }



    [Authorize]
    [HttpGet("game/{gameId}")]
    public async Task<IActionResult> GetGameAsync(int gameId)
    {
        var (game, error) = await _service.GetGameAsync(gameId);

        if(error != null)
        {
            return NotFound(new ErrorReturnerDto{Message = error});
        }

        return Ok(game);
    }


}