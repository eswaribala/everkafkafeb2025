// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using AvroMessageConsumer;
using Confluent.Kafka.SyncOverAsync;  // The namespace containing the Transaction class

public class Program
{
    public static async Task Main(string[] args)
    {
        // Kafka consumer configuration
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",
            // API Secret
            GroupId = "avro-consumer-group-v11",  // Kafka consumer group ID
            AutoOffsetReset = AutoOffsetReset.Earliest  // Start from earliest if no committed offset
        };

        // Schema registry configuration
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = "https://psrc-wrp99.us-central1.gcp.confluent.cloud",
            BasicAuthUserInfo = "3BFMWQFZ5OXHUDLQ:6D3IyPRg4kA2SmFJRR/CFqVkF3fd6CMVAVvTS8w7EK33nvYLJ/Uc9MJIptWhuBDC",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo

        };

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

        // Avro deserializer for Transaction class
        var avroDeserializer = new AvroDeserializer<Transaction>(schemaRegistry)
            .AsSyncOverAsync();  // Wrap async deserializer to work with sync Kafka Consumer

        using var consumer = new ConsumerBuilder<string, Transaction>(consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(avroDeserializer)
            .Build();


        

        consumer.Subscribe("transactions-avro");

        Console.WriteLine("Consuming messages from Kafka topic 'transactions-avro'...");

        try
        {
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(CancellationToken.None);
                    var transaction = consumeResult.Message.Value;

                    Console.WriteLine($"Received Transaction: ID={transaction.id}, Amount={transaction.amount}");
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Deserialization failed for record: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Closing consumer...");
        }
        finally
        {
            consumer.Close();
        }

    }
}

