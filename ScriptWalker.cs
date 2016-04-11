using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript
{
    class ScriptWalker
    {
        public Scope Scope = new Scope();
        public State State = new State();
        Stack<IEnumerator<Node>> NodeStack = new Stack<IEnumerator<Node>>();
        IEnumerator<Node> CurNode;

        public void Start(RValue Root)
        {
            Scope = new Scope();
            State = new State();
            Log.WriteLine("Starting Root");
            Scope.Push();
            CurNode = Root.Resolve(Scope, State, r => { }).GetEnumerator();
        }

        public bool Step()
        {
            Log.WriteLine("Running Node");
            if(CurNode == null)
            {
                return false;
            }
            if(!CurNode.MoveNext())
            {
                if(NodeStack.Count == 0)
                {
                    CurNode = null;
                    return false;
                }
                Log.WriteLine("Popping Node");
                CurNode = NodeStack.Pop();
                return true;
            }
			var NewNode = (CurNode.Current as Node)(Scope, State);
			if(NewNode == null)
			{
				return true;
			}
            var NewEnum = NewNode.GetEnumerator();
            NodeStack.Push(CurNode);
            Log.WriteLine("Pushing Node");
            CurNode = NewEnum;
            return true;
        }
    }
}
