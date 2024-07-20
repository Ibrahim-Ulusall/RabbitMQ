namespace RabbitMQ.Worker.Services;
public interface IRabbitMqConsumer
{
    void ConsumerMessage();
    void Call();
}
