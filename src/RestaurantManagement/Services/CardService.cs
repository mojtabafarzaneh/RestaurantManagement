using RestaurantManagement.Helper;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;

namespace RestaurantManagement.Services;

public class CardService: ICardService
{
    private readonly ILogger<CardService> _logger;
    private readonly UserHelper _userHelper;
    private readonly RoleHelper _roleHelper;
    private readonly ICardManager _cardManager;

    public CardService(ILogger<CardService> logger, ICardManager cardManager, RoleHelper roleHelper, UserHelper userHelper)
    {
        _logger = logger;
        _cardManager = cardManager;
        _roleHelper = roleHelper;
        _userHelper = userHelper;
    }

    public Task<List<Card>> GetAllCardsAsync()
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

            return _cardManager.GetAllCardsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        
    }

    public Task<Card> GetCardByIdAsync()
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

            return _cardManager.GetCardByIdAsync(userGuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task CreateCardAsync(CardItem request)
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

            if (!await _cardManager.DoesCustomerExists(userGuid))
            {
                throw new ArgumentException("no customer with this Id was found");
            }
            var card = new Card { CustomerId = userGuid };

            var menu = await _cardManager.DoesMenuExist(request.MenuId);
            if (menu == null)
            {
                throw new NullReferenceException("Invalid Menu Id");
            }
            if (request.Quantity > menu.QuantityAvailable)
            {
                throw new ArgumentException("There's not enough available quantity of this item.");
            }


            await _cardManager.CreateCardAsync(request, userGuid, card, menu);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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

            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid UserId format.");
            }

            var card = await _cardManager.DoesCardExist(userGuid);
            if (card == null)
            {
                throw new NullReferenceException("No card with that Id was found");
            }

            await _cardManager.DeleteCardAsync(card);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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

            var card = await _cardManager.DoesCardExist(userGuid);
            if (card == null)
            {
                throw new NullReferenceException("No card with that Id was found");
            }
            request.CartId = card.Id;
            
            var menu = await _cardManager.DoesMenuExist(request.MenuId);
            if (menu == null)
            {
                throw new NullReferenceException("Invalid Menu Id");
            }
            if (request.Quantity > menu.QuantityAvailable)
            {
                throw new ArgumentException("There's not enough available quantity of this item.");
            }

            if (!await _cardManager.DoesCardItemExist(menu.Id, card.Id))
            {
                throw new ArgumentException("This item is already created");
            }
            menu.QuantityAvailable -= request.Quantity;


            await _cardManager.CreateCardItemAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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
            var menu = await _cardManager.DoesMenuExist(request.MenuId);
            if (menu == null)
            {
                throw new NullReferenceException("Invalid Menu Id");
            }
            if (request.Quantity > menu.QuantityAvailable)
            {
                throw new ArgumentException("There's not enough available quantity of this item.");
            }

            await  _cardManager.UpdateCardItemAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
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

            var card = _cardManager.DoesCardExist(userGuid);
            if (card == null)
            {
                throw new ArgumentException("There is no card with the given Id.");
            }
            
            return await _cardManager.GetCardItemByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }

    }

    public Task<bool> DoesExist(Guid id)
    {
        return _cardManager.DoesExist(id);
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
                var card = await _cardManager.DoesCardExist(userGuid);
                if (card == null)
                {
                    throw new ArgumentException("There is no card with the given Id.");
                }
                if (card.CustomerId != userGuid)
                {
                    throw new UnauthorizedAccessException("you can not delete this item!");
                }
            }

            var cardItem = await _cardManager.GetCardItemByIdAsync(id);
            if (cardItem != null)
            {
                throw new ArgumentException("There is no cardItem with the given Id.");
            }

            if (cardItem != null) await _cardManager.DeleteCardItemAsync(cardItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public Task<List<CardItem>> GetAllCardItemsAsync()
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

            return _cardManager.GetAllCardItemsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        
    }
}