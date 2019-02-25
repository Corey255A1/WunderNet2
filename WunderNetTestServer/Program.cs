﻿using System;
using WunderNetLayer;
using WunderNet;
using System.Net;
namespace WunderNetTestServer
{
    class Program
    {
        static WunderTCPServer ws;
        static void Main(string[] args)
        {
            Console.WriteLine("WunderNet Test Server");
            ws = new WunderTCPServer(@"D:\Documents\CodeProjects\WunderNet2\WunderServer\ExampleNet.xml", IPAddress.Any, 1234);
            ws.NewConnection += NewConnection;
            ws.AddDataCallback("Message", ServerMessage);
            ws.AcceptConnections();

            Console.ReadKey();
        }
        static void ServerMessage(ClientHandler client, WunderPacket packet)
        {
            Console.WriteLine(packet.Get("MessageData"));
        }

        static void NewConnection(ClientHandler ch)
        {
            Console.WriteLine("Client Connected: " + ch.ClientInfo);
            var resp = ws.GetNewPacket("Message");
            resp.Set("MessageData", "I'm The Server Responding to the Client Connection");
            ch.Send(resp);
        }
    }
}
