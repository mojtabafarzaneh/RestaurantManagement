using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Contracts.Requests;
using RestaurantManagement.Models;

namespace RestaurantManagement.Repository;

public class AuthManager: IAuthManager
{
    private readonly UserManager<Customer> _userManager;
    private readonly IMapper _mapper;

    public AuthManager(UserManager<Customer> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }
    public async Task<IEnumerable<IdentityError>> Register(RegisterRequest request)
    {
        var user = _mapper.Map<Customer>(request);
        user.UserName = request.Email;
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Customer");
        }
        
        return result.Errors;
    }
}