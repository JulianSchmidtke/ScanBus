using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ScanBus.Models;
using ScanBus.Service;

namespace ScanBus.Function;
public class Function
{
    private readonly ILogger<Function> _logger;

    private static List<ProcessedNotice> notices = new();

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

        ProcessedNotice processedNotice = new()
        {
            Id = notices.Count(),
            Base64Image = busNotice.Base64Image,
            Longitude = busNotice.Longitude,
            Latitude = busNotice.Latitude,
            Driver = busNotice.Driver,
            BusId = busNotice.BusId
        };
        notices.Add(processedNotice);

        _ = Task.Run(async () => await ProcessNotice(processedNotice));

        return new OkResult();
    }

    [FunctionName("GetProcessedNotices")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "ScanBus" })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProcessedNotice), Description = "The OK response")]
    public IActionResult GetProcessedNotices(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "processedNotices")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        return new OkObjectResult(notices);
    }

    [FunctionName("GetProcessedNotice")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "ScanBus" })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProcessedNotice), Description = "The OK response")]
    public IActionResult GetProcessedNotice(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "processedNotice/{id}")] HttpRequest req, int id)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var notice = notices.Where(n => n.Id == id).FirstOrDefault();

        return new OkObjectResult(notice);
    }

    private async Task ProcessNotice(ProcessedNotice notice)
    {
        await ProcessGeoInformation(notice);
        await ProcessImage(notice);
    }

    private async Task ProcessGeoInformation(ProcessedNotice notice)
    {
        Thread.Sleep(2000);
        Address address = await CoordService.GetAddress(notice.Longitude, notice.Latitude);
        notice.Address = address;
    }

    private async Task ProcessImage(ProcessedNotice notice)
    {
        PredictionHelper predictionHelper = new();
        var predictions = predictionHelper.PredictImage(notice.Base64Image);

        Thread.Sleep(5000);
        (var carImage, var licensePlateImage) = PredictionHelper.GetCroppedImages(notice.Base64Image, predictions);
        notice.Base64ImageCar = carImage;
        notice.Base64ImageLicensePlate = licensePlateImage;

        Thread.Sleep(9000);
        string licensePlateText = await OCRHelper.AnalyzeImage(licensePlateImage);
        notice.LicensePlate = licensePlateText;
    }
}