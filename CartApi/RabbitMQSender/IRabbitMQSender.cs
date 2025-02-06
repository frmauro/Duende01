using MessageBus;

namespace CartApi.RabbitMQSender;

public interface IRabbitMQSender
{
    Task SendMessageAsync(BaseMessage baseMessage, string queueName);
}
