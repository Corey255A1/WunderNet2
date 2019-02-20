using System;
using WunderNet;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace WunderClient
{
    class Program
    {
        static WunderServer ws;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ws = new WunderServer(@"D:\Documents\CodeProjects\WunderNet\WunderServer\ExampleNet.xml");
            ws.NewConnection += NewConnection;
            ws.AcceptConnections();
            TcpClient tcpClient = new TcpClient("localhost", 1234);
            byte[] buff = new byte[1024];
            int read = tcpClient.GetStream().Read(buff, 0, 1024);
            Console.WriteLine("CLIENT:" + Encoding.ASCII.GetString(buff, 0, read));
            string test = "Client Response";
            tcpClient.GetStream().Write(Encoding.ASCII.GetBytes(test), 0, test.Length);
            tcpClient.Close();
            Console.ReadKey();
        }
        static void NewConnection(ClientHandler ch)
        {
            ch.WriteData("Server Response");
        }
    }
}
