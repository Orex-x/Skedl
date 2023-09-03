
using System.Text;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client.Events;
using Skedl.DataCatcher.Models.DB;
using Skedl.DataCatcher.Models.DTO;
using Skedl.DataCatcher.Services.DatabaseContexts;
using Skedl.DataCatcher.Services.HttpServices;
using Skedl.DataCatcher.Services.RabbitMqServices;

namespace Skedl.DataCatcher.Services.Quartz.Spbgu;

public class GroupCatchJob : IJob
{
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IHttpService _httpService;

    private readonly Dictionary<string, AsyncEventingBasicConsumer> _replyQueues;

    private readonly DatabaseSpbgu _db;
    
    public GroupCatchJob(IRabbitMqService rabbitMqService, DatabaseSpbgu db, IHttpService httpService)
    {
        _replyQueues = new Dictionary<string, AsyncEventingBasicConsumer>();
        _rabbitMqService = rabbitMqService;
        _httpService = httpService;
        _db = db;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var replyQueue = _rabbitMqService.QueueDeclare(exclusive: true);

        var consumer =_rabbitMqService.StartConsuming(replyQueue.QueueName, AsyncEventHandler);

        var headers = new Dictionary<string, string>
        {
            { "Reply-To", replyQueue.QueueName },
            { "User-Agent", "Skedl-DataCatcher" }
        };

        await _httpService.GetAsync("/spbgu/getGroups", headers);
        
        _replyQueues.Add(replyQueue.QueueName, consumer);
        
        Console.WriteLine($"ReplyTo {replyQueue.QueueName}");
      
        await Task.CompletedTask;
    }
    
    
   
    private async Task AsyncEventHandler(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            if (IsStopMessage(ea)) 
                return;

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var list = JsonConvert.DeserializeObject<List<GroupDto>>(message);

            foreach (var groupDto in list)
            {
            
                var a = _db.Groups.FirstOrDefault(x => x.Name == groupDto.Name);
                if (a == null)
                {
                     var link = groupDto.Link;
                     link = link.Replace("Online", "Primary");
                     link = link.Replace("Attestation", "Primary");
                     link = link.Replace("Final", "Primary");

                     var group = new Group
                    {
                        Name = groupDto.Name,
                        Link = link
                    };

                    await _db.Groups.AddAsync(group);
                }
                else
                {
                    var link = groupDto.Link;
                    link = link.Replace("Online", "Primary");
                    link = link.Replace("Attestation", "Primary");
                    link = link.Replace("Final", "Primary");
                    
                    a.Link = link;
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
    
    private bool IsStopMessage(BasicDeliverEventArgs ea)
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
                    return true;
                }
                else
                {
                    Console.WriteLine($"Сообщение без queueName");
                }
            }
        }

        return false;
    }

}