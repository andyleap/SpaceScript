using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    public class Variable : Expression, LValue, RValue
    {
        string name;
        public bool Declare = false;

        public Variable(string name)
        {
            this.name = name;
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            Result(scope.GetValue(name));
            yield break;
        }

        public IEnumerable<Node> Set(Scope scope, State state, IType Value)
        {
            scope.SetValue(name, Value, Declare);
            yield break;
        }
    }
}
