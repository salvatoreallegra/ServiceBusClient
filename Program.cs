using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace ServiceBusClientDemo
{
    internal class Program
    {
        private const string connectionString = "Endpoint=sb://salallegra.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=yUxjWv4i8YEH7P3LGjDZEWyyAzAcXwkbN+ASbBWhsgE=";
        private const string queueName = "salsqueue";

        private static ServiceBusClient client;
        private static ServiceBusProcessor processor;

        static async Task Main(string[] args)
        {
            // Initialize the client and processor
            client = new ServiceBusClient(connectionString);
            processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            try
            {
                // Register message and error handlers
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;

                // Start processing
                Console.WriteLine("Starting message processor...");
                await processor.StartProcessingAsync();

                Console.WriteLine("Press any key to stop the processor...");
                Console.ReadKey();

                // Stop processing
                Console.WriteLine("Stopping message processor...");
                await processor.StopProcessingAsync();
            }
            finally
            {
                // Dispose resources
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        private static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            // Process the message
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received message: {body}");

            // Complete the message
            await args.CompleteMessageAsync(args.Message);
        }

        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            // Handle errors
            Console.WriteLine($"Error occurred: {args.Exception}");
            return Task.CompletedTask;
        }
    }
}
