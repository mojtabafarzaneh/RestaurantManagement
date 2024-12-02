using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Helper;
namespace RestaurantManagement.Repository;

public class CardManager: ICardManager
{
    private readonly ApplicationDBContex _context;
    private readonly RoleHelper _roleHelper;
    private readonly UserHelper _userHelper;
    private readonly ILogger<CardManager> _logger;
    public CardManager(ApplicationDBContex context, UserHelper userHelper, RoleHelper roleHelper, ILogger<CardManager> logger)
    {
        _context = context;
        _userHelper = userHelper;
        _roleHelper = roleHelper;
        _logger = logger;
    }

    public async Task<List<Card>> GetAllCardsAsync()
    {
        var cards = await _context.Cards.ToListAsync();
        if (cards == null)
        {
            throw new NullReferenceException();
        }
        return cards;
    }

    public async Task<Card> GetCardByIdAsync(Guid id)
    {
        try
        {

            var card = await _context.Cards.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (card == null)
            {
                throw new NullReferenceException();
            }

            return card;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<bool> DoesCustomerExists(Guid id)
    {
        return await _context.Customers.AnyAsync(x => x.Id == id);

    }


    public async Task<Menu?> DoesMenuExist(Guid id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu == null)
        {
            return null;
        }
        return menu;
    }
    

    public async Task CreateCardAsync(CardItem request, Guid id, Card card, Menu menu)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try{
            var result = await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();
            
            request.CartId = result.Entity.Id;

            if (await _context.CardItems
                    .AnyAsync(x => x.MenuId == menu.Id && x.CartId == result.Entity.Id))
            {
                throw new InvalidOperationException("This item is already created");
            }
            menu.QuantityAvailable -= request.Quantity;
            await _context.CardItems.AddAsync(request);
            await _context.SaveChangesAsync();
        
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Card?> DoesCardExist(Guid id)
    {
        var card = await _context.Cards.FirstOrDefaultAsync(c => c.CustomerId == id);

        if (card == null)
        {
            return null;
        }
        return card;
    }
    public async Task DeleteCardAsync(Card card)
    {
        try
        {
            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<bool> DoesCardItemExist(Guid menuId, Guid cardId)
    {
        return await _context.CardItems
            .AnyAsync(x => x.MenuId == menuId && x.CartId == cardId);
    }

    public async Task CreateCardItemAsync(CardItem request)
    {
        try
        {
            await _context.CardItems.AddAsync(request);
            await _context.SaveChangesAsync();

        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task UpdateCardItemAsync(CardItem request)
    {
        try
        {
            _context.CardItems.Update(request);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }

    }

    public async Task<CardItem> GetCardItemByIdAsync(Guid id)
    {
        try
        {
            var cardItem = await _context.CardItems.FindAsync(id);
            if (cardItem == null)
            {
                throw new NullReferenceException("there are no cardItem with this id");
            }

            return cardItem;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }

        
    }

    public async Task DeleteCardItemAsync(CardItem cardItem)
    {
        try
        {
            _context.CardItems.Remove(cardItem);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<List<CardItem>> GetAllCardItemsAsync()
    {
        try
        {
            var cardItems = await _context.CardItems.ToListAsync();
            if (cardItems == null)
            {
                throw new NullReferenceException();
            }

            return cardItems;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
    public async Task<bool> DoesExist(Guid id)
    {
        var entity = await GetCardItemByIdAsync(id);
        return entity != null;
    }
}