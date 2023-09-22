
namespace ScanBus.Function
{
    public class ProcessedNotice{

        public string Base64Image { get; set;}
        public string ImageContentType {get; set; }
        public long Latitude {get; set;}
        public long Longitude {get; set;}
        public string Driver {get; set;}
        public string BusId {get; set; }
    }
}