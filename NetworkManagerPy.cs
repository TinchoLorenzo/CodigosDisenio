using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading;

//Tiene que ser singleton
public class NetworkManagerPy : MonoBehaviour{

    public static NetworkManagerPy Instance;
    private int id = new System.Random().Next(1,100);
    TcpClient tcpClient = new TcpClient();
    NetworkStream networkStream;

    const string IP_SERVER = "192.168.0.27";
    const int PORT = 2000;
    const double MEMORY_SIZE = 5e+6;
    const int TIME_LIMIT = 5000;
    public byte[] data = new byte[(int) MEMORY_SIZE];
    bool running=false;

    void Awake(){
        Instance=this;
    }
    private void connect(Action<bool> callback){
        bool connected = tcpClient.ConnectAsync(IP_SERVER,PORT).Wait(TIME_LIMIT);
        callback(connected);
    }

    void Start(){
        Instance=this;
        Debug.Log("IS RUNNING!");
        connect((bool connected) =>{
            if(connected){
                Debug.Log("Connection Stablished");
                networkStream = tcpClient.GetStream();
                //byte[] myWriteBuffer = Encoding.ASCII.GetBytes(id.ToString()); //Send Client id
                //networkStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); 
                running = true;
            }
            else
                Debug.LogError("Conection couldn't be stablished");
        });
    }

    public void SendData(string data){
        string data2 = "{ \"id\": " + id.ToString() + ", \"data\": " + data + " }";
        byte[] myWriteBuffer = Encoding.ASCII.GetBytes(data2); //Converting string to byte data
        networkStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); //Sending the data in Bytes to Python
    }

    void Update(){
        if(!running){
            connect((bool connected) =>{
            if(connected){
                Debug.Log("Connection Stablished");
                networkStream = tcpClient.GetStream();
                //byte[] myWriteBuffer = Encoding.ASCII.GetBytes(id.ToString()); //Send Client id
                //networkStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); 
                running = true;
            }
            else
                Debug.LogError("Conection couldn't be stablished");
        });
        }else{

            //if(networkStream.DataAvailable ){
                int dataSize = networkStream.Read(data,0,data.Length);
                string message = Encoding.UTF8.GetString(data,0,dataSize);
                Debug.Log("Message from server:" + message);
            //}
        }
    }


    private void onApplicationQuit(){
        //tcpClient.EndConnect();
        //tcpClient.Close();
        running = false;
    }

}
