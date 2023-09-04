import pika

def singleton(cls):
    instances = {}
    def get_instance(*args, **kwargs):
        if cls not in instances:
            instances[cls] = cls(*args, **kwargs)
        return instances[cls]
    return get_instance

@singleton
class RabbitMqService:

    def __init__(self, host):
        self.host = host
        self.connection_parameters = pika.ConnectionParameters('localhost')
        connection = pika.BlockingConnection(self.connection_parameters)
        self.channel = connection.channel()
    
    def get_channel(self):
        return self.channel
    
    def connect(self):
        connection = pika.BlockingConnection(self.connection_parameters)
        self.channel = connection.channel()



