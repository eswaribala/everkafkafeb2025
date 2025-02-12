// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Com.Veb.Kafkaproto;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Kafka producer configuration
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

        };

        // Schema Registry configuration
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = "https://psrc-wrp99.us-central1.gcp.confluent.cloud",
            BasicAuthUserInfo = "3BFMWQFZ5OXHUDLQ:6D3IyPRg4kA2SmFJRR/CFqVkF3fd6CMVAVvTS8w7EK33nvYLJ/Uc9MJIptWhuBDC",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo
        };

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);

        // ProtoBuf serializer configuration
        var protobufSerializer = new ProtobufSerializer<User>(schemaRegistry);

        using var producer = new ProducerBuilder<string, User>(producerConfig)
            .SetKeySerializer(Serializers.Utf8)  // Serialize key as UTF-8
            .SetValueSerializer(protobufSerializer)  // Serialize value using ProtoBuf
            .Build();

        // Sample Transaction object
        var user = new User
        {
            Age=28,
            Name="Vignesh"
        };

        try
        {
            // Send the message to the Kafka topic
            var result = await producer.ProduceAsync("users", new Message<string, User>
            {
                Key = user.Name,
                Value = user
            });

            Console.WriteLine($"Message sent successfully to {result.TopicPartitionOffset}");
        }
        catch (ProduceException<string, User> ex)
        {
            Console.WriteLine($"Message delivery failed: {ex.Error.Reason}");
        }
    }
}

