using GeekShopping.Email.Messages;
using GeekShopping.Email.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.Email.MessageConsumer;

#nullable disable
public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly EmailRepository _repository;
    private IConnection _connection;
    private IModel _channel;
    #region Default
    /*
    
    //*/
    #endregion

    #region Fanout
    /*
    private const string _exchangeName = "FanoutPaymentUpdateExchange";
    string _queueName = "";

    public RabbitMQPaymentConsumer(EmailRepository repository)
    {
        _repository = repository;
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Fanout);
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(_queueName, _exchangeName, "");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            UpdatePaymentResultMessage message = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);
            ProcessLogs(message).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };
        _channel.BasicConsume(_queueName, false, consumer);
        return Task.CompletedTask;
    }

    //*/
    #endregion

    #region Direct
    //*
    private const string _exchangeName = "DirectPaymentUpdateExchange";
    private const string _paymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";

    public RabbitMQPaymentConsumer(EmailRepository repository)
    {
        _repository = repository;
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(_paymentEmailUpdateQueueName, false, false, false, null);
        _channel.QueueBind(_paymentEmailUpdateQueueName, _exchangeName, "PaymentEmail");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            UpdatePaymentResultMessage message = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);
            ProcessLogs(message).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };
        _channel.BasicConsume(_paymentEmailUpdateQueueName, false, consumer);
        return Task.CompletedTask;
    }
    //*/
    #endregion

    private async Task ProcessLogs(UpdatePaymentResultMessage message)
    {
        try
        {
            await _repository.LogEmail(message);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
