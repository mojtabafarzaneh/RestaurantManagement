using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Contracts.Requests.OrderRequest;
using RestaurantManagement.Contracts.Responses.OrderResponse;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;
using RestaurantManagement.Services;

namespace RestaurantManagement.Controllers;
[ApiController]
public class OrderController: ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IMapper _mapper;
    private readonly IOrderService _orderService;
    

    public OrderController(ILogger<OrderController> logger, IMapper mapper, IOrderService orderService)
    {
        _logger = logger;
        _mapper = mapper;
        _orderService = orderService;
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

        var order = _mapper.Map<Order>(request);
        await _orderService.CreateOrder(order);
        return Ok();
    }

    [HttpGet(ApiEndpoints.Order.GetOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> GetOrder()
    {
        var response = await _orderService.GetOrderById(); 
        return Ok(response);
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

        var orders = await _orderService.GetOrders();
        return Ok(orders);

    }

    [HttpDelete(ApiEndpoints.Order.DeleteOrder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> DeleteOrder()
    {
        await _orderService.DeleteOrder();
        return NoContent();

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

        var isExist = await _orderService.IsExist(id);
        request.Id = isExist.Id;
        var order = _mapper.Map<Order>(request);
        await _orderService.UpdateOrder(order);
        return Ok();
            
    }

    [HttpGet(ApiEndpoints.Order.GetTicket)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> GetTicket()
    {

        var ticket = await _orderService.GetTicketById();
        var response = _mapper.Map<TicketResponse>(ticket);
        return Ok(response);
    }
}