using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Engine;
using Sandbox.Game;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRage.Game;
using VRage.Game.Components;
using VRage.ObjectBuilders;
using VRage.ModAPI;
using VRage;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    public class GroupAccess : RValue
    {
        string Name;
        public GroupAccess(string Name)
        {
            this.Name = Name;
        }

        public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
        {
            Log.WriteLine("Starting GroupAccess");
            Result(new BlockGroup(Name));
            Log.WriteLine("Finishing GroupAccess");
            yield break;
        }
    }
}
