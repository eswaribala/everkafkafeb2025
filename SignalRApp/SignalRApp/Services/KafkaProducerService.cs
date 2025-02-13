using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRApp.Hubs;
using System.Diagnostics;
using System.Net;

namespace SignalRApp.Services
{
    public class KafkaProducerService
    {
        
        private readonly ProducerConfig _config;
        private readonly string _topic;

        public KafkaProducerService(IHubContext<KafkaHub> hubContext, IConfiguration configuration)
        {
            
            _topic = configuration["TopicName"]; // Change to your Kafka topic

            _config = new ProducerConfig
            {
                BootstrapServers = "pkc-619z3.us-east1.gcp.confluent.cloud:9092",  // Example: "your-cluster-name.region.aws.confluent.cloud:9092"
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = "RMINMSPHVA25JTUF",
                SaslPassword = "WlkUBrN62WUyOXQri5TwZz1Z10lzp0/oLojeT9a1/OCKh74Ccm0pIwd5BqizafrG",
                ClientId = Dns.GetHostName()

                //SslCaLocation = configuration["Kafka:CaCertificate"], // Optional
            };
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                using (var producer = new ProducerBuilder
                <string, string>(_config).Build())
                {
                    var result = await producer.ProduceAsync
                    (_topic, new Message<string, string>
                    {
                        Key = new Random().Next(5).ToString(),
                        Value = message
                    });

                    Debug.WriteLine($"Delivery Timestamp:{result.Timestamp.UtcDateTime}");
                    producer.Flush(TimeSpan.FromSeconds(60));
                    //return await Task.FromResult($"Delivery Timestamp:{result.Timestamp.UtcDateTime}");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}");
            }

            //return await Task.FromResult("Not Published.....");

        }
    }
}
