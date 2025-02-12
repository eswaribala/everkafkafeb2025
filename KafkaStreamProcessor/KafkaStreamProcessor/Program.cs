// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

class KafkaStreamProcessor
{
    private static ConcurrentDictionary<string, int> _wordCounts = new();
    //word count
    public static async Task Main()
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

            GroupId = "wordcount-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var producerConfig = new ProducerConfig {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        using var producer = new ProducerBuilder<string, int>(producerConfig)
            .SetValueSerializer(new IntSerializer())
            .Build();

        consumer.Subscribe("dental-topic");

        while (true)
        {
            var consumeResult = consumer.Consume();
            var words = consumeResult.Message.Value
                .ToLower()
                .Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                _wordCounts.AddOrUpdate(word, 1, (key, count) => count + 1);
                await producer.ProduceAsync("processed-topic", new Message<string, int> { Key = word, Value = _wordCounts[word] });
                Console.WriteLine($"Processed word: {word}, Count: {_wordCounts[word]}");
            }
        }
    }
}

class IntSerializer : ISerializer<int>
{
    public byte[] Serialize(int data, SerializationContext context) => BitConverter.GetBytes(data);
}

