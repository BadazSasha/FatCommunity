namespace FatCommunity.ViewModels;

public class RestaurantFiltersVm
{
    public string? Category { get; set; }
    public int? MinStars { get; set; }      // 1..5
    public int? MinReviews { get; set; }    // 0..N
}