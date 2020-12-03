from pymongo import MongoClient
from pymongo.errors import ConnectionFailure, InvalidName
from abc import ABC,abstractmethod 
import json

class DAOAbstract(ABC):
    @abstractmethod
    def insert(self, collection, data):
        pass

class DAOMongoDB(DAOAbstract):
    def __init__(self, db=""):
        if db=="" or db is None:
            connect_url="mongodb://localhost"
        else:
            connect_url=db

        __client = MongoClient(connect_url)
        print("Client obtained")
        try:
            # The ismaster command is cheap and does not require auth.
            __client.admin.command('ismaster')
            self.db = __client["Swan"]
        except ConnectionFailure:
            print("Server not available")

    def delete_all(self, collection):
        self.db[collection].delete_many({})

    def insert(self, collection, data):
        try:
            self.db[collection].insert_one(data)
        except InvalidName:
            print("The collection doesn't exist")
    def update(self, collection, query, data):
        self.db[collection].update_one(query, {"$set": data})

    def show(self, collection):
        for x in self.db[collection].find():
            print(x)

    def insert_or_update(self, collection, query, data):
    
        results= list(self.db[collection].find( query ))
        if len(results)==0:
            print("Inserting...")
            self.insert(collection, data)
        else:
            print("Updating...")
            self.update(collection, query,data)