using RestaurantManagement.Models;

namespace RestaurantManagement.Repository;

public interface ICardManager
{
    public Task<List<Card>> GetAllCardsAsync();
    public Task<Card> GetCardByIdAsync();
    public Task<Card> CreateCardAsync();
    public Task DeleteCardAsync();
    public Task CreateCardItemAsync(CardItem request);
    public Task<CardItem> UpdateCardItemAsync(CardItem request);
    public Task<CardItem> DeleteCardItemAsync(Guid id);
    public Task<List<CardItem>> GetAllCardItemsAsync();
}