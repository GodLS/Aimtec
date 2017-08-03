using System;
using Aimtec.SDK.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zzzz
{
    class Program
    {
        private static Evade evade;

        static void Main(string[] args)
        {
            try
            {
                evade = new Evade();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
