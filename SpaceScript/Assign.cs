using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    class Assign : Expression, RValue
    {
        LValue lval;
        RValue rval;

        public Assign(Value lval, RValue rval, bool Declare)
        {
            this.lval = lval as LValue;
            if (this.lval == null)
            {
                throw new Exception("Cannot assign to " + lval.GetType().Name);
            }
            this.rval = rval;
            if(Declare)
            {
                var lvar = (lval as Variable);
                if(lvar == null)
                {
                    throw new Exception("Left side of declarative assignment is not a variable");
                }
                lvar.Declare = true;
            }
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            IType val = null;
            yield return (sc, st) => rval.Resolve(sc, st, r => { val = r; });
            yield return (sc, st) => lval.Set(sc, st, val);
            Result(val);
        }
    }
}
