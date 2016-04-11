using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;

namespace ScriptLCD.SpaceScript
{
    public class Scope
    {
        public Stack<Stack<ScopeData>> ClosureStack = new Stack<Stack<ScopeData>>();
        public Stack<ScopeData> DataStack = new Stack<ScopeData>();

        public void Push()
        {
            DataStack.Push(new ScopeData());
        }

        public void Pop()
        {
            DataStack.Pop();
        }

        public void PushClosure(Stack<ScopeData> closure)
        {
            ClosureStack.Push(DataStack);
            DataStack = closure;
        }

        public void PopClosure()
        {
            DataStack = ClosureStack.Pop();
        }

        public IType GetValue(string name)
        {
            foreach(var sd in DataStack)
            {
                if(sd.Variables.ContainsKey(name))
                {
                    return sd.Variables[name];
                }
            }
            return null;
        }

        public void SetValue(string name, IType Value, bool Declare)
        {
            if (Declare)
            {
                var ds = DataStack.Peek();
                if (ds.Variables.ContainsKey(name))
                {
                    throw new Exception(string.Format("Variable {0} already exists in this scope", name));
                }
                ds.Variables.Add(name, Value);
            }
            else
            {
                foreach (var sd in DataStack)
                {
                    if (sd.Variables.ContainsKey(name))
                    {
                        sd.Variables[name] = Value;
                        return;
                    }
                }
                throw new Exception(string.Format("Variable {0} does not exist", name));
            }
        }
    }

    public class ScopeData
    {
        public Dictionary<string, IType> Variables = new Dictionary<string, IType>();
    }
}
