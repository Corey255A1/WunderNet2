using System;
using System.Collections.Generic;
using System.Text;


//ToDo Support Arrays (types other than string)

namespace WunderNetLayer
{
    public class WunderPacketVariable: WunderPacket
    {
        private byte VariableCount = 0;
        public WunderPacketVariable() : base()
        {
            this.AddField("VariableCount", new VariableFieldDefinition("VariableCount", "Byte", 0));
        }

        public override void AddFieldDefinition(string name, string type, int size)
        {
            if (!Fields.ContainsKey(name))
            {
                this.AddField(name+"_VarID", new FieldHeaderDefinition(name + "_VarID", this.VariableCount++));
                this.AddField(name, new VariableFieldDefinition(name, type, size));
            }
        }
        public override void AddFieldDefinition(string name, Type type, int size)
        {
            if (!Fields.ContainsKey(name))
            {
                this.AddField(name + "_VarID", new FieldHeaderDefinition(name + "_VarID", this.VariableCount++));
                this.AddField(name, new VariableFieldDefinition(name, type, size));
            }
        }

        public override WunderPacket CreateFromBytes(byte[] bytes, ref int offset)
        {
            if (bytes.Length - offset >= this.PacketSize)
            {
                offset += DATAOFFSET;
                WunderPacketVariable newwp = this.CreateNew() as WunderPacketVariable;

                //First field is the count of fields that were sent
                offset = newwp.OrderdedFields[0].SetBytes(bytes, offset);
                byte count = (byte)newwp.Get("VariableCount");

                //Byte - FieldID
                //Bytes=>TheField;
                for(int f=0; f<count; ++f)
                {
                    int fieldID = (bytes[offset++]+1)*2;
                    if (fieldID < OrderdedFields.Count)
                    {
                        offset = newwp.OrderdedFields[fieldID].SetBytes(bytes, offset);
                    }
                }
                return newwp;
            }

            return null;
        }

        public override WunderPacket CreateNew()
        {
            var clone = new WunderPacketVariable
            {
                Name = this.Name,
                ID = this.ID,
                Version = this.Version
            };
            //Skip VariableCount
            for (int k = 1; k < this.OrderdedFields.Count; ++k)
            {
                clone.AddField(this.OrderdedFields[k].Name, this.OrderdedFields[k].CreateNew());
            }
            return clone;
        }

        public override byte[] GetBytes()
        {            
            int offset = 0;
            byte[] b = this.GetEmptyBuffer(ref offset);
            int countbyteoffset = offset++; //Key this point to set the count of fields we are sending
            
            //FIELDS - Skip the First VariableCount field
            for (int f = 1; f < this.OrderdedFields.Count; f+=2)
            {
                var varFieldDef = this.OrderdedFields[f] as FieldHeaderDefinition;
                var varField = this.OrderdedFields[f+1] as VariableFieldDefinition;
                if (varField.IsIncluded)
                {
                    var chunk = varFieldDef.GetBytes();
                    Array.Copy(chunk, 0, b, offset, chunk.Length);
                    offset += chunk.Length;
                    chunk = varField.GetBytes();
                    Array.Copy(chunk, 0, b, offset, chunk.Length);
                    offset += chunk.Length;

                    b[countbyteoffset]++;
                }
            }
            return b;
        }
    }

    public class WunderPacket
    {
        public static byte[] PREFIX = new byte[2] { (byte)'\n', (byte)'\0' };
        public Int32 Version;
        public Int16 ID;
        public string Name;
        public string PacketType;

        protected Dictionary<string, FieldDefinition> Fields = new Dictionary<string, FieldDefinition>();
        protected List<FieldDefinition> OrderdedFields = new List<FieldDefinition>();
        //PREFIX + VERSION + ID
        protected int PacketSize = sizeof(Byte) * 2 + sizeof(Int32) + sizeof(Int16);
        protected const int DATAOFFSET = 8;
        //Prefix 2Bytes
        //Version 4Bytes
        //ID 2Bytes

        protected byte[] GetEmptyBuffer(ref int offset)
        {
            byte[] b = new byte[this.PacketSize];
            //HEADER
            Array.Copy(PREFIX, b, PREFIX.Length);
            offset += PREFIX.Length;
            byte[] c = BitConverter.GetBytes(this.Version);
            Array.Copy(c, 0, b, offset, c.Length);
            offset += c.Length;
            c = BitConverter.GetBytes(this.ID);
            Array.Copy(c, 0, b, offset, c.Length);
            offset += c.Length;

            return b;
        }

        public static int GetPacketID(byte[] bytes, int offset)
        {
            return (int)BitConverter.ToInt16(bytes, offset+6);
        }
        public static int GetPacketVersion(byte[] bytes, int offset)
        {
            return BitConverter.ToInt32(bytes, offset+2);
        }

        public virtual void AddFieldDefinition(string name, string type, int size)
        {
            if (!Fields.ContainsKey(name))
            {
                this.AddField(name, new FieldDefinition(name, type, size));
            }
        }
        public virtual void AddFieldDefinition(string name, Type type, int size)
        {
            if (!Fields.ContainsKey(name))
            {
                this.AddField(name, new FieldDefinition(name, type, size));
            }
        }
        protected void AddField(string name, FieldDefinition fd)
        {
            this.PacketSize += fd.ByteSize;
            this.Fields.Add(name, fd);
            this.OrderdedFields.Add(fd);
        }

        public virtual WunderPacket CreateNew()
        {
            var clone = new WunderPacket
            {
                Name = this.Name,
                ID = this.ID,
                Version = this.Version
            };
            for(int k=0; k<this.OrderdedFields.Count;++k)
            {
                clone.AddField(this.OrderdedFields[k].Name, this.OrderdedFields[k].CreateNew());
            }
            return clone;
        }

        public virtual WunderPacket CreateFromBytes(byte[] bytes, ref int offset)
        {
            if (bytes.Length - offset >= this.PacketSize)
            {
                offset += DATAOFFSET;
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

        public virtual byte[] GetBytes()
        {
            int offset = 0;
            byte[] b = GetEmptyBuffer(ref offset);

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
