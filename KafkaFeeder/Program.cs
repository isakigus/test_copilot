using KafkaFeeder;
using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
var topic = configuration["Kafka:Topic"] ?? "messages";
var intervalSeconds = int.Parse(configuration["Feeder:IntervalSeconds"] ?? "5");
var messagesPerBatch = int.Parse(configuration["Feeder:MessagesPerBatch"] ?? "1");

Console.WriteLine($"Kafka Feeder starting...");
Console.WriteLine($"Bootstrap Servers: {bootstrapServers}");
Console.WriteLine($"Topic: {topic}");
Console.WriteLine($"Interval: {intervalSeconds} seconds");
Console.WriteLine($"Messages per batch: {messagesPerBatch}");

var services = new ServiceCollection();

services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { bootstrapServers })
        .CreateTopicIfNotExists(topic, 1, 1)
        .AddProducer(
            "kafka-feeder-producer",
            producer => producer
                .DefaultTopic(topic)
                .AddMiddlewares(middlewares => middlewares
                    .AddSerializer<JsonCoreSerializer>()
                )
        )
    )
);

var serviceProvider = services.BuildServiceProvider();
var bus = serviceProvider.CreateKafkaBus();
await bus.StartAsync();

var producer = serviceProvider.GetRequiredService<IProducerAccessor>()
    .GetProducer("kafka-feeder-producer");

var random = new Random();
var messageCount = 0;

var adjectives = new[] { "Amazing", "Brilliant", "Creative", "Dynamic", "Efficient", "Fast", "Great", "Happy", "Intelligent", "Joyful" };
var nouns = new[] { "Message", "Data", "Event", "Signal", "Notification", "Update", "Alert", "Record", "Entry", "Packet" };

Console.WriteLine("Feeder is running. Press Ctrl+C to stop.");

try
{
    while (true)
    {
        for (int i = 0; i < messagesPerBatch; i++)
        {
            var message = new KafkaMessage
            {
                Id = $"msg-{Guid.NewGuid()}",
                Content = $"{adjectives[random.Next(adjectives.Length)]} {nouns[random.Next(nouns.Length)]} #{++messageCount}",
                Timestamp = DateTime.UtcNow
            };

            await producer.ProduceAsync(topic, message.Id, message);
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Sent message {messageCount}: {message.Id} - {message.Content}");
        }

        await Task.Delay(TimeSpan.FromSeconds(intervalSeconds));
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Feeder stopping...");
}
finally
{
    await bus.StopAsync();
    Console.WriteLine("Feeder stopped.");
}
