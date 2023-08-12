from spbu_parser import Parser
import pika
import asyncio

parser = Parser("https://timetable.spbu.ru")

#methods
async def publish_groups(ch, prop):
    async for result in parser.get_all_groups():
        print(result)
        ch.basic_publish('', routing_key=prop.reply_to, body=result)

def on_request_get_all_groups(ch, method, prop, body):
    asyncio.run(publish_groups(ch, prop))

connection_parameters = pika.ConnectionParameters('localhost')

connection = pika.BlockingConnection(connection_parameters)

channel = connection.channel()

channel.queue_declare(queue='request_get_all_groups')

channel.basic_consume(queue='request_get_all_groups', auto_ack=True,
    on_message_callback=on_request_get_all_groups)

print("Starting Server")

channel.start_consuming()



