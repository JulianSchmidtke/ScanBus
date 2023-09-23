using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ScanBus.Function;

public class PredictionHelper
{

    private static readonly string Endpoint = "https://scanbus-prediction.cognitiveservices.azure.com";
    private static readonly string PredictionKey = "8122633cfddc4b99a6a2fac83fa42097";
    private static readonly Guid ProjectId = Guid.Parse("6243e34d-217b-4d21-b28b-8bfeff52c37c");
    private static readonly string PublishedModelName = "Iteration1";

    private readonly CustomVisionPredictionClient predictionApi;


    public PredictionHelper() : this(Endpoint, PredictionKey) { }

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

        List<Prediction> predictions = new();

        foreach (var c in result.Predictions)
        {
            if (c.Probability >= 0.9)
            {
                predictions.Add(new Prediction()
                {
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
            image.Mutate(i => i.Crop(new Rectangle()
            {
                X = (int)(image.Bounds.Width * x),
                Y = (int)(image.Bounds.Height * y),
                Height = Math.Max(50, (int)(image.Bounds.Height * height)),
                Width = Math.Max(50, (int)(image.Bounds.Width * width))
            }));
            image.Save(outStream, new JpegEncoder());
        }
        return Convert.ToBase64String(outStream.ToArray());
    }

    public static (Prediction, Prediction) GetClosestPredictions(List<Prediction> predictions, int x, int y)
    {
        Vector2 anchor = new(x, y);
        predictions.Sort((x, y) =>
        {
            var distanceOne = Vector2.Distance(new Vector2((float)(x.Left + x.Width / 2), (float)(x.Top + x.Height / 2)), anchor);
            var distanceTwo = Vector2.Distance(new Vector2((float)(y.Left + y.Width / 2), (float)(y.Top + y.Height / 2)), anchor);
            return distanceOne > distanceTwo ? 1 : (distanceOne < distanceTwo ? -1 : 0);
        });
        return (predictions.Where(x => x.Tag.Equals("Car", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault(), predictions.Where(x => x.Tag.Equals("Registration Number", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault());
    }

    public static (int, int) GetAnchorPoint(string Base64Image)
    {
        using var stream = new MemoryStream(Convert.FromBase64String(Base64Image));
        using var outStream = new MemoryStream();
        using (var image = Image.Load(stream))
        {
            return ((int)(image.Bounds.Width * 0.75), (int)(image.Bounds.Height * 0.75));
        }
    }

    public static (string, string) GetCroppedImages(string Base64Image, List<Prediction> predictions)
    {
        foreach (var pred in predictions)
        {
            pred.Base64CroppedImage = PredictionHelper.CropImage(Base64Image, pred.Left, pred.Top, pred.Width, pred.Height);
        }

        var anchorPoint = PredictionHelper.GetAnchorPoint(Base64Image);
        (var car, var licensePlate) = PredictionHelper.GetClosestPredictions(predictions, anchorPoint.Item1, anchorPoint.Item2);
        return (car.Base64CroppedImage, licensePlate.Base64CroppedImage);
    }


    public static Rgba32 GetMostCommonColor(Image<Rgba32> image)
    {
        Dictionary<Rgba32, int> colorMap = new Dictionary<Rgba32, int>();

        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                Rgba32 pixelColor = image[x, y];

                // Entfernt die Alpha-Komponente, um nur RGB zu betrachten.
                Rgba32 noAlphaColor = new Rgba32(pixelColor.R, pixelColor.G, pixelColor.B);

                if (colorMap.ContainsKey(noAlphaColor))
                {
                    colorMap[noAlphaColor]++;
                }
                else
                {
                    colorMap.Add(noAlphaColor, 1);
                }
            }
        }

        Rgba32 mostCommonColor = default;
        int maxOccurrences = 0;

        foreach (var pair in colorMap)
        {
            if (pair.Value > maxOccurrences)
            {
                maxOccurrences = pair.Value;
                mostCommonColor = pair.Key;
            }
        }

        return mostCommonColor;
    }

    public static string HexToPrimaryColor(Rgba32 color)
    {
        // Für Schwarz und Weiß
        if (color.R == 255 && color.G == 255 && color.B == 255 || Math.Abs(255 - color.R + 255 - color.G + 255 - color.B) < 20)
            return "White";
        if (color.R == 0 && color.G == 0 && color.B == 0 || Math.Abs(color.R - color.G - color.B) < 20)
            return "Black";

        // Prüfen, welche Farbkomponente dominiert
        byte maxComponent = Math.Max(color.R, Math.Max(color.G, color.B));
        byte minComponent = Math.Min(color.R, Math.Min(color.G, color.B));

        // Für Lila (Rot und Blau sind dominant)
        if (color.R == maxComponent && color.B == maxComponent && color.G == minComponent)
            return "Purple";

        // Für Rot, Grün oder Blau
        if (color.R == maxComponent)
            return "Red";
        if (color.G == maxComponent)
            return "Green";
        if (color.B == maxComponent)
            return "Blue";

        return "Unknown"; // Falls keine eindeutige Hauptfarbe erkannt wird
    }

}
