using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    class UserFunction : IType, IInvoke
    {
        List<string> Arguments = new List<string>();
        RValue Body;
        public Stack<ScopeData> Closure;

        public UserFunction(RValue Body, List<string> Arguments)
        {
            this.Body = Body;
            this.Arguments = Arguments;
        }

        public IEnumerable<Node> Invoke(Scope scope, State state, List<RValue> args, Action<IType> Result)
        {
            var Parameters = new List<IType>();

            foreach(var arg in args)
            {
                yield return (sc, st) => arg.Resolve(sc, st, r => { Parameters.Add(r); });
            }
            scope.PushClosure(Closure);
            scope.Push();
            try
            {
                for(int l1 = 0; l1 < Arguments.Count; l1++)
                {
                    if(l1 >= Parameters.Count)
                    {
                        throw new Exception("Not enough arguments to function");
                    }
                    scope.SetValue(Arguments[l1], Parameters[l1], true);
                }
                yield return (sc, st) => Body.Resolve(sc, st, Result);
            }
            finally
            {
                scope.Pop();
                scope.PopClosure();
            }
        }
    }
}
