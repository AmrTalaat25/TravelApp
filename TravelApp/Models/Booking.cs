using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; } // Primary Key
        [ForeignKey("Advertisement")]
        public int AdID { get; set; } // Foreign Key
        [ForeignKey("User")]
        public string UserID { get; set; } // Foreign Key
        public int NumberOfSeats { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } // e.g., Credit Card, PayPal, etc.
        public string PaymentDetails { get; set; }

        // Navigation Properties
        public Advertisement Advertisement { get; set; } // Many-to-one with Advertisement
        public User User { get; set; } // Many-to-one with User
        public Payment Payment { get; set; } // One-to-one with Payment


    }
}
