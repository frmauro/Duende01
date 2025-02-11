using MessageBus;
using OrderApi.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderApi.RabbitMQSender;

public class RabbitMQSender : IRabbitMQSender
{
    private readonly string _hostName;
    private readonly string _password;
    private readonly string _userName;
    private IConnection _connection;
    private bool _disposed;

    public RabbitMQSender()
    {
        _hostName = "localhost";
        _password = "guest";
        _userName = "guest";
    }
    public async Task SendMessageAsync(BaseMessage baseMessage, string queueName)
    {
        await EnsureConnectionAsync();
        using var channel = await _connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queueName, false, false, false, arguments: null);

        byte[] body = GetMessageAsByteArray(baseMessage);
        await channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body);
    }

    private byte[] GetMessageAsByteArray(BaseMessage message)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize<PaymentVO>((PaymentVO)message, options);
        var body = Encoding.UTF8.GetBytes(json);
        return body;
    }

    private async Task<IConnection> CreateConnectionAsync()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                Password = _password,
                UserName = _userName
            };
            using var connection = await factory.CreateConnectionAsync();
            return connection;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Could not create a connection to RabbitMQ", ex);
        }
    }

    private async Task EnsureConnectionAsync()
    {
        if (_connection == null)
        {
            _connection = await CreateConnectionAsync();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}


