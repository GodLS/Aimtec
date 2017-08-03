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
    class Twitch : ChampionPlugin
    {
        static Twitch()
        {
        }

        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.spellName == "TwitchSprayandPrayAttack")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_TwitchSprayandPrayAttack;
            }
        }

        private void ProcessSpell_TwitchSprayandPrayAttack(Obj_AI_Base hero, Obj_AI_BaseMissileClientDataEventArgs args, SpellData spellData, SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.spellName == "TwitchSprayandPrayAttack")
            {
                if (args.Target != null)
                {
                    var start = hero.ServerPosition;
                    var end = hero.ServerPosition + (args.Target.Position - hero.ServerPosition) * spellData.range;

                    var data = (SpellData)spellData.Clone();
                    data.spellDelay = hero.AttackCastDelay * 1000;

                    SpellDetector.CreateSpellData(hero, start, end, data);
                }
            }
        }
    }
}
