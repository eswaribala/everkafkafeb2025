// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using Confluent.Kafka;

class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

            Acks = Acks.All,  // Ensure message delivery acknowledgment
            EnableIdempotence = true  // Prevent duplicate messages
        };

        string mainTopic = "test1-topic";
        string dlqTopic = "test-dlq-topic";
        int maxRetries = 3;

        using (var producer = new ProducerBuilder<string, string>(config).Build())
        {
            string key = "key-1";
            string value = "Critical message to process";

            int retryCount = 0;
            bool success = false;

            while (retryCount < maxRetries && !success)
            {
                try
                {
                    // Produce to main topic
                    var deliveryReport = await producer.ProduceAsync(
                        mainTopic,
                        new Message<string, string> { Key = key, Value = value });

                    Console.WriteLine($"Message delivered to partition {deliveryReport.Partition} with offset {deliveryReport.Offset}");
                    success = true;  // Successfully delivered, exit retry loop
                }
                catch (ProduceException<string, string> e)
                {
                    Console.WriteLine($"Attempt {retryCount + 1} failed: {e.Error.Reason}");
                    retryCount++;

                    if (retryCount == maxRetries)
                    {
                        // Send the message to the DLQ
                        Console.WriteLine("Max retries reached. Sending message to DLQ...");
                        await producer.ProduceAsync(
                            dlqTopic,
                            new Message<string, string> { Key = key, Value = value }
                        );

                        Console.WriteLine("Message sent to DLQ");
                    }
                    else
                    {
                        // Optional: Delay before retrying
                        await Task.Delay(1000 * retryCount);
                    }
                }
            }
        }
    }
}
