namespace simplePayloadWebApi.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int HorsePower { get; set; }
        public double ZeroTo100 { get; set; }
        public int CarUploadId { get; set; }
        public CarUpload CarUpload { get; set; }
    }

}
