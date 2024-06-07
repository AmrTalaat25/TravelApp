namespace TravelApp.Dto
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDetails { get; set; }
    }
    public class CreatePaymentDTO
    {
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDetails { get; set; }
    }

    public class UpdatePaymentDTO
    {
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDetails { get; set; }

    }

}
