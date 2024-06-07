using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApp.Dto;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Models.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetALLReviews()
        {
            var reviews = await _context.Reviews
                .Select(r => new ReviewDTO
                {
                    ReviewID = r.ReviewID,
                    AdID = r.AdID,
                    //UserID = r.UserID,
                    UserName=r.Username,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    DatePosted = r.DatePosted
                })
                .ToListAsync();

            return reviews;
        }
        public async Task<ActionResult<ReviewDTO>> GetByIdReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                throw new Exception("This Review Not Found");
            }

            var reviewDTO = new ReviewDTO
            {
                ReviewID = review.ReviewID,
                AdID = review.AdID,
                //UserID = review.UserID,
                UserName=review.Username,
                Rating = review.Rating,
                Comment = review.Comment,
                DatePosted = review.DatePosted
            };

            return reviewDTO;
        }
        public async Task<ActionResult<ReviewDTO>> CreateReview(CreateReviewDTO createReviewDTO)
        {
            var review = new Review
            {
                AdID = createReviewDTO.AdID,
                UserID = createReviewDTO.UserID,
                Username=createReviewDTO.Username,
                Rating = createReviewDTO.Rating,
                Comment = createReviewDTO.Comment,
                DatePosted = createReviewDTO.DatePosted
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var reviewDTO = new ReviewDTO
            {
                ReviewID = review.ReviewID,
                AdID = review.AdID,
                //UserID = review.UserID,
                UserName = review.Username,
                Rating = review.Rating,
                Comment = review.Comment,
                DatePosted = review.DatePosted
            };

            return reviewDTO;
        }
        public async Task<ActionResult<string>> UpdateReview(int id, UpdateReviewDTO updateReviewDTO)
        {
            var existingReview = _context.Set<Review>().Find(id);
            if (existingReview != null)
            {
                existingReview.Rating = updateReviewDTO.Rating;
                existingReview.Comment = updateReviewDTO.Comment;
                await _context.SaveChangesAsync();
                return "Review is Updated";
                 
            }
            return "This Review Not Found";


        }
        public async Task<ActionResult<string>> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
            return "This Review Not Found";
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return "Review is Deleted";
        }
    }
}
