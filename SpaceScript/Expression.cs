using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    public class Expression
    {
        public static RValue Apply(RValue target, List<Func<RValue, RValue>> Subexprs)
        {
            foreach(var subexpr in Subexprs)
            {
                target = subexpr(target);
            }
            return target;
        }
    }
}
