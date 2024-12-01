using RestaurantManagement.Models;
using RestaurantManagement.Repository;

namespace RestaurantManagement.Services;

public class CardService: ICardService
{
    private readonly ILogger<CardService> _logger;
    private readonly ICardManager _cardManager;

    public CardService(ILogger<CardService> logger, ICardManager cardManager)
    {
        _logger = logger;
        _cardManager = cardManager;
    }

    public Task<List<Card>> GetAllCardsAsync()
    {
        return _cardManager.GetAllCardsAsync();
    }

    public Task<Card> GetCardByIdAsync()
    {
        return _cardManager.GetCardByIdAsync();
    }

    public Task CreateCardAsync(CardItem request)
    {
        return _cardManager.CreateCardAsync(request);
    }

    public Task DeleteCardAsync()
    {
        return _cardManager.DeleteCardAsync();
    }

    public Task CreateCardItemAsync(CardItem request)
    {
        return _cardManager.CreateCardItemAsync(request);
    }

    public Task UpdateCardItemAsync(CardItem request)
    {
        return _cardManager.UpdateCardItemAsync(request);
    }

    public Task<CardItem> GetCardItemByIdAsync(Guid id)
    {
        return _cardManager.GetCardItemByIdAsync(id);
    }

    public Task<bool> DoesExist(Guid id)
    {
        return _cardManager.DoesExist(id);
    }

    public Task DeleteCardItemAsync(Guid id)
    {
        return _cardManager.DeleteCardItemAsync(id);
    }

    public Task<List<CardItem>> GetAllCardItemsAsync()
    {
        return _cardManager.GetAllCardItemsAsync();
    }
}