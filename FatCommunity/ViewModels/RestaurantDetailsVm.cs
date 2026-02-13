using FatCommunity.Models;

namespace FatCommunity.ViewModels;

public class RestaurantDetailsVm
{
    public Restaurant Restaurant { get; set; } = null!;
    public double AvgRating { get; set; }
    public int ReviewCount { get; set; }

    public List<MenuItemSummaryVm> MenuItems { get; set; } = new();
}