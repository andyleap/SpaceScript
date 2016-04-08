using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    class String : IType
    {
        public string Value;

        public String(string Value)
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override IType Add(IType i2)
        {
            return new String(this.Value + i2.ToString());
        }

        public override bool Equals(object obj)
        {
            var strobj = obj as String;
            if (strobj != null)
            {
                return Value == strobj.Value;
            }
            return base.Equals(obj);
        }
    }
}
