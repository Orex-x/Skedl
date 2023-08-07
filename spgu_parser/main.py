
from fastapi import FastAPI, Body, status, Request, Depends, HTTPException
from fastapi.responses import JSONResponse, FileResponse
from spbu_parser import Parser
from fastapi_jwt_auth.exceptions import AuthJWTException
from pydantic import BaseModel
from fastapi_jwt_auth import AuthJWT
from config_manager import ConfigManager


parser = Parser("https://timetable.spbu.ru")
config_manager = ConfigManager()
config_data = config_manager.read_config()

app = FastAPI()

class User(BaseModel):
    username: str
    password: str

class Settings(BaseModel):
    authjwt_secret_key: str = config_data["Jwt"]["Key"]
    authjwt_decode_algorithms: set = {"HS512"}

@AuthJWT.load_config
def get_config():
    return Settings()

@app.exception_handler(AuthJWTException)
def authjwt_exception_handler(request: Request, exc: AuthJWTException):
    return JSONResponse(
        status_code=exc.status_code,
        content={"detail": exc.message}
    )

@app.get('/expired_time')
def expiredTime(Authorize: AuthJWT = Depends()):
    Authorize.jwt_required()
    return {"hello": "world"}


@app.get("/")
async def main():
    return FileResponse("public/index.html")


@app.get("/api/get_all_groups")
def get_all_groups():
    return parser.get_all_groups()

@app.get("/api/getFieldsOfStudy")
def get_fields_of_study():
    return parser.get_fields_of_study()

@app.get("/api/getFieldOfStudy/{link}")
def get_field_of_study(link):
    return parser.get_field_of_study(link)

@app.get("/api/getGroups")
async def get_groups(request: Request):
    data = await request.json()
    link = data.get("link")
    return parser.get_groups(link)

@app.put("/api/getScheduleWeek")
async def getScheduleWeek(request: Request):
    data = await request.json()
    link = data.get("link")
    return parser.getScheduleWeek(link)



if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)