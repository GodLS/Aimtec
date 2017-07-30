using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aimtec;
using Aimtec.SDK;
using Aimtec.SDK.Events;

namespace Seans_Damage_Indicator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameEvents.GameStart += GameStart;
        }

        private static void GameStart()
        {
            CheckVersion(0.2);

            Menu.Create();
            Spells.Create();

            Render.OnPresent += DamageIndicator.OnPresent;
        }

        private static void CheckVersion(double currentVersion)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://github.com/GodLS/Aimtec/blob/master/Seans%20Damage%20Indicator/Seans%20Damage%20Indicator/version.txt");
            if (stream != null)
            {
                StreamReader reader = new StreamReader(stream);
                String content = reader.ReadToEnd();
                Regex versionString = new Regex(@"version:\d+\.\d+");
                Regex versionNumber = new Regex(@"\d+.\d+");

                if (Convert.ToDouble(versionNumber.Match(versionString.Match(content).Value).Value) > currentVersion)
                {
                    for (int i = 0; i < 10; i++)
                        Console.WriteLine(">> [[ SEANS DAMAGE INDICATOR ]] - OUTDATED - PLEASE UPDATE <<");
                }
                else
                    Console.WriteLine(">> [[ SEANS DAMAGE INDICATOR ]] - UP TO DATE <<");


            }
        }
    }
}
