using RestaurantManagement.Models;

namespace RestaurantManagement.Repository;

public class CardManager: ICardManager
{
    public Task<List<Card>> GetAllCardsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Card> GetCardByIdAsync(Guid cardId)
    {
        throw new NotImplementedException();
    }

    public Task<Card> CreateCardAsync(Card request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCardAsync(Guid cardId)
    {
        throw new NotImplementedException();
    }

    public Task<CardItem> CreateCardItemAsync(CardItem request)
    {
        throw new NotImplementedException();
    }

    public Task<CardItem> UpdateCardItemAsync(CardItem request)
    {
        throw new NotImplementedException();
    }

    public Task<CardItem> DeleteCardItemAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<CardItem>> GetAllCardItemsAsync()
    {
        throw new NotImplementedException();
    }
}