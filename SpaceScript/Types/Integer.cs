using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    public class Integer : IType
    {
        public int Value;

        public Integer(int Value)
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override IType Add(IType i2)
        {
            var i2i = i2 as Integer;
            if(i2i != null)
            {
                return new Integer(this.Value + i2i.Value);
            }
            return base.Add(i2);
        }

        public override IType Sub(IType i2)
        {
            var i2i = i2 as Integer;
            if (i2i != null)
            {
                return new Integer(this.Value - i2i.Value);
            }
            return base.Sub(i2);
        }

        public override IType Mul(IType i2)
        {
            var i2i = i2 as Integer;
            if (i2i != null)
            {
                return new Integer(this.Value * i2i.Value);
            }
            return base.Mul(i2);
        }

        public override IType Div(IType i2)
        {
            var i2i = i2 as Integer;
            if (i2i != null)
            {
                return new Integer(this.Value / i2i.Value);
            }
            return base.Div(i2);
        }

        public override bool Equals(object obj)
        {
            var intobj = obj as Integer;
            if (intobj != null)
            {
                return Value == intobj.Value;
            }
            return base.Equals(obj);
        }

        public override T Cast<T>()
        {
            if(typeof(T) == typeof(Float))
            {
                return new Float(Value) as T;
            }
            if (typeof(T) == typeof(String))
            {
                return new String(Value.ToString()) as T;
            }
            if (typeof(T) == typeof(Bool))
            {
                return new Bool(Value != 0) as T;
            }
            return base.Cast<T>();
        }
    }
}
