using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

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

		public Integer GetLength(Scope scope, State state)
		{
			var blockgroup = state.TS.GetBlockGroupWithName(Name);
			return new Integer(blockgroup.Blocks.Count);
		}

		public void SetField(Scope scope, State state, string name, IType value)
        {
			var blockgroup = state.TS.GetBlockGroupWithName(Name);
			foreach (var block in blockgroup.Blocks)
			{
				var property = block.GetProperty(name);
				if (property != null)
				{
					switch (property.TypeName)
					{
						case "Bool":
							property.AsBool().SetValue(block, value.Cast<Bool>().Value);
							continue;
						case "Single":
							property.AsFloat().SetValue(block, value.Cast<Float>().Value);
							continue;
						case "Color":
							var c = value.Cast<Color>();
							property.AsColor().SetValue(block, new VRageMath.Color(c.R, c.G, c.B, c.A));
							continue;
					}
					throw new Exception("Unexpected property type: " + property.TypeName);
				}
			}
        }
    }
}
