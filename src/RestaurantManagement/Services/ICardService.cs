using RestaurantManagement.Models;

namespace RestaurantManagement.Services;

public interface ICardService
{
    public Task<List<Card>> GetAllCardsAsync();
    public Task<Card> GetCardByIdAsync();
    public Task CreateCardAsync(CardItem request);
    public Task DeleteCardAsync();
    public Task CreateCardItemAsync(CardItem request);
    public Task UpdateCardItemAsync(CardItem request);
    
    public Task<CardItem> GetCardItemByIdAsync(Guid id);
    public Task<bool> DoesExist(Guid id);
    public Task DeleteCardItemAsync(Guid id);
    public Task<List<CardItem>> GetAllCardItemsAsync();
    
}