namespace FatCommunity.ViewModels;

public class RestaurantsIndexPageVm
{
    public List<RestaurantIndexVm> Restaurants { get; set; } = new();

    public string? SelectedCategory { get; set; }
    public int? SelectedMinStars { get; set; }
    public int? SelectedMinReviews { get; set; }
    public string SelectedSort { get; set; } = "TopRated";


    public List<string> CategoryOptions { get; set; } = new();
}