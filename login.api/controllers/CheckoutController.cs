using login.api.data;
using login.api.dto;
using login.api.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace login.api.controllers;

[ApiController]
[Route("api/checkout")]

public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _service;
    private readonly AppDbContext _db;

    public CheckoutController(ICheckoutService service, AppDbContext db)
    {
        _service = service;
        _db = db;
    }

    [Authorize]
    [HttpPost("create-checkout")]
    public async Task<IActionResult> PostCheckoutGameAsync(CreateCheckoutDto metadata)
    {
        var existingUser = await _db.AppUsers.Include(u => u.Games).FirstOrDefaultAsync(u => u.Id == metadata.UserId);

        if(existingUser == null)
        {
            return BadRequest(new ErrorReturnerDto{Message = "A user with that ID was not found"});
        }
        
        if(existingUser.Games.Any(g => g.Title == metadata.GameTitle))
        {
            return BadRequest(new ErrorReturnerDto{Message = "User already owns the game"});
        }

        
        var (URI, error) = await _service.CreateCheckoutAsync(metadata);

        if(error != null)
        {
            return BadRequest(new ErrorReturnerDto{Message = error});
        }
        
        return Ok(new SuccesfulStringDto{Message = URI!});
    }
}