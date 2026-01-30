using KafkaWorkerService;
using KafkaFlow;
using KafkaFlow.Serializer;

var builder = Host.CreateApplicationBuilder(args);

var kafkaSettings = builder.Configuration.GetSection("Kafka").Get<KafkaSettings>() 
    ?? throw new InvalidOperationException("Kafka settings not configured");

builder.Services.AddKafka(kafka => kafka
    .UseConsoleLog()
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { kafkaSettings.BootstrapServers })
        .AddConsumer(consumer => consumer
            .Topic(kafkaSettings.Topic)
            .WithGroupId(kafkaSettings.GroupId)
            .WithBufferSize(100)
            .WithWorkersCount(10)
            .AddMiddlewares(middlewares => middlewares
                .AddDeserializer<JsonCoreDeserializer>()
                .AddTypedHandlers(handlers => handlers
                    .AddHandler<MessageHandler>()
                )
            )
        )
    )
);

var host = builder.Build();

var bus = host.Services.CreateKafkaBus();
await bus.StartAsync();

await host.RunAsync();

await bus.StopAsync();
