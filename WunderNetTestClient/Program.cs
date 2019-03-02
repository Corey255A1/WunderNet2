using System;
using WunderClient;
using WunderNetLayer;
namespace WunderNetTestClient
{
    class Program
    {
        static WunderTCPClient wc;
        static void Main(string[] args)
        {
            Console.WriteLine("WunderNet Test Client");
            wc = new WunderTCPClient(@"D:\Documents\CodeProjects\WunderNet2\WunderServer\ExampleNet.xml", "localhost", 1234);
            wc.AddDataCallback("Message", ClientMessage);
            wc.Connect();

            Console.ReadKey();
        }

        static void ClientMessage(WunderPacket packet)
        {
            Console.WriteLine(packet.ToString());
            var toSend = wc.GetNewPacket("VariableLengthPacket");
            toSend.Set("FieldOne", 42);
            wc.Send(toSend);
        }
    }
}
