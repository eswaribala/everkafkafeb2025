// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

public class Order
{
    public int orderId { get; set; }
    public int orderTime { get; set; }
    public string? orderAddress { get; set; }
}

class Program
{
    public static async Task Main(string[] args)
    {
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = "https://psrc-wrp99.us-central1.gcp.confluent.cloud",
            BasicAuthUserInfo = "JHTTEFVOEV6OZT5T:LE9QVmgjtJJKY31BqPXkxnLcOBL+Ggkqx9t4WNm7+0eeWl4wnN86IgTcteJoif4j",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo

        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG"

        };

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        using var producer = new ProducerBuilder<string, Order>(producerConfig)
            .SetValueSerializer(new AvroSerializer<Order>(schemaRegistry))
            .Build();

        string topic = "orders";

        var order = new Order { orderId=1,orderTime=11,orderAddress="Hyd"};

        try
        {
            var result = await producer.ProduceAsync(topic, new Message<string, Order>
            {
                Key = "user-key",
                Value = order
            });

            Console.WriteLine($"Avro message delivered to {result.TopicPartitionOffset}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to produce Avro message: {ex.Message}");
        }
    }
}

