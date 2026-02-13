using FatCommunity.Data;
using FatCommunity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FatCommunity.Controllers;

public class MenuItemReviewsController : Controller
{
    private readonly AppDbContext _db;

    public MenuItemReviewsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /MenuItemReviews/Create?menuItemId=10
    public async Task<IActionResult> Create(int menuItemId)
    {
        var item = await _db.MenuItems
            .Include(m => m.Restaurant)
            .FirstOrDefaultAsync(m => m.Id == menuItemId);

        if (item == null) return NotFound();

        ViewBag.MenuItemName = item.Name;
        ViewBag.RestaurantName = item.Restaurant.Name;

        return View(new MenuItemReview
        {
            MenuItemId = menuItemId,
            Rating = 5
        });
    }

    // POST: /MenuItemReviews/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuItemReview review)
    {
        // Prevent navigation-property validation issues
        ModelState.Remove("MenuItem");

        var item = await _db.MenuItems.FindAsync(review.MenuItemId);
        if (item == null) return NotFound();

        if (!ModelState.IsValid)
        {
            var itemWithRestaurant = await _db.MenuItems.Include(m => m.Restaurant)
                .FirstAsync(m => m.Id == review.MenuItemId);

            ViewBag.MenuItemName = itemWithRestaurant.Name;
            ViewBag.RestaurantName = itemWithRestaurant.Restaurant.Name;

            return View(review);
        }

        review.CreatedAtUtc = DateTime.UtcNow;

        _db.MenuItemReviews.Add(review);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "MenuItems", new { id = review.MenuItemId });
    }

    // Optional: delete
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var review = await _db.MenuItemReviews
            .Include(r => r.MenuItem)
            .FirstOrDefaultAsync(r => r.Id == id.Value);

        if (review == null) return NotFound();

        return View(review);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var review = await _db.MenuItemReviews.FindAsync(id);
        if (review == null) return RedirectToAction("Index", "Restaurants");

        var menuItemId = review.MenuItemId;

        _db.MenuItemReviews.Remove(review);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "MenuItems", new { id = menuItemId });
    }
}
