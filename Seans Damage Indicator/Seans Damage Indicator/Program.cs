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
            Menu.Create();
            Spells.Create();
            CheckVersion(0.1);

            Render.OnPresent += DamageIndicator.OnPresent;
        }

        private static void CheckVersion(double currentVersion)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://placeholder");
            if (stream != null)
            {
                StreamReader reader = new StreamReader(stream);
                String content = reader.ReadToEnd();
                Regex versionString = new Regex(@"version:[0-9]+");
                Regex versionNumber = new Regex(@"[0-9]+");

                if (Convert.ToDouble(versionNumber.IsMatch(versionString.Match(content).Value)) > currentVersion)
                {
                    for (int i = 0; i < 10; i++)
                        Console.WriteLine(">> [[ SEANS DAMAGE INDICATOR ]] - OUTDATED - PLEASE UPDATE <<");
                }
                else
                {
                    Console.WriteLine(">> [[ SEANS DAMAGE INDICATOR ]] - UP TO DATE <<");

                }
            }
        }
    }
}
