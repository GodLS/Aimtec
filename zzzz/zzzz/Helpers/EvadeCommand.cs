using System;
using Aimtec.SDK.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aimtec;
using Aimtec.SDK.Util.Cache;
using Aimtec.SDK;
//using SharpDX;

namespace zzzz
{
    public enum EvadeOrderCommand
    {
        None,
        MoveTo,
        Attack,
        CastSpell
    }

    class EvadeCommand
    {
        private static Obj_AI_Hero myHero { get { return ObjectManager.GetLocalPlayer(); } }

        public EvadeOrderCommand order;
        public Vector2 targetPosition;
        public Obj_AI_Base target;
        public float timestamp;
        public bool isProcessed;
        public EvadeSpellData evadeSpellData;

        public EvadeCommand()
        {
            this.timestamp = EvadeUtils.TickCount;
            this.isProcessed = false;
        }

        public static void MoveTo(Vector2 movePos)
        {
            // fix 
            if (!Situation.ShouldDodge())
            {
                return;
            }

            Evade.lastEvadeCommand = new EvadeCommand
            {
                order = EvadeOrderCommand.MoveTo,
                targetPosition = movePos,
                timestamp = EvadeUtils.TickCount,
                isProcessed = false
            };

            Evade.lastMoveToPosition = movePos;
            Evade.lastMoveToServerPos = myHero.ServerPosition.To2D();

            myHero.IssueOrder(OrderType.MoveTo, movePos.To3D(), false);
        }

        public static void Attack(EvadeSpellData spellData, Obj_AI_Base target)
        {
            EvadeSpell.lastSpellEvadeCommand = new EvadeCommand
            {
                order = EvadeOrderCommand.Attack,
                target = target,
                evadeSpellData = spellData,
                timestamp = EvadeUtils.TickCount,
                isProcessed = false
            };

            myHero.IssueOrder(OrderType.AttackUnit, target);
        }

        public static void CastSpell(EvadeSpellData spellData, Obj_AI_Base target)
        {
            EvadeSpell.lastSpellEvadeCommand = new EvadeCommand
            {
                order = EvadeOrderCommand.CastSpell,
                target = target,
                evadeSpellData = spellData,
                timestamp = EvadeUtils.TickCount,
                isProcessed = false
            };

            myHero.SpellBook.CastSpell(spellData.spellKey, target);
        }

        public static void CastSpell(EvadeSpellData spellData, Vector2 movePos)
        {
            EvadeSpell.lastSpellEvadeCommand = new EvadeCommand
            {
                order = EvadeOrderCommand.CastSpell,
                targetPosition = movePos,
                evadeSpellData = spellData,
                timestamp = EvadeUtils.TickCount,
                isProcessed = false
            };

            myHero.SpellBook.CastSpell(spellData.spellKey, movePos.To3D());
        }

        public static void CastSpell(EvadeSpellData spellData)
        {
            EvadeSpell.lastSpellEvadeCommand = new EvadeCommand
            {
                order = EvadeOrderCommand.CastSpell,
                evadeSpellData = spellData,
                timestamp = EvadeUtils.TickCount,
                isProcessed = false
            };

            myHero.SpellBook.CastSpell(spellData.spellKey);
        }
    }
}
