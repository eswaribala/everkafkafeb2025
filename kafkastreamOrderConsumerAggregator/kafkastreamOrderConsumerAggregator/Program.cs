// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;

class AggregatedOrderConsumer
{
    public static void Main()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

            GroupId = "aggregated-order-consumer-groupv1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, double>(config)
            .SetValueDeserializer(new DoubleDeserializer())
            .Build();

        consumer.Subscribe("aggregatedorders");

        while (true)
        {
            var consumeResult = consumer.Consume();
            Console.WriteLine($"User: {consumeResult.Message.Key}, Total Order Amount: {consumeResult.Message.Value:C}");
        }
    }
}

class DoubleDeserializer : IDeserializer<double>
{
    public double Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) => BitConverter.ToDouble(data);
}

