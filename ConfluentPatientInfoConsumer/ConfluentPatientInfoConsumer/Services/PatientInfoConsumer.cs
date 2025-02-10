
namespace ConfluentPatientInfoConsumer.Services
{
    public class PatientInfoConsumer : BackgroundService
    {
        private IPolicyRepo _policyRepo;
        private IConfiguration _configuration;
        private string response;
        public PolicyConsumerService(IPolicyRepo policyRepo,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _policyRepo = policyRepo;

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var groupId = _configuration["GroupId"];
            var bootStrapServer = _configuration["BootstrapServer"];
            var topicName = _configuration["TopicName"];
            return ConsumePolicyData(topicName, groupId, bootStrapServer);
        }

        public async Task<string> ConsumePolicyData(string TopicName,
            string Group_Id, string BootstarpServer)
        {

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = BootstarpServer,
                GroupId = Group_Id,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using (var c = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                c.Subscribe(TopicName);

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
                            var result = JsonConvert.DeserializeObject<List<Policy>>(cr.Value);
                            foreach (var policy in result)
                            {
                                policy.PolicyNo = policy.PolicyNo + new Random().Next(10000);
                                _policyRepo.AddPolicy(policy);
                            }
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
