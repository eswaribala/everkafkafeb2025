
using Confluent.Kafka;
using ConfluentPatientInfoConsumer.DTOs;
using Newtonsoft.Json;

namespace ConfluentPatientInfoConsumer.Services
{
    public class PatientInfoConsumer : BackgroundService
    {
        
        private IConfiguration _configuration;
        private string response;
        public PatientInfoConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
           

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
            return ConsumePatientData();
        }

        public async Task<string> ConsumePatientData()
        {

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _configuration["BootStrapServer"],
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = _configuration["UserName"],
                SaslPassword = _configuration["Password"],
                GroupId = _configuration["Group_Id"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using (var c = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                c.Subscribe(_configuration["TopicName"]);

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                            response = $"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.";
                            //var result = JsonConvert.DeserializeObject<List<Patient>>(cr.Value);
                            //foreach (var patient in result)
                            //{
                            //    Console.WriteLine(patient.FullName.FirstName);
                            //}
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                            response = $"Error occured: {e.Error.Reason}";
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
                return response;

            }

        }

    }
}
