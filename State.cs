using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;

namespace ScriptLCD.SpaceScript
{
    public class State
    {
        public IMyGridTerminalSystem TS;
		public List<Action> EventHandlerRemovers = new List<Action>();
		public Scheduler scheduler;
    }
}
