using System;

using System.Net;
using System.Net.Sockets;
using System.Text;
using WunderNetLayer;
using WunderNet;
using WunderClient;
namespace WunderNetTest
{
    class Program
    {
        static WunderTCPServer ws;
        static WunderTCPClient wc;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string xmlPathDefault = @"D:\Documents\CodeProjects\WunderNet2\WunderServer\ExampleNet.xml";
            if(!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                xmlPathDefault = @"/home/corey/Code/WunderNet2/WunderServer/ExampleNet.xml";
            }
            ws = new WunderTCPServer(xmlPathDefault, IPAddress.Any, 1234);
            wc = new WunderTCPClient(xmlPathDefault, "localhost", 1234);

            ws.NewConnection += NewConnection;
            ws.AddDataCallback("Message", ServerMessage);
            ws.AddDataCallback("WorldInfo", ServerWorldInfo);
            ws.AcceptConnections();


            wc.AddDataCallback("Message", ClientMessage);
            wc.Connect();

            Console.ReadKey();
        }

        static void ClientMessage(WunderPacket packet)
        {
            Console.WriteLine(packet.Get("MessageData"));

            var lotsofdata = wc.GetNewPacket("WorldInfo");

            for (int i = 0; i < 100; i++)
            {
                lotsofdata.Set("Width", i * 10);
                wc.Send(lotsofdata);
            }


            var resp = wc.GetNewPacket("Message");
            resp.Set("MessageData", "I'm a client responding to the server!");
            wc.Send(resp);
        }

        static void ServerWorldInfo(ClientHandler client, WunderPacket packet)
        {
            Console.WriteLine(packet.Get("Width"));
        }

        static void ServerMessage(ClientHandler client, WunderPacket packet)
        {
            Console.WriteLine(packet.Get("MessageData"));
            var resp = ws.GetNewPacket("Message");
            resp.Set("MessageData", "I'm the Server Responding to the Client Message");
            client.Send(resp);
            ws.Disconnect();
        }

        static void NewConnection(ClientHandler ch)
        {
            var resp = ws.GetNewPacket("Message");
            resp.Set("MessageData", "I'm The Server Responding to the Client Connection");
            ch.Send(resp);
        }
    }
}
