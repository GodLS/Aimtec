using System;
using Aimtec.SDK.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aimtec;
using Aimtec.SDK.Util.Cache;
using Aimtec.SDK;
using Aimtec.SDK.Events;
using Aimtec.SDK.Menu.Components;

//using SharpDX;

namespace zzzz
{
    public static class Situation
    {
        private static Obj_AI_Hero myHero => ObjectManager.GetLocalPlayer();

        static Situation()
        {

        }

        public static bool CheckTeam(this Obj_AI_Base unit)
        {
            return unit.Team != myHero.Team || Evade.devModeOn;
        }

        public static bool CheckTeam(this GameObject unit)
        {
            return unit.Team != myHero.Team || Evade.devModeOn;
        }

        //public static bool CheckTeam(this Obj_GeneralParticleEmitter emitter)
        //{
        //    return emitter.Name.ToLower().Contains("red") || 
        //          (emitter.Name.ToLower().Contains("green") || emitter.Name.ToLower().Contains("ally")) && Evade.devModeOn ||
        //          !emitter.Name.ToLower().Contains("green") && !emitter.Name.ToLower().Contains("ally");
        //}

        public static string EmitterColor()
        {
            return Evade.devModeOn ? "green" : "red";
        }

        public static string EmitterTeam()
        {
            return Evade.devModeOn ? "ally" : "enemy";
        }

        public static bool isNearEnemy(this Vector2 pos, float distance, bool alreadyNear = true)
        {
            if (Evade.miscMenu["PreventDodgingNearEnemy"].As<MenuBool>().Enabled)
            {
                var curDistToEnemies = ObjectCache.myHeroCache.serverPos2D.GetDistanceToChampions();
                var posDistToEnemies = pos.GetDistanceToChampions();

                if (curDistToEnemies < distance)
                {
                    if (curDistToEnemies > posDistToEnemies)
                    {
                        return true;
                    }
                }
                else
                {
                    if (posDistToEnemies < distance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsUnderTurret(this Vector2 pos, bool checkEnemy = true)
        {
            if (!Evade.miscMenu["PreventDodgingUnderTower"].As<MenuBool>().Enabled)
            {
                return false;
            }

            var turretRange = 875 + ObjectCache.myHeroCache.boundingRadius;

            foreach (var entry in ObjectCache.turrets)
            {
                var turret = entry.Value;
                if (turret == null || !turret.IsValid || turret.IsDead)
                {
                    DelayAction.Add(1, () => ObjectCache.turrets.Remove(entry.Key));
                    continue;
                }

                if (checkEnemy && turret.IsAlly)
                {
                    continue;
                }

                var distToTurret = pos.Distance(turret.Position.To2D());
                if (distToTurret <= turretRange)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ShouldDodge()
        {
            // fix
            if (Evade.keyMenu["DontDodgeKeyEnabled"].As<MenuBool>().Enabled &&
                Evade.keyMenu["DontDodgeKey"].As<MenuKeyBind>().Enabled)
            {
                return false;
            }

            if (Evade.mainMenu["DodgeSkillShots"].As<MenuKeyBind>().Enabled == false
                || CommonChecks()
                )
            {
                //has spellshield - sivir, noc, morgana
                //vlad pool
                //tryndamere r?
                //kayle ult buff?
                //hourglass
                //invulnerable
                //rooted
                //sion ult -> tenacity = 100?
                //stunned
                //elise e
                //zilean ulted
                //isdashing

                return false;
            }

            return true;
        }

        public static bool ShouldUseEvadeSpell()
        {
            // fix
            if (Evade.keyMenu["DontDodgeKeyEnabled"].As<MenuBool>().Enabled &&
                Evade.keyMenu["DontDodgeKey"].As<MenuKeyBind>().Enabled)
            {
                return false;
            }

            if (Evade.mainMenu["ActivateEvadeSpells"].As<MenuKeyBind>().Enabled == false
                || CommonChecks()
                || Evade.lastWindupTime - EvadeUtils.TickCount > 0)
            {
                return false;
            }

            return true;
        }

        public static bool CommonChecks()
        {
            // fix
            return

                Evade.isChanneling
                || (Evade.keyMenu["DodgeOnlyOnComboKeyEnabled"].As<MenuBool>().Value == true &&
                    Evade.keyMenu["DodgeComboKey"].As<MenuKeyBind>().Enabled == false)
                || myHero.IsDead
                || myHero.IsInvulnerable
                || myHero.IsTargetable == false
                || HasSpellShield(myHero)
                || ChampionSpecificChecks()
                || myHero.IsDashing()
                || Evade.hasGameEnded == true;
        }

        public static bool ChampionSpecificChecks()
        {
            return (myHero.ChampionName == "Sion" && myHero.HasBuff("SionR"))
                ;

            //Untargetable
            //|| (myHero.ChampionName == "KogMaw" && myHero.HasBuff("kogmawicathiansurprise"))
            //|| (myHero.ChampionName == "Karthus" && myHero.HasBuff("KarthusDeathDefiedBuff"))

            //Invulnerable
            //|| myHero.HasBuff("kalistarallyspelllock"); 
        }

        //from Evade by Esk0r
        public static bool HasSpellShield(Obj_AI_Hero unit)
        {
            if (ObjectManager.GetLocalPlayer().HasBuffOfType(BuffType.SpellShield))
            {
                return true;
            }

            if (ObjectManager.GetLocalPlayer().HasBuffOfType(BuffType.SpellImmunity))
            {
                return true;
            }

            //Sivir E
            if (unit.LastCastedSpellName() == "SivirE" && (EvadeUtils.TickCount - Evade.lastSpellCastTime) < 300)
            {
                return true;
            }

            //Morganas E
            if (unit.LastCastedSpellName() == "BlackShield" && (EvadeUtils.TickCount - Evade.lastSpellCastTime) < 300)
            {
                return true;
            }

            //Nocturnes E
            if (unit.LastCastedSpellName() == "NocturneShit" && (EvadeUtils.TickCount - Evade.lastSpellCastTime) < 300)
            {
                return true;
            }

            return false;
        }

    }
}
