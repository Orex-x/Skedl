def singleton(cls):
    instances = {}
    def get_instance(*args, **kwargs):
        if cls not in instances:
            instances[cls] = cls(*args, **kwargs)
        return instances[cls]
    return get_instance

@singleton
class SingletonClass:
    def __init__(self, host):
        self.host = host
        print(f"Singleton instance created. {host}")

    def get(self):
        return self.host

instance1 = SingletonClass("1")
instance2 = SingletonClass("2")

print(instance1 is instance2)  # Выведет: True
print(instance1.get())
