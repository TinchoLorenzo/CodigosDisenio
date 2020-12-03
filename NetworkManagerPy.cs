using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading;
using System.Linq;
using RabbitMQ.Client;


//Tiene que ser singleton
public class NetworkManagerPy : MonoBehaviour{

    public static NetworkManagerPy Instance;
    private int id = new System.Random().Next(1,100);
    bool running=false;

    void Awake(){
        Instance=this;
    }
    private void connect(Action<bool> callback){

    }

    void Start(){

    }

    public void SendData(string routingKey, string message){
        
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "topic_logs", routingKey: routingKey, basicProperties: null, body: body);
            Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
        }
    }

    void Update(){

    }


    private void onApplicationQuit(){
        //tcpClient.EndConnect();
        //tcpClient.Close();
        running = false;
    }

}
