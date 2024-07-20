
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Worker.Models;
using System.Text;

namespace RabbitMQ.Worker.Services;
public sealed class RabbitMqConsumer : RabbitMqConnectionService, IRabbitMqConsumer
{
    internal IConnection connection;
    private readonly ILogger<RabbitMqConsumer> logger = null!;
    IModel channelMessge;
    public RabbitMqConsumer(ILogger<RabbitMqConnectionService> connectionLogger, IConfiguration configuration,
        ILogger<RabbitMqConsumer> logger) : base(connectionLogger, configuration)
    {
        connection = InitalizeRabbitMQ();
        this.logger = logger;
    }
    public void Call()
    {
        ConsumerMessage();
    }
    public void ConsumerMessage()
    {
        channelMessge = connection.CreateModel();

        channelMessge.ExchangeDeclare(exchange: RabbitMQQueueConfiguration.MessageExchange, type: "direct", durable: true, autoDelete: false);
        channelMessge.QueueDeclare(queue: RabbitMQQueueConfiguration.MessageQueue, durable: true, exclusive: false, autoDelete: false);

        channelMessge.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        EventingBasicConsumer messageConsumer = new EventingBasicConsumer(channelMessge);

        messageConsumer.Received += async (ch, ea) =>
        {
            
            // Hanle Message
            channelMessge.BasicAck(ea.DeliveryTag, false);
            await Task.CompletedTask;
        };

        channelMessge.BasicConsume(queue: RabbitMQQueueConfiguration.MessageQueue, autoAck: false, consumer: messageConsumer);

        channelMessge.CallbackException += MessageChannel_CallbackException;

        messageConsumer.ConsumerCancelled += MessageConsumer_ConsumerCancelled;
        messageConsumer.Shutdown += MessageConsumer_Shutdown;
        messageConsumer.Registered += MessageConsumer_Registered;
        messageConsumer.Unregistered += MessageConsumer_Unregistered;
    }

    private void MessageChannel_CallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        logger.LogError($"RabbitMq Consumer Message Channel Exception Message : {e.Exception.Message}");
    }

    private void MessageConsumer_Unregistered(object? sender, ConsumerEventArgs e)
    {
        logger.LogInformation($"RabbitMQ Consumer (MessageConsumer) Unregistered");
    }

    private void MessageConsumer_Registered(object? sender, ConsumerEventArgs e)
    {
        logger.LogInformation($"RabbitMQ Consumer (MessageConsumer) Registred");
    }

    private void MessageConsumer_Shutdown(object? sender, ShutdownEventArgs e)
    {
        logger.LogInformation($"RabbitMQ Consumer (MessageConsumer) Shutdowned");
    }

    private void MessageConsumer_ConsumerCancelled(object? sender, ConsumerEventArgs e)
    {
        logger.LogInformation($"RabbitMQ Consumer (MessageConsumer) Cancelled");
    }

    public override void Dispose()
    {
        connection.Dispose();
        base.Dispose();
    }

    
}
