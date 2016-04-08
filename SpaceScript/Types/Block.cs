using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Interfaces;

namespace ScriptLCD.SpaceScript.Types
{
    public class Block : IType, IFields
    {
        IMyTerminalBlock block;
        public Block(IMyTerminalBlock block)
        {
            this.block = block;
        }

        public IType GetField(Scope scope, State state, string name)
        {
            var action = block.GetActionWithName(name);
            if (action != null)
            {
                return new NativeFunction((args) =>
                {
                    IMyTerminalBlock terminalblock = block;
                    action.Apply(terminalblock);
                    return null;
                });
            }
            var property = block.GetProperty(name);

            if (property != null)
            {
                switch (property.TypeName)
                {
                    case "Bool":
                        return new Bool(property.AsBool().GetValue(block));
                    case "Float":
                        return new Float(property.AsFloat().GetValue(block));
                    case "Color":
                        var c = property.AsColor().GetValue(block);
                        return new Color(c.R, c.G, c.B, c.A);
                }
            }
            if (block is IMyDoor && name == "IsOpen")
            {
                var door = block as IMyDoor;
                return new Bool(door.Open);
            }


            throw new Exception("Not Implemented");
        }

        public void SetField(Scope scope, State state, string name, IType value)
        {
            var property = block.GetProperty(name);
            if (property != null)
            {
                switch (property.TypeName)
                {
                    case "Bool":
                        property.AsBool().SetValue(block, value.Cast<Bool>().Value);
                        break;
                    case "Float":
                        property.AsFloat().SetValue(block, value.Cast<Float>().Value);
                        break;
                    case "Color":
                        var c = value.Cast<Color>();
                        property.AsColor().SetValue(block, new VRageMath.Color(c.R, c.G, c.B, c.A));
                        break;
                }
                return;
            }
            throw new Exception("Block does not have property: " + name);
        }
    }
}
