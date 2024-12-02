using RestaurantManagement.Models;

namespace RestaurantManagement.Repository;

public interface ICardManager
{
    public Task<List<Card>> GetAllCardsAsync();
    public Task<Card> GetCardByIdAsync(Guid id);
    public Task<bool> DoesCustomerExists(Guid id);
    public Task<Menu?> DoesMenuExist(Guid id);
    public Task<Card?> DoesCardExist(Guid id);
    public Task<bool> DoesCardItemExist(Guid menuId, Guid cardId);
    public Task CreateCardAsync(CardItem request, Guid id, Card card, Menu menu);
    public Task DeleteCardAsync(Card card);
    public Task CreateCardItemAsync(CardItem request);
    public Task UpdateCardItemAsync(CardItem request);
    
    public Task<CardItem> GetCardItemByIdAsync(Guid id);
    public Task<bool> DoesExist(Guid id);
    public Task DeleteCardItemAsync(CardItem cardItem);
    public Task<List<CardItem>> GetAllCardItemsAsync();
}