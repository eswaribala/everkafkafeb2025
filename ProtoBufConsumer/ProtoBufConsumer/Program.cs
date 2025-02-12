// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Com.Veb.Kafkaproto;
using Confluent.Kafka.SyncOverAsync;  // The namespace containing the generated Transaction class

public class Program
{
    public static async Task Main(string[] args)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",


            GroupId = "protobuf-consumer-group-v1",
            AutoOffsetReset = AutoOffsetReset.Earliest // Start consuming from the beginning
        };

        //var schemaRegistryConfig = new SchemaRegistryConfig
        //{
        //    Url = "https://psrc-wrp99.us-central1.gcp.confluent.cloud",
        //    BasicAuthUserInfo = "3BFMWQFZ5OXHUDLQ:6D3IyPRg4kA2SmFJRR/CFqVkF3fd6CMVAVvTS8w7EK33nvYLJ/Uc9MJIptWhuBDC",
        //    BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo

        //};

        //using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        var protobufDeserializer = new ProtobufDeserializer<User>().AsSyncOverAsync();

        using var consumer = new ConsumerBuilder<string, User>(consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(protobufDeserializer)  // ✅ Correct usage
            .Build();

        consumer.Subscribe("users");

        Console.WriteLine("Consuming ProtoBuf messages from Kafka...");

        try
        {
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(CancellationToken.None);
                    var user = consumeResult.Message.Value;
                    Console.WriteLine($"Received User: Name={user.Name}, Age={user.Age}");
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Deserialization failed: {ex.Message}");
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
