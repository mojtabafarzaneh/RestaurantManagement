using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RestaurantManagement.Contracts.Responses.OrderResponse;
using RestaurantManagement.Helper;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;
using RestaurantManagement.Services;

namespace RestaurantManagement.Test.Unit;

public class OrderTest
{
    private readonly TestUserFixture _fixture;
    private readonly UserHelper _userHelper;
    private readonly IOrderManager _orderManager;
    private readonly RoleHelper _roleHelper;
    private readonly ILogger<OrderService> _logger;
    private readonly OrderService _orderService;

    public OrderTest()
    {
        _fixture = new TestUserFixture();
        _orderManager = Substitute.For<IOrderManager>();
        _logger = Substitute.For<ILogger<OrderService>>();
        var contextAccessor = _fixture.CreateHttpContextAccessor(
            Guid.NewGuid().ToString(), "testuser@example.com", "Customer");
        _userHelper = new UserHelper(contextAccessor);
        _roleHelper = new RoleHelper(contextAccessor);
        _orderService = new OrderService(_orderManager, _logger ,_roleHelper, _userHelper );
    }

    [Fact]
    public async Task CreateOrder_ShouldThrowUnauthorizedAccessException_WhenUserIdIsNull()
    {
        var context = _fixture.CreateHttpContextAccessor("", "", "");
        var userHelper = new UserHelper(context);
        var roleHelper = new RoleHelper(context);
        var orderService = new OrderService(_orderManager, _logger, roleHelper, userHelper);
        var order = new Order { TypeOfOrder = Order.OrderType.Online, TableNumber = 2 };
        
        //act 
        Func<Task> act = async () => await orderService.CreateOrder(order);
        
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task CreateOrder_ShouldThrowArgumentException_WhenUserIdIsInvalid()
    {
        // Arrange
        var contextAccessor = _fixture.CreateHttpContextAccessor("12345", "testuser@example.com", "Customer");
        var userHelper = new UserHelper(contextAccessor);
        var roleHelper = new RoleHelper(contextAccessor);
        var orderService = new OrderService(_orderManager, _logger, roleHelper, userHelper);
        var order = new Order { TypeOfOrder = Order.OrderType.InHouse, TableNumber = 5 };

        Func<Task> act = async () => await orderService.CreateOrder(order);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Invalid UserId format.");
    }

    [Fact]
    public async Task CreateOrder_ShouldThrowArgumentException_WhenOrderTypeIsEmpty()
    {
        // Arrange
        var order = new Order { TypeOfOrder = 0, TableNumber = 5 };

        // Act
        Func<Task> act = async () => await _orderService.CreateOrder(order);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Type of order cannot be empty");
    }
    
    [Fact]
    public async Task CreateOrder_ShouldThrowArgumentException_WhenTableNumberIsInvalidForInHouseOrder()
    {
        // Arrange
        var order = new Order { TypeOfOrder = Order.OrderType.InHouse, TableNumber = 15 };

        // Act
        Func<Task> act = async () => await _orderService.CreateOrder(order);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Table number must be between 0 and 10");
    }

    [Fact]
    public async Task CreateOrder_ShouldThrowArgumentException_WhenDeliveryOrderHasTableNumber()
    {
        // Arrange
        var order = new Order { TypeOfOrder = Order.OrderType.Delivery, TableNumber = 10 };

        // Act
        Func<Task> act = async () => await _orderService.CreateOrder(order);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("This order cannot have a table number");
    }

    [Fact]
    public async Task CreateOrder_ShouldThrowArgumentException_WhenOnlineOrderHasTableNumber()
    {
        // Arrange
        var order = new Order { TypeOfOrder = Order.OrderType.Online, TableNumber = 10 };

        // Act
        Func<Task> act = async () => await _orderService.CreateOrder(order);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("This order cannot have a table number");
    }

    [Fact]
    public async Task CreateOrder_ShouldThrowArgumentException_WhenUserHasAlreadyOrdered()
    {
        _orderManager.DoesUserHasOrdered(Arg.Any<Guid>()).Returns(true);
        var order = new Order { TypeOfOrder = Order.OrderType.InHouse, TableNumber = 10 };
        
        Func<Task> act = async () => await _orderService.CreateOrder(order);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("This user has already ordered");
    }

    [Fact]
    public async Task CreateOrder_ShouldSetCorrectOrderStatus_WhenOrderIsInHouse()
    {
        _orderManager.DoesUserHasOrdered(Arg.Any<Guid>()).Returns(false);
        var order = new Order { TypeOfOrder = Order.OrderType.InHouse, TableNumber = 10 };
        
        await _orderService.CreateOrder(order);
        
        order.StatusType.Should().Be(Order.Status.Preparing);
    }

    [Fact]
    public async Task CreateOrder_ShouldSetCorrectOrderStatus_WhenOrderIsOnline()
    {
        _orderManager.DoesUserHasOrdered(Arg.Any<Guid>()).Returns(false);
        var order = new Order { TypeOfOrder = Order.OrderType.Online, TableNumber = null };
        
        await _orderService.CreateOrder(order);

        order.StatusType.Should().Be(Order.Status.Pending);
    }

    [Fact]
    public async Task CreateOrder_ShouldCallCreateOrder_OnOrderManager()
    {
        var card = new Card();
        _orderManager.DoesUserHasOrdered(Arg.Any<Guid>()).Returns(false);
        _orderManager.DoesUserHasCard(Arg.Any<Guid>()).Returns(card);
        var order = new Order { TypeOfOrder = Order.OrderType.Online, TableNumber = null };
        
        await _orderService.CreateOrder(order);

        await _orderManager.Received(1)
            .CreateOrder(order, Arg.Any<Guid>(), card, false);
    }

    [Fact]
    public async Task GetOrders_ShouldThrowUnauthorizedAccessException_WhenUserRoleIsCustomer()
    {
        var contextAccessor = _fixture.CreateHttpContextAccessor(Guid.NewGuid().ToString(), "testuser@example.com", "Customer");
        var userHelper = new UserHelper(contextAccessor);
        var roleHelper = new RoleHelper(contextAccessor);
        var orderService = new OrderService(_orderManager, _logger, roleHelper, userHelper);
        
        Func<Task> act = async () => await orderService.GetOrders();

        await act.Should().ThrowAsync<UnauthorizedAccessException>();

    }

    [Fact]
    public async Task GetOrders_ShouldAccessUser_WhenUserRoleIsManagerOrChef()
    {
        var context = _fixture.CreateHttpContextAccessor(
            userId: Guid.NewGuid().ToString(),
            email: "testmanager@example.com",
            roles: "Chef");
        var roleHelper = new RoleHelper(context);
        var userHelper = new UserHelper(context);
        var orderService = new OrderService(_orderManager, _logger, roleHelper, userHelper);
        var orders = new List<Order>
        {
            new Order { Id = Guid.NewGuid(), TypeOfOrder = Order.OrderType.Online }
        };
        var orderResponse = new List<OrderResponse>
        {
            new OrderResponse { Id = orders[0].Id.ToString(), TypeOfOrder = Order.OrderType.Online },
        };
        _orderManager.DoesAnyOrdersExist().Returns(orders);
        _orderManager.GetOrders(orders).Returns(orderResponse);
        
        var result = await orderService.GetOrders();

        result.Should().NotBeNull();
        result.Should().HaveCount(1);

    }

    [Fact]
    public async Task GetOrders_ShouldThrowArgumentException_WhenOrderIsNull()
    {
        _orderManager.DoesAnyOrdersExist().Returns((List<Order>)null);
        var context = _fixture.CreateHttpContextAccessor(
            userId: Guid.NewGuid().ToString(),
            email: "testmanager@example.com",
            roles: "Chef");
        var roleHelper = new RoleHelper(context);
        var userHelper = new UserHelper(context);
        var orderService = new OrderService(_orderManager, _logger, roleHelper, userHelper);

        
        // Act
        Func<Task> act = async () => await orderService.GetOrders();

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("there are no orders!");
    }

    [Fact]
    public async Task GetOrderById_ShouldThrowUnauthorizedAccessException_whenUserIdIsNull()
    {
        var context = _fixture.CreateHttpContextAccessor("", "testuser@example.com", "Customer");
        var roleHelper = new RoleHelper(context);
        var userHelper = new UserHelper(context);
        var orderService = new OrderService(_orderManager, _logger, roleHelper, userHelper);

        Func<Task> act = async () => await orderService.GetOrderById();
        
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task GetOrderById_ShouldThrowArgumentException_whenUserHadNotOrdered()
    {
        _orderManager.DoesUserHasOrdered(Arg.Any<Guid>()).Returns(false);
        
        Func<Task> act = async () => await _orderService.GetOrderById();
        
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("There are no orders for this user!");
        
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnOrderResponse_WhenOrderExists()
    {
        // Arrange
        var validUserId = Guid.NewGuid().ToString();
        var userGuid = Guid.Parse(validUserId);
        var context = _fixture.CreateHttpContextAccessor(
            userId: validUserId, "email@email.com", "Customer");
        var userHelper = new UserHelper(context);
        var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

        var order = new Order
        {
            Id = Guid.NewGuid(),
        };
        var orderResponse = new OrderResponse
        {
            Id = order.Id.ToString(),
        };

        _orderManager.FindOrderById(userGuid).Returns(order);
        _orderManager.GetOrderById(order).Returns(orderResponse);

        // Act
        var result = await orderService.GetOrderById();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id.ToString());
    }
    
    [Fact]
public async Task UpdateOrder_ShouldThrowArgumentException_WhenUserIdIsInvalid()
{
    // Arrange
    var invalidUserId = "invalid-guid";
    var context = _fixture.CreateHttpContextAccessor(userId: invalidUserId, "email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);
    var request = new Order();

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() => orderService.UpdateOrder(request));
}

[Fact]
public async Task UpdateOrder_ShouldThrowArgumentException_WhenOrderNotFound()
{
    // Arrange
    var validUserId = Guid.NewGuid().ToString();
    var context = _fixture.CreateHttpContextAccessor(userId: validUserId, "email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);
    var request = new Order();

    _orderManager.FindOrderById(Arg.Any<Guid>()).Returns((Order)null);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<ArgumentException>(() => orderService.UpdateOrder(request));
    exception.Message.Should().Be("There are no orders for this user!");
}

[Fact]
public async Task UpdateOrder_ShouldUpdateOrderAndRemoveTicket_WhenStatusCancelled()
{
    // Arrange
    var validUserId = Guid.NewGuid().ToString();
    var userGuid = Guid.Parse(validUserId);
    var context = _fixture.CreateHttpContextAccessor(userId: validUserId, "email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

    var order = new Order
    {
        Id = Guid.NewGuid(),
        StatusType = Order.Status.Pending
    };

    var ticket = new Ticket
    {
        Id = Guid.NewGuid(),
        TicketStatus = Ticket.Status.Waiting
    };

    _orderManager.FindOrderById(userGuid).Returns(order);
    _orderManager.FindTicket(order.Id).Returns(ticket);

    var request = new Order
    {
        StatusType = Order.Status.Cancelled
    };

    // Act
    await orderService.UpdateOrder(request);

    // Assert
    await _orderManager.Received(1).RemoveTicket(ticket);
    await _orderManager.Received(1).UpdateOrder(order);
    order.StatusType.Should().Be(Order.Status.Cancelled);
}

[Fact]
public async Task UpdateOrder_ShouldUpdateTicketAndOrder_WhenStatusDelayed()
{
    // Arrange
    var validUserId = Guid.NewGuid().ToString();
    var userGuid = Guid.Parse(validUserId);
    var context = _fixture.CreateHttpContextAccessor(userId: validUserId, "email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

    var order = new Order
    {
        Id = Guid.NewGuid(),
        StatusType = Order.Status.Pending
    };

    var ticket = new Ticket
    {
        Id = Guid.NewGuid(),
        TicketStatus = Ticket.Status.Waiting,
        IsFlagged = false
    };

    _orderManager.FindOrderById(userGuid).Returns(order);
    _orderManager.FindTicket(order.Id).Returns(ticket);

    var request = new Order
    {
        StatusType = Order.Status.Delayed
    };

    // Act
    await orderService.UpdateOrder(request);

    // Assert
    ticket.TicketStatus.Should().Be(Ticket.Status.Delayed);
    ticket.IsFlagged.Should().BeTrue();
    order.StatusType.Should().Be(Order.Status.Delayed);
    await _orderManager.Received(1).UpdateOrder(order);
}

[Fact]
public async Task UpdateOrder_ShouldUpdateTicketAndOrder_WhenStatusCompleted()
{
    // Arrange
    var validUserId = Guid.NewGuid().ToString();
    var userGuid = Guid.Parse(validUserId);
    var context = _fixture.CreateHttpContextAccessor(userId: validUserId, "email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

    var order = new Order
    {
        Id = Guid.NewGuid(),
        StatusType = Order.Status.Preparing
    };

    var ticket = new Ticket
    {
        Id = Guid.NewGuid(),
        TicketStatus = Ticket.Status.Waiting
    };

    _orderManager.FindOrderById(userGuid).Returns(order);
    _orderManager.FindTicket(order.Id).Returns(ticket);

    var request = new Order
    {
        StatusType = Order.Status.Completed
    };

    // Act
    await orderService.UpdateOrder(request);

    // Assert
    ticket.TicketStatus.Should().Be(Ticket.Status.Served);
    order.StatusType.Should().Be(Order.Status.Completed);
    await _orderManager.Received(1).UpdateOrder(order);
}
[Fact]
public async Task DeleteOrder_ShouldThrowUnauthorizedAccessException_WhenUserIdIsNull()
{
    // Arrange
    var context = _fixture.CreateHttpContextAccessor(userId: "", "email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

    // Act & Assert
    await Assert.ThrowsAsync<UnauthorizedAccessException>(() => orderService.DeleteOrder());
}

[Fact]
public async Task DeleteOrder_ShouldThrowArgumentException_WhenUserIdIsInvalid()
{
    // Arrange
    var invalidUserId = "invalid-guid";
    var context = _fixture.CreateHttpContextAccessor(userId: invalidUserId, "email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<ArgumentException>(() => orderService.DeleteOrder());
    exception.Message.Should().Be("Invalid UserId format.");
}

[Fact]
public async Task DeleteOrder_ShouldThrowArgumentException_WhenOrderNotFound()
{
    // Arrange
    var validUserId = Guid.NewGuid().ToString();
    var context = _fixture.CreateHttpContextAccessor(userId: validUserId,"email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

    _orderManager.GetOrderById(Arg.Any<Guid>()).Returns((Order)null);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<ArgumentException>(() => orderService.DeleteOrder());
    exception.Message.Should().Be("There are no orders for this user!");
}

[Fact]
public async Task DeleteOrder_ShouldDeleteOrder_WhenOrderExists()
{
    // Arrange
    var validUserId = Guid.NewGuid().ToString();
    var userGuid = Guid.Parse(validUserId);
    var context = _fixture.CreateHttpContextAccessor(userId: validUserId, "email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

    var order = new Order
    {
        Id = Guid.NewGuid(),
        StatusType = Order.Status.Pending
    };

    _orderManager.GetOrderById(userGuid).Returns(order);

    // Act
    await orderService.DeleteOrder();

    // Assert
    await _orderManager.Received(1).DeleteOrder(order);
}
[Fact]
public async Task GetTicketById_ShouldThrowUnauthorizedAccessException_WhenUserIdIsNull()
{
    // Arrange
    var context = _fixture.CreateHttpContextAccessor(userId: "","email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

    // Act & Assert
    await Assert.ThrowsAsync<UnauthorizedAccessException>(() => orderService.GetTicketById());
}

[Fact]
public async Task GetTicketById_ShouldThrowArgumentException_WhenUserIdIsInvalid()
{
    // Arrange
    var invalidUserId = "invalid-guid";
    var context = _fixture.CreateHttpContextAccessor(userId: invalidUserId,"email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<ArgumentException>(() => orderService.GetTicketById());
    exception.Message.Should().Be("Invalid UserId format.");
}

[Fact]
public async Task GetTicketById_ShouldReturnNull_WhenTicketDoesNotExist()
{
    // Arrange
    var validUserId = Guid.NewGuid().ToString();
    var userGuid = Guid.Parse(validUserId);
    var context = _fixture.CreateHttpContextAccessor(userId: validUserId,"email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

    _orderManager.GetTicketById(userGuid).Returns((Ticket)null);

    // Act
    var result = await orderService.GetTicketById();

    // Assert
    result.Should().BeNull();
}

[Fact]
public async Task GetTicketById_ShouldReturnTicket_WhenTicketExists()
{
    // Arrange
    var validUserId = Guid.NewGuid().ToString();
    var userGuid = Guid.Parse(validUserId);
    var context = _fixture.CreateHttpContextAccessor(userId: validUserId, "email@email.com", "Customer");
    var userHelper = new UserHelper(context);
    var orderService = new OrderService(_orderManager, _logger, _roleHelper, userHelper);

    var ticket = new Ticket
    {
        Id = Guid.NewGuid(),
        TicketStatus = Ticket.Status.Waiting
    };

    _orderManager.GetTicketById(userGuid).Returns(ticket);

    // Act
    var result = await orderService.GetTicketById();

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEquivalentTo(ticket);
}

}

