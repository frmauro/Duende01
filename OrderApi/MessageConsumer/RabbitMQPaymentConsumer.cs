using Microsoft.EntityFrameworkCore.Metadata;
using OrderApi.Messages;
using OrderApi.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderApi.MessageConsumer;

public class RabbitMQPaymentConsumer(OrderRepository _repository) : BackgroundService
{
    private const string exchangeName = "FanoutPaymentUpdateExchange";
    string queueName = "";
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
        var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout);
        await channel.QueueDeclareAsync();
        await channel.QueueBindAsync(queueName, exchangeName, "");

        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var vo = JsonSerializer.Deserialize<UpdatePaymentResultVO>(content);
            UpdatePaymentStatus(vo).GetAwaiter().GetResult();
            await channel.BasicAckAsync(evt.DeliveryTag, false);
        };
        await channel.BasicConsumeAsync(queueName, false, consumer);

        // Mantém o serviço rodando enquanto a aplicação estiver ativa
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(2000, stoppingToken); // Evita loop infinito sem necessidade
        }
    }

    private async Task UpdatePaymentStatus(UpdatePaymentResultVO vo)
    {
        try
        {
            await _repository.UpdateOrderPaymentStatus(vo.OrderId, vo.Status);
        }
        catch (Exception)
        {
            //Log
            throw;
        }
    }
}
