using AutoMapper;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Contracts.Responses;
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
    }
}