// See https://aka.ms/new-console-template for more information
using AvroMessage;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Threading.Tasks;

class AvroProducer
{
    static async Task Main(string[] args)
    {
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = "https://psrc-wrp99.us-central1.gcp.confluent.cloud" // Schema Registry URL
           

        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

        };

        using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
        using (var producer = new ProducerBuilder<string, Order>(producerConfig)
            // .SetValueSerializer(new AvroSerializer<Order>(schemaRegistry))
            .SetValueSerializer(new AvroSerializer<Order>(schemaRegistry, new AvroSerializerConfig() { AutoRegisterSchemas = false, BufferBytes = 100 }))
            .Build())
        {
            var order = new Order
            {
                OrderId = 12345,
               OrderTime=11,
               OrderAddress="HYD"
            };
           

            var message = new Message<string, Order>
            {
                Key = Convert.ToString(order.OrderId),
                Value = order
            };
            
            // the schema Id will be included as a parameter of the content type
            Console.WriteLine(eventData.ContentType);

            // the serialized Avro data will be stored in the EventBody
            Console.WriteLine(eventData.EventBody);

            try
            {
                var deliveryResult = await producer.ProduceAsync("orders", message);
                Console.WriteLine($"Produced message to: {deliveryResult.TopicPartitionOffset}");
            }
            catch (ProduceException<string, Order> e)
            {
                Console.WriteLine($"Failed to produce message: {e.Error.Reason}");
            }
        }
    }
}
