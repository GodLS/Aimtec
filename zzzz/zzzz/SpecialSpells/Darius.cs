using System;
using Aimtec.SDK.Extensions;
using System.Linq;
using Aimtec;
using Aimtec.SDK.Util.Cache;
using Aimtec.SDK;

namespace zzzz.SpecialSpells
{
    class Darius : ChampionPlugin
    {
        static Darius()
        {
            // todo: fix for multiple darius' on same team (one for all)
        }

        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.spellName == "DariusCleave")
            {
                var hero = GameObjects.Heroes.FirstOrDefault(x => x.ChampionName == "Darius");
                if (hero != null && hero.CheckTeam())
                {
                    Game.OnUpdate += () => Game_OnUpdate(hero);
                }
            }
        }

        private void Game_OnUpdate(Obj_AI_Base hero)
        {
            foreach (var spell in SpellDetector.detectedSpells.Where(x => x.Value.heroID == hero.NetworkId))
            {
                if (spell.Value.info.spellName == "DariusCleave")
                {
                    spell.Value.startPos = hero.ServerPosition.To2D();
                    spell.Value.endPos = hero.ServerPosition.To2D() + spell.Value.direction * spell.Value.info.range;
                }
            }
        }
    }
}
