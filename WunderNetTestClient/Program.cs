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
            string xmlPathDefault = @"D:\Documents\CodeProjects\WunderNet2\WunderServer\ExampleNet.xml";
            if(!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                xmlPathDefault = @"/home/corey/Code/WunderNet2/WunderServer/ExampleNet.xml";
            }
            wc = new WunderTCPClient(xmlPathDefault, "localhost", 1234);
            wc.AddDataCallback("Message", ClientMessage);
            wc.Connect();

            Console.ReadKey();
        }

        static void ClientMessage(WunderPacket packet)
        {
            Console.WriteLine(packet.ToString());
            var toSend = wc.GetNewPacket("VariableLengthPacket");
            var resp = wc.GetNewPacket("Message");
            for (int i = 0; i < 50; i++)
            {
                toSend.Set("FieldOne", 42);
                toSend.Clear("FieldString");
                wc.Send(toSend);

                
                resp.Set("MessageData", "I'm The Client Sending a lot of data!");
                wc.Send(resp);

                toSend.Set("FieldOne", 37);
                toSend.Set("FieldString", "A Variable Packet");
                wc.Send(toSend);
            }
        }
    }
}
