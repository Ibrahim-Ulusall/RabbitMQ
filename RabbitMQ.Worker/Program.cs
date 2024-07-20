using RabbitMQ.Worker;
using RabbitMQ.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();
builder.Services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();

var host = builder.Build();
host.Run();
