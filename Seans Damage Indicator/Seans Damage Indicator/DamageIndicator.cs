using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec;
using Aimtec.SDK;
using Aimtec.SDK.Damage;
using Aimtec.SDK.Damage.JSON;
using Aimtec.SDK.Events;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Util.Cache;

namespace Seans_Damage_Indicator
{
    internal class DamageIndicator
    {
        internal static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        internal static int Height => 10;
        internal static int Width => 103;

        internal static int XOffset => 10;
        internal static int YOffset => 13;

        public static void OnPresent()
        {
            if (!Menu._menu["enabled"].As<MenuBool>().Enabled)
                return;

            foreach (var enemy in GameObjects.EnemyHeroes.Where(e => e.IsValid && e.IsFloatingHealthBarActive && e.IsVisible))
            {
                var barPos = enemy.FloatingHealthBarPosition;

                if (Math.Abs(barPos.X) < 0.0000001) continue;
                if (Math.Abs(barPos.Y) < 0.0000001) continue;

                double qDamage = 0;
                double wDamage = 0;
                double eDamage = 0;
                double rDamage = 0;
                double aaDamage = 0;
                double totalDamage = 0;

                if (Spells.Q.Ready)
                    qDamage = Player.GetSpellDamage(enemy, SpellSlot.Q);

                if (Spells.W.Ready)
                    wDamage = Player.GetSpellDamage(enemy, SpellSlot.W);

                if (Spells.E.Ready)
                    eDamage = Player.GetSpellDamage(enemy, SpellSlot.E);
                
                if (Spells.R.Ready && !Spells.PetSpells.Contains(Player.GetSpell(SpellSlot.R).SpellData.Name))
                    rDamage = Player.GetSpellDamage(enemy, SpellSlot.R);

                aaDamage = Player.GetAutoAttackDamage(enemy) * (Menu._menu["autoattackcount"].As<MenuSliderBool>().Enabled ? Menu._menu["autoattackcount"].Value : 0);

                totalDamage = qDamage + wDamage + eDamage + rDamage + aaDamage;
            
                var percentHealthAfterDamage = Math.Max(0, enemy.Health - totalDamage) / enemy.MaxHealth;
                var posY = barPos.Y + YOffset;
                var posDamageX = barPos.X + YOffset + Width * percentHealthAfterDamage;
                var posCurrentHealthX = barPos.X + YOffset + Width * enemy.Health / enemy.MaxHealth;
                var difference = posCurrentHealthX - posDamageX;
                var xPos = barPos.X + XOffset + (Width * percentHealthAfterDamage);

                Render.Rectangle(new Vector2((float) xPos, posY), (float)difference, Height, Color.FromArgb(100, 255, 255, 255));

            }
        }


    }
}
