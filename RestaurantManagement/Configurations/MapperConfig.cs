using AutoMapper;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Requests.CardRequest;
using RestaurantManagement.Contracts.Requests.OrderRequest;
using RestaurantManagement.Contracts.Responses;
using RestaurantManagement.Contracts.Responses.CardResponse;
using RestaurantManagement.Contracts.Responses.OrderResponse;
using RestaurantManagement.Models;

namespace RestaurantManagement.Configurations;

public class MapperConfig: Profile
{
    public MapperConfig()
    {
        //Registeration/AuthManager Mappers
        CreateMap<Customer, RegisterRequest>().ReverseMap();
        CreateMap<Customer, LoginRequest>().ReverseMap();
        CreateMap<Customer, CustomerResponse>().ReverseMap();
        
        //Menus Mappers
        CreateMap<Menu, MenuResponse>().ReverseMap();
        CreateMap<Menu, MenuRequest>().ReverseMap();
        CreateMap<Menu, MenuUpdate>().ReverseMap();
        
        //Card and CardItems 
        CreateMap<Card, CardRequest>().ReverseMap();
        CreateMap<Card, CardResponse>().ReverseMap();
        CreateMap<CardItem, CardItemsUpdateRequest>().ReverseMap();
        CreateMap<CardItem, CardItemRequest>().ReverseMap();
        CreateMap<CardItem, CardItemResponse>().ReverseMap();
        
        // Order and OrderItem
        CreateMap<Order, OrderResponse>().ReverseMap();
        CreateMap<Order, OrderRequest>().ReverseMap();

    }
}