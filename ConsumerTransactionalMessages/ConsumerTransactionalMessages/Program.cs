// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
using Confluent.Kafka;

class Program
{
    public static void Main(string[] args)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",
            GroupId = "my-transactional-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,  // Manually commit offsets
            IsolationLevel = IsolationLevel.ReadCommitted  // Read only committed transactions
        };

        string topic = "test-topic";

        using (var consumer = new ConsumerBuilder<string, string>(config).Build())
        {
            consumer.Subscribe(topic);

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume(CancellationToken.None);

                    // Process the message
                    Console.WriteLine($"Consumed message '{consumeResult.Message.Value}' from partition {consumeResult.Partition} at offset {consumeResult.Offset}");

                    // Commit the offset after successful processing
                    consumer.Commit(consumeResult);
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Consume error: {e.Error.Reason}");
            }
        }
    }
}

