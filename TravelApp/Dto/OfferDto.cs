namespace TravelApp.Dto
{
    public class OfferDTO
    {
        public int OfferID { get; set; }
        public string OfferName { get; set; }
        public string Details { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int AdID { get; set; }
    }

    public class CreateOfferDTO
    {
        public string OfferName { get; set; }
        public string Details { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int AdID { get; set; }
    }

    public class UpdateOfferDTO
    {
        public string OfferName { get; set; }
        public string Details { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

}
