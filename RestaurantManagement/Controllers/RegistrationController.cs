using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Data;
using RestaurantManagement.Repository;
using LoginRequest = RestaurantManagement.Contracts.Requests.LoginRequest;
using RegisterRequest = RestaurantManagement.Contracts.Requests.RegisterRequest;

namespace RestaurantManagement.Controllers;

[ApiController]
public class RegistrationController: ControllerBase
{
    private readonly ILogger<RegistrationController> _logger;
    private readonly IAuthManager _authManager;
    private readonly IMapper _mapper;

    public RegistrationController(ILogger<RegistrationController> logger, 
        IMapper mapper, 
        IAuthManager authManager)
    {
        _logger = logger;
        _mapper = mapper;
        _authManager = authManager;
    }


    [HttpPost(ApiEndpoints.Registration.Signup)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var errors = await _authManager.Register(request);
        if (errors.Any())
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
            
        }

        return Ok();
    }

    [HttpPost(ApiEndpoints.Registration.Login)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var authResponse = await _authManager.Login(request);
        if (authResponse is null)
        {
            return Unauthorized();
        }
        return Ok(authResponse);
    }

    [HttpPost(ApiEndpoints.Registration.RefreshToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody]AuthCustomerResponse request)
    {
        var authResponse = await _authManager.VerifyRefreshToken(request);
        if (authResponse is null)
        {
            return Unauthorized();
        }
        return Ok(authResponse);
    }
}