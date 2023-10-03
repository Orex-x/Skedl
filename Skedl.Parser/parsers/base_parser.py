import requests
import time
import random
from requests.exceptions import RequestException, ConnectionError, Timeout, TooManyRedirects, HTTPError

class BaseParser:
    def __init__(self):
        self.session = requests.Session()

    def retry_request(self, url, retries=3, delay_range=(30, 60)):
        for _ in range(retries):
            try:
                response = self.session.get(url, headers={"Cookie":"_culture=ru"})
                response.raise_for_status()
                return response
            except (ConnectionError, Timeout, TooManyRedirects, HTTPError) as e:
                print(f"Error during request: {e}")
                print("Retrying after a delay...")
                time.sleep(random.uniform(*delay_range))
            except RequestException as e:
                print(f"Request error: {e}")
                print("Retrying after a delay...")
                time.sleep(random.uniform(*delay_range))
                break

        return None
