using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util.Cache;

namespace Seans_ChoGath
{
    class Menu
    {
        public static Aimtec.SDK.Menu.Menu _menu = new Aimtec.SDK.Menu.Menu("seans.chogath", "Seans Cho'Gath", true);

        public static void Create()
        {
            Orbwalker.Implementation.Attach(_menu);

            var comboMenu = new Aimtec.SDK.Menu.Menu("combo", "Combo");
            _menu.Add(comboMenu);

            comboMenu.Add(new MenuBool("combo.attackselectedonly", "Only attack selected target"));

            var comboQMenu = new Aimtec.SDK.Menu.Menu("combo.q.menu", "Q Settings");
            comboMenu.Add(comboQMenu);

            comboQMenu.Add(new MenuBool("combo.q", "Use Q"));
            comboQMenu.Add(new MenuBool("combo.q.aoe", "AoE Q"));
            comboQMenu.Add(new MenuSlider("combo.q.aoe.mintargets", ">> minimum targets to hit", 2, 1, 5));
            comboQMenu.Add(new MenuBool("combo.q.aoe.maintargetonly", ">> only if hitting main target"));

            var comboWMenu = new Aimtec.SDK.Menu.Menu("combo.w.menu", "W Settings");
            comboMenu.Add(comboWMenu);

            comboWMenu.Add(new MenuBool("combo.w", "Use W"));
            comboWMenu.Add(new MenuBool("combo.w.aoe", "AoE W"));
            comboWMenu.Add(new MenuSlider("combo.w.aoe.mintargets", ">> minimum targets to hit", 1, 1, 5));
            comboWMenu.Add(new MenuBool("combo.w.aoe.maintargetonly", ">> only if hitting main target", false));

            var comboEMenu = new Aimtec.SDK.Menu.Menu("combo.e.menu", "E Settings");
            comboMenu.Add(comboEMenu);

            comboEMenu.Add(new MenuBool("combo.e", "Use E"));
            comboEMenu.Add(new MenuBool("combo.e.resetaaonly", ">> only if resetting AA", false));


            var comboRMenu = new Aimtec.SDK.Menu.Menu("combo.r.menu", "R Settings");
            comboMenu.Add(comboRMenu);

            comboRMenu.Add(new MenuBool("combo.r", "Use R"));
            var comboRWhitelistMenu = new Aimtec.SDK.Menu.Menu("combo.r.whitelist", "Whitelist");
            comboRMenu.Add(comboRWhitelistMenu);
            foreach (var hero in GameObjects.EnemyHeroes)
                comboRWhitelistMenu.Add(new MenuBool(hero.ChampionName, hero.ChampionName));
            comboRMenu.Add(new MenuBool("combo.r.willkill", ">> only if will kill"));
            //comboRMenu.Add(new MenuBool("combo.r.willdietodot", ">> will die to DoT", false));
            comboRMenu.Add(new MenuBool("combo.r.beforedeath", ">> before death", false));




            var harassMenu = new Aimtec.SDK.Menu.Menu("harass", "Harass");
            _menu.Add(harassMenu);
            harassMenu.Add(new MenuSlider("harass.mana", "Minimum mana percent", 40, 0, 100));

            harassMenu.Add(new MenuBool("harass.attackselectedonly", "Only attack selected target"));


            var harassQMenu = new Aimtec.SDK.Menu.Menu("harass.q.menu", "Q Settings");
            harassMenu.Add(harassQMenu);

            harassQMenu.Add(new MenuBool("harass.q", "Use Q"));
            harassQMenu.Add(new MenuBool("harass.q.aoe", "AoE Q"));
            harassQMenu.Add(new MenuSlider("harass.q.aoe.mintargets", ">> minimum targets to hit", 2, 1, 5));
            harassQMenu.Add(new MenuBool("harass.q.aoe.maintargetonly", ">> only if hitting main target"));

            var harassWMenu = new Aimtec.SDK.Menu.Menu("harass.w.menu", "W Settings");
            harassMenu.Add(harassWMenu);

            harassWMenu.Add(new MenuBool("harass.w", "Use W"));
            harassWMenu.Add(new MenuBool("harass.w.aoe", "AoE W"));
            harassWMenu.Add(new MenuSlider("harass.w.aoe.mintargets", ">> minimum targets to hit", 1, 1, 5));
            harassWMenu.Add(new MenuBool("harass.w.aoe.maintargetonly", ">> only if hitting main target", false));

            var harassEMenu = new Aimtec.SDK.Menu.Menu("harass.e.menu", "E Settings");
            harassMenu.Add(harassEMenu);

            harassEMenu.Add(new MenuBool("harass.e", "Use E"));
            harassEMenu.Add(new MenuBool("harass.e.resetaaonly", ">> only if resetting AA", false));


            var harassRMenu = new Aimtec.SDK.Menu.Menu("harass.r.menu", "R Settings");
            harassMenu.Add(harassRMenu);

            harassRMenu.Add(new MenuBool("harass.r", "Use R"));
            var harassRWhitelistMenu = new Aimtec.SDK.Menu.Menu("harass.r.whitelist", "Whitelist");
            harassRMenu.Add(harassRWhitelistMenu);
            foreach (var hero in GameObjects.EnemyHeroes)
                harassRWhitelistMenu.Add(new MenuBool(hero.ChampionName, hero.ChampionName));
            harassRMenu.Add(new MenuBool("harass.r.willkill", ">> only if will kill"));
            harassRMenu.Add(new MenuBool("harass.r.willdietodot", ">> will die to DoT", false));
            harassRMenu.Add(new MenuBool("harass.r.beforedeath", ">> before you die", false));




            var miscMenu = new Aimtec.SDK.Menu.Menu("misc", "Misc.");
            _menu.Add(miscMenu);
            miscMenu.Add(new MenuBool("misc.ks.q", "KS Q"));
            miscMenu.Add(new MenuBool("misc.ks.w", "KS W"));
            miscMenu.Add(new MenuBool("misc.ks.r", "KS R"));
            var miscRWhitelistMenu = new Aimtec.SDK.Menu.Menu("misc.ks.r.whitelist", "Whitelist");
            miscMenu.Add(miscRWhitelistMenu);
            foreach (var hero in GameObjects.EnemyHeroes)
                miscRWhitelistMenu.Add(new MenuBool(hero.ChampionName, hero.ChampionName));
            //var miscGapcloseMenu = new Aimtec.SDK.Menu.Menu("misc.gapclose", "Gapclosers");
            //foreach (var hero in GameObjects.EnemyHeroes.Where(e=>e.SpellBook.Spells.Contains())
            //    miscGapcloseMenu.Add(new MenuBool(hero.ChampionName, hero.ChampionName));


            var laneclearMenu = new Aimtec.SDK.Menu.Menu("laneclear", "Laneclear");
            _menu.Add(laneclearMenu);
            laneclearMenu.Add(new MenuSlider("laneclear.mana", "Minimum mana percent", 40, 0, 100));
            laneclearMenu.Add(new MenuBool("laneclear.q", "Use Q"));
            laneclearMenu.Add(new MenuSlider("laneclear.q.min", ">> min minions", 3, 1, 6));
            laneclearMenu.Add(new MenuBool("laneclear.w", "Use W"));
            laneclearMenu.Add(new MenuSlider("laneclear.w.min", ">> min minions", 3, 1, 6));




            var drawingsMenu = new Aimtec.SDK.Menu.Menu("drawings", "Drawings");
            _menu.Add(drawingsMenu);
            drawingsMenu.Add(new MenuBool("drawings.q", "Draw Q"));
            drawingsMenu.Add(new MenuBool("drawings.w", "Draw W"));
            drawingsMenu.Add(new MenuBool("drawings.r", "Draw R"));

            _menu.Attach();
        }
    }
}
