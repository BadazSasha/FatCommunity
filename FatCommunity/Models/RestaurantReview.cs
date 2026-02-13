using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FatCommunity.Models;

public class RestaurantReview
{
    public int Id { get; set; }

    public int RestaurantId { get; set; }
    [ValidateNever]
    public Restaurant Restaurant { get; set; } = null!;

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(120)]
    public string Title { get; set; } = "";

    [StringLength(4000)]
    public string Body { get; set; } = "";

    [StringLength(80)]
    public string ReviewerName { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}