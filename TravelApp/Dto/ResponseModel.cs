namespace TravelApp.Dto
{
    public class ResponseModel<T>
    {
        public List<string>? Errors { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; } = true;
        public T? Data { get; set; }
    }
}
