using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common;
using Sandbox.Game.Entities;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Engine;
using Sandbox.Game;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRage.Game;
using VRage.Game.Components;
using VRage.ObjectBuilders;
using VRage.ModAPI;
using VRage;
using ScriptLCD.SpaceScript.Parser;
using VRage.Game.ModAPI;
using Sandbox.ModAPI;

namespace ScriptLCD
{
    //here you can use any objectbuiler e.g. MyObjectBuilder_Door, MyObjectBuilder_Decoy
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_TextPanel))]
    public class ScriptLCDLogic : MyGameLogicComponent
    {
        MyObjectBuilder_EntityBase m_objectBuilder = null;
        //here you can use any inferface to your block type e.g. Sandbox.ModAPI.IMyDoort
        //if block is missing in Sandbox.ModAPI, you can use Sandbox.ModAPI.Ingame namespace to search for blockt
        Sandbox.ModAPI.IMyTextPanel Panel;
		Queue<string> Messages = new Queue<string>();
		string Content = "";
		bool ScriptPanel = false;

        //if you suscribed to events, please always unsuscribe them in close method 
        public override void Close()
        {
			if (scheduler != null)
			{
				scheduler.Shutdown();
			}
		}

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
			m_objectBuilder = objectBuilder;
			Panel = Entity as Sandbox.ModAPI.IMyTextPanel;
			//here you can add new update interval, in this case we would like to update each 100TH frame
			//you can also update each frame, each 10Th frame 
			// you can combine update intervals, so you can update every frame , every 10TH frame and every 100TH frame
			if ((Panel.BlockDefinition.SubtypeName == "LargeScriptPanel" || Panel.BlockDefinition.SubtypeName == "SmallScriptPanel") && (MyAPIGateway.Multiplayer == null || MyAPIGateway.Multiplayer.IsServer))
			{
				ScriptPanel = true;
				Entity.NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
			}
			
        }

		public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return m_objectBuilder;
        }

        //diferrence between UpdateAfter and UpdateBefore simulation is that UpdateAfter  is called after physics simulation and UpdateBefore is called
        //before physics simulation

        //this is called when  MyEntityUpdateEnum.EACH_FRAME is used as update interval

        public SpaceScript.RValue compiledProgram;

        public SpaceScript.Scheduler scheduler;

		bool NeedRedraw = false;
		int lastTickCount = 0;
        public override void UpdateAfterSimulation()
        {
			if(!ScriptPanel)
			{
				return;
			}
            IMyGridTerminalSystem ts = Sandbox.ModAPI.MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(Panel.CubeGrid as IMyCubeGrid);
            
            //Panel.WritePublicText("", false);
            try {

                if (Content != Panel.GetPrivateText())
                {
					if (scheduler != null)
					{
						scheduler.Shutdown();
					}
					scheduler = null;
					Messages.Clear();
					NeedRedraw = true;
                    Content = Panel.GetPrivateText();
                    var program = SpaceScript.Grammar.ExpressionList.TryParse(Content);
                    if (program.WasSuccessful)
                    {
                        compiledProgram = program.Value;
                        scheduler = new SpaceScript.Scheduler(compiledProgram);
						scheduler.PrintEvent.Add((msg) => { Messages.Enqueue(msg); NeedRedraw = true; });
						scheduler.ClearEvent.Add(() => { Messages.Clear(); NeedRedraw = true; });
					}
                    else
                    {
                        Messages.Enqueue(program.Message);
					}
                }
            }
            catch(Exception e)
            {
                Messages.Enqueue(e.ToString());
				NeedRedraw = true;
			}
			if (scheduler != null)
			{
				try
				{
					scheduler.MainState.TS = ts;
					lastTickCount = scheduler.RunAll();
				}
				catch (Exception e)
				{
					Messages.Enqueue(e.ToString());
					NeedRedraw = true;
				}
			}
			if (NeedRedraw)
			{
				while(Messages.Count > 10)
				{
					Messages.Dequeue();
				}
				NeedRedraw = false;
				Panel.WritePublicText(string.Join("\n", Messages.Reverse()), false);
				Panel.ShowTextureOnScreen();
				Panel.ShowPublicTextOnScreen();
			}
        }

        //this is called when  MyEntityUpdateEnum.EACH_10TH_FRAME is used as update interval
        public override void UpdateAfterSimulation10()
        {
			
        }

        //this is called when  MyEntityUpdateEnum.EACH_100TH_FRAME is used as update interval
        public override void UpdateAfterSimulation100()
        {
            
        }

        public override void UpdateBeforeSimulation()
        {
        }

        public override void UpdateBeforeSimulation10()
        {
        }

        public override void UpdateBeforeSimulation100()
        {
        }

    }
}
