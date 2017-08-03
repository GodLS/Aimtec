using System;
using Aimtec.SDK.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec;
using Aimtec.SDK;
using Aimtec.SDK.Util.Cache;

namespace zzzz.SpecialSpells
{
    class Ahri : ChampionPlugin
    {
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.spellName == "AhriOrbofDeception2")
            {
                var hero = GameObjects.Heroes.FirstOrDefault(x => x.ChampionName == "Ahri");
                if (hero != null && hero.CheckTeam())
                {
                    Game.OnUpdate += () => Game_OnUpdate(hero);
                }
            }
        }

        private void Game_OnUpdate(Obj_AI_Hero hero)
        {
            foreach (
                var spell in
                SpellDetector.detectedSpells.Where(
                    s =>
                        s.Value.heroID == hero.NetworkId &&
                        s.Value.info.spellName.ToLower() == "ahriorbofdeception2"))
            {
                spell.Value.endPos = hero.ServerPosition.To2D();
            }
        }
    }
}
