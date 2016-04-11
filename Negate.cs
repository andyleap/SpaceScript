using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    class Negate : Expression, RValue
    {
        RValue expr;
        public Negate(RValue expr)
        {
            this.expr = expr;
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            IType val = null;
            yield return (sc, st) => expr.Resolve(sc, st, r => { val = r; });
            if(val is Integer)
            {
                (val as Integer).Value = -(val as Integer).Value;
                Result(val);
                yield break;
            }
            if (val is Float)
            {
                (val as Float).Value = -(val as Float).Value;
                Result(val);
                yield break;
            }
            throw new Exception("Cannot negate value of type " + val.GetType().Name);
        }
    }
}
