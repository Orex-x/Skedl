using System.Text;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Skedl.DataStorage.Services.RabbitMq;

namespace Skedl.DataStorage.Services;

public class HelloJob : IJob
{

    private readonly IRabbitMqService _rabbitMqService;
    public HelloJob(IRabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        
    }

}