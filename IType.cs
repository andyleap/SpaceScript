using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    public class IType
    {
        public virtual IType Add(IType i2)
        {
            if(i2 is String)
            {
                return new String(this.ToString() + i2.ToString());
            }
            throw new Exception(string.Format("Addition not supported between {0} and {1}", this.GetType().Name, i2.GetType().Name));
        }

        public virtual IType Sub(IType i2)
        {
            throw new Exception(string.Format("Subtraction not supported between {0} and {1}", this.GetType().Name, i2.GetType().Name));
        }

        public virtual IType Mul(IType i2)
        {
            throw new Exception(string.Format("Multiplication not supported between {0} and {1}", this.GetType().Name, i2.GetType().Name));
        }

        public virtual IType Div(IType i2)
        {
            throw new Exception(string.Format("Division not supported between {0} and {1}", this.GetType().Name, i2.GetType().Name));
        }

        public virtual T Cast<T>() where T : IType
        {
            if(this is T)
            {
                return this as T;
            }
            throw new Exception(string.Format("Cannot cast variable of type {0} to {1}", this.GetType().Name, typeof(T).Name));
        }

		public virtual int Compare(IType i2)
		{
			throw new Exception(string.Format("Comparison not supported between {0} and {1}", this.GetType().Name, i2.GetType().Name));
		}
    }
}
