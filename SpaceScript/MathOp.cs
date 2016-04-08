using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    class MathOp : Expression, RValue
    {
        RValue v1;
        char op;
        RValue v2;

        public MathOp(RValue v1, char op, RValue v2)
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
            switch(op)
            {
                case '+':
                    Result(val1.Add(val2));
                    break;
                case '-':
                    Result(val1.Sub(val2));
                    break;
                case '*':
                    Result(val1.Mul(val2));
                    break;
                case '/':
                    Result(val1.Div(val2));
                    break;
            }
        }
    }
}
