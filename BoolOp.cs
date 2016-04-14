using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    class BoolOp : Expression, RValue
    {
        RValue v1;
        string op;
        RValue v2;

        public BoolOp(RValue v1, string op, RValue v2)
        {
            this.v1 = v1;
            this.op = op;
            this.v2 = v2;
        }
        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            IType val1 = null;
            IType val2 = null;
			bool bool1 = false;
			bool bool2 = false;
			yield return (sc, st) => v1.Resolve(sc, st, r => { val1 = r; });
			if(!(val1 is Bool))
			{
				throw new Exception(op + " only valid on boolean types");
			}
			bool1 = (val1 as Bool).Value;
            yield return (sc, st) => v2.Resolve(sc, st, r => { val2 = r; });
			if (!(val2 is Bool))
			{
				throw new Exception(op + " only valid on boolean types");
			}
			bool2 = (val2 as Bool).Value;
			switch (op)
            {
                case "&&":
                    Result(new Bool(bool1 && bool2));
                    break;
                case "||":
                    Result(new Bool(bool1 || bool2));
                    break;
			}
        }
    }
}
