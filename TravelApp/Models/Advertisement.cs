using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Models
{
    public class Advertisement
    {
        [Key]
        public int AdID { get; set; } // Primary Key
        [ForeignKey("Company")]
        public int CompanyID { get; set; } // Foreign Key
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public decimal price { get; set; }
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string TravelTo { get; set; } 
        public string TravelFrom { get; set; }

        // Navigation Property
        public Company Company { get; set; } // Many-to-one with Company
        public ICollection<Booking> Bookings { get; set; } // One-to-many with Booking
        public ICollection<Review> Reviews { get; set; } // One-to-many with Review
        public ICollection<AdvertisementImage> Images { get; set; } // One-to-many with AdvertisementImage
        public ICollection<Wishlist> Wishlists { get; set; }
    }
}
