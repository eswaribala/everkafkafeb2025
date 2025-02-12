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
            new { OrderId = "11", UserId = "U16", Amount = 100.5 , Timestamp = DateTime.UtcNow.AddSeconds(-40)},
            new { OrderId = "12", UserId = "U17", Amount = 200.0, Timestamp = DateTime.UtcNow},
            new { OrderId = "13", UserId = "U18", Amount = 50.75, Timestamp = DateTime.UtcNow.AddSeconds(-10)},
            new { OrderId = "14", UserId = "U19", Amount = 300.25,Timestamp = DateTime.UtcNow.AddSeconds(-21) },
            new { OrderId = "15", UserId = "U20", Amount = 150.0,Timestamp = DateTime.UtcNow.AddSeconds(-13) }
        };
        
        foreach (var order in orders)
        {
            var headers = new Headers
            {
                { "timestamp", BitConverter.GetBytes(order.Timestamp.ToBinary()) }
            };
            string orderJson = JsonSerializer.Serialize(order);
            await producer.ProduceAsync("ordersv3", new Message<string, string>
            {
                Key = order.UserId,
                Value = orderJson,
                Headers = headers
            });
            Console.WriteLine($"Produced Order: {orderJson}");
        }
    }
}

