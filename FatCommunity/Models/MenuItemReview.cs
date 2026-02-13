using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FatCommunity.Models;

public class MenuItemReview
{
    public int Id { get; set; }

    public int MenuItemId { get; set; }
    [ValidateNever]
    public MenuItem MenuItem { get; set; } = null!;

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