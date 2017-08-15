using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using System;
using System.Linq;

namespace HelloGoogleCloudVisionApi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args.Length);
            if (args.Length != 1) { Console.WriteLine("Usage:HelloGoogleCloudVisionApi <path_to_image>"); return; }

            string imagePath = args[0];
            byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
            string imageContent = Convert.ToBase64String(imageArray);

            var v = CreateAuthorizedClient();
            var result = v.Images.Annotate(new BatchAnnotateImagesRequest()
            {
                Requests = new[]
                {
                    new AnnotateImageRequest() {
                        Features = new [] { new Feature() { Type =
                          "TEXT_DETECTION"}},
                        Image = new Image() { Content = imageContent }
                    }
                }
            }).Execute();
            result.Responses.ToList().ForEach(a => a.TextAnnotations.ToList().ForEach(b => Console.WriteLine(b.Description)));

            Console.WriteLine("Press any key:");
            Console.ReadKey();

        }

        public static VisionService CreateAuthorizedClient()
        {
            var credential = GoogleCredential.FromJson(System.Configuration.ConfigurationManager.AppSettings["SERVICE_ACCOUNT_JSON"]);

            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    VisionService.Scope.CloudPlatform
                });
            }
            return new VisionService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });
        }
    }
}
