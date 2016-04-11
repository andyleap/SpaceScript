using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    public interface Value
    {
        
    }

    public interface RValue : Value
    {
        IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result);
    }

    public interface LValue
    {
        IEnumerable<Node> Set(Scope scope, State state, IType Value);
    }

    public interface LRValue : RValue, LValue
    {

    }
}
