using System.Text;
using System.Text.Json;
using Quartz;
using RabbitMQ.Client.Events;
using Skedl.DataCatcher.Models.DB;
using Skedl.DataCatcher.Models.DTO;
using Skedl.DataCatcher.Services.DatabaseContexts;
using Skedl.DataCatcher.Services.RabbitMqServices;

namespace Skedl.DataCatcher.Services.Quartz;

public class SpbguCatcherJob : IJob
{
    private readonly IRabbitMqService _rabbitMqService;

    private Dictionary<string, AsyncEventingBasicConsumer> _replyQueues;

    private DatabaseSpbgu? _db;
    
    public SpbguCatcherJob(IRabbitMqService rabbitMqService, DatabaseSpbgu db)
    {
        _replyQueues = new Dictionary<string, AsyncEventingBasicConsumer>();
        _rabbitMqService = rabbitMqService;
        _db = db;
    }
    
    public Task Execute(IJobExecutionContext context)
    {
        Register("request_get_all_groups", AsyncEventHandler);
      
        return Task.CompletedTask;
    }
    
    private void Register(string routingKey, AsyncEventHandler<BasicDeliverEventArgs> asyncEventHandler)
    {
        var replyQueue = _rabbitMqService.QueueDeclare(exclusive: true);

        var consumer =_rabbitMqService.StartConsuming(replyQueue.QueueName, asyncEventHandler);

        var guid = Guid.NewGuid().ToString();
        
        var prop = _rabbitMqService.CreateBasicProperties();
        prop.ReplyTo = replyQueue.QueueName;
        prop.CorrelationId = guid;
        
        _rabbitMqService.Publish(routingKey, basicProperties: prop);
        
        _replyQueues.Add(replyQueue.QueueName, consumer);
        
        Console.WriteLine($"ReplyTo {replyQueue.QueueName}");
    }

    private void CheckStop(BasicDeliverEventArgs ea)
    {
        if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.TryGetValue("type", out var headerType))
        {
            var type = Encoding.UTF8.GetString((byte[]) headerType);
                    
            if (type == "last")
            {
                if (ea.BasicProperties.Headers.TryGetValue("queueName", out var headerQueueName))
                {
                    var queueName = Encoding.UTF8.GetString((byte[]) headerQueueName);
                    _rabbitMqService.StopConsuming(_replyQueues[queueName]);
                    Console.WriteLine($"StopConsuming {queueName}");
                }
                else
                {
                    Console.WriteLine($"Сообщение без queueName");
                }
            }
        }
    }

    private async Task AsyncEventHandler(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            CheckStop(ea);
        
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var list = JsonSerializer.Deserialize<List<GroupDto>>(message);

            foreach (var groupDto in list)
            {
            
                var a = _db.Groups.FirstOrDefault(x => x.Name == groupDto.Name);
                if (a == null)
                {
                    var group = new Group
                    {
                        Name = groupDto.Name,
                        Link = groupDto.Link
                    };

                    await _db.Groups.AddAsync(group);
                }
                else
                {
                    a.Link = groupDto.Link;
                    _db.Groups.Update(a);
                }

                await _db.SaveChangesAsync();
            }
        
            Console.WriteLine(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
     
        await Task.CompletedTask;
    }
}