using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    class IndexAccess : Expression, RValue
    {
        RValue target;
        RValue index;

        public IndexAccess(RValue target, RValue index)
        {
            this.target = target;
            this.index = index;
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            Log.WriteLine("Starting IndexAccess");
            IType val = null;
            yield return (sc, st) => target.Resolve(sc, st, r => { val = r; });
            var valIndex = val as IIndex;
            if (valIndex == null)
            {
                throw new Exception("target does not expose indices");
            }
            IType indexval = null;
            yield return (sc, st) => index.Resolve(sc, st, r => { indexval = r; });
            Integer indexvalint = indexval as Integer;
            if(indexvalint == null)
            {
                throw new Exception("index is not an integer");
            }
            IType result = valIndex.GetIndex(scope, state, indexvalint.Value);
            Result(result);
            Log.WriteLine("Finishing IndexAccess");
        }
    }
}
