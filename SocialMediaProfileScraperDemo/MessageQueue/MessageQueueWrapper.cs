using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SocialMediaProfileScraperDemo.MessageQueue;

public class MessageQueueWrapper
{
    private readonly IConnectionFactory _connectionFactory;

    public MessageQueueWrapper(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void QueueDeclare(string queueName, bool durable = true)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        
        channel.QueueDeclare(queue: queueName, durable: durable, exclusive: false, autoDelete: false, arguments: null);
    }

    public void BasicPublish(string queueName, string message, string exchange = "")
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        
        var bytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange, queueName, null, bytes);
    }

    public void BasicConsume(string queueName, EventHandler<BasicDeliverEventArgs> onReceivedEventHandler, bool autoAck = false)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += onReceivedEventHandler;
        
        channel.BasicConsume(queue: queueName, autoAck: autoAck, consumer: consumer);
    }

    public BasicGetResult? BasicGet(string queueName, bool autoAck = true)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        
        return channel.BasicGet(queue: queueName, autoAck: autoAck);
    }

    public void BasicAck(ulong deliveryTag)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        
        channel.BasicAck(deliveryTag, false);
    }
}