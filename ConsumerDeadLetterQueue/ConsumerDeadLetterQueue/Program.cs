// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
using Confluent.Kafka;

class Program
{
    public static void Main(string[] args)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",
            GroupId = "my-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false  // Manually commit offsets for better retry control
        };

        string topic = "test-topic";
        string dlqTopic = "test-dlq-topic";

        using (var consumer = new ConsumerBuilder<string, string>(config).Build())
        {
            consumer.Subscribe(topic);
            using var dlqProducer = new ProducerBuilder<string, string>(config).Build();

            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume();

                    int maxRetryAttempts = 3;
                    int retryCount = 0;
                    bool messageProcessed = false;

                    while (retryCount < maxRetryAttempts && !messageProcessed)
                    {
                        try
                        {
                            // Simulate message processing
                            Console.WriteLine($"Processing message: {consumeResult.Message.Value}");

                            // Simulate failure (for testing purposes)
                            if (retryCount < 2) throw new Exception("Simulated processing failure");

                            // Successfully processed
                            consumer.Commit(consumeResult);
                            messageProcessed = true;
                            Console.WriteLine("Message processed successfully.");
                        }
                        catch (Exception ex)
                        {
                            retryCount++;
                            Console.WriteLine($"Processing failed (attempt {retryCount}): {ex.Message}");

                            if (retryCount == maxRetryAttempts)
                            {
                                Console.WriteLine($"Max retries reached for message '{consumeResult.Message.Value}'. Sending to DLQ...");

                                // Send failed message to DLQ
                                dlqProducer.Produce(dlqTopic, new Message<string, string>
                                {
                                    Key = consumeResult.Message.Key,
                                    Value = consumeResult.Message.Value
                                }, report => {
                                    if (report.Error.IsError)
                                    {
                                        Console.WriteLine($"Error sending to DLQ: {report.Error.Reason}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Message sent to DLQ successfully: {consumeResult.Message.Value}");
                                    }
                                });
                            }
                        }
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consumer error: {e.Error.Reason}");
                }
            }
        }
    }
}

