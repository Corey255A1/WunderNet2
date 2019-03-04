using System;
using System.Collections.Generic;
using System.Text;

namespace WunderNetLayer
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
            int offset = 0;
            WunderPacket decode = layerTest.GetFromBytes(test, ref offset);
            Console.WriteLine("Decoded Data: " + decode.Get("MessageData"));
            test = o.GetBytes();
            offset = 0;
            decode = layerTest.GetFromBytes(test, ref offset);
            Console.WriteLine("Decoded Data: " + decode.Get("VX"));
            return true;
        }
    }
}
