from fastapi import APIRouter, Depends, Header
from parsers.spgbu_parser import *
from services.rabbitmq_service import *
from typing import Annotated
import pika
from models.fastapi_body.link_model import *


parser = SpbguParser("https://timetable.spbu.ru")

cont = parser.get_content('/GSOM/StudentGroupEvents/Primary/367163')

print(cont)