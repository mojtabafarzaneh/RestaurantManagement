using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestaurantManagement.Models;

namespace RestaurantManagement.Contracts.Requests;

public class MenuUpdate
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

}