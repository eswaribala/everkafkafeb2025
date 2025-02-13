using Microsoft.AspNetCore.SignalR;
using SignalRApp.Services;
using System.Text.Json;

namespace SignalRApp.Hubs
{
    public class KafkaHub:Hub
    {
        private readonly ILogger<KafkaHub> _logger;
        private readonly KafkaProducerService _kafkaProducer;

        public KafkaHub(ILogger<KafkaHub> logger, KafkaProducerService kafkaProducer)
        {
            _logger = logger;
            _kafkaProducer = kafkaProducer;
        }
        public async Task SendMessage(string user, string message)
        {
            _logger.LogInformation($"Received message from : {message}");

           
           

            await _kafkaProducer.SendMessageAsync(message);

            _logger.LogInformation("Message published to Kafka successfully.");
            await Clients.All.SendAsync("ReceiveKafkaMessage", message);
        }
    }
}
