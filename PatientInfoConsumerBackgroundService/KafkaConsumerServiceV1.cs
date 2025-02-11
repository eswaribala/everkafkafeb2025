using Confluent.Kafka;
using System.Threading;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;
namespace PatientInfoConsumerBackgroundService;

public class KafkaConsumerServiceV1 : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IConfiguration _configuration;

    public KafkaConsumerServiceV1(ILogger<KafkaConsumerService> logger, IConfiguration configuration)
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
        

        try
        {
            
           

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = ConsumeBatch(2, consumer);
                    foreach (var item in result)
                    {
                        _logger.LogInformation($"Consumed message: '{item.Message.Value}', Partition: {item.Partition}, Offset: {item.Offset}");
                    }
                   
                   // _logger.LogInformation($"Consumed message: '{result.Message.Value}', Partition: {result.Partition}, Offset: {result.Offset}");

                    // Simulate message processing
                    await Task.Delay(100, stoppingToken);

                    // Manually commit the offset after processing
                   // consumer.Commit(result);
                }
                catch (ConsumeException e)
                {
                    _logger.LogError($"Kafka consume error: {e.Error.Reason}");
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }

    public IEnumerable<ConsumeResult<Ignore, string>> ConsumeBatch(int batchSize,IConsumer<Ignore,string> _consumer)
    {
        List<ConsumeResult<Ignore, string>> consumedMessages = new List<ConsumeResult<Ignore, string>>();

        int latestPartition = -1; // The partition from where we consumed the last message

        for (int i = 0; i < batchSize; i++)
        {
            var result = _consumer.Consume(100);

            if (result != null)
            {
                if (latestPartition == -1 || result.Partition.Value == latestPartition)
                {
                    consumedMessages.Add(result);
                    latestPartition = result.Partition.Value;
                }
                else
                {
                    // This call will guarantee that this message that will not be included in the current batch, will be included in another batch later
                    _consumer.Seek(result.TopicPartitionOffset); // IMPORTANT LINE!!!!!!!
                    break;
                }
            }
            else
                break;
        }

        return consumedMessages;
    }
}
