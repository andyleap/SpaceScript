using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    class MethodCall : Expression, RValue
    {
        RValue target;
        List<RValue> parameters;

        public MethodCall(RValue target, List<RValue> parameters)
        {
            this.target = target;
            this.parameters = parameters;
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            Log.WriteLine("Starting MethodCall");
            IType method = null;
            yield return (sc, st) => target.Resolve(sc, st, r => { method = r; });
            var invokeable = method as IInvoke;
            if (invokeable == null)
            {
                throw new Exception("Method is not invokable");
            }
            yield return (sc, st) => invokeable.Invoke(sc, st, parameters, Result);
			Log.WriteLine("Finishing MethodCall");
        }
    }
}
