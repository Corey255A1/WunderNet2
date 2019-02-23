using System;
using System.Collections.Generic;
using System.Text;

namespace WunderNet
{
    class TestClass
    {
        public static bool TestWunderLayer(string xmlpath)
        {
            var layerTest = new WunderLayer(xmlpath);
            WunderPacket w = layerTest.GetNewPacket("Message");
            w.Set("MessageData", "This is a test");
            byte[] test = w.GetBytes();
            Console.WriteLine(test.Length);
            WunderPacket o = layerTest.GetNewPacket("ObjectInfo");
            o.Set("VX", 3.4);
            Console.WriteLine(w.ToString());
            Console.WriteLine(o.ToString());
            WunderPacket decode = layerTest.GetFromBytes(test);
            Console.WriteLine("Decoded Data: " + decode.Get("MessageData"));
            test = o.GetBytes();
            decode = layerTest.GetFromBytes(test);
            Console.WriteLine("Decoded Data: " + decode.Get("VX"));
            return true;
        }
    }
}
