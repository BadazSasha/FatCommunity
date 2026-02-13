using FatCommunity.Data;
using FatCommunity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FatCommunity.Controllers;

public class RestaurantReviewsController : Controller
{
    private readonly AppDbContext _db;

    public RestaurantReviewsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /RestaurantReviews/Create?restaurantId=5
    public async Task<IActionResult> Create(int restaurantId)
    {
        var restaurant = await _db.Restaurants.FindAsync(restaurantId);
        if (restaurant == null) return NotFound();

        ViewBag.RestaurantName = restaurant.Name;

        return View(new RestaurantReview
        {
            RestaurantId = restaurantId,
            Rating = 5
        });
    }

    // POST: /RestaurantReviews/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RestaurantReview review)
    {
        // Prevent navigation-property validation issues
        ModelState.Remove("Restaurant");

        var restaurant = await _db.Restaurants.FindAsync(review.RestaurantId);
        if (restaurant == null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.RestaurantName = restaurant.Name;
            return View(review);
        }

        review.CreatedAtUtc = DateTime.UtcNow;

        _db.RestaurantReviews.Add(review);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Restaurants", new { id = review.RestaurantId });
    }

    // (Optional but useful) GET: /RestaurantReviews/Delete/10
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var review = await _db.RestaurantReviews
            .Include(r => r.Restaurant)
            .FirstOrDefaultAsync(r => r.Id == id.Value);

        if (review == null) return NotFound();

        return View(review);
    }

    // POST: /RestaurantReviews/Delete/10
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var review = await _db.RestaurantReviews.FindAsync(id);
        if (review == null) return RedirectToAction("Index", "Restaurants");

        var restaurantId = review.RestaurantId;

        _db.RestaurantReviews.Remove(review);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Restaurants", new { id = restaurantId });
    }
}
