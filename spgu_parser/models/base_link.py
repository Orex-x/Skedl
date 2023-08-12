class BaseLink():
    def __init__(self, name, link):
        self.name = name
        self.link = link

    def to_dict(self):
        return {
            "name": self.name,
            "link": self.link
        }
