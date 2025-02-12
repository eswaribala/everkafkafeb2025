// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Threading.Tasks;

class OrderProducer
{
    public static async Task Main()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",
        };
        using var producer = new ProducerBuilder<string, string>(config).Build();

        var order = new { OrderId = "O1", UserId = "U1", Amount = 100 };
        string orderJson = JsonSerializer.Serialize(order);

        await producer.ProduceAsync("ordersv1", new Message<string, string> { Key = order.OrderId, Value = orderJson });
        Console.WriteLine($"Produced Order: {orderJson}");
    }
}

