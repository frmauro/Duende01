using MessageBus;

namespace PaymentApi.RabbitMQSender;

public interface IRabbitMQSender
{
    Task SendMessageAsync(BaseMessage baseMessage, string queueName);
}
