using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Worker.Models;
using System.Net.Sockets;

namespace RabbitMQ.Worker.Services;
public abstract class RabbitMqConnectionService : IRabbitMQConnectionService, IDisposable
{
    private readonly RabbitMq rabbitMq = null!;
    private IConnection connection = null!;
    private readonly ILogger<RabbitMqConnectionService> logger;
    private object lockObject = new object();
    public RabbitMqConnectionService(ILogger<RabbitMqConnectionService> logger, IConfiguration configuration)
    {
        this.logger = logger;
        rabbitMq = configuration.GetSection(nameof(RabbitMq)).Get<RabbitMq>() ??
            throw new ArgumentNullException($"appsettings.json {nameof(RabbitMq)} not found");

    }


    public IConnection InitalizeRabbitMQ()
    {
        lock (lockObject)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = rabbitMq.Username,
                Password = rabbitMq.Password,
                HostName = rabbitMq.Hostname,
                Port = rabbitMq.Port,
                VirtualHost = rabbitMq.VirtualHost,
                RequestedHeartbeat = TimeSpan.FromSeconds(rabbitMq.RequestedHeartbeat),
                TopologyRecoveryEnabled = rabbitMq.TopologyRecoveryEnabled,
                AutomaticRecoveryEnabled = rabbitMq.AutomaticRecoveryEnabled,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(rabbitMq.NetworkRecoveryInterval),
                Ssl = { Enabled = rabbitMq.Ssl }
            };

            Policy policy = Policy.Handle<SocketException>().Or<BrokerUnreachableException>()
                .WaitAndRetry(rabbitMq.ReTryCount, reTryAttempt => TimeSpan.FromSeconds(Math.Pow(2, reTryAttempt)), (exception, time) =>
                {
                    logger.LogError($"RabbitMq Producer Connection Failed.\n Message: {exception.Message}");
                });
            policy.Execute(() =>
            {
                connection = factory.CreateConnection();
            });

            if (connection.IsOpen)
            {
                connection.ConnectionShutdown += Connection_ConnectionShutdown;
                connection.ConnectionBlocked += Connection_ConnectionBlocked;
                connection.ConnectionUnblocked += Connection_ConnectionUnblocked;
                connection.CallbackException += Connection_CallbackException;
            }
            return connection;
        }
    }

    private void Connection_CallbackException(object? sender, Client.Events.CallbackExceptionEventArgs e)
    {
        logger.LogError($"RabbitMQ Producer Exception : {e.Exception.Message}");
    }

    private void Connection_ConnectionUnblocked(object? sender, EventArgs e)
    {
        logger.LogInformation("RabbitMq connection unblocked");
    }

    private void Connection_ConnectionBlocked(object? sender, Client.Events.ConnectionBlockedEventArgs e)
    {
        logger.LogInformation("RabbitMq connection blocked");
    }

    private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        logger.LogInformation("RabbitMQ connection shutdowned");
    }
    public virtual void Dispose()
    {
        connection.Dispose();
        GC.SuppressFinalize(this);
    }

}
