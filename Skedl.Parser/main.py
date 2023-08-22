from fastapi import FastAPI, Body, status, Request, Depends
from fastapi.responses import JSONResponse, FileResponse
from controllers import spbgu_controller
from services.rabbitmq_service import * 

channel = RabbitMqService("localhost")

app = FastAPI()
app.include_router(spbgu_controller.router)


@app.get("/parser")
async def root():
    return {"message": "Hello Bigger Applications!"}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=5000)