namespace simplePayloadWebApi.Models
{
    public class CarUpload
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; }
        public string Status { get; set; } = "Pending";
        public List<Car> Cars { get; set; } = new();
    }

}
