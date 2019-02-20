using System;
using System.Collections.Generic;
using System.Text;
using WunderNet.Serializers;
namespace WunderNet
{
    public class FieldDefinition
    {
        public string Type;
        public int Size;
        public object Value;

    }
    public class WunderPacket
    {
        private static byte[] PREFIX = new byte[2]{ (byte)'\n', (byte)'\0' };
        public Int16 ID;
        public Int32 Version;
        private Dictionary<string, FieldDefinition> Fields = new Dictionary<string, FieldDefinition>();
        public void AddFieldDefinition(string name, string type, int size)
        {
            if(!Fields.ContainsKey(name))
            {
                var fd = new FieldDefinition() { Type = type };
                switch(type.ToLower())
                {
                    case "string": fd.Value = new String(""); fd.Size = size;  break;
                    case "byte": fd.Value = new Byte(); fd.Size = sizeof(Byte); break;
                    case "int16": fd.Value = new Int16(); fd.Size = sizeof(Int16); break;
                    case "int32": fd.Value = new Int32(); fd.Size = sizeof(Int32); break;
                    case "int64": fd.Value = new Int64(); fd.Size = sizeof(Int64); break;
                    case "float": fd.Value = new Double(); fd.Size = sizeof(float); break;
                    case "double": fd.Value = new Double(); fd.Size = sizeof(Double); break;
                }
                Fields.Add(name,fd);
            }
        }
    }
    public class WunderLayer
    {
        private Dictionary<string, WunderPacket> PacketDefinitions = new Dictionary<string, WunderPacket>();
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
                    var wp = new WunderPacket() { ID = ids, Version = Convert.ToInt32(packets.Version) };
                    foreach(var f in p.FieldList)
                    {
                        wp.AddFieldDefinition(f.Name, f.Type, f.Size);
                    }
                    PacketDefinitions.Add(p.Name, wp);
                }
            }
        }

    }
}
