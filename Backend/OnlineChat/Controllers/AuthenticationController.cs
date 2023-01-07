﻿using BusinessLogic.Commands.Auth.Login;
using BusinessLogic.Commands.Auth.Logout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineChat.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
    
    [Authorize]
    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand());
        return Ok();
    }
}