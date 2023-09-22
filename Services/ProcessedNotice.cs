
using ScanBus.Models;

namespace ScanBus.Function;
public class ProcessedNotice
{

    public string Base64Image { get; set; }
    public string ImageContentType { get; set; }
    public Address Address { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string Driver { get; set; }
    public string BusId { get; set; }
}