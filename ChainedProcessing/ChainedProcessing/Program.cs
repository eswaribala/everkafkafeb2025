// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using Confluent.Kafka;

class Program
{
    public static async Task Main(string[] args)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

            EnableIdempotence = true,
            TransactionalId = "my-chained-processor",
            TransactionTimeoutMs = 60000,  // Total transaction timeout
            RequestTimeoutMs = 10000       // Timeout for requests to Kafka
        };

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

            GroupId = "chained-processing-group",
            IsolationLevel = IsolationLevel.ReadCommitted
        };

        using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
        using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
        {
            consumer.Subscribe("test-topic");
            producer.InitTransactions(TimeSpan.FromSeconds(10));

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume();
                    producer.BeginTransaction();

                    try
                    {
                        var processedValue = ProcessMessage(consumeResult.Message.Value);

                        // Produce processed message to output topic
                        await producer.ProduceAsync("test-topic", new Message<string, string> { Key = consumeResult.Message.Key, Value = processedValue });


                        // Commit transaction and offset
                        producer.SendOffsetsToTransaction(
                                 new[] { new TopicPartitionOffset(consumeResult.TopicPartition, consumeResult.Offset + 1) },
                                 consumer.ConsumerGroupMetadata, TimeSpan.FromSeconds(2)
                             );
                        producer.CommitTransaction();
                        Console.WriteLine($"Successfully processed message: {consumeResult.Message.Value}");


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                        producer.AbortTransaction();  // Abort the transaction on failure
                    }
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Consumer error: {e.Error.Reason}");
            }
        }
    }

    static string ProcessMessage(string message)
    {
        // Simulate some processing logic
        return $"Processed: {message}";
    }
}

