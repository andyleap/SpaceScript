using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    class Closure : Expression, RValue
    {
        UserFunction func;

        public Closure(UserFunction func)
        {
            this.func = func;
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            func.Closure = new Stack<ScopeData>(scope.DataStack);
            Result(func);
            yield break;
        }
    }
}
