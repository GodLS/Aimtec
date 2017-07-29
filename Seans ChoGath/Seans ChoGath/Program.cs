using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec;
using Aimtec.SDK.Events;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Orbwalking;

namespace Seans_ChoGath
{
    class Program
    {
        static void Main(string[] args)
        {
            GameEvents.GameStart += GameStart;
        }

        private static void GameStart()
        {
            Menu.Create();
            Spells.Create();

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnLevelUp += ChoGath.OnLevelUp;
            Orbwalker.Implementation.PostAttack += ChoGath.PostAttack;
            Render.OnPresent += ChoGath.OnPresent;

        }

        private static void OnUpdate()
        {
            ChoGath.Run();
        }
    }
}
