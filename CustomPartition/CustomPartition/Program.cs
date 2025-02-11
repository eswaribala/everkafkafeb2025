// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",

            GroupId = "rebalance-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(config)
            .SetPartitionsAssignedHandler((c, partitions) =>
            {
                Console.WriteLine("Partitions Assigned:");
                foreach (var partition in partitions)
                {
                    Console.WriteLine($"- {partition.Topic} [{partition.Partition}]");
                }

                // Optionally customize the assignment
                //return (IEnumerable<TopicPartitionOffset>)partitions;
            })
            .SetPartitionsRevokedHandler((c, partitions) =>
            {
                Console.WriteLine("Partitions Revoked:");
                foreach (var partition in partitions)
                {
                    Console.WriteLine($"- {partition.Topic} [{partition.Partition}]");
                }
            })
            .Build())
        {
            consumer.Subscribe("test-topic");

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume(cts.Token);
                    Console.WriteLine($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");
                }
            }
            catch (OperationCanceledException)
            {
                // Handle shutdown
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}

