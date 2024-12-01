using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Data;
using RestaurantManagement.Middleware;
using RestaurantManagement.Repository;
using RestaurantManagement.Services;
using LoginRequest = RestaurantManagement.Contracts.Requests.LoginRequest;
using RegisterRequest = RestaurantManagement.Contracts.Requests.RegisterRequest;

namespace RestaurantManagement.Controllers;

[ApiController]
public class RegistrationController: ControllerBase
{
    private readonly ILogger<RegistrationController> _logger;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public RegistrationController(ILogger<RegistrationController> logger, 
        IMapper mapper, 
        IAuthService authService)
    {
        _logger = logger;
        _mapper = mapper;
        _authService = authService;
    }


    [HttpPost(ApiEndpoints.Registration.Signup)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        await _authService.Register(request);
        return Ok();
    }

    [HttpPost(ApiEndpoints.Registration.Login)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var authResponse = await _authService.Login(request);
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
        var authResponse = await _authService.VerifyRefreshToken(request);
        return Ok(authResponse);
    }

    [HttpPost(ApiEndpoints.Registration.AdminRegister)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> AdminRegister([FromBody] ChangeUserRoleRequest request)
    {
        await _authService.ChangeCustomerRole(request);
        return Ok();
    }

    [HttpGet(ApiEndpoints.Registration.Profile)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var response = await _authService.Me();
        
        return Ok(response);
    }
    
}