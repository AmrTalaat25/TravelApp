namespace TravelApp.Dto
{
    public class AdvertisementReviewsDTO
    {
        public ICollection<ReviewDTO> Reviews { get; set; }
    }
    public class AdvertisementDTO
    {
        public int AdID { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public decimal price { get; set; }
        public decimal AverageRating { get; set; }
        public decimal ReviewCount { get; set; }
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string TravelTo { get; set; }
        public string TravelFrom { get; set; }
        public List<string>? Base64Images { get; set; }
    }

    public class CreateAdvertisementDTO
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public decimal price { get; set; }
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string TravelTo { get; set; }
        public string TravelFrom { get; set; }

    }

    public class UpdateAdvertisementDTO
    {
        //public int AdID { get; set; }
        public string Title { get; set; }
        public decimal price { get; set; }
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string TravelTo { get; set;}
        public string TravelFrom { get; set;}
        
    }
}
