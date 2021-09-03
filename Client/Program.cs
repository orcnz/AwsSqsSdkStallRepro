using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Enable SDK debug logging
            AWSConfigs.LoggingConfig.LogTo = LoggingOptions.Console;
            AWSConfigs.LoggingConfig.LogResponses = ResponseLoggingOption.Always;

            // Call our example server that only sends the header bytes and none of the content bytes
            var config = new AmazonSQSConfig
            {
                ServiceURL = "http://localhost:5000/",
                UseHttp = true
            };

            var client = new AmazonSQSClient(config);

            var request = new ReceiveMessageRequest();

            var response = await client.ReceiveMessageAsync(request);

            // Unreachable, due to stall
            Console.WriteLine($"Received {response.Messages.Count} messages.");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey(true);
        }
    }
}
