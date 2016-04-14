using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptLCD.SpaceScript.Types;



namespace ScriptLCD.SpaceScript
{
    public class Scheduler
    {
        public List<Thread> Threads = new List<Thread>();
        public State MainState = new State();
        Thread CurThread = null;

		const int MaxExecution = 10000;
		const int ShortPoolPerTick = 500;
		const int ReplenishPerTick = 100;
		const int MaxShortPool = 2000;

		public int ExecutionPool = 0;
		public int ShortPool = 0;


        public Scheduler(RValue program)
        {
			MainState.scheduler = this;
			var mainthread = new Thread(program);
            mainthread.Walker.Scope.SetValue("Sleep", new NativeFunction(args =>
            {
                if (args.Count != 1 || !(args[0] is Integer))
                {
                    throw new Exception("Sleep expects a single integer argument");
                }
                if (CurThread == null)
                {
                    throw new Exception("Not in thread");
                }
                CurThread.Sleep((args[0] as Integer).Value);
                return new Bool(true);
            }), true);
            mainthread.Walker.Scope.SetValue("Thread", new Types.Object()
            {
                {"New", new NativeFunction(args =>
					{
						if(args.Count < 1 || !(args[0] is IInvoke))
						{
							throw new Exception("Thread.New expects an IInvokable");
						}
						Threads.Add(new Thread(args[0] as IInvoke, args.Skip(1).ToList()));
						return new Bool(true);
					})}
            }, true);

			mainthread.Walker.Scope.SetValue("Print", new NativeFunction(args =>
			{
				string value = "";
				foreach(var arg in args)
				{
					if (arg == null)
					{
						value += "<null>";
					}
					else
					{
						value += arg.ToString();
					}
				}
				Print(value);
				return new Bool(true);
			}), true);

			mainthread.Walker.Scope.SetValue("Clear", new NativeFunction(args =>
			{
				Clear();
				return new Bool(true);
			}), true);

			mainthread.Walker.Scope.SetValue("Block", new Types.Object()
			{
				{"Get", new NativeFunction(args =>
					{
						if(args.Count != 1 || !(args[0] is Types.String))
						{
							throw new Exception("Block.Get expects a String");
						}
						var block = MainState.TS.GetBlockWithName((args[0] as Types.String).Value);
						return new Block(block);
					})}
			}, true);



			mainthread.Walker.Scope.SetValue("Math", new Types.Object()
			{
				{"Sin", new NativeFunction(args =>
				{
					if(args.Count != 1)
					{
						throw new Exception("Math.Sin expects a single argument");
					}
					return new Float((float)Math.Sin(args[0].Cast<Float>().Value));
				}) }
			}, true);

			Threads.Add(mainthread);
        }

		public List<Action<string>> PrintEvent = new List<Action<string>>();

		private void Print(string value)
		{
			foreach(var print in PrintEvent)
			{
				print(value);
			}
		}

		public List<Action> ClearEvent = new List<Action>();

		private void Clear()
		{
			foreach (var clear in ClearEvent)
			{
				clear();
			}
		}

		public int RunAll()
        {
			int usedTicks = 0;
			ExecutionPool += ReplenishPerTick;
			var ShortPoolReplenish = Math.Min(Math.Min(MaxShortPool - ShortPool, ShortPoolPerTick), ExecutionPool);
			ShortPool += ShortPoolReplenish;
			ExecutionPool -= ShortPoolReplenish;
			if(ExecutionPool > MaxExecution)
			{
				ExecutionPool = MaxExecution;
			}
			if(Threads.Count == 0)
			{
				return 0;
			} 

			var deadThreads = new List<Thread>();
			int Share = ShortPool / Threads.Count;
            foreach(var thread in Threads.ToList())
            {
                CurThread = thread;
                thread.Walker.State = MainState;
				int usedShare = 0;
				try
				{
					if (!thread.Run(Share, out usedShare))
					{
						deadThreads.Add(thread);
					}
				}
				catch(Exception e)
				{
					Print(e.Message);
					deadThreads.Add(thread);
				}
				ShortPool -= usedShare;
				usedTicks += usedShare;
                CurThread = null;
            }

            foreach(var thread in deadThreads)
            {
                Threads.Remove(thread);
            }
			return usedTicks;
        }

		public void Shutdown()
		{
			Threads.Clear();
			MainState.EventHandlerRemovers.ForEach(a => a());
		}
    }

    public class Thread
    {
        private class ThreadWrapper : RValue
        {
            IInvoke method;
			List<RValue> args = new List<RValue>();

            public ThreadWrapper(IInvoke method, List<IType> args = null)
            {
                this.method = method;
				if(args != null)
				{
					this.args = args.Select(a => new Constant(a)).ToList<RValue>();
				}
            }

            public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
            {
                yield return (sc, st) => method.Invoke(sc, st, args, Result);
            }
        }
        internal ScriptWalker Walker;

        bool IsSleeping = false;
        DateTime SleepUntil;
        public bool Running = true;

		public Thread(IInvoke Method, List<IType> args) : this(new ThreadWrapper(Method, args))
		{
		}

		public Thread(IInvoke Method) : this(new ThreadWrapper(Method))
        {
        }

        public Thread(RValue Method)
        {
            Walker = new ScriptWalker();
            Walker.Start(Method);
        }

        public void Sleep(int milliseconds)
        {
            SleepUntil = DateTime.Now.AddMilliseconds(milliseconds);
            IsSleeping = true;
        }

        public bool Run(int steps, out int usedsteps)
        {
			usedsteps = 0;
            if(!Running)
            {
                return false;
            }
            if(IsSleeping)
            {
                if(DateTime.Now > SleepUntil)
                {
                    IsSleeping = false;
                }
                else
                {
                    return true;
                }
            }
            for (int l1 = 0; l1 < steps; l1++)
            {
                if (!Walker.Step())
                {
                    Running = false;
                    return false;
                }
				usedsteps += 1;
                if(IsSleeping)
                {
                    return true;
                }
            }
            return true;
        }
    }
}
