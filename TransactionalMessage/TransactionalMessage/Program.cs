// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using Confluent.Kafka;

class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",
            /*
             * EnableIdempotence = true: Enables exactly-once semantics for writes.
                TransactionalId: Unique identifier for the transaction.
             */
            EnableIdempotence = true,  // Enable exactly-once semantics
            TransactionalId = "my-transactional-producer"  // Unique transactional ID
        };

        using (var producer = new ProducerBuilder<string, string>(config).Build())
        {
            try
            {
                // Initialize transaction
                producer.InitTransactions(TimeSpan.FromSeconds(10));

                // Begin transaction
                producer.BeginTransaction();

                // Produce messages atomically
                for (int i = 0; i < 5; i++)
                {
                    var key = $"key-{i}";
                    var value = $"Transactional message {i}";

                    await producer.ProduceAsync("test-topic", new Message<string, string> { Key = key, Value = value });
                    Console.WriteLine($"Message {i} sent successfully.");
                }

                // Commit the transaction if all messages were produced successfully
                producer.CommitTransaction();
                Console.WriteLine("Transaction committed.");
            }
            catch (KafkaException ex)
            {
                // Abort transaction in case of failure
                Console.WriteLine($"Transaction failed: {ex.Message}");
                producer.AbortTransaction();
            }
        }
    }
}

