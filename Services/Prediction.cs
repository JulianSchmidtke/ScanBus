using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace ScanBus.Function;

public class Prediction
{
    public string Tag {get; set; }
    public double Probability { get; set; }
    public double Top { get; set; }
    public double Left { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
    public string Base64CroppedImage { get; set; }
}