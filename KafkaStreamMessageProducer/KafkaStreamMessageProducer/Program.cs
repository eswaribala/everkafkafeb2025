// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System;
using System.Threading.Tasks;

class KafkaProducer
{
    public static async Task Main()
    {


        var config = new ProducerConfig
        {
            BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "RMINMSPHVA25JTUF",
            SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG"
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();

        string[] messages = { "Prosthodontics and Implant", "Oral MaxilioFacial Surgery", 
            "Endodontics", "Orthodontics","Oral Surgery","Paedodontics","Periodontics","Public Health Department" };

        foreach (var message in messages)
        {
            await producer.ProduceAsync("dental-topic", new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = message });
            Console.WriteLine($"Produced: {message}");
        }
    }
}

