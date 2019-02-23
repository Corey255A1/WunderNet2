using System;
using System.Collections.Generic;
using WunderNetLayer.Serializers;
namespace WunderNetLayer
{
    public class WunderLayer
    {
        private Dictionary<string, WunderPacket> PacketDefinitions = new Dictionary<string, WunderPacket>();
        private List<WunderPacket> OrderedDefinitions = new List<WunderPacket>();
        public WunderLayer(string xmldescription)
        {
            Packets packets = GenericXMLTools.ReadXML<Packets>(xmldescription);
            Console.WriteLine("VERSION: " + packets.Version);
            if(packets.PacketList!=null)
            {
                Int16 ids = 0;
                foreach(var p in packets.PacketList)
                {
                    Console.WriteLine(p.ToString() +"\n");
                    var wp = new WunderPacket() { ID = ids++, Version = Convert.ToInt32(packets.Version) };
                    foreach(var f in p.FieldList)
                    {
                        wp.AddFieldDefinition(f.Name, f.Type, f.Size);
                    }
                    PacketDefinitions.Add(p.Name, wp);
                    OrderedDefinitions.Add(wp);
                }
            }
        }

        public WunderPacket GetFromBytes(byte[] bytes)
        {
            if (bytes[0] == WunderPacket.PREFIX[0] && bytes[1] == WunderPacket.PREFIX[1])
            {
                int id = WunderPacket.GetPacketID(bytes);
                if (id < OrderedDefinitions.Count)
                {
                    return OrderedDefinitions[id].CreateFromBytes(bytes);
                }
            }
            return null;
        }

        public WunderPacket GetNewPacket(string packetname)
        {
            if(PacketDefinitions.ContainsKey(packetname))
            {
                return PacketDefinitions[packetname].CreateNew();
            }
            return null;
        }

    }
}
