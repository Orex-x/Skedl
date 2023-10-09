from fastapi import APIRouter, Depends, Header
from parsers.spgbu_parser import *
from services.rabbitmq_service import *
from typing import Annotated
import pika
from models.fastapi_body.link_model import *
import os
from dotenv import load_dotenv
load_dotenv()

SPBGU_PARSER_BASE_URL = os.getenv('SPBGU_PARSER_BASE_URL')
RABBIT_MQ_HOST = os.getenv('RABBIT_MQ_HOST')
RABBIT_MQ_USERNAME = os.getenv('RABBIT_MQ_USERNAME')
RABBIT_MQ_PASSWORD = os.getenv('RABBIT_MQ_PASSWORD')

router = APIRouter(prefix="/spbgu", tags=["spbgu"])
parser = SpbguParser(SPBGU_PARSER_BASE_URL)
rabbit = RabbitMqService(host=RABBIT_MQ_HOST, username=RABBIT_MQ_USERNAME, password=RABBIT_MQ_PASSWORD)

MAX_RECONNECT_ATTEMPTS = 3

@router.get("/")
async def ping():
    return "spbgu hello"

@router.get("/getGroups")
async def get_groups(reply_to: str = Header(None, convert_underscores=True)):
    print('get groups')
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
        reconnect_attempts = 0
        while reconnect_attempts < MAX_RECONNECT_ATTEMPTS:
            try:
                rabbit.get_channel().basic_publish('', routing_key=reply_to, body=result)
            except pika.exceptions.StreamLostError:
                print(f"Соединение с RabbitMQ было потеряно. Попытка восстановления {reconnect_attempts + 1} из {MAX_RECONNECT_ATTEMPTS}...")
                # Пересоздаем соединение с RabbitMQ
                rabbit.connect()
                reconnect_attempts += 1
            else:
                # Соединение восстановлено успешно, сбрасываем счетчик попыток
                reconnect_attempts = 0
                break
        if reconnect_attempts == MAX_RECONNECT_ATTEMPTS:
            print(f"Превышено максимальное количество попыток подключения ({MAX_RECONNECT_ATTEMPTS}). Прекращение попыток.")

    

    
