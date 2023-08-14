from spbu_parser import Parser
import pika
import asyncio

parser = Parser("https://timetable.spbu.ru")




#methods
async def publish_groups(ch, prop):
    i = 0
    async for result in parser.get_all_groups():
        print(result)
        ch.basic_publish('', routing_key=prop.reply_to, body=result)
        i+=1
        if(i == 3):
            properties = pika.BasicProperties(headers={'type': 'last', "queueName" : prop.reply_to})
            ch.basic_publish('', routing_key=prop.reply_to, body=result, properties=properties)
            break

def on_request_get_all_groups(ch, method, prop, body):
    asyncio.run(publish_groups(ch, prop))

connection_parameters = pika.ConnectionParameters('localhost')

connection = pika.BlockingConnection(connection_parameters)

channel = connection.channel()

channel.queue_declare(queue='request_get_all_groups')

channel.basic_consume(queue='request_get_all_groups', on_message_callback=on_request_get_all_groups)

print("Starting Server")

channel.start_consuming()



