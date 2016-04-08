using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    public class Bool : IType
    {
        public bool Value;

        public Bool(bool Value)
        {
            this.Value = Value;
        }

        public override bool Equals(object obj)
        {
            var boolobj = obj as Bool;
            if (boolobj != null)
            {
                return Value == boolobj.Value;
            }
            return base.Equals(obj);
        }
    }
}
