// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Concurrent;
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

            Acks = Acks.All // Ensure message delivery confirmation
        };

        string topic = "patient-topic";

        using (var producer = new ProducerBuilder<string, string>(config).Build())
        {
            for (int i = 0; i < 10; i++)
            {
                string key = $"key-{i}";
                string value = $"Message-{i}";

                // Custom logic to select a partition
               // int targetPartition = (i % 10 == 0) ? 0 : 1;  // Example: Alternate between partition 0 and 1

                try
                {
                    // Produce message with explicit partition
                    var deliveryReport = await producer.ProduceAsync(new TopicPartition(topic, new Partition(i)),
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

