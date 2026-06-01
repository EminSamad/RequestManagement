using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RequestManagement.Application.Interfaces;

namespace RequestManagement.Application.Services;

public class RabbitMQService : IRabbitMQService, IDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMQService(IConfiguration configuration)
    {
        var host = configuration["RabbitMQ:Host"];
        if (string.IsNullOrEmpty(host)) return;

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = host,
                UserName = configuration["RabbitMQ:Username"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RabbitMQ connection failed: {ex.Message}");
        }
    }

//     public void PublishMessageAsync(string queueName, string message)
//     {
//         _channel.QueueDeclareAsync(
//             queue: queueName,
//             durable: true,
//             exclusive: false,
//             autoDelete: false).GetAwaiter().GetResult();

//         var body = Encoding.UTF8.GetBytes(message);

//         _channel.BasicPublishAsync(
//             exchange: string.Empty,
//             routingKey: queueName,
//             body: body).GetAwaiter().GetResult();
//     }

//     public void ConsumeMessageAsync(string queueName, Func<string, Task> onMessage)
//     {
//         _channel.QueueDeclareAsync(
//             queue: queueName,
//             durable: true,
//             exclusive: false,
//             autoDelete: false).GetAwaiter().GetResult();

//         var consumer = new AsyncEventingBasicConsumer(_channel);
//         consumer.ReceivedAsync += async (model, ea) =>
// {
//     var body = ea.Body.ToArray();
//     var message = Encoding.UTF8.GetString(body);

//     await onMessage(message);

//     await _channel.BasicAckAsync(ea.DeliveryTag, false);
// };
//         _channel.BasicConsumeAsync(
//             queue: queueName,
//             autoAck: false,
//             consumer: consumer).GetAwaiter().GetResult();
//     }

    public void Dispose()
    {
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _connection?.CloseAsync().GetAwaiter().GetResult();
    }

    public void PublishMessageAsync(string queueName, string message)
    {
        if (_channel == null) return;
        // ...
    }

    public void ConsumeMessageAsync(string queueName, Func<string, Task> onMessage)
    {
        if (_channel == null) return;
        // ...
    }


}