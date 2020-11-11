import requests
import time

class RequestsRetry:
    default_retries = 3
    default_delay = 1

    @staticmethod
    def get_retry_requests(url:str, headers:dict, retries:int = default_retries, delay:int = default_delay):
        response = requests.get(url=url, headers=headers)
        x = 1
        while response.status_code != 200 and response.status_code != 201 and x < 3:
            time.sleep(delay)
            response = requests.get(url=url, headers=headers)
            x = x + 1
            print(f'Ran attempt {x}')

        return response