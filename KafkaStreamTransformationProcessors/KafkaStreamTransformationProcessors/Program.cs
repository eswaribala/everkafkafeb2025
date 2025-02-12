// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Threading.Tasks;

class KafkaStreamProcessor
{
    public static async Task Main()
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",
            GroupId = "stream-transform-group",
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
        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

        consumer.Subscribe("dental-topic-v2");

        while (true)
        {
            var consumeResult = consumer.Consume();
            var parts = consumeResult.Message.Value.Split(':');

            var transformedEvent = new
            {
                UserId = parts[0],
                Action = parts[1],
                Timestamp = DateTime.UtcNow
            };

            /*
             Filter

             if (parts[1] == "login") // Filter condition
                {
                    await producer.ProduceAsync("login-events", new Message<string, string> { Key = consumeResult.Message.Key, Value = consumeResult.Message.Value });
                    Console.WriteLine($"Filtered Event: {consumeResult.Message.Value}");
                }
             */



            string jsonEvent = JsonSerializer.Serialize(transformedEvent);
            await producer.ProduceAsync("transformed-events", new Message<string, string> { Key = consumeResult.Message.Key, Value = jsonEvent });

            Console.WriteLine($"Transformed: {jsonEvent}");
        }
    }
}

