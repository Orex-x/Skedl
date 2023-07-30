import requests
from bs4 import BeautifulSoup
from models.field_of_study import FieldOfStudy
from models.base_link import *
from models.edu_programme import *

from models.schedule_week import *
from models.schedule_day import *
from models.schedule_lecture import *


import re

r = requests.get("https://timetable.spbu.ru/")
soup = BeautifulSoup(r.content, 'html.parser')

work_panels = soup.find_all("div", class_="panel panel-default")
panels = work_panels[1].find_all("li", class_="list-group-item")

fields_of_study = [BaseLink(panel.text.strip(), panel.a.get('href')[1:]) for panel in panels]

print(fields_of_study)
