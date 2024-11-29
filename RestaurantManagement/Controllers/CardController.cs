using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Contracts.Requests.CardRequest;
using RestaurantManagement.Contracts.Responses.CardResponse;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;

namespace RestaurantManagement.Controllers;

[ApiController]
public class CardController : ControllerBase
{

    private readonly ILogger<CardController> _logger;
    private readonly IMapper _mapper;
    private readonly ICardManager _cardManager;

    public CardController(ILogger<CardController> logger, IMapper mapper, ICardManager cardManager)
    {
        _logger = logger;
        _mapper = mapper;
        _cardManager = cardManager;
    }

    [HttpPost(ApiEndpoints.Card.CreateCard)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> CreateCard([FromBody] CardItemRequest request)
    {
        try
        {
            var card = _mapper.Map<CardItem>(request);
            await _cardManager.CreateCardAsync(card);
            return Ok();
        }
        catch (NullReferenceException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet(ApiEndpoints.Card.AllCards)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> GetAllCards()
    {
        try
        {
            var cards = await _cardManager.GetAllCardsAsync();
            var response = _mapper.Map<List<CardResponse>>(cards);
            return Ok(response);
        }
        catch (NullReferenceException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpGet(ApiEndpoints.Card.MyCard)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> GetMyCard()
    {
        try
        {
            var card = await _cardManager.GetCardByIdAsync();
            var response = _mapper.Map<CardResponse>(card);
            return Ok(response);
        }
        catch (NullReferenceException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }

    }

    [HttpDelete(ApiEndpoints.Card.DeleteCard)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> DeleteCard()
    {
        try
        {
            await _cardManager.DeleteCardAsync();
            return NoContent();
        }
        catch (NullReferenceException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost(ApiEndpoints.Card.CardItems)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> AddCardItem([FromBody] CardItemRequest request)
    {
        try
        {
            var cardItem = _mapper.Map<CardItem>(request);
            await _cardManager.CreateCardItemAsync(cardItem);
            return Ok();
        }
        catch (NullReferenceException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet(ApiEndpoints.Card.CardItemsById)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> GetCardItemsById([FromRoute] Guid id)
    {
        try
        {
            var items = await _cardManager.GetCardItemByIdAsync(id);
            var cardItem = _mapper.Map<CardItemResponse>(items);
            return Ok(cardItem);

        }
        catch (NullReferenceException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut(ApiEndpoints.Card.UpdateCardItems)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> UpdateCardItem([FromRoute] Guid id,[FromBody] CardItemsUpdateRequest request)
    {
        var cardItemId = await _cardManager.GetCardItemByIdAsync(id);
        if (cardItemId.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var mapeCardItem = _mapper.Map(request, cardItemId);
            await _cardManager.UpdateCardItemAsync(mapeCardItem);
            return Ok();

        }
        catch (NullReferenceException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        return BadRequest(ModelState);
    }

    [HttpGet(ApiEndpoints.Card.CardItems)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> GetAllCardItems()
    {
        try
        {
            var cardItems = await _cardManager.GetAllCardItemsAsync();
            var response = _mapper.Map<List<CardItemResponse>>(cardItems);
            return Ok(response);
        }
        catch (NullReferenceException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete(ApiEndpoints.Card.DeleteCardItems)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> DeleteCardItems([FromRoute] Guid id)
    {
        try
        {
            await _cardManager.DeleteCardItemAsync(id);
            return NoContent();
        }
        catch (NullReferenceException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}