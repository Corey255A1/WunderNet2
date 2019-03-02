using System.Xml;
using System.Xml.Serialization;
namespace WunderNetLayer.Serializers
{
    [XmlRoot("Packets")]
    public class Packets : Stringable
    {
        [XmlAttribute("Version")]
        public string Version;

        [XmlElement("Packet")]
        public Packet[] PacketList;
    }

    public class Packet : Stringable
    {
        [XmlAttribute("Name")]
        public string Name;
        [XmlAttribute("Type")]
        public string PacketType;
        [XmlElement("Field")]
        public Field[] FieldList;

    }
    public class Field : Stringable
    {
        [XmlAttribute("Name")]
        public string Name;
        [XmlAttribute("Type")]
        public string Type;
        [XmlAttribute("Size")]
        public int Size;
    }

}
