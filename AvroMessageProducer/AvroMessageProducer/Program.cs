using System;
using System.Threading.Tasks;
using AvroMessageProducer;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Kafka configuration
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",
            // API Secret
        };

        // Schema registry configuration
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = "https://psrc-wrp99.us-central1.gcp.confluent.cloud",
            BasicAuthUserInfo = "3BFMWQFZ5OXHUDLQ:6D3IyPRg4kA2SmFJRR/CFqVkF3fd6CMVAVvTS8w7EK33nvYLJ/Uc9MJIptWhuBDC",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo
        };

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        var avroSerializer = new AvroSerializer<Transaction>(schemaRegistry, new AvroSerializerConfig { AutoRegisterSchemas = true });

        using var producer = new ProducerBuilder<string, Transaction>(producerConfig)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(avroSerializer)
            .Build();

        var transaction = new Transaction
        {
            id = Guid.NewGuid().ToString(),
            amount = 15000.85
        };

        try
        {
            //var serializedData = await avroSerializer.SerializeAsync(transaction, SerializationContext.Empty);
            //Console.WriteLine("Serialization successful");
            var result = await producer.ProduceAsync("transactions-avro", new Message<string, Transaction>
            {
                Key = transaction.id,
                Value = transaction
            });

            Console.WriteLine($"Message delivered to {result.TopicPartitionOffset}");



        }
        catch (Exception ex)
        {
            Console.WriteLine($"Serialization failed: {ex.Message}");
        }
    }
}
