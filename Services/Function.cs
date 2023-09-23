using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ScanBus.Models;
using ScanBus.Service;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ScanBus.Function;
public class Function
{
    private readonly ILogger<Function> _logger;

    private static List<BusNotice> busNotices = new();
    private static List<ProcessedNotice> processedNotices = new();

    public Function(ILogger<Function> log)
    {
        _logger = log;
    }

            
    [FunctionName("PostBusNotice")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "ScanBus" })]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(BusNotice), Description = "BusNotice", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> PostBusNotice(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "busNotices")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var busNotice = JsonConvert.DeserializeObject<BusNotice>(requestBody);
        busNotices.Add(busNotice);

        _ = Task.Run(async () => await processNotice(busNotice));
        
        return new OkResult();
    }

    [FunctionName("GetProcessedNotices")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "ScanBus" })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProcessedNotice), Description = "The OK response")]
    public async Task<IActionResult> GetProcessedNotices(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "processedNotices")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        
        return new OkObjectResult(processedNotices);
    }

    [FunctionName("GetBusNotices")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "ScanBus" })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(BusNotice), Description = "The OK response")]
    public async Task<IActionResult> GetBusNotices(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "busNotices")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        
        return new OkObjectResult(busNotices);
    }


    private async Task processNotice(BusNotice busNotice)
    {

        PredictionHelper predictionHelper = new();
        var predictions = predictionHelper.PredictImage(busNotice.Base64Image);

        (var carImage, var licensePlateImage) = PredictionHelper.GetCroppedImages(busNotice.Base64Image, predictions);
        
        Address address = await CoordService.GetAddress(busNotice.Longitude, busNotice.Latitude);
        
        //Todo: OCR
        var licensePlateText = OCRHelper.AnalyzeImage(licensePlateImage);
        //Todo: Auto

        ProcessedNotice processedNotice = new()
        {
            Address = address,
            Base64Image = busNotice.Base64Image,
            Base64ImageCar = carImage,
            Base64ImageLicensePlate = licensePlateImage,
            Longitude = busNotice.Longitude,
            Latitude = busNotice.Latitude,
            Driver = busNotice.Driver,
            BusId = busNotice.BusId
        };
        processedNotices.Add(processedNotice);
    }


}