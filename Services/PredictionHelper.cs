using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ScanBus.Function;

public class PredictionHelper{

    private static readonly string Endpoint = "https://scanbus-prediction.cognitiveservices.azure.com";
    private static readonly string PredictionKey = "8122633cfddc4b99a6a2fac83fa42097";
    private static readonly Guid ProjectId =  Guid.Parse("6243e34d-217b-4d21-b28b-8bfeff52c37c");
    private static readonly string PublishedModelName = "Iteration1";

    private readonly CustomVisionPredictionClient predictionApi;


    public PredictionHelper() : this(Endpoint, PredictionKey){}

    public PredictionHelper(string endpoint, string predictionKey)
    {
        predictionApi = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(predictionKey))
        {
            Endpoint = endpoint
        };
    }

    public List<Prediction> PredictImage(string Base64Image)
    {
        using MemoryStream stream = new MemoryStream(Convert.FromBase64String(Base64Image));
        var result = predictionApi.DetectImage(ProjectId, PublishedModelName, stream);
        
        List<Prediction> predictions = new ();
        
        foreach(var c in result.Predictions)
        {
            if(c.Probability >= 0.9){
                predictions.Add(new Prediction(){
                    Tag = c.TagName, 
                    Probability = c.Probability,
                    Top = c.BoundingBox.Top,
                    Height = c.BoundingBox.Height,
                    Left = c.BoundingBox.Left,
                    Width = c.BoundingBox.Width
                    });
            }
        }
        return predictions;
    }

    public static string CropImage(string Base64Image, double x, double y, double width, double height)
    {
        using var stream = new MemoryStream(Convert.FromBase64String(Base64Image));
        using var outStream = new MemoryStream();
        using (var image = Image.Load(stream))
        {
            image.Mutate(i => i.Crop(new Rectangle(){X = (int)(image.Bounds.Width * x), Y = (int)(image.Bounds.Height * y), Height = (int)(image.Bounds.Height * height), Width = (int)(image.Bounds.Width * width)}));
            image.Save(outStream, new JpegEncoder());
        }
        return Convert.ToBase64String(outStream.ToArray());
    }
}
