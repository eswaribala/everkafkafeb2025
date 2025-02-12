// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

class OrderWindowAggregator
{
    private static ConcurrentDictionary<string, double> windowedAggregations = new();
    private static Timer _windowTimer;

    public static async Task Main()
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

            GroupId = "window-groupv3",
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

        consumer.Subscribe("ordersv3");

        // Windowing: Reset aggregation every minute
        _windowTimer = new Timer(60000); // 1-minute window
        _windowTimer.Elapsed += async (sender, args) =>
        {
            foreach (var entry in windowedAggregations)
            {
                await producer.ProduceAsync("windowed-topic", new Message<string, double> { Key = entry.Key, Value = entry.Value });
                Console.WriteLine($"Emitting Windowed Aggregation - User: {entry.Key}, Total: {entry.Value:C}");
                windowedAggregations.TryRemove(entry.Key, out _);
            }
        };
        _windowTimer.Start();

        while (true)
        {
            var consumeResult = consumer.Consume();
            var order = JsonSerializer.Deserialize<Order>(consumeResult.Message.Value);
          

                // Extract timestamp from headers
                long timestampBinary = BitConverter.ToInt64(consumeResult.Message.Headers.GetLastBytes("timestamp"));
                DateTime eventTimestamp = DateTime.FromBinary(timestampBinary);

                // Handle late event (older than 1-minute)
                if (DateTime.UtcNow - eventTimestamp > TimeSpan.FromMinutes(1))
                {
                    Console.WriteLine($"Late Event Ignored: {order.OrderId}");
                    continue;
                }

                windowedAggregations.AddOrUpdate(order.UserId, order.Amount, (key, total) => total + order.Amount);
                Console.WriteLine($"Processed Order - User: {order.UserId}, Amount: {order.Amount:C}, Timestamp: {eventTimestamp}");
            }
            
            
    }
}

class Order { public string OrderId { get; set; } public string UserId { get; set; } public double Amount { get; set; } }
class DoubleSerializer : ISerializer<double> { public byte[] Serialize(double data, SerializationContext context) => BitConverter.GetBytes(data); }

