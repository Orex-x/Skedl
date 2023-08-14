using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Skedl.DataStorage.Models.ApiModels;
using Skedl.DataStorage.Services.Data.DataGroup;

namespace Skedl.DataStorage.Services.Data;

public class DataCatcher : IDataCatcher
{

    private readonly IGroupService _groupService;
    public DataCatcher(IGroupService groupService)
    {
        _groupService = groupService;
    }
    
    public void UpdateGroups()
    {
        var backTask = new BackTask();
        
        backTask.Start(() =>
        {
            var factory = new ConnectionFactory() { HostName = "192.168.0.103" };
        
            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();
        
            var replyQueue = channel.QueueDeclare("", exclusive: true);
            channel.QueueDeclare("request_get_all_groups", exclusive: false, autoDelete: false);
        
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Reply Recieved: {message}");
                
                if(message.ToLower() == "stop") backTask.Stop();

                var baseLinks = JsonSerializer.Deserialize<List<BaseLink>>(message);
                _groupService.UpdateOrCreateGroups(baseLinks);
            };
        
            channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);

            var properties = channel.CreateBasicProperties();
            properties.ReplyTo = replyQueue.QueueName;
            properties.CorrelationId = Guid.NewGuid().ToString();

            var body = Encoding.UTF8.GetBytes("");

            Console.WriteLine($"Sending Request: {properties.CorrelationId}");

            channel.BasicPublish("", "request_get_all_groups", properties, body);
        });
    }
}