// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Threading.Tasks;

class PaymentProducer
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

        var payment = new { OrderId = "O1", Status = "Paid" };
        string paymentJson = JsonSerializer.Serialize(payment);

        await producer.ProduceAsync("payments", new Message<string, string> { Key = payment.OrderId, Value = paymentJson });
        Console.WriteLine($"Produced Payment: {paymentJson}");
    }
}

