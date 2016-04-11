using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    class CompOp : Expression, RValue
    {
        RValue v1;
        string op;
        RValue v2;

        public CompOp(RValue v1, string op, RValue v2)
        {
            this.v1 = v1;
            this.op = op;
            this.v2 = v2;
        }
        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            IType val1 = null;
            IType val2 = null;
            yield return (sc, st) => v1.Resolve(sc, st, r => { val1 = r; });
            yield return (sc, st) => v2.Resolve(sc, st, r => { val2 = r; });
            switch (op)
            {
                case "==":
                    Result(new Bool(val1.Equals(val2)));
                    break;
                case "!=":
                    Result(new Bool(!val1.Equals(val2)));
                    break;

            }
        }
    }
}
