using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec;
using Spell = Aimtec.SDK.Spell;

namespace Seans_Damage_Indicator
{
    internal class Spells
    {
        public static Spell Q { get; set; }
        public static Spell W { get; set; }
        public static Spell E { get; set; }
        public static Spell R { get; set; }
        public static Spell Ignite { get; set; }

        public static void Create()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            var ignite = ObjectManager.GetLocalPlayer().SpellBook.Spells.FirstOrDefault(x => x.Name.ToLower() == "summonerdot");
            if (ignite != null)
                Ignite = new Spell(ignite.Slot);


        }

        public static string[] PetSpells =
        {
            // Annie
            "InfernalGuardianGuide",
            // Shaco
            //"HallucinateGuide"
        };
    }
}