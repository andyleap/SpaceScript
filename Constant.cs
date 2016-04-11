using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    class Constant : Expression, RValue
    {
        IType value;

        public Constant(IType value)
        {
            this.value = value;
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            Result(value);
            yield break;
        }
    }
}
