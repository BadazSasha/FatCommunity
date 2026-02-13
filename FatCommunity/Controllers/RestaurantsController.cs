using FatCommunity.Data;
using FatCommunity.Models;
using FatCommunity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FatCommunity.Controllers;

public class RestaurantsController : Controller
{
    private readonly AppDbContext _db;

    public RestaurantsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /Restaurants
    // Supports filters via query string:
    // /Restaurants?category=Fast%20Food&minStars=4&minReviews=2
    // GET: /Restaurants
    [HttpGet]
public async Task<IActionResult> Index(string? category, int? minStars, int? minReviews, string? sort)
{
    // Base query: compute AvgRating + ReviewCount per restaurant
    var query =
        from r in _db.Restaurants.AsNoTracking()
        join rr in _db.RestaurantReviews.AsNoTracking()
            on r.Id equals rr.RestaurantId into reviews
        select new
        {
            Restaurant = r,
            ReviewCount = reviews.Count(),
            AvgRating = reviews.Select(x => (double?)x.Rating).Average() ?? 0.0
        };

    // Filters
    if (!string.IsNullOrWhiteSpace(category))
        query = query.Where(x => x.Restaurant.Category == category);

    if (minStars.HasValue)
        query = query.Where(x => x.AvgRating >= minStars.Value);

    if (minReviews.HasValue)
        query = query.Where(x => x.ReviewCount >= minReviews.Value);

    // Sorting
    sort = string.IsNullOrWhiteSpace(sort) ? "TopRated" : sort;

    query = sort switch
    {
        "MostReviewed" => query
            .OrderByDescending(x => x.ReviewCount)
            .ThenByDescending(x => x.AvgRating)
            .ThenBy(x => x.Restaurant.Name),

        "Name" => query
            .OrderBy(x => x.Restaurant.Name),

        "Newest" => query
            .OrderByDescending(x => x.Restaurant.CreatedAtUtc)
            .ThenBy(x => x.Restaurant.Name),

        _ => query // TopRated (default)
            .OrderByDescending(x => x.AvgRating)
            .ThenByDescending(x => x.ReviewCount)
            .ThenBy(x => x.Restaurant.Name)
    };

    var restaurants = await query
        .Select(x => new RestaurantIndexVm
        {
            Id = x.Restaurant.Id,
            Name = x.Restaurant.Name,
            City = x.Restaurant.City,
            Category = x.Restaurant.Category,
            PhotoUrl = x.Restaurant.PhotoUrl,
            AvgRating = x.AvgRating,
            ReviewCount = x.ReviewCount
        })
        .ToListAsync();

    var vm = new RestaurantsIndexPageVm
    {
        Restaurants = restaurants,
        SelectedCategory = category,
        SelectedMinStars = minStars,
        SelectedMinReviews = minReviews,
        CategoryOptions = new List<string> { "Fast Food", "Restaurant and pub", "Fancy Restaurant" },

        SelectedSort = sort
    };

    return View(vm);
}


    // GET: /Restaurants/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var restaurant = await _db.Restaurants
            .Include(r => r.Reviews)
            .FirstOrDefaultAsync(r => r.Id == id.Value);

        if (restaurant == null) return NotFound();

        var restaurantReviewsQuery = _db.RestaurantReviews.Where(rr => rr.RestaurantId == restaurant.Id);

        var menuItems = await _db.MenuItems
            .Where(m => m.RestaurantId == restaurant.Id)
            .OrderBy(m => m.Name)
            .Select(m => new FatCommunity.ViewModels.MenuItemSummaryVm
            {
                Id = m.Id,
                RestaurantId = m.RestaurantId,
                Name = m.Name,
                Price = m.Price,
                Category = m.Category,
                IsAvailable = m.IsAvailable,

                AvgRating = _db.MenuItemReviews
                    .Where(r => r.MenuItemId == m.Id)
                    .Select(r => (double?)r.Rating)
                    .Average() ?? 0,
                ReviewCount = _db.MenuItemReviews.Count(r => r.MenuItemId == m.Id)
            })
            .ToListAsync();

        var vm = new FatCommunity.ViewModels.RestaurantDetailsVm
        {
            Restaurant = restaurant,
            AvgRating = await restaurantReviewsQuery.Select(rr => (double?)rr.Rating).AverageAsync() ?? 0,
            ReviewCount = await restaurantReviewsQuery.CountAsync(),
            MenuItems = menuItems
        };

        return View(vm);
    }

    // GET: /Restaurants/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Restaurants/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Restaurant restaurant)
    {
        if (!ModelState.IsValid) return View(restaurant);

        restaurant.CreatedAtUtc = DateTime.UtcNow;
        restaurant.UpdatedAtUtc = DateTime.UtcNow;

        _db.Restaurants.Add(restaurant);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Restaurants/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var restaurant = await _db.Restaurants.FindAsync(id.Value);
        if (restaurant == null) return NotFound();

        return View(restaurant);
    }

    // POST: /Restaurants/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Restaurant restaurant)
    {
        if (id != restaurant.Id) return NotFound();
        if (!ModelState.IsValid) return View(restaurant);

        var existing = await _db.Restaurants.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = restaurant.Name;
        existing.Street = restaurant.Street;
        existing.City = restaurant.City;
        existing.PhotoUrl = restaurant.PhotoUrl;
        existing.Description = restaurant.Description;
        existing.Category = restaurant.Category;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Restaurants/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var restaurant = await _db.Restaurants.FirstOrDefaultAsync(r => r.Id == id.Value);
        if (restaurant == null) return NotFound();

        return View(restaurant);
    }

    // POST: /Restaurants/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var restaurant = await _db.Restaurants.FindAsync(id);
        if (restaurant != null)
        {
            _db.Restaurants.Remove(restaurant);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
