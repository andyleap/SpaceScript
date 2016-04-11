using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    public class ExprBlock : Expression, RValue
    {
        List<RValue> expressions = new List<RValue>();
        public ExprBlock(List<RValue> expressions)
        {
            this.expressions = expressions;
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            IType val = null;
			scope.Push();
            foreach(var expr in expressions)
            {
                yield return (sc, st) => expr.Resolve(sc, st, r => { val = r; });
            }
			scope.Pop();
            Result(val);
        }
    }
}
