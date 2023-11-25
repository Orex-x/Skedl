from fastapi import FastAPI, Body, status, Request, Depends
from fastapi.responses import JSONResponse, FileResponse
from controllers import spbgu_controller
from services.rabbitmq_service import * 
from models.fastapi_body.link_model import *
import os
from dotenv import load_dotenv
from fastapi import FastAPI

load_dotenv()

RABBIT_MQ_HOST = os.getenv('RABBIT_MQ_HOST')
RABBIT_MQ_USERNAME = os.getenv('RABBIT_MQ_USERNAME')
RABBIT_MQ_PASSWORD = os.getenv('RABBIT_MQ_PASSWORD')

channel = RabbitMqService(host=RABBIT_MQ_HOST, username=RABBIT_MQ_USERNAME, password=RABBIT_MQ_PASSWORD)

app = FastAPI()
app.include_router(spbgu_controller.router)


@app.get("/")
async def root():
    return {"message": "Hello Bigger Applications!"}


@app.post("/getScheduleWeek")
async def get_schedule_week(link_model: LinkModel):
    return link_model.link

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=5004)