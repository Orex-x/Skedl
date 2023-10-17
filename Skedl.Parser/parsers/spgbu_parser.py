import requests
from bs4 import BeautifulSoup
from models.field_of_study import *
from models.base_link import *
from models.edu_programme import *
from models.schedule_lecture_status import ScheduleLectureStatus
from models.schedule_week import *
from models.schedule_day import *
from models.schedule_lecture import *
from .base_parser import *
import re
import time
import json
import random


class SpbguParser(BaseParser):

    def __init__(self, url):
        super().__init__()
        self.url = url
    
    def get_fields_of_study(self):
        r = self.retry_request(self.url)
        soup = BeautifulSoup(r.content, 'html.parser')
        work_panels = soup.find_all("div", class_="panel panel-default")
        panels = work_panels[0].find_all("li", class_="list-group-item")
        fields_of_study = [BaseLink(panel.text.strip(), panel.a.get('href')[1:]) for panel in panels]
        return fields_of_study
    

    def get_field_of_study(self, link):
        r = self.retry_request(self.url + "/" +  link)
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

                        base_link.name = name
                        base_link.link = link
                    
                    edu_programme.years.append(base_link)
                
                field_of_study.edu_programmes.append(edu_programme)
            
            fields_of_study.append(field_of_study)

        return fields_of_study

    def get_groups(self, link):
        r = self.retry_request(self.url + link)
        soup = BeautifulSoup(r.content, 'html.parser')

        ul_panel = soup.find(attrs={"class" : "panel-collapse nopadding nomargin"})

        if ul_panel:

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



    def get_groups_with_retry(self, link):
            response = self.retry_request(self.url + link)
            if response:
                soup = BeautifulSoup(response.content, 'html.parser')

                ul_panel = soup.find(attrs={"id" : ["studentGroupsForCurrentYear"]})
                
                if ul_panel:

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
            return None
    

    def get_schedule_week(self, link):
        r = self.retry_request(self.url + link)
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

                moreinfo = lecture_panel.find_all(attrs={"class" : "moreinfo"})
                hoverable = lecture_panel.find_all(attrs={"class" : "hoverable"})

                time_panel = moreinfo[0]
                if not time_panel: continue
                time = time_panel.span.text.strip() if time_panel.span else time_panel.text.strip()

                subject_panel = moreinfo[1]
                if not subject_panel: continue
                subject = subject_panel.span.text.strip() if subject_panel.span else subject_panel.text.strip()
                
                locations_panel = hoverable[0]
                if not locations_panel: continue
                locations = locations_panel.span.text.strip() if locations_panel.span else locations_panel.text.strip()

                teacher_panel = hoverable[1]
                if not teacher_panel: continue
                teacher = teacher_panel.span.text.strip() if teacher_panel.span else teacher_panel.text.strip()

                time_panel_class = time_panel.get('class')
                subject_panel_class = subject_panel.get('class')
                locations_panel_class = locations_panel.get('class')
                teacher_panel_class = teacher_panel.get('class')

                status = ScheduleLectureStatus.SCHEDULED

                if "cancelled" in teacher_panel_class:
                    status = ScheduleLectureStatus.CANCELLATION
                
                if "changed" in time_panel_class:
                    status = ScheduleLectureStatus.REPLACEMENT
                
                if "changed" in subject_panel_class:
                    status = ScheduleLectureStatus.REPLACEMENT
                
                if "changed" in locations_panel_class:
                    status = ScheduleLectureStatus.REPLACEMENT
                
                if "changed" in teacher_panel_class:
                    status = ScheduleLectureStatus.REPLACEMENT
                    
                schedule_day.lectures.append(ScheduleLecture(time, subject, locations, teacher, status))
            
            schedule_week.days.append(schedule_day)
        
        return schedule_week


    async def get_all_groups(self):
        fields_of_study = self.get_fields_of_study()
        for item_filed_of_study in fields_of_study:

            filed = self.get_field_of_study(item_filed_of_study.link)
            for item_filed in filed:

                edu_programmes = item_filed.edu_programmes
                for edu_programme_item in edu_programmes:

                    years = edu_programme_item.years
                    for year_item in years:
                        groups = self.get_groups_with_retry(year_item.link)
                        print('get groups')
                        if(groups):
                            link_dicts = [link.to_dict() for link in groups]
                            json_string = json.dumps(link_dicts, indent=4)
                            print(json_string)
                            yield json_string
                        time.sleep(random.uniform(5, 15))

    def get_content(self, url):
        r = self.retry_request(self.url + url)
        return BeautifulSoup(r.content, 'html.parser')
