namespace FatCommunity.ViewModels;

public class RestaurantIndexVm
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string City { get; set; } = "";
    public string Category { get; set; } = "";
    public double AvgRating { get; set; }
    public int ReviewCount { get; set; }
    public string PhotoUrl { get; set; } = "";

}