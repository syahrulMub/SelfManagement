using MyPrivateManager.Models;
namespace MyPrivateManager.IDatabaseServices;

public interface IRatingServices
{
    Task<IEnumerable<Rating>> GetRatingsAsync();
    Task<Rating?> GetRatingByIdAsync(int ratingId);
    Task<IEnumerable<Rating>> GetRatingsByOrderIdAsync(int orderId);
    Task<bool> CreateRatingAsync(Rating rating);
    Task<bool> UpdateRatingAsync(int ratingId, Rating rating);
    Task<bool> DeleteRatingAsync(int ratingId);
}
