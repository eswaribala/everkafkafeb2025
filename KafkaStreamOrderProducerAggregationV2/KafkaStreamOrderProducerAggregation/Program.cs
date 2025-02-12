// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Threading.Tasks;

class OrderProducer
{
    public static async Task Main()
    {
        var config = new ProducerConfig {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

        };
        using var producer = new ProducerBuilder<string, string>(config).Build();

        var order = new { Id=Guid.NewGuid().ToString(), OrderId = "O100", UserId = "U1", Amount = 250.0, Timestamp = DateTime.UtcNow };

        string orderJson = JsonSerializer.Serialize(order);
        await producer.ProduceAsync("ordersv5", new Message<string, string> { Key = order.UserId, Value = orderJson });

        Console.WriteLine($"Produced Order: {orderJson}");
    }
}

