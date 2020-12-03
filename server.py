from twisted.internet import reactor, protocol
from twisted.internet.protocol import ServerFactory as SFactory
from twisted.internet.endpoints import TCP4ServerEndpoint
import importlib
import json
DAO = importlib.import_module("DAO")

class Server(protocol.Protocol):
    def __init__(self, clients:set, db):
        self.db=db
        self.clients=clients

    def connectionMade(self):
        print("New Connection")
        self.clients.add(self)
        
    def send_message(self, data : str, where=None):
        if where:
            where.transport.write(data.encode("utf-8"))
        else:
            self.transport.write("The data was saved in the database")
        #__db.insert("Player", data)
        #self.transport.write("Hello from server".encode("utf-8"))
        #self.transport.loseConnection()

    def dataReceived(self, data):
        data= data.decode('utf-8')
        print("Recieved from Client: {}".format(data))
        json_data = json.loads(data)
        #print("json data {} ".format(type(json_data)))
        if 'data' in json_data.keys():
            if 'actualRoom' in json_data["data"].keys():
                self.db.insert_or_update("Player", { "id": json_data["id"] }, { "id":json_data["id"], "actualRoom": json_data["data"]["actualRoom"]} )

            if 'key_pressed' in json_data["data"].keys():
                self.db.show("Player")

        for client in self.clients:
            self.send_message(data, client)



class ServerFactory(SFactory):
    def __init__(self, db):
        self.db=db
        self.clients=set()

    def buildProtocol(self, addr):
        return Server(self.clients,self.db)


if __name__ == '__main__':
    db = DAO.DAOMongoDB()
    db.delete_all("Player")
    print("running...")
    endpoint = TCP4ServerEndpoint(reactor, 2000)
    endpoint.listen(ServerFactory(db))
    reactor.run()