using RabbitMQ.Client;

namespace RabbitMQ.Worker.Services;
public interface IRabbitMQConnectionService
{
    IConnection InitalizeRabbitMQ();
}
