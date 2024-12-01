using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Contracts.Requests.CardRequest;
using RestaurantManagement.Contracts.Responses.CardResponse;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;
using RestaurantManagement.Services;

namespace RestaurantManagement.Controllers;

[ApiController]
public class CardController : ControllerBase
{

    private readonly ILogger<CardController> _logger;
    private readonly IMapper _mapper;
    private readonly ICardService _cardService;

    public CardController(ILogger<CardController> logger, IMapper mapper, ICardService cardService)
    {
        _logger = logger;
        _mapper = mapper;
        _cardService = cardService;
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
       var mappedCard = _mapper.Map<CardItem>(request);
       await _cardService.CreateCardAsync(mappedCard);
       return Ok();
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
        var cards = await _cardService.GetAllCardsAsync();
        var mappedCards = _mapper.Map<List<Card>>(cards);
        return Ok(mappedCards);
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
        var request = await _cardService.GetCardByIdAsync();
        var mappedCard = _mapper.Map<Card>(request);
        return Ok(mappedCard);

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
        await _cardService.DeleteCardAsync();
        return NoContent();
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
       var mappedCard = _mapper.Map<CardItem>(request);
       await _cardService.CreateCardItemAsync(mappedCard);
       return Ok();
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
        var req =await _cardService.GetCardItemByIdAsync(id);
        var mappedCard = _mapper.Map<CardItem>(req);
        return Ok(mappedCard);
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
        var cardItemId = await _cardService.GetCardItemByIdAsync(id);
        if (cardItemId.Id != id)
        {
            return BadRequest(ModelState);
        }
        var mappedCardItem = _mapper.Map(request, cardItemId);
        await _cardService.UpdateCardItemAsync(mappedCardItem); 
        return Ok();
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
        var cardItems = await _cardService.GetAllCardItemsAsync(); 
        var response = _mapper.Map<List<CardItemResponse>>(cardItems); 
        return Ok(response);
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

        await _cardService.DeleteCardItemAsync(id);
        return NoContent();
    }
}