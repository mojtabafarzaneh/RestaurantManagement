using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;

namespace RestaurantManagement.Controllers;

[ApiController]
public class MenuController:ControllerBase
{
    private readonly ILogger<MenuController> _logger;
    private readonly IMenuManager _menuManager;
    private readonly IMapper _mapper;

    public MenuController(ILogger<MenuController> logger,
        IMapper mapper,IMenuManager menuManager)
    {
        _logger = logger;
        _mapper = mapper;
        _menuManager = menuManager;
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
        try
        {
            var query = await _menuManager.GetAllMenus();
            if (query.Count == 0)
            {
                return NotFound("No menus found");
            }

            var response = _mapper.Map<List<MenuResponse>>(query);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
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
        try
        {
            var menu = _mapper.Map<Menu>(request);
            await _menuManager.CreateMenu(menu);
            if (menu is null)
            {
                return BadRequest();
            }
            var response = _mapper.Map<MenuResponse>(menu);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message); 
        }
        
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
        try
        {
            var query = await _menuManager.GetMenuById(id);
            var response = _mapper.Map<MenuResponse>(query);
            return Ok(response);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return NotFound(ex.Message);
        }

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
        
        var menu = await _menuManager.IsExist(id);
        if (!menu)
        {
            return NotFound("Menu not found");
        }
        try
        {
            await _menuManager.DeleteMenuById(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
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
        try
        {
            var menu = await _menuManager.GetMenuById(id);
            if (id != menu.Id)
            {
                return NotFound(id);
            }
            _mapper.Map(request, menu);
            await _menuManager.UpdateMenuById(menu);
            
            return NoContent();
                
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message); 
        }
        
    }
}