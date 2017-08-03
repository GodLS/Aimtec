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
    class Malzahar : ChampionPlugin
    {
        static Malzahar()
        {

        }

        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.spellName == "MalzaharQ")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_AlZaharCalloftheVoid;
            }
        }

        private static void ProcessSpell_AlZaharCalloftheVoid(Obj_AI_Base hero, Obj_AI_BaseMissileClientDataEventArgs args, SpellData spellData,
            SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.spellName == "MalzaharQ")
            {
                var direction = (args.End.To2D() - args.Start.To2D()).Normalized();
                var pDirection = direction.Perpendicular();
                var targetPoint = args.End.To2D();

                var pos1 = targetPoint - pDirection * spellData.sideRadius;
                var pos2 = targetPoint + pDirection * spellData.sideRadius;

                SpellDetector.CreateSpellData(hero, pos1.To3D(), pos2.To3D(), spellData, null, 0, false);
                SpellDetector.CreateSpellData(hero, pos2.To3D(), pos1.To3D(), spellData, null, 0);

                specialSpellArgs.noProcess = true;
            }
        }
    }
}
