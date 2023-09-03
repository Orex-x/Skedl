from fastapi import APIRouter, Depends, Header
from parsers.spgbu_parser import *
from services.rabbitmq_service import *
from typing import Annotated
import pika
from models.fastapi_body.link_model import *

router = APIRouter(prefix="/spbgu", tags=["spbgu"])
parser = SpbguParser("https://timetable.spbu.ru")
channel = RabbitMqService("localhost").get_channel()

@router.get("/")
async def ping():
    return "spbgu hello"

@router.get("/getGroups")
async def get_groups(reply_to: str = Header(None, convert_underscores=True)):
    if reply_to:
        await publish_groups(reply_to)
    else:
        return "Параметр Reply-To не указан"
    
@router.post("/getScheduleWeek")
async def get_schedule_week(link_model: LinkModel):
    return parser.get_schedule_week(link_model.link)



async def publish_groups(reply_to):
    async for result in parser.get_all_groups():
        print(result)
        channel.basic_publish('', routing_key=reply_to, body=result)
    
    properties = pika.BasicProperties(headers={'type': 'last', "queueName" : reply_to})
    channel.basic_publish('', routing_key=reply_to, body='', properties=properties)
