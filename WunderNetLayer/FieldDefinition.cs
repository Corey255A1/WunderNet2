using System;
using WunderNetLayer.Utilities;
namespace WunderNetLayer
{
    public class FieldDefinition
    {
        public string Name;
        public int Count;
        public int ByteSize;
        public Type ValueType;
        public object Value;

        public FieldDefinition(string name, string type, int count)
        {
            Init(name, HelperFunctions.StringToType(type), count);
        }
        public FieldDefinition(string name, Type valuetype, int count)
        {
            Init(name, valuetype, count);
        }
        private void Init(string name, Type valuetype, int count)
        {
            this.Name = name;
            this.ValueType = valuetype;
            this.Value = HelperFunctions.GetDefault(this.ValueType);
            this.Count = count;

            //ToDo Support arrays
            this.ByteSize = HelperFunctions.GetSize(this.ValueType);
            if (count > 0)
            {
                this.ByteSize *= count;
            }
        }

        public virtual FieldDefinition CreateNew()
        {
            return new FieldDefinition(this.Name, this.ValueType, this.Count);
        }

        public virtual void ClearValue()
        {
            this.Value = HelperFunctions.GetDefault(this.ValueType);
        }

        public virtual void SetValue(object value)
        {
            try
            {
                this.Value = HelperFunctions.GenericValueSet(this.ValueType, value);

            }
            catch
            {
                Console.WriteLine("Couldn't Set " + value);
            }
        }
        public byte[] GetBytes()
        {
            return HelperFunctions.GetBytes(this.ValueType, this.Value, this.Count);
        }
        public int SetBytes(byte[] bytes, int offset)
        {
            this.Value = HelperFunctions.ConvertBytes(this.ValueType, bytes, this.Count, ref offset);
            return offset;
        }

        public override string ToString()
        {
            return String.Format("[{0}] = {1}", this.Name, this.Value??"");
        }
    }

    public class FieldHeaderDefinition : FieldDefinition
    {
        public FieldHeaderDefinition(string name, byte id) :
            base(name, HelperFunctions.StringToType("Byte"), 0)
        {
            this.Value = id;
        }

        public override FieldDefinition CreateNew()
        {
            return new FieldHeaderDefinition(this.Name, (byte)this.Value);
        }
    }

    public class VariableFieldDefinition : FieldDefinition
    {
        public bool IsIncluded = false;
        public VariableFieldDefinition(string name, string type, int count) :
            base(name, HelperFunctions.StringToType(type), count)
        {
        }

        public VariableFieldDefinition(string name, Type valuetype, int count) :
            base(name, valuetype, count)
        {
        }

        public override FieldDefinition CreateNew()
        {
            return new VariableFieldDefinition(this.Name, this.ValueType, this.Count);
        }

        public override void SetValue(object value)
        {
            base.SetValue(value);
            IsIncluded = true; //Assuming if you set it, you want it sent out
        }
        public override void ClearValue()
        {
            base.ClearValue();
            IsIncluded = false;
        }
    }


}
