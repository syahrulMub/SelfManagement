using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class RatingController : Controller
{
    private readonly IRatingServices _ratingServices;
    private readonly ILogger<RatingController> _logger;

    public RatingController(IRatingServices services, ILogger<RatingController> logger)
    {
        _ratingServices = services;
        _logger = logger;
    }

    [HttpGet("/Rating/Ratings")]
    public async Task<IActionResult> GetRatings()
    {
        try
        {
            var ratings = await _ratingServices.GetRatingsAsync();
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ratings");
            return View("Error");
        }
    }

    [HttpGet("/Rating/GetRating/{ratingId}")]
    public async Task<IActionResult> GetRating(int ratingId)
    {
        try
        {
            var rating = await _ratingServices.GetRatingByIdAsync(ratingId);
            if (rating == null)
            {
                _logger.LogError("Rating not found");
                return NotFound();
            }
            _logger.LogInformation("Successfully retrieved rating data");
            return Ok(rating);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rating");
            return View("Error");
        }
    }

    [HttpGet("/Rating/GetRatingsByOrder/{orderId}")]
    public async Task<IActionResult> GetRatingsByOrder(int orderId)
    {
        try
        {
            var ratings = await _ratingServices.GetRatingsByOrderIdAsync(orderId);
            _logger.LogInformation("Successfully retrieved ratings for order");
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving ratings by order");
            return View("Error");
        }
    }

    [HttpPost("/Rating/CreateRating")]
    public async Task<IActionResult> CreateRating(Rating rating)
    {
        try
        {
            var success = await _ratingServices.CreateRatingAsync(rating);
            _logger.LogInformation("Successfully created rating");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rating");
            return View("Error");
        }
    }

    [HttpPost("/Rating/UpdateRating/{ratingId}")]
    public async Task<IActionResult> UpdateRating(int ratingId, Rating rating)
    {
        try
        {
            await _ratingServices.UpdateRatingAsync(ratingId, rating);
            _logger.LogInformation("Successfully updated rating");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating rating");
            return View("Error");
        }
    }

    [HttpDelete("/Rating/DeleteRating/{ratingId}")]
    public async Task<ActionResult> DeleteRating(int ratingId)
    {
        try
        {
            await _ratingServices.DeleteRatingAsync(ratingId);
            _logger.LogInformation("Successfully deleted rating");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting rating");
            return View("Error");
        }
    }
}
