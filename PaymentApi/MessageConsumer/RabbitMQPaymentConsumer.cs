using PaymentApi.Messages;
using PaymentApi.RabbitMQSender;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


namespace PaymentApi.MessageConsumer;

public class RabbitMQPaymentConsumer(
    IPaymentProcessor paymentProcessor, 
    IRabbitMQSender rabbitMQSender) : BackgroundService
{
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
        await channel.QueueDeclareAsync(queue: "orderpaymentprocessqueue", false, false, false, arguments: null);


        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var vo = JsonSerializer.Deserialize<PaymentMessage>(content);
            ProcessPaymentAsync(vo).GetAwaiter().GetResult();
            await channel.BasicAckAsync(evt.DeliveryTag, false);
        };
        await channel.BasicConsumeAsync("orderpaymentprocessqueue", false, consumer);
    }

    private async Task ProcessPaymentAsync(PaymentMessage payment)
    {
        var result = paymentProcessor.PaymentProcessor();

        UpdatePaymentResultMessage paymentResult = new()
        {
            Status = result,
            OrderId = payment.OrderId,
            Email = payment.Email
        };

        try
        {
       
            await rabbitMQSender.SendMessageAsync(paymentResult, "orderpaymentresultqueue");
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
