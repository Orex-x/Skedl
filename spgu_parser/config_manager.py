import json

class ConfigManager:
    def __init__(self, file_path='appsettings.json'):
        self.file_path = file_path

    def read_config(self):
        # Загружаем данные из файла в формате JSON
        with open(self.file_path, 'r') as json_file:
            config_data = json.load(json_file)
        return config_data