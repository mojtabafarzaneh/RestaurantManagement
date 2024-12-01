using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;
using RestaurantManagement.Services;

namespace RestaurantManagement.Controllers;

[ApiController]
public class MenuController:ControllerBase
{
    private readonly ILogger<MenuController> _logger;
    private readonly IMenuService _menuService;
    private readonly IMapper _mapper;

    public MenuController(ILogger<MenuController> logger,
        IMapper mapper,IMenuService menuService)
    {
        _logger = logger;
        _mapper = mapper;
        _menuService = menuService;
    }

    [HttpGet(ApiEndpoints.Menu.Menus)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult> GetAllMenus()
    {
        var query = await _menuService.GetAllMenus();
        var response = _mapper.Map<List<MenuResponse>>(query);
        return Ok(response);
    }

    [HttpPost(ApiEndpoints.Menu.Menus)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult> CreateMenu(MenuRequest request)
    {
        var menu = _mapper.Map<Menu>(request);
        var result = await _menuService.CreateMenu(menu);
        var mapped = _mapper.Map<MenuResponse>(result);
        return Ok(mapped);
    }

    [HttpGet(ApiEndpoints.Menu.ThisMenu)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult> GetThisMenu([FromRoute]Guid id)
    {
        var query = await _menuService.GetMenuById(id); 
        var response = _mapper.Map<MenuResponse>(query); 
        return Ok(response);

    }

    [HttpDelete(ApiEndpoints.Menu.DeleteMenu)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult> DeleteThisMenu([FromRoute] Guid id)
    {
        await _menuService.DeleteMenuById(id);
        return Ok();
    }

    [HttpPut(ApiEndpoints.Menu.UpdateMenu)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult> UpdateMenu([FromRoute] Guid id, [FromBody] MenuUpdate request)
    {
        
        var menu = _mapper.Map<Menu>(request);
        await _menuService.UpdateMenuById(menu);
        return NoContent();
        
    }
}