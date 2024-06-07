namespace TravelApp.Dto
{
    public class BookingDTO
    {
        public int BookingID { get; set; }
        public int AdID { get; set; }
        public string UserID { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfSeats { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } // e.g., Credit Card, PayPal, etc.
        public string PaymentDetails { get; set; }

    }

    public class CreateBookingDTO
    {
        public int AdID { get; set; }
        public string UserID { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfSeats { get; set; }
        public string Status { get; set; }
        //public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } // e.g., Credit Card, PayPal, etc.
        public string PaymentDetails { get; set; }

    }

    public class UpdateBookingDTO
    {
        public int NumberOfSeats { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; } // e.g., Credit Card, PayPal, etc.
        public string PaymentDetails { get; set; }
    }
}
