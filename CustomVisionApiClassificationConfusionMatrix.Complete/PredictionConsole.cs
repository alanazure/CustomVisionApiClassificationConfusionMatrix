using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CustomVisionApiClassificationConfusionMatrix.Complete
{
    class PredictionConsole
    {
        // Set your Custom Vision service details here...
        static string Endpoint = " ";
        static string PredictionKey = " ";

        // Set your Custom Vision project details here...

        // Fast Cars
        // Set your Custom Vision project details here...
        static string ProjetId = "";
        static string PublishedName = "";

        // Set the paths to your test images and output prediction folder here...
        static string TestImageFolder = @"C:\AIData\Cars\Test";
        static string PredictionImageFolder = @"C:\AIData\Cars\Prediction";



        static async Task Main(string[] args)
        {
            Console.WriteLine("Custom Vision API - Classification - ConfusionMatrix");

            var confusionMatrix = await PredictTestImageFolderAsync();
            Console.WriteLine();
            confusionMatrix.WriteToConsole();
            Console.WriteLine();

        }




        static async Task<ConfusionMatrix> PredictTestImageFolderAsync()
        {
            var classes = new List<string>();

            // Iterate through the test folders
            var testImageFolders = Directory.EnumerateDirectories(TestImageFolder);

            // Create the prediction folders
            foreach (var testFolder in testImageFolders)
            {
                var testTagName = Path.GetFileName(testFolder);
                classes.Add(testTagName);
                var predictionsFolder = Path.Combine(PredictionImageFolder, testTagName);
                if (!Directory.Exists(predictionsFolder))
                {
                    Directory.CreateDirectory(predictionsFolder);
                }
            }


            var theMatrix = new ConfusionMatrix(classes);


            foreach (var testFolder in testImageFolders)
            {

                // Get the name of the tag
                var testTagName = Path.GetFileName(testFolder);
                Console.WriteLine($"Processing { testTagName }...");

                // Iterate through the test images
                var testImages = Directory.GetFiles(testFolder);
                foreach (var testImage in testImages)
                {
                    var testImageFileName = Path.GetFileName(testImage);

                    // Get the top prediction
                    var predictions = await GetImagePredictionsAsync(testImage);
                    var topPredicton = 
                        predictions.Predictions.OrderByDescending(q => q.Probability).FirstOrDefault();

                    // Update the Confusion Matrix
                    theMatrix.Predictions
                        [classes.IndexOf(topPredicton.TagName), classes.IndexOf(testTagName)]++;

                    bool isCorrect = testTagName == topPredicton.TagName;

                    // Display the results
                    var temp = Console.ForegroundColor;
                    Console.ForegroundColor = isCorrect ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine($"    { testImageFileName } predicted as { topPredicton.TagName } with { (int)(topPredicton.Probability * 100) }");
                    Console.ForegroundColor = temp;

                    // Copy the image to the predicted folder
                    File.Copy
                        (testImage, Path.Combine(PredictionImageFolder, topPredicton.TagName, $"{ testTagName }_{ testImageFileName }"), true);
                }
            }

            return theMatrix;
        }

        static async Task<ImagePrediction> GetImagePredictionsAsync(string imageFile)
        {
            // Create a prediction client
            var predictionClient = new CustomVisionPredictionClient()
            {
                Endpoint = Endpoint,
                ApiKey = PredictionKey
            };

            // Get predictions from the image
            using (var imageStream = new FileStream(imageFile, FileMode.Open))
            {
                var predictions = await predictionClient.ClassifyImageAsync
                    (new Guid(ProjetId), PublishedName, imageStream);
                return predictions;
            };
        }


    }
}
