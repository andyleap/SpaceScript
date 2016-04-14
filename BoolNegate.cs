using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    class BoolNegate : Expression, RValue
    {
        RValue v1;

        public BoolNegate(RValue v1)
        {
            this.v1 = v1;
        }
        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            IType val1 = null;
			bool bool1 = false;
			yield return (sc, st) => v1.Resolve(sc, st, r => { val1 = r; });
			if(!(val1 is Bool))
			{
				throw new Exception("! only valid on boolean type");
			}
			bool1 = (val1 as Bool).Value;
            Result(new Bool(!bool1));
        }
    }
}
