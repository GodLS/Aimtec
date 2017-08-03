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
    class Heimerdinger : ChampionPlugin
    {
        static Heimerdinger()
        {

        }

        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.spellName == "HeimerdingerTurretEnergyBlast"
                || spellData.spellName == "HeimerdingerTurretBigEnergyBlast")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_HeimerdingerTurretEnergyBlast;
            }

            if (spellData.spellName == "HeimerdingerW")
            {
                //SpellDetector.OnProcessSpecialSpell += ProcessSpell_HeimerdingerW;
            }
        }

        private void ProcessSpell_HeimerdingerW(Obj_AI_Base hero, Obj_AI_BaseMissileClientDataEventArgs args, SpellData spellData, SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.spellName == "HeimerdingerW")
            {
                //SpellDetector.CreateSpellData(hero, args.Start, args.End, spellData);

                specialSpellArgs.noProcess = true;
            }
        }

        private static void ProcessSpell_HeimerdingerTurretEnergyBlast(Obj_AI_Base hero, Obj_AI_BaseMissileClientDataEventArgs args, SpellData spellData, SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.spellName == "HeimerdingerTurretEnergyBlast"
                || spellData.spellName == "HeimerdingerTurretBigEnergyBlast")
            {
                SpellDetector.CreateSpellData(hero, args.Start, args.End, spellData);

                specialSpellArgs.noProcess = true;
            }
        }
    }
}
