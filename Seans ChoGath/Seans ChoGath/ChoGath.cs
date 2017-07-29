using System.Linq;
using Aimtec;
using Aimtec.SDK.Damage;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Prediction.Skillshots;
using Aimtec.SDK.Prediction.Skillshots.AoE;
using Aimtec.SDK.TargetSelector;
using Aimtec.SDK.Util.Cache;

namespace Seans_ChoGath
{
    internal class ChoGath
    {
        public static Obj_AI_Hero Player => GameObjects.Player;

        public static void Run()
        {
            if (Player.IsDead)
                return;

            AutoKS();

            switch (Orbwalker.Implementation.Mode)
            {
                case OrbwalkingMode.None:
                    break;
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
                case OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case OrbwalkingMode.Laneclear:
                    Laneclear();
                    break;
                case OrbwalkingMode.Lasthit:
                    break;
                case OrbwalkingMode.Freeze:
                    break;
                case OrbwalkingMode.Custom:
                    break;
                default: break;
            }
        }


        private static double GetQDamage(Obj_AI_Base hero)
        {
            var qDamage = Player.CalculateDamage(hero, DamageType.Magical,
                new double[] { 80, 135, 190, 245, 300 }[Player.SpellBook.GetSpell(SpellSlot.Q).Level - 1] +
                Player.TotalAbilityDamage);

            return qDamage;
        }

        private static double GetWDamage(Obj_AI_Base hero)
        {
            var wDamage = Player.CalculateDamage(hero, DamageType.Magical,
                new double[] { 75, 125, 175, 225, 275 }[Player.SpellBook.GetSpell(SpellSlot.W).Level - 1] +
                Player.TotalAbilityDamage * .7f);

            return wDamage;
        }

        private static double GetRDamage(Obj_AI_Base hero)
        {
            var bonusHealth = Player.MaxHealth - (574.480 + 56.480 * Player.Level);

            var rDamage = Player.CalculateDamage(hero, DamageType.True,
                new double[] { 300, 475, 650 }[Player.SpellBook.GetSpell(SpellSlot.R).Level - 1] +
                Player.TotalAbilityDamage * .5f + .1 * bonusHealth);

            return rDamage;
        }


        private static void AutoKS()
        {
            foreach (var hero in GameObjects.EnemyHeroes.Where(h => h.IsValidTarget(Spells.Q.Range)))
            {
                if (Spells.R.Ready && hero.IsValidTarget(Spells.R.Range) &&
                    Menu._menu["misc"]["misc.ks.r"].As<MenuBool>().Enabled &&
                    Menu._menu["misc"]["misc.ks.r.whitelist"][hero.ChampionName].As<MenuBool>().Enabled)
                {
                    var rDamage = GetRDamage(hero);

                    if (hero.Health <= rDamage)
                        Spells.R.CastOnUnit(hero);
                }

                if (Spells.W.Ready && hero.IsValidTarget(Spells.W.Range) &&
                    Menu._menu["misc"]["misc.ks.w"].As<MenuBool>().Enabled)
                    if (hero.Health <= GetWDamage(hero))
                    {
                        var wPred = FixAOEPred.GetCirclePrediction(Spells.W.GetPredictionInput(hero));

                        if (wPred.HitChance >= HitChance.High)
                            if (wPred.AoeTargetsHit.Contains(hero))
                                Spells.W.Cast(wPred.CastPosition);
                    }

                if (Spells.Q.Ready && hero.IsValidTarget() && Menu._menu["misc"]["misc.ks.q"].As<MenuBool>().Enabled)
                    if (hero.Health <= GetQDamage(hero))
                    {
                        var qPred = FixAOEPred.GetCirclePrediction(Spells.Q.GetPredictionInput(hero));

                        if (qPred.HitChance >= HitChance.High)
                            if (qPred.AoeTargetsHit.Contains(hero))
                                Spells.Q.Cast(qPred.CastPosition);
                    }
            }
        }

        public static void Combo() // Potentially make it so only Q against targets with blinks if theyre silenced
        {
            var selectedTarget = TargetSelector.GetSelectedTarget();

            if (Spells.R.Ready && Menu._menu["combo"]["combo.r.menu"]["combo.r"].As<MenuBool>().Enabled)
            {
                var rTarget = selectedTarget.IsValidTarget(Spells.R.Range)
                    ? selectedTarget
                    : TargetSelector.GetTarget(Spells.R.Range);
                if (Menu._menu["combo"]["combo.attackselectedonly"].As<MenuBool>().Enabled)
                    rTarget = selectedTarget.IsValidTarget()
                        ? selectedTarget
                        : TargetSelector.GetTarget(Spells.R.Range);

                if (rTarget != null)
                {
                    if (!Menu._menu["combo"]["combo.r.menu"]["combo.r.whitelist"][rTarget.ChampionName].As<MenuBool>()
                            .Enabled && selectedTarget != rTarget)
                        return;

                    if (Menu._menu["combo"]["combo.r.menu"]["combo.r.willkill"].As<MenuBool>().Enabled)
                        if (rTarget.Health <= GetRDamage(rTarget))
                        {
                            Spells.R.Cast(rTarget);
                            return;
                        }

                    if (Player.HealthPercent() <= 10 && Player.CountEnemyHeroesInRange(1000f) >= 1)
                    {
                        Spells.R.Cast(rTarget);
                        return;
                    }
                }
            }

            if (Spells.E.Ready && Menu._menu["combo"]["combo.e.menu"]["combo.e"].As<MenuBool>().Enabled)
            {
                var eTarget = selectedTarget.IsValidTarget(Spells.E.Range)
                    ? selectedTarget
                    : TargetSelector.GetTarget(Spells.E.Range);
                if (Menu._menu["combo"]["combo.attackselectedonly"].As<MenuBool>().Enabled)
                    eTarget = selectedTarget.IsValidTarget()
                        ? selectedTarget
                        : TargetSelector.GetTarget(Spells.E.Range);
                if (eTarget != null)
                    if (!Menu._menu["combo"]["combo.e.menu"]["combo.e.resetaaonly"].As<MenuBool>().Enabled)
                    {
                        Spells.E.Cast();
                        return;
                    }
            }


            if (Spells.W.Ready && Menu._menu["combo"]["combo.w.menu"]["combo.w"].As<MenuBool>().Enabled)
            {
                var wTarget = selectedTarget.IsValidTarget(Spells.W.Range)
                    ? selectedTarget
                    : TargetSelector.GetTarget(Spells.W.Range);
                if (Menu._menu["combo"]["combo.attackselectedonly"].As<MenuBool>().Enabled)
                    wTarget = selectedTarget.IsValidTarget()
                        ? selectedTarget
                        : TargetSelector.GetTarget(Spells.W.Range);
                if (wTarget != null)
                    if (Menu._menu["combo"]["combo.w.menu"]["combo.w.aoe"].As<MenuBool>().Enabled)
                    {
                        foreach (var hero in GameObjects.EnemyHeroes.Where(
                            h => h != null && h.IsValidTarget(Spells.W.Range)))
                        {
                            var wAoePrediction =
                                AoePrediction.GetAoEPrediction(Spells.W
                                    .GetPredictionInput(
                                        hero));

                            // Cone AoE pred is so buggin and Im too retarded to fix, so we use this. LUL
                            if (Player.CountEnemyHeroesInRange(Spells.W.Range) == 1)
                                wAoePrediction = FixAOEPred.GetConePrediction(Spells.W.GetPredictionInput(hero));


                            if (wAoePrediction.AoeHitCount >=
                                Menu._menu["combo"]["combo.w.menu"]["combo.w.aoe.mintargets"].As<MenuSlider>().Value)
                            {
                                if (!Menu._menu["combo"]["combo.w.menu"]["combo.w.aoe.maintargetonly"].As<MenuBool>()
                                    .Enabled)
                                {
                                    // if W doesnt need to only hit main target
                                    Spells.W.Cast(wAoePrediction.CastPosition);
                                    return;
                                }

                                // if W must hit main target
                                if (wAoePrediction.AoeTargetsHit.Contains(wTarget))
                                {
                                    Spells.W.Cast(wAoePrediction.CastPosition);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        Spells.W.Cast(wTarget);
                        return;
                    }
            }

            if (Spells.Q.Ready && Menu._menu["combo"]["combo.q.menu"]["combo.q"].As<MenuBool>().Enabled)
            {
                var qTarget = selectedTarget.IsValidTarget(Spells.Q.Range)
                    ? selectedTarget
                    : TargetSelector.GetTarget(Spells.Q.Range + Spells.Q.Width / 2);
                if (Menu._menu["combo"]["combo.attackselectedonly"].As<MenuBool>().Enabled)
                    qTarget = selectedTarget.IsValidTarget()
                        ? selectedTarget
                        : TargetSelector.GetTarget(Spells.Q.Range);

                if (qTarget != null)
                    if (Menu._menu["combo"]["combo.q.menu"]["combo.q.aoe"].As<MenuBool>().Enabled)
                        foreach (var hero in GameObjects.EnemyHeroes.Where(
                            h => h.IsValidTarget(Spells.Q.Range)))
                        {
                            var qAoePrediction =
                                FixAOEPred.GetCirclePrediction(Spells.Q.GetPredictionInput(hero));

                            if (qAoePrediction.AoeHitCount >=
                                Menu._menu["combo"]["combo.q.menu"]["combo.q.aoe.mintargets"].As<MenuSlider>().Value)
                            {
                                if (!Menu._menu["combo"]["combo.q.menu"]["combo.q.aoe.maintargetonly"].As<MenuBool>()
                                    .Enabled)
                                {
                                    // if Q doesnt need to only hit main target
                                    Spells.Q.Cast(qAoePrediction.CastPosition);
                                    return;
                                }

                                // if Q must hit main target
                                if (qAoePrediction.AoeTargetsHit.Contains(qTarget))
                                {
                                    Spells.Q.Cast(qAoePrediction.CastPosition);
                                    return;
                                }
                            }
                        }
                    else
                        Spells.Q.Cast(qTarget);
            }
        }

        public static void Harass()
        {
            var selectedTarget = TargetSelector.GetSelectedTarget();

            if (Spells.R.Ready && Menu._menu["harass"]["harass.r.menu"]["harass.r"].As<MenuBool>().Enabled)
            {
                var rTarget = selectedTarget.IsValidTarget(Spells.R.Range)
                    ? selectedTarget
                    : TargetSelector.GetTarget(Spells.R.Range);
                if (Menu._menu["harass"]["harass.attackselectedonly"].As<MenuBool>().Enabled)
                    rTarget = selectedTarget.IsValidTarget()
                        ? selectedTarget
                        : TargetSelector.GetTarget(Spells.R.Range);

                if (rTarget != null)
                {
                    if (!Menu._menu["harass"]["harass.r.menu"]["harass.r.whitelist"][rTarget.ChampionName]
                            .As<MenuBool>()
                            .Enabled && selectedTarget != rTarget)
                        return;

                    if (Menu._menu["harass"]["harass.r.menu"]["harass.r.willkill"].As<MenuBool>().Enabled)
                        if (rTarget.Health <= GetRDamage(rTarget))
                        {
                            Spells.R.Cast(rTarget);
                            return;
                        }

                    if (Player.HealthPercent() <= 10 && Player.CountEnemyHeroesInRange(1000f) >= 1)
                    {
                        Spells.R.Cast(rTarget);
                        return;
                    }
                }
            }

            if (Spells.E.Ready && Menu._menu["harass"]["harass.e.menu"]["harass.e"].As<MenuBool>().Enabled)
            {
                var eTarget = selectedTarget.IsValidTarget(Spells.E.Range)
                    ? selectedTarget
                    : TargetSelector.GetTarget(Spells.E.Range);
                if (Menu._menu["harass"]["harass.attackselectedonly"].As<MenuBool>().Enabled)
                    eTarget = selectedTarget.IsValidTarget()
                        ? selectedTarget
                        : TargetSelector.GetTarget(Spells.E.Range);
                if (eTarget != null)
                    if (!Menu._menu["harass"]["harass.e.menu"]["harass.e.resetaaonly"].As<MenuBool>().Enabled)
                    {
                        Spells.E.Cast();
                        return;
                    }
            }


            if (Spells.W.Ready && Menu._menu["harass"]["harass.w.menu"]["harass.w"].As<MenuBool>().Enabled)
            {
                var wTarget = selectedTarget.IsValidTarget(Spells.W.Range)
                    ? selectedTarget
                    : TargetSelector.GetTarget(Spells.W.Range);
                if (Menu._menu["harass"]["harass.attackselectedonly"].As<MenuBool>().Enabled)
                    wTarget = selectedTarget.IsValidTarget()
                        ? selectedTarget
                        : TargetSelector.GetTarget(Spells.W.Range);
                if (wTarget != null)
                    if (Menu._menu["harass"]["harass.w.menu"]["harass.w.aoe"].As<MenuBool>().Enabled)
                    {
                        foreach (var hero in GameObjects.EnemyHeroes.Where(
                            h => h != null && h.IsValidTarget(Spells.W.Range)))
                        {
                            var wAoePrediction =
                                AoePrediction.GetAoEPrediction(Spells.W
                                    .GetPredictionInput(
                                        hero));

                            // Cone AoE pred is so buggin and Im too retarded to fix, so we use this. LUL
                            if (Player.CountEnemyHeroesInRange(Spells.W.Range) == 1)
                                wAoePrediction = FixAOEPred.GetConePrediction(Spells.W.GetPredictionInput(hero));


                            if (wAoePrediction.AoeHitCount >=
                                Menu._menu["harass"]["harass.w.menu"]["harass.w.aoe.mintargets"].As<MenuSlider>().Value)
                            {
                                if (!Menu._menu["harass"]["harass.w.menu"]["harass.w.aoe.maintargetonly"].As<MenuBool>()
                                    .Enabled)
                                {
                                    // if W doesnt need to only hit main target
                                    Spells.W.Cast(wAoePrediction.CastPosition);
                                    return;
                                }

                                // if W must hit main target
                                if (wAoePrediction.AoeTargetsHit.Contains(wTarget))
                                {
                                    Spells.W.Cast(wAoePrediction.CastPosition);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        Spells.W.Cast(wTarget);
                        return;
                    }
            }

            if (Spells.Q.Ready && Menu._menu["harass"]["harass.q.menu"]["harass.q"].As<MenuBool>().Enabled)
            {
                var qTarget = selectedTarget.IsValidTarget(Spells.Q.Range)
                    ? selectedTarget
                    : TargetSelector.GetTarget(Spells.Q.Range + Spells.Q.Width / 2);
                if (Menu._menu["harass"]["harass.attackselectedonly"].As<MenuBool>().Enabled)
                    qTarget = selectedTarget.IsValidTarget()
                        ? selectedTarget
                        : TargetSelector.GetTarget(Spells.Q.Range);

                if (qTarget != null)
                    if (Menu._menu["harass"]["harass.q.menu"]["harass.q.aoe"].As<MenuBool>().Enabled)
                    {
                        foreach (var hero in GameObjects.EnemyHeroes.Where(
                            h => h.IsValidTarget(Spells.Q.Range)))
                        {
                            var qAoePrediction =
                                FixAOEPred.GetCirclePrediction(Spells.Q.GetPredictionInput(hero));

                            if (qAoePrediction.AoeHitCount >=
                                Menu._menu["harass"]["harass.q.menu"]["harass.q.aoe.mintargets"].As<MenuSlider>().Value)
                            {
                                if (!Menu._menu["harass"]["harass.q.menu"]["harass.q.aoe.maintargetonly"].As<MenuBool>()
                                    .Enabled)
                                {
                                    // if Q doesnt need to only hit main target
                                    Spells.Q.Cast(qAoePrediction.CastPosition);
                                    return;
                                }

                                // if Q must hit main target
                                if (qAoePrediction.AoeTargetsHit.Contains(qTarget))
                                {
                                    Spells.Q.Cast(qAoePrediction.CastPosition);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        Spells.Q.Cast(qTarget);
                    }
            }
        }

        private static void Laneclear()
        {
            if (Player.ManaPercent() < Menu._menu["laneclear"]["laneclear.mana"].Value)
                return;

            if (Spells.Q.Ready && Menu._menu["laneclear"]["laneclear.q"].As<MenuBool>().Enabled)
                foreach (var minion in GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Spells.Q.Range)))
                {
                    var m = GameObjects.EnemyMinions
                        .Where(x => x.IsValidTarget(Spells.Q.Width, false, true, minion.ServerPosition)).ToList();

                    if (m.Count >= Menu._menu["laneclear"]["laneclear.q.min"].Value)
                    {
                        Spells.Q.Cast(minion);
                        return;
                    }
                }

            if (Spells.W.Ready && Menu._menu["laneclear"]["laneclear.w"].As<MenuBool>().Enabled)
                foreach (var minion in GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Spells.W.Range)))
                {
                    var m = GameObjects.EnemyMinions
                        .Where(x => x.IsValidTarget(Spells.W.Width, false, true, minion.ServerPosition)).ToList();

                    if (m.Count >= Menu._menu["laneclear"]["laneclear.w.min"].Value)
                    {
                        Spells.W.Cast(minion);
                        return;
                    }
                }
        }


        public static void PostAttack(object sender, PostAttackEventArgs e)
        {
            if (sender == null || e.Target == null || !e.Target.IsValidTarget())
                return;

            switch (Orbwalker.Implementation.Mode)
            {
                case OrbwalkingMode.Combo:
                    if (Spells.E.Ready && Menu._menu["combo"]["combo.e.menu"]["combo.e"].As<MenuBool>().Enabled)
                        Spells.E.Cast();
                    break;
                case OrbwalkingMode.Mixed:
                    if (Spells.E.Ready && Menu._menu["harass"]["harass.e.menu"]["harass.e"].As<MenuBool>().Enabled)
                        Spells.E.Cast();
                    break;
                default: break;
            }
        }

        public static void OnPresent()
        {
            if (Menu._menu["drawings"]["drawings.q"].As<MenuBool>().Enabled)
            {
                var color = Spells.Q.Ready ? System.Drawing.Color.White : System.Drawing.Color.Gray;
                Render.Circle(Player.Position, Spells.Q.Range, 50, color);
            }
            if (Menu._menu["drawings"]["drawings.w"].As<MenuBool>().Enabled)
            {
                var color = Spells.W.Ready ? System.Drawing.Color.White : System.Drawing.Color.Gray;
                Render.Circle(Player.Position, Spells.W.Range, 50, color);
            }
            if (Menu._menu["drawings"]["drawings.r"].As<MenuBool>().Enabled)
            {
                var color = Spells.R.Ready ? System.Drawing.Color.White : System.Drawing.Color.Gray;
                Render.Circle(Player.Position, Spells.R.Range, 50, color);
            }
        }

        public static void OnLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs e)
        {
            // Eventually change this to an event when your max hp changes. Cant use onAddbuff because it only adds the first stack. Can compare the count manually and do it that way, but this is fine
            if (sender.IsMe)
                Spells.E.Range = Player.AttackRange + 50f;
        }
    }
}