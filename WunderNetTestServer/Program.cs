using System;
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
            string xmlPathDefault = @"D:\Documents\CodeProjects\WunderNet2\WunderServer\ExampleNet.xml";
            if(!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                xmlPathDefault = @"/home/corey/Code/WunderNet2/WunderServer/ExampleNet.xml";
            }
            ws = new WunderTCPServer(xmlPathDefault, IPAddress.Any, 1234);
            ws.NewConnection += NewConnection;
            ws.AddDataCallback("Message", ServerMessage);
            ws.AddDataCallback("VariableLengthPacket", ServerMessage);
            Console.WriteLine(ws.GetNewPacket("VariableLengthPacket").ToString());
            ws.AcceptConnections();

            Console.ReadKey();
        }
        static void ServerMessage(ClientHandler client, WunderPacket packet)
        {
            Console.WriteLine(packet.ToString());
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
