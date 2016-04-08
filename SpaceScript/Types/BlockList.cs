using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    public class BlockList : IType
    {
        List<Sandbox.ModAPI.Ingame.IMyTerminalBlock> Items;
        public BlockList(List<Sandbox.ModAPI.Ingame.IMyTerminalBlock> Items)
        {
            this.Items = Items;
        }

        public IType GetItem(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                return new Block(Items[index]);
            }
            return null;
        }
    }
}
