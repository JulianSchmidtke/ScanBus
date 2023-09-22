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
public class HelloWorld
{
    private readonly ILogger<HelloWorld> _logger;

    private static List<BusNotice> busNotices = new();
    private static List<ProcessedNotice> processedNotices = new();

    public HelloWorld(ILogger<HelloWorld> log)
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
        var res = predictionHelper.PredictImage(busNotice.Base64Image);

        foreach(var pred in res)
        {
            pred.Base64CroppedImage = PredictionHelper.CropImage(busNotice.Base64Image, pred.Left, pred.Top, pred.Width, pred.Height);
        }

        var anchorPoint = GetAnchorPoint(busNotice.Base64Image);
        (var car, var licensePlate) = GetClosestPredictions(res, anchorPoint.Item1, anchorPoint.Item2);

        
        Address address = await CoordService.GetAddress(busNotice.Longitude, busNotice.Latitude);
        ProcessedNotice processedNotice = new()
        {
            Address = address,
            Base64Image = busNotice.Base64Image,
            Base64ImageCar = car.Base64CroppedImage,
            Base64ImageLicensePlate = licensePlate.Base64CroppedImage,
            Longitude = busNotice.Longitude,
            Latitude = busNotice.Latitude,
            Driver = busNotice.Driver,
            BusId = busNotice.BusId
        };

        
        //Todo: OCR
        //Todo: Auto
        
        processedNotices.Add(processedNotice);
    }

    public (Prediction, Prediction) GetClosestPredictions(List<Prediction> predictions, int x, int y)
    {
        Vector2 anchor = new (x, y);
        predictions.Sort((x,y) => 
        {
            var distanceOne = Vector2.Distance(new Vector2((float)(x.Left + x.Width/2), (float)(x.Top + x.Height / 2)), anchor);
            var distanceTwo = Vector2.Distance(new Vector2((float)(y.Left + y.Width/2), (float)(y.Top + y.Height / 2)), anchor);
            return distanceOne > distanceTwo ? 1 : (distanceOne < distanceTwo ? - 1 : 0);
        });
        return (predictions.Where(x => x.Tag.Equals("Car", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault(), predictions.Where(x => x.Tag.Equals("Registration Number", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault());
    }

    public (int, int) GetAnchorPoint(string Base64Image)
    {
        using var stream = new MemoryStream(Convert.FromBase64String(Base64Image));
        using var outStream = new MemoryStream();
        using (var image = Image.Load(stream))
        {
            return ((int)(image.Bounds.Width * 0.75), (int)(image.Bounds.Height * 0.75));
        }
    }

}