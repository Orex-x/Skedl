using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Skedl.DataStorage.Services.RabbitMqService.RabbitMqServices;

public class RabbitMqService : IRabbitMqService
{
    private IConnection _connection;
    private IModel _model;
    private readonly string _baseQueueName = "request_queue_data_storage";
    
    public RabbitMqService()
    {
        ConnectionFactory connection = new ConnectionFactory
        {
            HostName = "localhost"
        };
        connection.DispatchConsumersAsync = true;
        _connection = connection.CreateConnection();
        _model = _connection.CreateModel();

        StartBaseConsuming();
    }

    private async void StartBaseConsuming()
    {
        _model.QueueDeclare(_baseQueueName, 
            durable: false,
            exclusive: false, 
            autoDelete: false);
        
        var consumer = new AsyncEventingBasicConsumer(_model);
        
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var text = Encoding.UTF8.GetString(body);
            Console.WriteLine(text);
            await Task.CompletedTask;
        };
        
        _model.BasicConsume(queue: _baseQueueName, autoAck: true, consumer: consumer);
        await Task.CompletedTask;
    }
    
    public AsyncEventingBasicConsumer StartConsuming(string queueName,  AsyncEventHandler<BasicDeliverEventArgs> asyncEventHandler)
    {
        
        var consumer = new AsyncEventingBasicConsumer(_model);

        consumer.Received += asyncEventHandler;
        
        _model.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        return consumer;
    }

    public void StopConsuming(AsyncEventingBasicConsumer consumer)
    {
        foreach (var tag in  consumer.ConsumerTags)
        {
            _model.BasicCancel(tag);
        }
    }
    
    public QueueDeclareOk QueueDeclare(string queue = "", bool durable = false, bool exclusive = true,
        bool autoDelete = true, IDictionary<string, object>? arguments = null)
    {
        return _model.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
    }

    public IBasicProperties CreateBasicProperties()
    {
        return _model.CreateBasicProperties();
    }

    public void Publish(string routingKey, IBasicProperties basicProperties, string exchange, string message = "")
    {
        var body = Encoding.UTF8.GetBytes(message);
        _model.BasicPublish(exchange, routingKey, basicProperties, body);
    }
}