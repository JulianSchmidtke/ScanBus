using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace ScanBus.Function;

public class OCRHelper
{
    private static readonly string visionEndpoint = "https://vision-scanbus.cognitiveservices.azure.com/";
    private static readonly string visionKey = "7edf75ebe3764cc29a349ad9e9cba8a3";

    public static async Task<string> AnalyzeImage(string image)
    {
        ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(visionKey))
              { Endpoint = visionEndpoint };
        
        using var stream = new MemoryStream(Convert.FromBase64String(image));
        var textHeaders = await client.ReadInStreamAsync(stream);


        string operationLocation = textHeaders.OperationLocation;
        Thread.Sleep(2000);

        // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
        // We only need the ID and not the full URL
        const int numberOfCharsInOperationId = 36;
        string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

        // Extract the text
        ReadOperationResult results;
        do
        {
            results = await client.GetReadResultAsync(Guid.Parse(operationId));
        }
        while ((results.Status == OperationStatusCodes.Running ||
            results.Status == OperationStatusCodes.NotStarted));

        // Display the found text.

        var text = "";
        var textUrlFileResults = results.AnalyzeResult.ReadResults;
        foreach (ReadResult page in textUrlFileResults)
        {
            foreach (Line line in page.Lines)
            {
                text = text+line.Text;
            }
        }
        return text.Trim('&');
    }
}