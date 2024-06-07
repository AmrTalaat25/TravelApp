using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelApp.Dto;
using TravelApp.Models;

namespace TravelApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/{userId}")]
        public async Task<IActionResult> GetUserWishlist(string userId)
        {
            var wishlist = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .ToListAsync();
            return Ok(new ResponseModel<IEnumerable<Wishlist>>
            {
                Message = "Retrieved wishlist successfully",
                Data = wishlist
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserWishlist()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")!.Value;
            var wishlist = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .ToListAsync();
            return Ok(new ResponseModel<IEnumerable<Wishlist>>
            {
                Message = "Retrieved wishlist successfully",
                Data = wishlist
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWishlist(int AdId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")!.Value;
            var existingWishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.AdId == AdId);

            if (existingWishlistItem != null)
            {
                return BadRequest("Advertisement already exists in user's wishlist.");
            }

            var wishlist = new Wishlist
            {
                UserId = userId,
                AdId = AdId
            };

            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();

            return Ok(new ResponseModel<Wishlist>
            {
                Message = "Added wishlist item successfully",
                Data = wishlist
            });
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveFromWishlist(int AdId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")!.Value;
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.AdId == AdId);

            if (wishlist == null)
            {
                return NotFound("Failed to find wishlist item");
            }

            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();

            return Ok(new ResponseModel<Wishlist>
            {
                Message = "Advertisement removed from wishlist.",
                Data = wishlist
            });
        }
    }
}
