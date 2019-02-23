using System;
using WunderNet;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WunderNetLayer;
namespace WunderClient
{
    class Program
    {
        static WunderServer ws;
        static WunderNetLayer.WunderLayer decoder;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ws = new WunderServer(@"D:\Documents\CodeProjects\WunderNet2\WunderServer\ExampleNet.xml");
            decoder = new WunderNetLayer.WunderLayer(@"D:\Documents\CodeProjects\WunderNet2\WunderServer\ExampleNet.xml");
            ws.NewConnection += NewConnection;
            ws.AddDataCallback("Message", Message);
            ws.AcceptConnections();
            TcpClient tcpClient = new TcpClient("localhost", 1234);
            byte[] buff = new byte[1024];
            int read = tcpClient.GetStream().Read(buff, 0, 1024);
            int offset = 0;
            var resp = decoder.GetFromBytes(buff, ref offset);
            Console.WriteLine("CLIENT:" + resp.Get("MessageData"));

            var message = decoder.GetNewPacket("Message");
            message.Set("MessageData", "THIS IS THE ULTIMATE TEST");
            byte[] b = message.GetBytes();
            tcpClient.GetStream().Write(b, 0, b.Length);

            tcpClient.Close();
            Console.ReadKey();
        }

        static void Message(System.Net.EndPoint client, WunderPacket packet)
        {
            Console.WriteLine(packet.Get("MessageData"));
        }

        static void NewConnection(ClientHandler ch)
        {
            var resp = decoder.GetNewPacket("Message");
            resp.Set("MessageData", "ServerResponse");
            ch.WriteData(resp);
        }
    }
}
