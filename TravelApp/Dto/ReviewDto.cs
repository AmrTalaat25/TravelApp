namespace TravelApp.Dto
{
    public class ReviewDTO
    {
        public int ReviewID { get; set; }
        public int AdID { get; set; }
        //public int UserID { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime DatePosted { get; set; }
    }

    public class CreateReviewDTO
    {
        public int AdID { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime DatePosted { get; set; }
    }

    public class UpdateReviewDTO
    {
        //public int ReviewID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
