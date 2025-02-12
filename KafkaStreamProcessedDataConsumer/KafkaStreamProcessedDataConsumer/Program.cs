// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;

class KafkaConsumer
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

            GroupId = "wordcount-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, int>(config)
            .SetValueDeserializer(new IntDeserializer())
            .Build();

        consumer.Subscribe("processed-topic");

        while (true)
        {
            var consumeResult = consumer.Consume();
            Console.WriteLine($"Word: {consumeResult.Message.Key}, Count: {consumeResult.Message.Value}");
        }
    }
}

class IntDeserializer : IDeserializer<int>
{
    public int Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) => BitConverter.ToInt32(data);
}
