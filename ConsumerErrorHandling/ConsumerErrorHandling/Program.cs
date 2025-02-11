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

            GroupId = "consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,  // Start from earliest offset if no committed offset is found
            EnableAutoCommit = false  // Manual commit to control retries
        };

        string topic = "patient-topic";

        using (var consumer = new ConsumerBuilder<string, string>(config).Build())
        {
            consumer.Subscribe(topic);
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

                    int maxRetryAttempts = 3;
                    int retryCount = 0;
                    bool messageProcessed = false;

                    while (retryCount < maxRetryAttempts && !messageProcessed)
                    {
                        try
                        {
                            // Simulate message processing
                            Console.WriteLine($"Processing message with key: {consumeResult.Message.Key} and value: {consumeResult.Message.Value}");

                            // Simulate failure for demonstration
                            if (retryCount < 2) throw new Exception("Simulated processing failure");

                            // If processing succeeds, commit offset
                            consumer.Commit(consumeResult);
                            Console.WriteLine($"Successfully processed message: {consumeResult.Message.Value}");
                            messageProcessed = true;
                        }
                        catch (Exception ex)
                        {
                            retryCount++;
                            Console.WriteLine($"Error processing message (attempt {retryCount}): {ex.Message}");

                            if (retryCount == maxRetryAttempts)
                            {
                                Console.WriteLine($"Max retry attempts reached for message: {consumeResult.Message.Value}. Logging error and skipping.");
                                // Optionally log to an external system for dead-letter processing
                            }
                            else
                            {
                                // Optional: Wait before retrying
                                Thread.Sleep(1000 * retryCount);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Consumer cancelled.");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
