using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace ScanBus.Function
{
    public class HelloWorld
    {
        private readonly ILogger<HelloWorld> _logger;

        private static List<BusNotice> busNotices = new ();
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
    }
}

