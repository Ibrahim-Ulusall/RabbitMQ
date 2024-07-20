namespace RabbitMQ.Worker.Services;
public interface IRabbitMQProducer
{
    void PublishMessage(string message);
}
