using Microsoft.AspNetCore.Mvc;
using TravelApp.Dto;

namespace TravelApp.Models.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ActionResult<ReviewDTO>> CreateReview(CreateReviewDTO createReviewDTO);
        Task<ActionResult<string>> DeleteReview(int id);
        Task<ActionResult<IEnumerable<ReviewDTO>>> GetALLReviews();
        Task<ActionResult<ReviewDTO>> GetByIdReview(int id);
        Task<ActionResult<string>> UpdateReview(int id, UpdateReviewDTO updateReviewDTO);
    }
}