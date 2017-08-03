using System;
using Aimtec.SDK.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aimtec;
using Aimtec.SDK.Util.Cache;
using Aimtec.SDK;
//using SharpDX;

namespace zzzz.SpecialSpells
{
    class Ekko : ChampionPlugin
    {
        static Ekko()
        {

        }

        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.spellName == "EkkoR")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_EkkoR;
            }
        }

        private static void ProcessSpell_EkkoR(Obj_AI_Base hero, Obj_AI_BaseMissileClientDataEventArgs args, SpellData spellData,
            SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.spellName == "EkkoR")
            {
                foreach (var obj in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (obj != null && obj.IsValid && !obj.IsDead && obj.Name == "Ekko" && obj.CheckTeam())
                    {
                        var blinkPos = obj.ServerPosition.To2D();

                        SpellDetector.CreateSpellData(hero, args.Start, blinkPos.To3D(), spellData);
                    }
                }

                specialSpellArgs.noProcess = true;
            }
        }
    }
}
