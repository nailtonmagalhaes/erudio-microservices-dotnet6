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

    #region Queue
    /*

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
    //*/
    #endregion

    #region Fanout
    /*

    private const string _exchangeName = "FanoutPaymentUpdateExchange";

    public void SendMessage(BaseMessage message)
    {
        _logger.LogInformation($"Starting {nameof(RabbitMQMessageSender)}-{nameof(SendMessage)}");
        if (ConnctionExists())
        {
            byte[] body = GetMessageAsClientArray(message);
            using var channel = _connection.CreateModel();
            _logger.LogInformation($"{nameof(RabbitMQMessageSender)}-{nameof(SendMessage)} sanding message to exchange {_exchangeName}!");

            _logger.LogInformation($"{nameof(RabbitMQMessageSender)}-{nameof(SendMessage)} message sent to exchange {_exchangeName}!");
            channel.ExchangeDeclare(_exchangeName, ExchangeType.Fanout, durable: false);
            channel.BasicPublish(exchange: _exchangeName, "", basicProperties: null, body: body);
        }
        _logger.LogInformation($"Ending {nameof(RabbitMQMessageSender)}-{nameof(SendMessage)}");
    }
    //*/
    #endregion

    #region Direct
    //*
    private const string _exchangeName = "DirectPaymentUpdateExchange";
    private const string _paymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
    private const string _paymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

    public void SendMessage(BaseMessage message)
    {
        _logger.LogInformation($"Starting {nameof(RabbitMQMessageSender)}-{nameof(SendMessage)}");
        if (ConnctionExists())
        {
            byte[] body = GetMessageAsClientArray(message);
            using var channel = _connection.CreateModel();

            channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: false);

            channel.QueueDeclare(_paymentEmailUpdateQueueName, false, false, false, null);
            channel.QueueDeclare(_paymentOrderUpdateQueueName, false, false, false, null);

            channel.QueueBind(_paymentEmailUpdateQueueName, _exchangeName, "PaymentEmail");
            channel.QueueBind(_paymentOrderUpdateQueueName, _exchangeName, "PaymentOrder");

            _logger.LogInformation($"{nameof(RabbitMQMessageSender)}-{nameof(SendMessage)} sanding message to queue PaymentEmail!");
            channel.BasicPublish(exchange: _exchangeName, "PaymentEmail", basicProperties: null, body: body);
            _logger.LogInformation($"{nameof(RabbitMQMessageSender)}-{nameof(SendMessage)} message sent to queue PaymentEmail!");

            _logger.LogInformation($"{nameof(RabbitMQMessageSender)}-{nameof(SendMessage)} sanding message to queue PaymentOrder!");
            channel.BasicPublish(exchange: _exchangeName, "PaymentOrder", basicProperties: null, body: body);
            _logger.LogInformation($"{nameof(RabbitMQMessageSender)}-{nameof(SendMessage)} message sent to queue PaymentOrder!");
        }
        _logger.LogInformation($"Ending {nameof(RabbitMQMessageSender)}-{nameof(SendMessage)}");
    }
    //*/
    #endregion

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
