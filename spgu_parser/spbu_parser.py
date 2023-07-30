import requests
from bs4 import BeautifulSoup
from models.field_of_study import *
from models.base_link import *
from models.edu_programme import *
from models.schedule_week import *
from models.schedule_day import *
from models.schedule_lecture import *
import re


class Parser:

    def __init__(self, url):
        self.url = url
    
    def get_fields_of_study(self):
        r = requests.get(self.url)
        soup = BeautifulSoup(r.content, 'html.parser')
        work_panels = soup.find_all("div", class_="panel panel-default")
        panels = work_panels[1].find_all("li", class_="list-group-item")
        fields_of_study = [BaseLink(panel.text.strip(), panel.a.get('href')[1:]) for panel in panels]
        return fields_of_study
    

    def get_field_of_study(self, link):
        r = requests.get(self.url + "/" +  link)
        soup = BeautifulSoup(r.content, 'html.parser')
        fields = soup.find_all(attrs={"data-parent": "#accordion"})

        fields_of_study = []

        for field in fields:
            programm_work_place = soup.find(id=field.get('href')[1:])
            programm_panels = programm_work_place.find_all('li')[1:]
            
            #create field of study
            field_of_study = FieldOfStudy(field.text.strip(), [])
            for programm_panel in programm_panels:
                name = programm_panel.find(attrs={"class": "col-sm-5"}).text.strip()
                years = programm_panel.find_all(attrs={"class": "col-sm-1"})
                
                #create edu programme
                edu_programme = EduProgramme(name, [])
                for item in years:
                    #create base link
                    base_link = BaseLink("", "")
                    
                    if item.a and item.a.has_attr('href'):
                        link = item.a.get('href')
                        year = item.a.text.strip()
                        
                        base_link.name = year
                        base_link.link = link
                    else:
                        hidden_inputs = item.form.find_all(attrs={"type": "hidden"})
                        submit_input = item.form.find(attrs={"type": "submit"})
                        link = item.form.get("action") + "?"
                        for hidden_input in hidden_inputs:
                            link += hidden_input.get("name") + "=" + hidden_input.get("value") + "&"
                        name = submit_input.get("value")    

                        base_link.name = year
                        base_link.link = link
                    
                    edu_programme.years.append(base_link)
                
                field_of_study.edu_programmes.append(edu_programme)
            
            fields_of_study.append(field_of_study)

        return fields_of_study

    def get_groups(self, link, code):
        r = requests.get(self.url + "/" + link + "/StudyProgram/" + code)
        soup = BeautifulSoup(r.content, 'html.parser')

        ul_panel = soup.find(attrs={"class" : "panel-collapse nopadding nomargin"})
        link_panels = ul_panel.find_all('li')

        base_links = [] 

        for link_panel in link_panels:
            onclick_value = link_panel.div.get('onclick')
            pattern = r"\'(.*?)\'"
            match = re.search(pattern, onclick_value)

            link = match.group(1)
            name = link_panel.find(attrs={"class" : "col-sm-4"}).text
            base_link = BaseLink(name.strip(), link.strip())
            base_links.append(base_link)
        
        return base_links
    
    def getScheduleWeek(self, link):
        r = requests.get(self.url + link)
        soup = BeautifulSoup(r.content, 'html.parser')

        previous_week_link = soup.find(attrs={"class" : "prev-week"}).get('href')
        next_week_link = soup.find(attrs={"class" : "next-week"}).get('href')

        days_panel = soup.find(id='accordion').find_all(attrs={"class" : "panel panel-default"})

        schedule_week = ScheduleWeek(previous_week_link, next_week_link, []) 

        for day_panel in days_panel:  
            
            date = day_panel.find(attrs={"class" : "panel-title"}).text.strip()

            schedule_day = ScheduleDay(date, [])

            lectures_panel = day_panel.find('ul').find_all('li')

            for lecture_panel in lectures_panel:  

                time_panel = lecture_panel.find(attrs={"title" : "Time"})
                time = time_panel.span.text.strip() if time_panel.span else time_panel.text.strip()
            
                subject_panel = lecture_panel.find(attrs={"title" : "Subject"})
                subject = subject_panel.span.text.strip() if subject_panel.span else subject_panel.text.strip()
                
                locations_panel = lecture_panel.find(attrs={"title": "Locations"})
                locations = locations_panel.span.text.strip() if locations_panel.span else locations_panel.text.strip()

                teacher_panel = lecture_panel.find(attrs={"title" : "Teachers"})
                teacher = teacher_panel.span.text.strip() if teacher_panel.span else teacher_panel.text.strip()

                schedule_day.lectures.append(ScheduleLecture(time, subject, locations, teacher))
            
            schedule_week.days.append(schedule_day)
        
        return schedule_week

    def get_content(self):
        return requests.get(self.url)
