using FatCommunity.Data;
using FatCommunity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FatCommunity.ViewModels;

namespace FatCommunity.Controllers;

public class MenuItemsController : Controller
{
    private readonly AppDbContext _db;

    public MenuItemsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /MenuItems/Create?restaurantId=5
    public async Task<IActionResult> Create(int restaurantId)
    {
        var restaurant = await _db.Restaurants.FindAsync(restaurantId);
        if (restaurant == null) return NotFound();

        ViewBag.RestaurantName = restaurant.Name;

        return View(new MenuItem
        {
            RestaurantId = restaurantId
        });
    }

    // POST: /MenuItems/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuItem menuItem)
    {
        var restaurant = await _db.Restaurants.FindAsync(menuItem.RestaurantId);
        if (restaurant == null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.RestaurantName = restaurant.Name;
            return View(menuItem);
        }

        menuItem.CreatedAtUtc = DateTime.UtcNow;
        menuItem.UpdatedAtUtc = DateTime.UtcNow;

        _db.MenuItems.Add(menuItem);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Restaurants", new { id = menuItem.RestaurantId });
    }

    // GET: /MenuItems/Edit/10
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var menuItem = await _db.MenuItems
            .Include(m => m.Restaurant)
            .FirstOrDefaultAsync(m => m.Id == id.Value);

        if (menuItem == null) return NotFound();

        ViewBag.RestaurantName = menuItem.Restaurant.Name;
        return View(menuItem);
    }

    // POST: /MenuItems/Edit/10
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MenuItem menuItem)
    {
        if (id != menuItem.Id) return NotFound();

        var existing = await _db.MenuItems.FindAsync(id);
        if (existing == null) return NotFound();

        if (!ModelState.IsValid)
        {
            var restaurant = await _db.Restaurants.FindAsync(existing.RestaurantId);
            ViewBag.RestaurantName = restaurant?.Name ?? "";
            return View(menuItem);
        }

        existing.Name = menuItem.Name;
        existing.Description = menuItem.Description;
        existing.PhotoUrl = menuItem.PhotoUrl;
        existing.Price = menuItem.Price;
        existing.Category = menuItem.Category;
        existing.IsAvailable = menuItem.IsAvailable;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Restaurants", new { id = existing.RestaurantId });
    }

    // GET: /MenuItems/Delete/10
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var menuItem = await _db.MenuItems
            .Include(m => m.Restaurant)
            .FirstOrDefaultAsync(m => m.Id == id.Value);

        if (menuItem == null) return NotFound();

        return View(menuItem);
    }

    // POST: /MenuItems/Delete/10
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var menuItem = await _db.MenuItems.FindAsync(id);
        if (menuItem == null) return RedirectToAction("Index", "Restaurants");

        var restaurantId = menuItem.RestaurantId;

        _db.MenuItems.Remove(menuItem);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Restaurants", new { id = restaurantId });
    }
    
    // GET: /MenuItems/Details/10
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var menuItem = await _db.MenuItems
            .Include(m => m.Restaurant)
            .FirstOrDefaultAsync(m => m.Id == id.Value);

        if (menuItem == null) return NotFound();

        var reviewsQuery = _db.MenuItemReviews.Where(r => r.MenuItemId == menuItem.Id);

        var vm = new MenuItemDetailsVm
        {
            MenuItem = menuItem,
            AvgRating = await reviewsQuery.Select(r => (double?)r.Rating).AverageAsync() ?? 0,
            ReviewCount = await reviewsQuery.CountAsync(),
            Reviews = await reviewsQuery
                .OrderByDescending(r => r.CreatedAtUtc)
                .ToListAsync()
        };

        return View(vm);
    }
}
