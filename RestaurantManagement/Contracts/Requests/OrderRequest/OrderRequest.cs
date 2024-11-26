﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestaurantManagement.Models;

namespace RestaurantManagement.Contracts.Requests.OrderRequest;

public class OrderRequest
{
    [Required] 
    public Order.OrderType TypeOfOrder { get; set; }

    public int? TableNumber { get; set; }
    
}