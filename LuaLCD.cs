using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common;
using Sandbox.Game.Entities;
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
using ScriptLCD.SpaceScript.Parser;

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
        string Content = "";
        string Result = "";
        int ChangeCount = 0;

        //if you suscribed to events, please always unsuscribe them in close method 
        public override void Close()
        {
        }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            //here you can add new update interval, in this case we would like to update each 100TH frame
            //you can also update each frame, each 10Th frame 
            // you can combine update intervals, so you can update every frame , every 10TH frame and every 100TH frame
            Entity.NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME | MyEntityUpdateEnum.EACH_10TH_FRAME;
            m_objectBuilder = objectBuilder;
            Panel = Entity as Sandbox.ModAPI.IMyTextPanel;
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

        public double compileTime = 0;
        public double runTime = 0;
        string ParseError = "";
        public override void UpdateAfterSimulation()
        {
            IMyGridTerminalSystem ts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(Panel.CubeGrid as Sandbox.ModAPI.IMyCubeGrid);
            
            //Panel.WritePublicText("", false);
            try {

                if (Content != Panel.GetPrivateText())
                {
                    ParseError = "";
                    Content = Panel.GetPrivateText();
                    var compiletimer = System.Diagnostics.Stopwatch.StartNew();
                    var program = SpaceScript.Grammar.ExpressionList.TryParse(Content);
                    compiletimer.Stop();
                    compileTime = compiletimer.Elapsed.TotalMilliseconds;
                    if (program.WasSuccessful)
                    {
                        compiledProgram = program.Value;
                        scheduler = new SpaceScript.Scheduler(compiledProgram);
                        var runTimer = System.Diagnostics.Stopwatch.StartNew();
                        scheduler.MainState.TS = ts;
                        scheduler.RunAll();
                        runTimer.Stop();
                        runTime = runTimer.Elapsed.TotalMilliseconds;
                    }
                    else
                    {
                        Result = program.Message;
                    }
                }
            }
            catch(Exception e)
            {
                ParseError = e.ToString();
            }
            Panel.WritePublicText(Content, false);
            Panel.WritePublicText("\n", true);
            Panel.WritePublicText(ParseError, true);
            Panel.WritePublicText("\n", true);
            Panel.WritePublicText(Result, true);
            Panel.WritePublicText(String.Format("\nCompile {0}ms\nRun {1}ms", compileTime, runTime), true);
        }

        //this is called when  MyEntityUpdateEnum.EACH_10TH_FRAME is used as update interval
        public override void UpdateAfterSimulation10()
        {
            IMyGridTerminalSystem ts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(Panel.CubeGrid as Sandbox.ModAPI.IMyCubeGrid);
            try
            {
                var runTimer = System.Diagnostics.Stopwatch.StartNew();
                scheduler.MainState.TS = ts;
                scheduler.RunAll();
                runTimer.Stop();
                runTime = runTimer.Elapsed.TotalMilliseconds;
            }
            catch (Exception e)
            {
                Result = e.ToString();
            }
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
