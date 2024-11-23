using RestaurantManagement.Models;

namespace RestaurantManagement.Repository;

public interface ICardManager
{
    public Task<List<Card>> GetAllCardsAsync();
    public Task<Card> GetCardByIdAsync(Guid cardId);
    public Task<Card> CreateCardAsync(Card request);
    public Task DeleteCardAsync(Guid cardId);
    public Task<CardItem> CreateCardItemAsync(CardItem request);
    public Task<CardItem> UpdateCardItemAsync(CardItem request);
    public Task<CardItem> DeleteCardItemAsync(Guid id);
    public Task<List<CardItem>> GetAllCardItemsAsync();
}