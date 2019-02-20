using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
namespace WunderNet.Serializers
{

    public class Stringable
    {
        public override string ToString()
        {
            string s = this.GetType().Name;
            foreach(FieldInfo f in this.GetType().GetFields())
            {
                if(f.FieldType.IsArray)
                {
                    object[] val = (object[])f.GetValue(this);
                    if(val!=null)
                    {
                        foreach(var a in val)
                        {
                            s +="\n"+ a.ToString();
                        }
                    }
                }
                else
                {
                    s += String.Format(", {0}={1}", f.Name, f.GetValue(this));
                }
                
            }
            return s;
        }
    }

    public class GenericXMLTools
    {
        public static T ReadXML<T>(string path)
        {
            try
            {
                using (var s = new FileStream(path, FileMode.Open))
                {
                    var xsz = new XmlSerializer(typeof(T));
                    return (T)xsz.Deserialize(s);                    
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return default(T);
        }
    }
}
