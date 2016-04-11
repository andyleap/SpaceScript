using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI;
using System.IO;

namespace ScriptLCD.SpaceScript
{
    class Log
    {
        static StringBuilder log;

        static Log()
        {
            log = new StringBuilder();
        }

        public static void WriteLine(string Line)
        {
            log.AppendLine(Line);
        }
    }
}
