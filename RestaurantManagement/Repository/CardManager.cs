using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Services;

namespace RestaurantManagement.Repository;

public class CardManager: ICardManager
{
    private readonly ApplicationDBContex _context;
    private readonly RoleService _roleService;
    private readonly UserService _userService;

    public CardManager(ApplicationDBContex context, UserService userService, RoleService roleService)
    {
        _context = context;
        _userService = userService;
        _roleService = roleService;
    }

    public async Task<List<Card>> GetAllCardsAsync()
    {
        var isChef = _roleService.IsUserChef();
        var isManager = _roleService.IsManagerUser();
        var isAdmin = _roleService.IsAdminUser();
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
        var isUser = _userService.UserId();
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

    public async Task<Card> CreateCardAsync()
    {
        var userId = _userService.UserId();
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
        var card = new Card { CustomerId = customer.Id };
        var result = await _context.Cards.AddAsync(card);
        await _context.SaveChangesAsync();
        
        return result.Entity;
    }

    public async Task DeleteCardAsync()
    {
        var userId = _userService.UserId();
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

    public async Task CreateCardItemAsync(CardItem request)
    {
        var userId = _userService.UserId();
        if (userId == null)
        {
            throw new UnauthorizedAccessException();
        }
        if (!Guid.TryParse(userId, out var userGuid))
        {
            throw new ArgumentException("Invalid UserId format.");
        }
        var cardId = await _context.Cards.FirstOrDefaultAsync(c => c.CustomerId == userGuid);
        if (cardId == null)
        {
            throw new NullReferenceException("Invalid Card Id");
        }
        request.CartId = cardId.Id;
        var menu = await _context.Menus.FirstOrDefaultAsync(x => x.Id == request.MenuId);
        if (menu == null)
        {
            throw new NullReferenceException("Invalid Menu Id");
        }

        if (request.Quantity > menu.QuantityAvailable)
        {
            throw new ArgumentException("There's not enough available quantity");
        }
        await _context.CardItems.AddAsync(request);
        await _context.SaveChangesAsync();
        
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