using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelApp.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; } // Primary Key
        [ForeignKey("Booking")]
        public int BookingID { get; set; } // Foreign Key
        public string PaymentMethod { get; set; } // e.g., Credit Card, PayPal, etc.
        public string PaymentDetails { get; set; } // Specific payment details (e.g., credit card number, PayPal email, etc.)

        // Navigation Property to Booking
        public Booking Booking { get; set; } // One-to-one with Booking
    }
}
