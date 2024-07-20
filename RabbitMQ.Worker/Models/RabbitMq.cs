namespace RabbitMQ.Worker.Models;
public sealed class RabbitMq
{
    public string Hostname { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = string.Empty;
    public bool Ssl { get; set; }
    /// <summary>
    /// Belirli aralıklarda bağlantı durumunu kontrol eder.
    /// </summary>
    public int RequestedHeartbeat { get; set; }
    /// <summary>
    /// Bağlantı kesildiğinde otomatik olarak tekrar bağlanmaya çalışır
    /// </summary>
    public bool AutomaticRecoveryEnabled { get; set; } 
    /// <summary>
    /// Servis yeniden başladığında kuyrukları yeninden oluşturur.
    /// </summary>
    public bool TopologyRecoveryEnabled { get; set; }
    /// <summary>
    /// Bağlantı koptuğunda ne kadar süre beklenileceğini belirtir.
    /// </summary>
    public int NetworkRecoveryInterval { get; set; }
    public int ReTryCount { get; set; }
}
