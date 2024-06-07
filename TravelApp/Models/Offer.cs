using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Models
{
    public class Offer
    {
        public int OfferID { get; set; }
        public string OfferName { get; set; }
        public string details { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime post_date { get; set; }
        public DateTime expiry_date { get; set; }

        [ForeignKey("Advertisement")]
        public int AdID { get; set; } // Foreign Key

        // Navigation Property
        public Advertisement Advertisement { get; set; }

    }
}
