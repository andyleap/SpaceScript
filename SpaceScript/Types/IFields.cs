using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    public interface IFields
    {
        IType GetField(Scope scope, State state, string name);
        void SetField(Scope scope, State state, string name, IType value);
    }
}
