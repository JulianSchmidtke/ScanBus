using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.WebJobs.Script.Description;

namespace ScanBus.Function;

public class PredictionHelper{

    private static readonly string Endpoint = "https://scanbus.cognitiveservices.azure.com/";
    private static readonly string PredictionKey = "438cbb64702d461ebe1cf91c3ee42e4d";
    private static readonly Guid ProjectId =  Guid.Parse("5ed0919b-f827-4af8-b708-beeb80072ec1");
    private static readonly string PublishedName = "ScanBus";

    private readonly CustomVisionPredictionClient predictionApi;


    public PredictionHelper() : this(Endpoint, PredictionKey){}

    public PredictionHelper(string endpoint, string predictionKey)
    {
        predictionApi = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(predictionKey))
        {
            Endpoint = endpoint
        };
    }

    public Dictionary<string, double> PredictImage(string Base64Image)
    {
        using MemoryStream stream = new MemoryStream(Convert.FromBase64String(Base64Image));
        
        var result = predictionApi.ClassifyImage(ProjectId, PublishedName, stream);
        
        Dictionary<string, double> predictions = new ();
        
        foreach(var c in result.Predictions)
        {
            predictions.Add(c.TagName, c.Probability);
        }
        return predictions;
    }
}
