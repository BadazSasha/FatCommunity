namespace FatCommunity.ViewModels;

public class MenuItemSummaryVm
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string Category { get; set; } = "";
    public bool IsAvailable { get; set; }

    public double AvgRating { get; set; }
    public int ReviewCount { get; set; }
}