using System;
using System.Text;

namespace WunderNetLayer.Utilities
{
    public class HelperFunctions
    {
        public static Type StringToType(string type)
        {
            switch (type.ToLower())
            {
                case "string": return typeof(String);
                case "byte": return typeof(Byte);
                case "int16": return typeof(Int16);
                case "int32": return typeof(Int32);
                case "int64": return typeof(Int64);
                case "uint16": return typeof(UInt16);
                case "uint32": return typeof(UInt32);
                case "uint64": return typeof(UInt64);
                case "float": return typeof(Single);
                case "double": return typeof(Double);
                default: throw new Exception("Cannot Determine Type");
            }
        }

        public static object ConvertBytes(Type type, byte[] bytes, int count, ref int offset)
        {
            if (type == typeof(String)) {  object ret = Encoding.ASCII.GetString(bytes, offset, count); offset += count; return ret; }
            else if (type == typeof(Byte))  {  object ret = (bytes[offset]); offset += sizeof(Byte); return ret; }
            else if (type == typeof(Int16)) {  object ret = BitConverter.ToInt16(bytes, offset); offset += sizeof(Int16); return ret; }
            else if (type == typeof(Int32)) {  object ret = BitConverter.ToInt32(bytes, offset); offset += sizeof(Int32); return ret; }
            else if (type == typeof(Int64)) {  object ret = BitConverter.ToInt64(bytes, offset); offset += sizeof(Int64); return ret; }
            else if (type == typeof(UInt16)){  object ret = BitConverter.ToUInt16(bytes, offset); offset += sizeof(UInt16); return ret; }
            else if (type == typeof(UInt32)){  object ret = BitConverter.ToUInt32(bytes, offset); offset += sizeof(UInt32); return ret; }
            else if (type == typeof(UInt64)){  object ret = BitConverter.ToUInt64(bytes, offset); offset += sizeof(UInt64); return ret; }
            else if (type == typeof(Single)){  object ret = BitConverter.ToSingle(bytes, offset); offset += sizeof(Single); return ret; }
            else if (type == typeof(Double)){  object ret = BitConverter.ToDouble(bytes, offset); offset += sizeof(Double); return ret; }
            else throw new Exception("Cannot Get Data for this Type");
        }

        public static byte[] GetBytes(Type type, object obj, int size)
        {
            if (type == typeof(String))
            {
                String s = (String)obj;
                if (s.Length < size)
                {
                    byte[] sbytes = Encoding.ASCII.GetBytes(s);
                    byte[] b = new byte[size];
                    Array.Copy(sbytes, b, sbytes.Length);
                    return b;
                }
                else
                {
                    return Encoding.ASCII.GetBytes(s, 0, size);
                }
            }
            else if (type == typeof(Byte)) return new Byte[] { (Byte)obj };
            else if (type == typeof(Int16)) return BitConverter.GetBytes((Int16)obj);
            else if (type == typeof(Int32)) return BitConverter.GetBytes((Int32)obj);
            else if (type == typeof(Int64)) return BitConverter.GetBytes((Int64)obj);
            else if (type == typeof(UInt16)) return BitConverter.GetBytes((UInt16)obj);
            else if (type == typeof(UInt32)) return BitConverter.GetBytes((UInt32)obj);
            else if (type == typeof(UInt64)) return BitConverter.GetBytes((UInt64)obj);
            else if (type == typeof(Single)) return BitConverter.GetBytes((Single)obj);
            else if (type == typeof(Double)) return BitConverter.GetBytes((Double)obj);
            else throw new Exception("Cannot Get Bytes for this Type");
        }

        public static int GetSize(Type type)
        {
            if (type == typeof(String)) return sizeof(Byte); //ASCII .. maybe support unicode in the future ..
            else if (type == typeof(Byte)) return sizeof(Byte);
            else if (type == typeof(Int16)) return sizeof(Int16);
            else if (type == typeof(Int32)) return sizeof(Int32);
            else if (type == typeof(Int64)) return sizeof(Int64);
            else if (type == typeof(UInt16)) return sizeof(UInt16);
            else if (type == typeof(UInt32)) return sizeof(UInt32);
            else if (type == typeof(UInt64)) return sizeof(UInt64);
            else if (type == typeof(Single)) return sizeof(Single);
            else if (type == typeof(Double)) return sizeof(Double);
            else throw new Exception("Cannot Get Size for this Type");
        }

        public static object GetDefault(Type type)
        {
            if (type == typeof(String)) return "";
            else if (type == typeof(Byte)) return 0;
            else if (type == typeof(Int16)) return 0;
            else if (type == typeof(Int32)) return 0;
            else if (type == typeof(Int64)) return 0;
            else if (type == typeof(UInt16)) return 0;
            else if (type == typeof(UInt32)) return 0;
            else if (type == typeof(UInt64)) return 0;
            else if (type == typeof(Single)) return 0.0f;
            else if (type == typeof(Double)) return 0.0;
            else return Activator.CreateInstance(type); //Throws Exception if there is no parameterless constructor
        }

        public static object GenericValueSet(Type typeToReturn, object value)
        {
            if (typeToReturn == typeof(String)) return value.ToString();
            else if (typeToReturn == typeof(Byte)) return Convert.ToByte(value);
            else if (typeToReturn == typeof(Int16)) return Convert.ToInt16(value);
            else if (typeToReturn == typeof(Int32)) return Convert.ToInt32(value);
            else if (typeToReturn == typeof(Int64)) return Convert.ToInt64(value);
            else if (typeToReturn == typeof(UInt16)) return Convert.ToUInt16(value);
            else if (typeToReturn == typeof(UInt32)) return Convert.ToUInt32(value);
            else if (typeToReturn == typeof(UInt64)) return Convert.ToUInt64(value);
            else if (typeToReturn == typeof(Single)) return Convert.ToSingle(value);
            else if (typeToReturn == typeof(Double)) return Convert.ToDouble(value);
            else throw new Exception("Cannot Convert to this Type");
        }
    }
}
