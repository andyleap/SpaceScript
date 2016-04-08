using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    public class NativeFunction : IType, IInvoke
    {
        Func<List<IType>, IType> Func;
        public NativeFunction(Func<List<IType>, IType> Func)
        {
            this.Func = Func;
        }

        public IEnumerable<Node> Invoke(Scope scope, State state, List<RValue> args, Action<IType> Result)
        {
            var parameters = new List<IType>();
            foreach(var arg in args)
            {
                yield return (sc, st) => arg.Resolve(sc, st, r => { parameters.Add(r); });
            }
            Result(Func(parameters));
        }
    }
}
