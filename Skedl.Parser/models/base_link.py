class BaseLink():
    def __init__(self, name, link):
        self.name = name
        self.link = link

    def to_dict(self):
        return {
            "Name": self.name,
            "Link": self.link
        }
