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
        List<Thread> Threads = new List<Thread>();
        public State MainState = new State();
        Thread TickThread = null;
        IInvoke TickMethod = null;
        Thread CurThread = null;

        public Scheduler(RValue program)
        {
            var mainthread = new Thread(program);
            mainthread.Walker.Scope.SetValue("Tick", new NativeFunction(args =>
            {
                if(args.Count != 1 || !(args[0] is IInvoke))
                {
                    throw new Exception("Tick expects a single invokable argument");
                }
                TickMethod = args[0] as IInvoke;
                return new Bool(true);
            }), true);
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
                    if(args.Count != 1 || !(args[0] is IInvoke))
                    {
                        throw new Exception("Thread.New expects an IInvokable");
                    }
                    Threads.Add(new Thread(args[0] as IInvoke));
                    return new Bool(true);
                }) }
            }, true);

            Threads.Add(mainthread);
        }

        public void RunAll()
        {
            if((TickThread == null || !TickThread.Running) && TickMethod != null)
            {
                TickThread = new Thread(TickMethod);
                Threads.Add(TickThread);
            }
            var deadThreads = new List<Thread>();
            foreach(var thread in Threads)
            {
                CurThread = thread;
                thread.Walker.State = MainState;
                if(!thread.Run(100))
                {
                    deadThreads.Add(thread);
                }
                CurThread = null;
            }
            foreach(var thread in deadThreads)
            {
                Threads.Remove(thread);
            }
        }


    }

    class Thread
    {
        private class ThreadWrapper : RValue
        {
            IInvoke method;

            public ThreadWrapper(IInvoke method)
            {
                this.method = method;
            }

            public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
            {
                yield return (sc, st) => method.Invoke(sc, st, new List<RValue>(), Result);
            }
        }
        public ScriptWalker Walker;

        bool IsSleeping = false;
        DateTime SleepUntil;
        public bool Running = true;

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

        public bool Run(int steps)
        {
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
            for (int l1 = 0; l1 < 100; l1++)
            {
                if (!Walker.Step())
                {
                    Running = false;
                    return false;
                }
                if(IsSleeping)
                {
                    return true;
                }
            }
            return true;
        }
    }
}
