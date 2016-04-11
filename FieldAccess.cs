using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    class FieldAccess : Expression, RValue, LValue
    {
        RValue target;
        string field;

        public FieldAccess(RValue target, string field)
        {
            this.target = target;
            this.field = field;
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            Log.WriteLine("Starting FieldAccess");
            IType val = null;
            yield return (sc, st) => target.Resolve(sc, st, r => { val = r; });
            var valFields = val as IFields;
            if(valFields == null)
            {
                throw new Exception("target does not expose fields");
            }
            IType result = valFields.GetField(scope, state, field);
            Result(result);
            Log.WriteLine("Finishing FieldAccess");
        }

        public IEnumerable<Node> Set(Scope scope, State state, IType Value)
        {
            Log.WriteLine("Starting FieldAccess");
            IType val = null;
            yield return (sc, st) => target.Resolve(sc, st, r => { val = r; });
            var valFields = val as IFields;
            if (valFields == null)
            {
                throw new Exception("target does not expose fields");
            }
            valFields.SetField(scope, state, field, Value);
            Log.WriteLine("Finishing FieldAccess");
        }
    }
}
