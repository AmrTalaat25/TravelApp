using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelApp.Dto;
using TravelApp.Models;
using TravelApp.Models.Services.Interfaces;

namespace TravelApp.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IReviewService _service;

        public ReviewsController(ApplicationDbContext context, IReviewService service)
        {
            _context = context;
            _service = service;
        }

        // GET: api/reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetAllReviews()
        {
            return await _service.GetALLReviews();  
        }

        // GET: api/reviews/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDTO>> GetReview(int id)
        {
            return await _service.GetByIdReview(id);
        }

        // POST: api/reviews
        [HttpPost]
        public async Task<ActionResult<ReviewDTO>> CreateReview(CreateReviewDTO createReviewDTO)
        {
            return await _service.CreateReview(createReviewDTO);
        }

        // PUT: api/reviews/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateReview(int id, UpdateReviewDTO updateReviewDTO)
        {
            return await _service.UpdateReview(id, updateReviewDTO);
        }

        // DELETE: api/reviews/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteReview(int id)
        {
            return await _service.DeleteReview(id);
        }
    }
}
