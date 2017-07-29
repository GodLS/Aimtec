using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Prediction.Skillshots;
using Spell = Aimtec.SDK.Spell;

namespace Seans_ChoGath
{
    class Spells
    {
        public static Spell Q, W, E, R;

        public static void Create()
        {
            Q = new Spell(SpellSlot.Q, 950f);
            W = new Spell(SpellSlot.W, 750f);
            E = new Spell(SpellSlot.E, ObjectManager.GetLocalPlayer().AttackRange + 50f);
            R = new Spell(SpellSlot.R, 175f);

            Q.SetSkillshot(1.2f, 250f, int.MaxValue, false, SkillshotType.Circle, false, HitChance.None);
            W.SetSkillshot(.25f, 175, 1750, false, SkillshotType.Cone, false, HitChance.None);

        }
    }
}
