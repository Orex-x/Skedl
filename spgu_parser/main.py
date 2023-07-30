from fastapi import FastAPI, Body, status, Request
from fastapi.responses import JSONResponse, FileResponse
from spbu_parser import Parser

parser = Parser("https://timetable.spbu.ru")

app = FastAPI()

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