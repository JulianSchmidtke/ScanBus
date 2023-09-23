
using ScanBus.Models;

namespace ScanBus.Function;
public class ProcessedNotice
{
    public int Id { get; set; }
    public string BusId { get; set; }
    public string Driver { get; set; }
    public Address Address { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string LicensePlate { get; set; }
    public string Base64Image { get; set; }
    public string Base64ImageCar { get; set; }
    public string Base64ImageLicensePlate { get; set; }
    public string ImageContentType { get; set; }
}