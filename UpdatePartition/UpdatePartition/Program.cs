// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

class Program
{
    static async Task Main(string[] args)
    {
        // Define Confluent Kafka configuration
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG"
        };

        // Topic name and new partition count
        string topicName = "test-topic";
        int newPartitionCount = 20;  // Must be greater than the current partition count

        try
        {
            using (var adminClient = new AdminClientBuilder(adminConfig).Build())
            {
                // Alter topic partitions
                await adminClient.CreatePartitionsAsync(new List<PartitionsSpecification>
                {
                    new PartitionsSpecification
                    {
                        Topic = topicName,
                        IncreaseTo = newPartitionCount
                    }
                });

                Console.WriteLine($"Successfully increased partitions for topic '{topicName}' to {newPartitionCount}.");
            }
        }
        catch (CreatePartitionsException e)
        {
            Console.WriteLine($"Failed to increase partitions for topic '{topicName}': {e.Message}");
        }
    }
}

