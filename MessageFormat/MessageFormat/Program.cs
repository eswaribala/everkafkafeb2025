// See https://aka.ms/new-console-template for more information
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

public class Order
{
    public int OrderId { get; set; }
    public string ProductName { get; set; }
    public double Price { get; set; }
}

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
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG"

        };

        using var producer = new ProducerBuilder<string, string>(config).Build();

        var order = new Order { OrderId = 1, ProductName = "Laptop", Price = 999.99 };
        string orderJson = JsonSerializer.Serialize(order);

        try
        {
            var result = await producer.ProduceAsync("test-topic", new Message<string, string>
            {
                Key = "order-1",
                Value = orderJson
            });

            Console.WriteLine($"JSON message delivered to {result.TopicPartitionOffset}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to produce JSON message: {ex.Message}");
        }
    }
}

