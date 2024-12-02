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
        var isChef = _roleHelper.IsUserChef();
        var isManager = _roleHelper.IsManagerUser();
        var isAdmin = _roleHelper.IsAdminUser();
        if (!isChef && !isManager && !isAdmin)
        {
            throw new UnauthorizedAccessException();
        }
        var cards = await _context.Cards.ToListAsync();
        if (cards == null)
        {
            throw new NullReferenceException();
        }
        return cards;
    }

    public async Task<Card> GetCardByIdAsync()
    {
        try
        {
            var isUser = _userHelper.UserId();
            if (isUser == null)
            {
                throw new UnauthorizedAccessException();
            }

            var card = await _context.Cards.FirstOrDefaultAsync(c => c.CustomerId.ToString() == isUser);
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

    public async Task CreateCardAsync(CardItem request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try{
            
            var userId = _userHelper.UserId();
            if (userId == null)
            {
                throw new NullReferenceException();
            }

            if (await _context.Cards.AnyAsync(c => c.CustomerId.ToString() == userId))
            {
                throw new InvalidOperationException("The card is already created");
            }
            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid UserId format.");
            }

            var customer = await _context.Customers.FindAsync(userGuid);
            if (customer == null)
            {
                throw new NullReferenceException("no customer with this Id was found");
            }
        
            var card = new Card { CustomerId = customer.Id };
            var result = await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();
            
            
            request.CartId = result.Entity.Id;
            var menu = await _context.Menus.FirstOrDefaultAsync(x => x.Id == request.MenuId);
            if (menu == null)
            {
                throw new NullReferenceException("Invalid Menu Id");
            }

            if (request.Quantity > menu.QuantityAvailable)
            {
                throw new ArgumentException("There's not enough available quantity of this item.");
            }

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

    public async Task DeleteCardAsync()
    {
        try
        {
            var userId = _userHelper.UserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            var card = _context.Cards.FirstOrDefault(c => c.CustomerId.ToString() == userId);
            if (card == null)
            {
                throw new NullReferenceException();
            }

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task CreateCardItemAsync(CardItem request)
    {
        try
        {
            var userId = _userHelper.UserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid UserId format.");
            }

            var card = await _context.Cards.FirstOrDefaultAsync(c => c.CustomerId == userGuid);
            if (card == null)
            {
                throw new NullReferenceException("Invalid Card Id");
            }

            request.CartId = card.Id;
            var menu = await _context.Menus.FirstOrDefaultAsync(x => x.Id == request.MenuId);
            if (menu == null)
            {
                throw new NullReferenceException("Invalid Menu Id");
            }

            if (request.Quantity > menu.QuantityAvailable)
            {
                throw new ArgumentException("There's not enough available quantity of this item.");
            }

            if (await _context.CardItems
                    .AnyAsync(x => x.MenuId == menu.Id && x.CartId == card.Id))
            {
                throw new InvalidOperationException("This item is already created");
            }

            menu.QuantityAvailable -= request.Quantity;
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
            var userId = _userHelper.UserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (request == null)
            {
                throw new NullReferenceException("Please provide a request.");
            }

            var menu = await _context.Menus.FirstOrDefaultAsync(x => x.Id == request.MenuId);
            if (menu == null)
            {
                throw new NullReferenceException("Invalid Menu Id");
            }

            if (request.Quantity > menu.QuantityAvailable)
            {
                throw new ArgumentException("There's not enough available quantity of this item.");
            }

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
            var userId = _userHelper.UserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid UserId format.");
            }

            if (!await _context.Cards.AnyAsync(x => x.CustomerId == userGuid))
            {
                throw new UnauthorizedAccessException("you can not get items of this card!");
            }

            if (id == Guid.Empty)
            {
                throw new NullReferenceException("card id can not be empty.");
            }

            var cardItem = await _context.CardItems.FindAsync(id);
            if (cardItem == null)
            {
                throw new NullReferenceException();
            }

            return cardItem;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }

        
    }

    public async Task DeleteCardItemAsync(Guid id)
    {
        try
        {
            var userId = _userHelper.UserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            var isChef = _roleHelper.IsUserChef();
            if (!isChef)
            {
                if (!Guid.TryParse(userId, out var userGuid))
                {
                    throw new ArgumentException("Invalid UserId format.");
                }

                var card = await _context.Cards.FirstOrDefaultAsync(x => x.CustomerId == userGuid);
                if (card == null)
                {
                    throw new NullReferenceException();
                }

                if (card.CustomerId != userGuid)
                {
                    throw new UnauthorizedAccessException("you can not delete this item!");
                }
            }

            var cardItem = await _context.CardItems.FindAsync(id);
            if (cardItem == null)
            {
                throw new NullReferenceException("there are no card item in this card!");
            }

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
            var isChef = _roleHelper.IsUserChef();
            var isManager = _roleHelper.IsManagerUser();
            var isAdmin = _roleHelper.IsAdminUser();
            if (!isChef && !isManager && !isAdmin)
            {
                throw new UnauthorizedAccessException();
            }

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