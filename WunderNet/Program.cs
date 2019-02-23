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
            ws = new WunderServer(@"D:\Documents\CodeProjects\WunderNet2\WunderServer\ExampleNet.xml");
            ws.NewConnection += NewConnection;
            ws.AcceptConnections();
            TcpClient tcpClient = new TcpClient("localhost", 1234);
            byte[] buff = new byte[1024];
            int read = tcpClient.GetStream().Read(buff, 0, 1024);
            Console.WriteLine("CLIENT:" + Encoding.ASCII.GetString(buff, 0, read));
            string test = "Client Response";
            tcpClient.GetStream().Write(Encoding.ASCII.GetBytes(test), 0, test.Length);

            read = tcpClient.GetStream().Read(buff, 0, 1024);
            Console.WriteLine("CLIENT:" + Encoding.ASCII.GetString(buff, 0, read));
            test = "Client Response 2";
            tcpClient.GetStream().Write(Encoding.ASCII.GetBytes(test), 0, test.Length);
            tcpClient.Close();
            Console.ReadKey();
        }
        static async void NewConnection(ClientHandler ch)
        {
            await ch.WriteData("Server Response");
        }
    }
}
