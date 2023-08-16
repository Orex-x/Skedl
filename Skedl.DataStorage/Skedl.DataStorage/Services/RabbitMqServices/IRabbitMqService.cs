using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Skedl.DataStorage.Services.RabbitMqService.RabbitMqServices;

public interface IRabbitMqService
{
    AsyncEventingBasicConsumer StartConsuming(string queueName,  AsyncEventHandler<BasicDeliverEventArgs> asyncEventHandler);

    void StopConsuming(AsyncEventingBasicConsumer consumer);

    void Publish(string routingKey, IBasicProperties? basicProperties = null, string exchange = "", string message = "");

    QueueDeclareOk QueueDeclare(string queue = "", bool durable = false, bool exclusive = true,
        bool autoDelete = true, IDictionary<string, object>? arguments = null);

    IBasicProperties CreateBasicProperties();
}