using Confluent.Kafka;

namespace PatientInfoConsumerBackgroundService;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IConfiguration _configuration;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var kafkaConfig = new ConsumerConfig
        {
            BootstrapServers = _configuration["BootStrapServer"],  // Replace with LoadBalancer IP or NodePort
            GroupId = _configuration["GroupId"],
            //AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            // Update for TLS or SASL if required
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = _configuration["UserNameValue"],
            SaslPassword = _configuration["PasswordValue"],
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_configuration["AutoOffsetReset"]),
          
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(kafkaConfig).Build();
        consumer.Subscribe(_configuration["TopicName"]);

        _logger.LogInformation("Kafka Consumer started and listening to topic.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    _logger.LogInformation($"Consumed message: {result.Message.Value}, at offset {result.Offset}");

                    // Simulate message processing
                    await Task.Delay(50, stoppingToken);
                }
                catch (ConsumeException e)
                {
                    _logger.LogError($"Consume error: {e.Error.Reason}");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Operation canceled. Exiting Kafka consumer.");
                }
            }
        }
        finally
        {
            consumer.Close();  // Graceful shutdown
        }
    }
}
