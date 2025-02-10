
using Confluent.Kafka;
using System.Diagnostics;
using System.Net;

namespace PatientAPI.Repositories
{
    public class PatientPublishRepo : IPatientPublishRepo
    {
        public async Task<string> PublishMessage(string Message, IConfiguration configuration)
        {
            ProducerConfig ProducerConfig = new ProducerConfig
            {
                BootstrapServers = configuration["BootStrapServer"],
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = configuration["UserName"],
                SaslPassword = configuration["Password"],
                

                ClientId = Dns.GetHostName()

            };
            try
            {
                using (var producer = new ProducerBuilder
                <string, string>(ProducerConfig).Build())
                {
                    var result = await producer.ProduceAsync
                    (configuration["TopicName"], new Message<string, string>
                    {
                        Key = new Random().Next(5).ToString(),
                        Value = Message
                    });

                    Debug.WriteLine($"Delivery Timestamp:{result.Timestamp.UtcDateTime}");
                    producer.Flush(TimeSpan.FromSeconds(60));
                    return await Task.FromResult($"Delivery Timestamp:{result.Timestamp.UtcDateTime}");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured: {ex.Message}");
            }

            return await Task.FromResult("Not Published.....");
        }
    }
}
