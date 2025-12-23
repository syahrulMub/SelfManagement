using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace DatabaseServices
{
    public class RatingServices : IRatingServices
    {
        private readonly DatabaseContext _dbContext;

        public RatingServices(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Rating>> GetRatingsAsync()
        {
            return await _dbContext.Ratings
                            .Include(r => r.Order)
                            .ToListAsync();
        }

        public async Task<Rating?> GetRatingByIdAsync(int ratingId)
        {
            return await _dbContext.Ratings
                    .Include(r => r.Order)
                    .Where(r => r.RatingId == ratingId)
                    .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Rating>> GetRatingsByOrderIdAsync(int orderId)
        {
            return await _dbContext.Ratings
                    .Include(r => r.Order)
                    .Where(r => r.OrderId == orderId)
                    .ToListAsync();
        }

        public async Task<bool> CreateRatingAsync(Rating rating)
        {
            _dbContext.Ratings.Add(rating);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRatingAsync(int ratingId, Rating rating)
        {
            var existingRating = await _dbContext.Ratings.FirstOrDefaultAsync(r => r.RatingId == ratingId);

            if (existingRating != null)
            {
                existingRating.OrderId = rating.OrderId;
                existingRating.Score = rating.Score;
                existingRating.Comment = rating.Comment;
                _dbContext.Ratings.Update(existingRating);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteRatingAsync(int ratingId)
        {
            var rating = await _dbContext.Ratings.FirstOrDefaultAsync(r => r.RatingId == ratingId);

            if (rating != null)
            {
                _dbContext.Ratings.Remove(rating);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
