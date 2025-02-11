using MessageBus;

namespace OrderApi.RabbitMQSender;

public interface IRabbitMQSender
{
    Task SendMessageAsync(BaseMessage baseMessage, string queueName);
}
