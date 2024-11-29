using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Contracts.Requests.OrderRequest;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;

namespace RestaurantManagement.Controllers;
[ApiController]
public class OrderController: ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IMapper _mapper;
    private readonly IOrderManager _orderManager;
    

    public OrderController(ILogger<OrderController> logger, IMapper mapper, IOrderManager orderManager)
    {
        _logger = logger;
        _mapper = mapper;
        _orderManager = orderManager;
    }

    [HttpPost(ApiEndpoints.Order.CreateOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
    {
        if (request is { TypeOfOrder: 0 })
        {
            return BadRequest("The type of order cannot be null");
        }

        try
        {
            var order = _mapper.Map<Order>(request);
            await _orderManager.CreateOrder(order);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet(ApiEndpoints.Order.GetOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> GetOrder([FromRoute] Guid id)
    {
        try
        {
            var response = await _orderManager.GetOrderById(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet(ApiEndpoints.Order.GetAllOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> GetAllOrder()
    {
        try
        {
            var orders = await _orderManager.GetOrders();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete(ApiEndpoints.Order.DeleteOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> DeleteOrder([FromRoute] Guid id)
    {
        try
        {
            await _orderManager.DeleteOrder(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut(ApiEndpoints.Order.UpdateOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> UpdateOrder([FromRoute] Guid id, [FromBody] UpdateOrderRequest request)
    {
        if (request is { StatusType: 0 })
        {
            return BadRequest("The type of order cannot be null");
        }

        var isExist = await _orderManager.IsExist(id);

        try
        {
            request.Id = isExist.Id;
            var order = _mapper.Map<Order>(request);
            await _orderManager.UpdateOrder(order);
            return Ok();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ex.Message);
        }
    }
}