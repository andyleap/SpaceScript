using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    public class If : RValue
    {
        RValue Cond;
        RValue TrueBranch;
        RValue FalseBranch;

        public If(RValue cond, RValue truebranch, RValue falsebranch)
        {
            Cond = cond;
            TrueBranch = truebranch;
            FalseBranch = falsebranch;
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            IType condVal = null;
            yield return (sc, st) => Cond.Resolve(sc, st, r => { condVal = r; });
            if(!(condVal is Bool))
            {
                throw new Exception(string.Format("{0} is not Bool", condVal));
            }
            IType val = null;
            if((condVal as Bool).Value)
            {
                yield return (sc, st) => TrueBranch.Resolve(sc, st, r => { val = r; });
            }
            else
            {
                if (FalseBranch != null)
                {
                    yield return (sc, st) => FalseBranch.Resolve(sc, st, r => { val = r; });
                }
                else
                {
                    val = new Bool(false);
                }
            }
            Result(val);
        }
    }
}
