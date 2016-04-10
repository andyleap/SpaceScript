using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common;
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

namespace ScriptLCD.SpaceScript.Types
{
    public class List : IType
    {
        List<Variable> Items;
        public List(List<Variable> Items)
        {
            this.Items = Items;
        }

        public Variable GetItem(int index)
        {
            if(index >= 0 && index < Items.Count)
            {
                return Items[index];
            }
            return null;
        }

        public void SetItem(int index, Variable value)
        {
            if (index >= 0 && index < Items.Count)
            {
                Items[index] = value;
            }
        }
    }
}
