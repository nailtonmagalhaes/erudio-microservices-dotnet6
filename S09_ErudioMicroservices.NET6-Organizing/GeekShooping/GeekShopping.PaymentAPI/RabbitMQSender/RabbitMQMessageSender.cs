using GeekShopping.MessageBus;
using GeekShopping.PaymentAPI.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.RabbitMQSender;

#nullable disable
public class RabbitMQMessageSender : IRabbitMQMessageSender
{
    private readonly ILogger<RabbitMQMessageSender> _logger;
    private readonly string _hostName = "localhost";
    private readonly string _password = "guest";
    private readonly string _userName = "guest";
    private IConnection _connection;

    public RabbitMQMessageSender(ILogger<RabbitMQMessageSender> logger)
    {
        _logger = logger;
    }

    public void SendMessage(BaseMessage message, string queueName)
    {
        _logger.LogInformation($"Starting {nameof(RabbitMQMessageSender)}-{nameof(SendMessage)}");
        if (ConnctionExists())
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);
            byte[] body = GetMessageAsClientArray(message);
            _logger.LogInformation($"{nameof(RabbitMQMessageSender)}-{nameof(SendMessage)} sanding message to queue {queueName}!");
            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
            _logger.LogInformation($"{nameof(RabbitMQMessageSender)}-{nameof(SendMessage)} message sent to queue {queueName}!");
        }
        _logger.LogInformation($"Ending {nameof(RabbitMQMessageSender)}-{nameof(SendMessage)}");
    }

    private byte[] GetMessageAsClientArray(BaseMessage message)
    {
        var options = new JsonSerializerOptions() { WriteIndented = true };
        var json = JsonSerializer.Serialize((UpdatePaymentResultMessage)message, options);
        return Encoding.UTF8.GetBytes(json);
    }

    private bool CreateConnection()
    {
        _logger.LogInformation($"Starting {nameof(RabbitMQMessageSender)}-{nameof(CreateConnection)}");
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };
            _connection = factory.CreateConnection();
            _logger.LogInformation($"Ending {nameof(RabbitMQMessageSender)}-{nameof(CreateConnection)}");
            return _connection != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has occurred while trying to create the connection!");
            throw;
        }
    }

    private bool ConnctionExists()
    {
        if (_connection != null) return true;
        return CreateConnection();
    }
}
