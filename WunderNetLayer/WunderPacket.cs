using System;
using System.Collections.Generic;
using System.Text;


//ToDo Support Arrays (types other than string)

namespace WunderNetLayer
{
    public class WunderPacket
    {
        public static byte[] PREFIX = new byte[2] { (byte)'\n', (byte)'\0' };
        public Int32 Version;
        public Int16 ID;
        private Dictionary<string, FieldDefinition> Fields = new Dictionary<string, FieldDefinition>();
        private List<FieldDefinition> OrderdedFields = new List<FieldDefinition>();
        //PREFIX + VERSION + ID
        private int PacketSize = sizeof(Byte) * 2 + sizeof(Int32) + sizeof(Int16);
        private const int DATAOFFSET = 8;
        //Prefix 2Bytes
        //Version 4Bytes
        //ID 2Bytes
        public static int GetPacketID(byte[] bytes)
        {
            return (int)BitConverter.ToInt16(bytes, 6);
        }
        public static int GetPacketVersion(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 2);
        }


        public void AddFieldDefinition(string name, string type, int size)
        {
            if (!Fields.ContainsKey(name))
            {
                this.AddField(name, new FieldDefinition(name, type, size));
            }
        }
        public void AddFieldDefinition(string name, Type type, int size)
        {
            if (!Fields.ContainsKey(name))
            {
                this.AddField(name, new FieldDefinition(name, type, size));
            }
        }
        private void AddField(string name, FieldDefinition fd)
        {
            this.PacketSize += fd.ByteSize;
            this.Fields.Add(name, fd);
            this.OrderdedFields.Add(fd);
        }

        public WunderPacket CreateNew()
        {
            var clone = new WunderPacket();
            clone.ID = this.ID;
            clone.Version = this.Version;
            foreach (var key in this.Fields.Keys)
            {
                clone.AddFieldDefinition(key, this.Fields[key].ValueType, this.Fields[key].Count);
            }
            return clone;
        }

        public WunderPacket CreateFromBytes(byte[] bytes)
        {
            if (bytes.Length == this.PacketSize)
            {
                int offset = DATAOFFSET;
                WunderPacket newwp = this.CreateNew();
                for (int f = 0; f < newwp.OrderdedFields.Count; ++f)
                {
                    offset = newwp.OrderdedFields[f].SetBytes(bytes, offset);
                }
                return newwp;
            }

            return null;
        }

        public bool Set(string fieldname, object value)
        {
            if (this.Fields.ContainsKey(fieldname))
            {
                try
                {
                    this.Fields[fieldname].SetValue(value);
                    return true;
                }
                catch
                {
                    Console.WriteLine("Field [" + fieldname + "] Set Value failed...");
                    return false;
                }
            }
            return false;
        }
        public object Get(string fieldname)
        {
            if (this.Fields.ContainsKey(fieldname))
            {
                return Fields[fieldname].Value;
            }
            return null;
        }

        public byte[] GetBytes()
        {
            byte[] b = new byte[this.PacketSize];
            int offset = 0;

            //HEADER
            Array.Copy(PREFIX, b, PREFIX.Length);
            offset += PREFIX.Length;
            byte[] c = BitConverter.GetBytes(this.Version);
            Array.Copy(c, 0, b, offset, c.Length);
            offset += c.Length;
            c = BitConverter.GetBytes(this.ID);
            Array.Copy(c, 0, b, offset, c.Length);
            offset += c.Length;

            //FIELDS
            for (int f=0; f<this.OrderdedFields.Count; ++f)
            {
                var chunk = this.OrderdedFields[f].GetBytes();
                Array.Copy(chunk, 0, b, offset, chunk.Length);
                offset += chunk.Length;
            }           
            return b;
        }

        public override string ToString()
        {
            string fields = "";
            foreach(var s in this.OrderdedFields)
            {
                fields += s.ToString() + "\n";
            }
            return fields;
        }



    }
}
