using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    interface IIndex
    {
        IType GetIndex(Scope scope, State state, int index);
		Integer GetLength(Scope scope, State state);
    }
}
