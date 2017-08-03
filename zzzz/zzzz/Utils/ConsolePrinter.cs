using System;
using Aimtec.SDK.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zzzz
{
    public static class ConsolePrinter
    {
        private static float lastPrintTime = 0;

        static ConsolePrinter()
        {

        }

        public static void Print(string str)
        {
            //return;

            var timeDiff = EvadeUtils.TickCount - lastPrintTime;

            var finalStr = "[" + timeDiff + "] " + str;

            Console.WriteLine(finalStr);

            lastPrintTime = EvadeUtils.TickCount;
        }
    }
}
