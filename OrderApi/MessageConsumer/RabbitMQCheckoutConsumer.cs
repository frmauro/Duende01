using Microsoft.EntityFrameworkCore.Metadata;
using OrderApi.Messages;
using OrderApi.Model;
using OrderApi.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


namespace OrderApi.MessageConsumer;

public class RabbitMQCheckoutConsumer(OrderRepository repository) : BackgroundService
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
        await channel.QueueDeclareAsync(queue: "checkoutqueue", false, false, false, arguments: null);


        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var vo = JsonSerializer.Deserialize<CheckoutHeaderVO>(content);
            ProcessOrder(vo).GetAwaiter().GetResult();
            await channel.BasicAckAsync(evt.DeliveryTag, false);
        };
        await channel.BasicConsumeAsync("checkoutqueue", false, consumer);
    }

    private async Task ProcessOrder(CheckoutHeaderVO vo)
    {
        OrderHeader order = new()
        {
            UserId = vo.UserId,
            FirstName = vo.FirstName,
            LastName = vo.LastName,
            OrderDetails = new List<OrderDetail>(),
            CardNumber = vo.CardNumber,
            CouponCode = vo.CouponCode,
            CVV = vo.CVV,
            DiscountAmount = vo.DiscountAmount,
            Email = vo.Email,
            ExpiryMonthYear = vo.ExpiryMothYear,
            OrderTime = DateTime.Now,
            PurchaseAmount = vo.PurchaseAmount,
            PaymentStatus = false,
            Phone = vo.Phone,
            DateTime = vo.DateTime
        };

        foreach (var details in vo.CartDetails)
        {
            OrderDetail detail = new()
            {
                ProductId = details.ProductId,
                ProductName = details.Product.Name,
                Price = details.Product.Price,
                Count = details.Count,
            };
            order.CartTotalItens += details.Count;
            order.OrderDetails.Add(detail);
        }
        try
        {
            await repository.AddOrder(order);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
