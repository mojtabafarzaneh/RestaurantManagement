using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestaurantManagement.Models;

namespace RestaurantManagement.Contracts.Requests;

public class MenuRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    [Required]
    public Menu.CategoryType Category { get; set; }
    [Required]
    public int EstimatedPrepTime { get; set; }
    [Required]
    public int QuantityAvailable { get; set; }
}