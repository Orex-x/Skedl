from fastapi import FastAPI, Body, Depends, HTTPException, status, Request
from fastapi.responses import JSONResponse, FileResponse
from spbu_parser import Parser
from fastapi_jwt_auth import AuthJWT
from fastapi_jwt_auth.exceptions import AuthJWTException
from pydantic import BaseModel

parser = Parser("https://timetable.spbu.ru")

app = FastAPI()

class Settings(BaseModel):
    authjwt_secret_key: str = "secret"

# exception handler for authjwt
# in production, you can tweak performance using orjson response
@app.exception_handler(AuthJWTException)
def authjwt_exception_handler(request: Request, exc: AuthJWTException):
    return JSONResponse(
        status_code=exc.status_code,
        content={"detail": exc.message}
    )

# protect endpoint with function jwt_required(), which requires
# a valid access token in the request headers to access.
@app.get('/expired_time')
def expiredTime(Authorize: AuthJWT = Depends()):
    Authorize.jwt_required()

    expired_time = Authorize._get_expired_time()
    return {"expired_time": expired_time}



@app.get("/")
async def main():
    return FileResponse("public/index.html")

@app.get("/api/getFieldsOfStudy")
def get_fields_of_study():
    return parser.get_fields_of_study()

@app.get("/api/getFieldOfStudy/{link}")
def get_field_of_study(link):
    return parser.get_field_of_study(link)

@app.get("/api/getGroups/{link}/{code}")
def get_groups(link, code):
    return parser.get_groups(link, code)


@app.put("/api/getScheduleWeek")
async def getScheduleWeek(request: Request):
    data = await request.json()
    link = data.get("link")
    return parser.getScheduleWeek(link)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)