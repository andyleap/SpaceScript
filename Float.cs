using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    class Float : IType
    {
        public float Value;

        public Float(float Value)
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override IType Add(IType i2)
        {
            var i2f = i2 as Float;
            if (i2f != null)
            {
                return new Float(this.Value + i2f.Value);
            }
            return base.Add(i2);
        }

        public override IType Sub(IType i2)
        {
            var i2f = i2 as Float;
            if (i2f != null)
            {
                return new Float(this.Value - i2f.Value);
            }
            return base.Sub(i2);
        }

        public override IType Mul(IType i2)
        {
            var i2f = i2 as Float;
            if (i2f != null)
            {
                return new Float(this.Value * i2f.Value);
            }
            return base.Mul(i2);
        }

        public override IType Div(IType i2)
        {
            var i2f = i2 as Float;
            if (i2f != null)
            {
                return new Float(this.Value / i2f.Value);
            }
            return base.Div(i2);
        }

        public override bool Equals(object obj)
        {
            var intobj = obj as Float;
            if (intobj != null)
            {
                return Value == intobj.Value;
            }
            return base.Equals(obj);
        }

        public override T Cast<T>()
        {
            if (typeof(T) == typeof(Integer))
            {
                return new Integer((int)Value) as T;
            }
            if (typeof(T) == typeof(String))
            {
                return new String(Value.ToString()) as T;
            }
            return base.Cast<T>();
        }

		public override int Compare(IType i2)
		{
			Float other = i2.Cast<Float>();
			if (Value > other.Value)
			{
				return 1;
			}
			else if (Value < other.Value)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}
	}
}
