// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using Confluent.Kafka;

class Program
{
    public static async Task Main(string[] args)
    {
        // Kafka configuration for Confluent Cloud
        var config = new ProducerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",
            Acks = Acks.All // Ensure message delivery is confirmed
        };

        string topic = "test-topic";

        using (var producer = new ProducerBuilder<string, string>(config).Build())
        {
            for (int i = 0; i < 10; i++)
            {
                string key = $"key-{i % 2}";  // Alternate keys ("key-0" and "key-1")
                string value = $"Message-{i}";

                try
                {
                    // Send message with key
                    var deliveryReport = await producer.ProduceAsync(
                        topic,
                        new Message<string, string> { Key = key, Value = value });

                    Console.WriteLine($"Delivered '{deliveryReport.Value}' to partition {deliveryReport.Partition} with key '{deliveryReport.Key}'");
                }
                catch (ProduceException<string, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
    }
}

