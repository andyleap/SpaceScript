using ScriptLCD.SpaceScript.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript
{
    class While : RValue
    {
        RValue Cond;
        RValue Loop;

        public While(RValue cond, RValue loop)
        {
            Cond = cond;
            Loop = loop;
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            IType condVal = null;

            yield return (sc, st) => Cond.Resolve(sc, st, r => { condVal = r; });
            if (!(condVal is Bool))
            {
                throw new Exception(string.Format("{0} is not Bool", condVal));
            }
            IType val = null;
            while ((condVal as Bool).Value)
            {
                yield return (sc, st) => Loop.Resolve(sc, st, r => { val = r; });
                yield return (sc, st) => Cond.Resolve(sc, st, r => { condVal = r; });
                if (!(condVal is Bool))
                {
                    throw new Exception(string.Format("{0} is not Bool", condVal));
                }
            }
            Result(val);
        }


    }
}
