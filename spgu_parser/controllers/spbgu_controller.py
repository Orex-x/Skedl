from fastapi import APIRouter, Depends, Header
from parsers.spgbu_parser import *
from services.rabbitmq_service import *
from typing import Annotated
import pika
import asyncio


router = APIRouter(prefix="/spbgu", tags=["spbgu"])
parser = SpbguParser("https://timetable.spbu.ru/")
channel = RabbitMqService("localhost").get_channel()

@router.get("/")
async def ping():
    return "spbgu hello"

@router.get("/getGroups")
async def get_groups(reply_to: str = Header(None, convert_underscores=True)):
   await publish_groups(reply_to)

#methods
async def publish_groups(reply_to):
    async for result in parser.get_all_groups():
        print(result)
        channel.basic_publish('', routing_key=reply_to, body=result)
    
    properties = pika.BasicProperties(headers={'type': 'last', "queueName" : reply_to})
    channel.basic_publish('', routing_key=reply_to, body=result, properties=properties)
