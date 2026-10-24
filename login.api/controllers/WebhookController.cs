using login.api.dto;
using login.api.services;
using Microsoft.AspNetCore.Mvc;

namespace login.api.controllers;

[ApiController]
[Route("api/webhook")]

public class WebhookController : ControllerBase
{
    private readonly IWebhookService _service;

    public WebhookController(IWebhookService service)
    {
        _service = service;
    }


    [HttpPost("stripe")]
    public async Task<IActionResult> StripeIndexAsync()
    {
        // 1. Read the raw JSON string from the HTTP request body
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        // 2. Extract the signature Stripe places in the HTTP headers
        var signatureHeader = Request.Headers["Stripe-Signature"];

        // 3. Pass the strings to your service for processing
        string? error = await _service.ProcessWebhookAsync(json, signatureHeader);

        if(error != null)
        {
            return BadRequest(new ErrorReturnerDto{Message = error});
        }

        return Ok();
    }
}