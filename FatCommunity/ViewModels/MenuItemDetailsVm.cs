using FatCommunity.Models;

namespace FatCommunity.ViewModels;

public class MenuItemDetailsVm
{
    public MenuItem MenuItem { get; set; } = null!;
    public double AvgRating { get; set; }
    public int ReviewCount { get; set; }

    public List<MenuItemReview> Reviews { get; set; } = new();
}