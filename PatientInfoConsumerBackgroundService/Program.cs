using PatientInfoConsumerBackgroundService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<KafkaConsumerServiceV1>();

var host = builder.Build();
host.Run();
