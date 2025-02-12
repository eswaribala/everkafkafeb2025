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

        var orders = new[]
        {
            new { OrderId = "O6", UserId = "U11", Amount = 100.5 , Timestamp = DateTime.UtcNow.AddSeconds(-30)},
            new { OrderId = "O7", UserId = "U12", Amount = 200.0, Timestamp = DateTime.UtcNow},
            new { OrderId = "O8", UserId = "U13", Amount = 50.75, Timestamp = DateTime.UtcNow.AddSeconds(-20)},
            new { OrderId = "O9", UserId = "U14", Amount = 300.25,Timestamp = DateTime.UtcNow.AddSeconds(-10) },
            new { OrderId = "O10", UserId = "U15", Amount = 150.0,Timestamp = DateTime.UtcNow.AddSeconds(-2) }
        };

        foreach (var order in orders)
        {
            string orderJson = JsonSerializer.Serialize(order);
            await producer.ProduceAsync("ordersv2", new Message<string, string> { Key = order.UserId, Value = orderJson });
            Console.WriteLine($"Produced Order: {orderJson}");
        }
    }
}

