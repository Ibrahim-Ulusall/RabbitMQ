using RabbitMQ.Client;
using RabbitMQ.Worker.Models;
using System.Text;

namespace RabbitMQ.Worker.Services;
public sealed class RabbitMQProducer : RabbitMqConnectionService, IRabbitMQProducer
{
    internal IConnection connection;
    public RabbitMQProducer(ILogger<RabbitMqConnectionService> logger,
        IConfiguration configuration) : base(logger, configuration)
    {
        connection = InitalizeRabbitMQ();
    }

    public void PublishMessage(string message)
    {
        using IModel channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: RabbitMQQueueConfiguration.MessageExchange,
                                type: "direct", durable: true, autoDelete: false);

        channel.QueueDeclare(queue: RabbitMQQueueConfiguration.MessageQueue,
                             durable: true, exclusive: false, autoDelete: false);

        channel.QueueBind(queue: RabbitMQQueueConfiguration.MessageQueue,
                          exchange: RabbitMQQueueConfiguration.MessageExchange,
                          routingKey: RabbitMQQueueConfiguration.MessageQueue);

        IBasicProperties properties = channel.CreateBasicProperties();

        properties.Persistent = true;
        properties.Expiration = "5000";

        channel.BasicPublish(exchange: RabbitMQQueueConfiguration.MessageExchange,
                             routingKey: RabbitMQQueueConfiguration.MessageQueue,
                             basicProperties: properties,
                             body: Encoding.UTF8.GetBytes(message));
    }



}
