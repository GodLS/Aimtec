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

namespace SeansChoGath
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
            Obj_AI_Base.OnProcessSpellCast += ChoGath.OnProcessSpellCast;
            Orbwalker.Implementation.PostAttack += ChoGath.PostAttack;
        }

        private static void OnUpdate()
        {
            ChoGath.Run();
        }
    }
}
