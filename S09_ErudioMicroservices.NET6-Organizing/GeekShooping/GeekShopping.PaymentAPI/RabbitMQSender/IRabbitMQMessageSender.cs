using GeekShopping.MessageBus;

namespace GeekShopping.PaymentAPI.RabbitMQSender;

public interface IRabbitMQMessageSender
{
    //By queue
    //void SendMessage(BaseMessage baseMessage, string queueName);

    //By exchange (fanout)
    void SendMessage(BaseMessage baseMessage);

    //By direct
}
