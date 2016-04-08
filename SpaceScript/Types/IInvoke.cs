using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    public interface IInvoke
    {
        IEnumerable<Node> Invoke(Scope scope, State state, List<RValue> args, Action<IType> Result);
    }
}
