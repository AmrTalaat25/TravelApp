using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TravelApp.Models.Refresh_Token;

namespace TravelApp.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }

        // Navigation Properties
        public ICollection<Company> Companies { get; set; } // One-to-many with Company

        public ICollection<PermissionRequest> PermissionRequests { get; set; } // One-to-many with PermissionRequest
        public ICollection<Review> Reviews { get; set; } // One-to-many with Review
        public ICollection<Booking> Bookings { get; set; } // One-to-many with Booking
        public ICollection<Wishlist> Wishlists { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
