// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;

class OrderAggregator
{
    private static ConcurrentDictionary<string, double> userTotalAmounts = new();

    public static async Task Main()
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

            GroupId = "order-aggregation-group-v1",
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
        using var producer = new ProducerBuilder<string, double>(producerConfig)
            .SetValueSerializer(new DoubleSerializer())
            .Build();

        consumer.Subscribe("ordersv1");

        while (true)
        {
            var consumeResult = consumer.Consume();
            var order = JsonSerializer.Deserialize<Order>(consumeResult.Message.Value);

            // Aggregate the order amount per user
            userTotalAmounts.AddOrUpdate(order.UserId, order.Amount, (key, total) => total + order.Amount);

            // Publish the aggregated result
            await producer.ProduceAsync("aggregatedorders", new Message<string, double>
            {
                Key = order.UserId,
                Value = userTotalAmounts[order.UserId]
            });

            Console.WriteLine($"Aggregated Order - User: {order.UserId}, Total Amount: {userTotalAmounts[order.UserId]:C}");
        }
    }
}

class Order
{
    public string OrderId { get; set; }
    public string UserId { get; set; }
    public double Amount { get; set; }
}

class DoubleSerializer : ISerializer<double>
{
    public byte[] Serialize(double data, SerializationContext context) => BitConverter.GetBytes(data);
}

