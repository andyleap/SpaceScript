using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;

namespace ScriptLCD.SpaceScript.Types
{
    public class BlockGroup : IType, IFields, IIndex
    {
        string Name;
        public BlockGroup(string Name)
        {
            this.Name = Name;
        }

        public IType GetField(Scope scope, State state, string name)
        {
            var blockgroup = state.TS.GetBlockGroupWithName(Name);
            if(blockgroup.Blocks.Any(b => b.GetActionWithName(name) != null))
            {
                    return new NativeFunction((args) =>
                    {
                        foreach(var block in blockgroup.Blocks)
                        {
                            block.ApplyAction(name);
                        }
                        return null;
                    });
            }
            throw new Exception("Not implemented");
        }

        public IType GetIndex(Scope scope, State state, int index)
        {
            var blockgroup = state.TS.GetBlockGroupWithName(Name);
            if (index >= 0 && index < blockgroup.Blocks.Count)
            {
                return new Block(blockgroup.Blocks[index]);
            }
            return null;
        }

        public void SetField(Scope scope, State state, string name, IType value)
        {
            throw new Exception("Not Implemented yet");
        }
    }
}
