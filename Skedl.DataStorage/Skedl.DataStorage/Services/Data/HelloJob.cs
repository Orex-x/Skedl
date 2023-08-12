using System.Text;
using Quartz;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Skedl.DataStorage.Services;

public class HelloJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await Task.Run(() =>
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            var replyQueue = channel.QueueDeclare(queue: "", exclusive: true);

            channel.QueueDeclare("request_get_all_groups", exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);

                if (message == "stop")
                {
                    Console.WriteLine("connection close");
                    connection.Close();
                }
            };
            
            channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);
            
            var prop = channel.CreateBasicProperties();
            prop.ReplyTo = replyQueue.QueueName;
            prop.CorrelationId = Guid.NewGuid().ToString();
            
            var message = "get_all_groups";
            var body = Encoding.UTF8.GetBytes(message);
            
            channel.BasicPublish("", "request_get_all_groups", prop, body);
            
            Console.WriteLine("consuming");
        });
    }
}