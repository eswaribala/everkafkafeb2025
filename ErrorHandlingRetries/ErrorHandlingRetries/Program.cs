// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
using Confluent.Kafka;

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

            Acks = Acks.All,  // Ensure all brokers acknowledge message
            MessageTimeoutMs = 1000,  // Timeout for message delivery
            RetryBackoffMs = 1000,  // Initial backoff before retrying
            EnableIdempotence = true,  // Avoid duplicate messages
            MaxInFlight = 5  // Limit number of in-flight requests
        };

        string topic = "test-topic";

        using (var producer = new ProducerBuilder<string, string>(config).Build())
        {
            int retryCount = 3;

            for (int i = 0; i < 10; i++)
            {
                string key = $"key-{i}";
                string value = $"Message-{i}";

                int attempt = 0;
                bool success = false;

                while (attempt < retryCount && !success)
                {
                    try
                    {
                        var deliveryReport = await producer.ProduceAsync(
                            topic,
                            new Message<string, string> { Key = key, Value = value }
                        );

                        Console.WriteLine($"Delivered '{deliveryReport.Value}' to partition {deliveryReport.Partition} with key '{deliveryReport.Key}'");
                        success = true;  // Message successfully delivered
                    }
                    catch (ProduceException<string, string> e)
                    {
                        Console.WriteLine($"Attempt {attempt + 1} failed: {e.Error.Reason}");
                        if (++attempt == retryCount)
                        {
                            Console.WriteLine("Max retries reached. Logging error and continuing.");
                        }
                        else
                        {
                            // Optional: Wait before retrying (e.g., exponential backoff)
                            await Task.Delay(1000 * attempt);
                        }
                    }
                }
            }
        }
    }
}
