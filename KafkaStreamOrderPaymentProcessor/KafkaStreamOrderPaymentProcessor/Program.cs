// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;

class OrderPaymentJoinProcessor
{
    private static ConcurrentDictionary<string, string> orders = new();
    private static ConcurrentDictionary<string, string> payments = new();

    public static async Task Main()
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

            GroupId = "join-groupv2",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

        consumer.Subscribe(new[] { "ordersv1", "payments" });

        while (true)
        {
            var consumeResult = consumer.Consume();
            var key = consumeResult.Message.Key;

            if (consumeResult.Topic == "ordersv1")
                orders[key] = consumeResult.Message.Value;
            else if (consumeResult.Topic == "payments")
                payments[key] = consumeResult.Message.Value;

            if (orders.ContainsKey(key) && payments.ContainsKey(key))
            {
                var order = JsonSerializer.Deserialize<Order>(orders[key]);
                var payment = JsonSerializer.Deserialize<Payment>(payments[key]);

                var joinedData = new
                {
                    order.OrderId,
                    order.UserId,
                    order.Amount,
                    payment.Status
                };

                string joinedJson = JsonSerializer.Serialize(joinedData);
                await producer.ProduceAsync("order-payments", new Message<string, string> { Key = key, Value = joinedJson });

                Console.WriteLine($"Joined Data: {joinedJson}");
            }
        }
    }
}

class Order { public string OrderId { get; set; } public string UserId { get; set; } public double Amount { get; set; } }
class Payment { public string OrderId { get; set; } public string Status { get; set; } }

