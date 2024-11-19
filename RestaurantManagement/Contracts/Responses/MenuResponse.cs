using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestaurantManagement.Models;

namespace RestaurantManagement.Contracts.Responses;

public class MenuResponse
{

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public Menu.CategoryType Category { get; set; }

    public int EstimatedPrepTime { get; set; }

    public int QuantityAvailable { get; set; }

    public bool Available { get; set; }
}