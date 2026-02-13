using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FatCommunity.Models;

public class MenuItem
{
    public int Id { get; set; }

    public int RestaurantId { get; set; }
    [ValidateNever]
    public Restaurant Restaurant { get; set; } = null!;

    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    [StringLength(1000)]
    public string Description { get; set; } = "";

    [StringLength(500)]
    public string PhotoUrl { get; set; } = "";

    // ✅ PRICE – as you requested
    [Range(0, 9999)]
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; } = true;

    [StringLength(40)]
    public string Category { get; set; } = "";   // Burger, Drink, Dessert...

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<MenuItemReview> Reviews { get; set; } = new();
}