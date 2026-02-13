using System.ComponentModel.DataAnnotations;

namespace FatCommunity.Models;

public class Restaurant
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    // Simple location (you can expand later if needed)
    [StringLength(120)]
    public string Street { get; set; } = "";

    [StringLength(80)]
    public string City { get; set; } = "";

    // Photo stored as URL or file path
    [StringLength(500)]
    public string PhotoUrl { get; set; } = "";

    [StringLength(2000)]
    public string Description { get; set; } = "";

    [StringLength(60)]
    public string Category { get; set; } = "";   // Fast Food, Cafe, etc.

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<MenuItem> MenuItems { get; set; } = new();
    public List<RestaurantReview> Reviews { get; set; } = new();
}