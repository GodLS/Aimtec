using System;
using Aimtec.SDK.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Color = System.Drawing.Color;

using Aimtec;
using Aimtec.SDK.Util.Cache;
using Aimtec.SDK;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Prediction.Skillshots.AoE;

//using SharpDX;

namespace zzzz
{
    internal class SpellDrawer
    {
        public static Menu menu;

        private static Obj_AI_Hero myHero { get { return ObjectManager.GetLocalPlayer(); } }


        public SpellDrawer(Menu mainMenu)
        {
            Render.OnPresent += Render_OnPresent;

            menu = mainMenu;
            Game_OnGameLoad();
        }

        private void Game_OnGameLoad()
        {
            //Console.WriteLine("SpellDrawer loaded");

            Evade.drawMenu = new Menu("Draw", "Draw");
            Evade.drawMenu.Add(new MenuBool("DrawSkillShots", "Draw SkillShots"));
            Evade.drawMenu.Add(new MenuBool("ShowStatus", "Show Evade Status"));
            Evade.drawMenu.Add(new MenuBool("DrawSpellPos", "Draw Spell Position"));
            Evade.drawMenu.Add(new MenuBool("DrawEvadePosition", "Draw Evade Position"));

            Menu dangerMenu = new Menu("DangerLevelDrawings", "Danger Level Drawings");
            dangerMenu.Add(new MenuSlider("LowWidth", "Line Width", 3, 1, 15));
            //lowDangerMenu.Add(new MenuComponent("LowColor", "Color").SetValue(new Circle(true, Color.FromArgb(60, 255, 255, 255))));

            dangerMenu.Add(new MenuSlider("NormalWidth", "Line Width", 3, 1, 15));
            //normalDangerMenu.Add(new MenuComponent("NormalColor", "Color").SetValue(new Circle(true, Color.FromArgb(140, 255, 255, 255))));

            dangerMenu.Add(new MenuSlider("HighWidth", "Line Width", 3, 1, 15));
            //highDangerMenu.Add(new MenuComponent("HighColor", "Color").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));

            dangerMenu.Add(new MenuSlider("ExtremeWidth", "Line Width", 4, 1, 15));
            //extremeDangerMenu.Add(new MenuComponent("ExtremeColor", "Color").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));

            /*
            Menu undodgeableDangerMenu = new Menu("Undodgeable", "Undodgeable");
            undodgeableDangerMenu.Add(new MenuComponent("Width", "Line Width").SetValue(new MenuSlider(6, 1, 15)));
            undodgeableDangerMenu.Add(new MenuComponent("Color", "Color").SetValue(new Circle(true, Color.FromArgb(255, 255, 0, 0))));*/

            Evade.drawMenu.Add(dangerMenu);

            menu.Add(Evade.drawMenu);
        }

        private void DrawLineRectangle(Vector2 start, Vector2 end, int radius, int width, Color color)
        {
            //var dir = (end - start).Normalized();
            //var pDir = dir.Perpendicular();

            //var rightStartPos = start + pDir * radius;
            //var leftStartPos = start - pDir * radius;
            //var rightEndPos = end + pDir * radius;
            //var leftEndPos = end - pDir * radius;

            //Vector2 rStartPos;
            //Vector2 lStartPos;
            //Vector2 rEndPos;
            //Vector2 lEndPos;

            //Render.WorldToScreen(new Vector3(rightStartPos.X, rightStartPos.Y, myHero.Position.Z), out rStartPos);
            //Render.WorldToScreen(new Vector3(leftStartPos.X, leftStartPos.Y, myHero.Position.Z), out lStartPos);
            //Render.WorldToScreen(new Vector3(rightEndPos.X, rightEndPos.Y, myHero.Position.Z), out rEndPos);
            //Render.WorldToScreen(new Vector3(leftEndPos.X, leftEndPos.Y, myHero.Position.Z), out lEndPos);

            //Render.Line(rStartPos, rEndPos, color);
            //Render.Line(lStartPos, lEndPos, color);
            //Render.Line(rStartPos, lStartPos, color);
            //Render.Line(lEndPos, rEndPos, color);
            var rectangle = new Geometry.Rectangle(start.To3D(), end.To3D(), radius);
            rectangle.ToPolygon().Draw(Color.White);
        }

        private void DrawLineTriangle(Vector2 start, Vector2 end, int radius, int width, Color color)
        {
            var dir = (end - start).Normalized();
            var pDir = dir.Perpendicular();

            var initStartPos = start + dir;
            var rightEndPos = end + pDir * radius;
            var leftEndPos = end - pDir * radius;

            Vector2 iStartPos;
            Render.WorldToScreen(new Vector3(initStartPos.X, initStartPos.Y, myHero.Position.Z), out iStartPos);
            Vector2 rEndPos;
           Render.WorldToScreen(new Vector3(rightEndPos.X, rightEndPos.Y, myHero.Position.Z), out rEndPos);
            Vector2 lEndPos;
            Render.WorldToScreen(new Vector3(leftEndPos.X, leftEndPos.Y, myHero.Position.Z), out lEndPos);

            Render.Line(iStartPos, rEndPos, color);
            Render.Line(iStartPos, lEndPos, color); 
            Render.Line(rEndPos, lEndPos, color);
        }

        private void DrawEvadeStatus()
        {
            // fix
            //if (Evade.menu["ShowStatus"].As<MenuBool>().Enabled)
            {
                Vector2 heroPos;
                Render.WorldToScreen(ObjectManager.GetLocalPlayer().Position, out heroPos);

                if (Evade.mainMenu["DodgeSkillShots"].As<MenuKeyBind>().Enabled)
                {
                    if (Evade.isDodging)
                    {
                        Render.Text(heroPos.X - 10, heroPos.Y, Color.Red, "Evade: ON");
                    }
                    else
                    {
                        if (Evade.keyMenu["DodgeOnlyOnComboKeyEnabled"].As<MenuBool>().Enabled
                         && Evade.keyMenu["DodgeComboKey"].As<MenuKeyBind>().Enabled == false)
                        {
                            Render.Text(heroPos.X - 10, heroPos.Y, Color.Gray, "Evade: OFF");
                        }
                        else
                        {
                            if (Evade.keyMenu["DontDodgeKeyEnabled"].As<MenuBool>().Value == true
                         && Evade.keyMenu["DontDodgeKey"].As<MenuKeyBind>().Enabled == true)
                                Render.Text(heroPos.X - 10, heroPos.Y, Color.Gray, "Evade: OFF");
                            else if (Evade.isDodgeDangerousEnabled())
                                Render.Text(heroPos.X - 10, heroPos.Y, Color.Yellow, "Evade: ON");
                            else
                                Render.Text(heroPos.X - 10, heroPos.Y, Color.Lime, "Evade: ON");
                        }
                    }
                }
                else
                {
                    if (Evade.mainMenu["ActivateEvadeSpells"].As<MenuKeyBind>().Enabled)
                    {
                        if (Evade.keyMenu["DodgeOnlyOnComboKeyEnabled"].As<MenuBool>().Enabled
                         && Evade.keyMenu["DodgeComboKey"].As<MenuKeyBind>().Enabled == false)
                        {
                            Render.Text(heroPos.X - 10, heroPos.Y, Color.Gray, "Evade: OFF");
                        }
                        else
                        {
                            if (Evade.isDodgeDangerousEnabled())
                                Render.Text(heroPos.X - 10, heroPos.Y, Color.Yellow, "Evade: Spell");
                            else
                                Render.Text(heroPos.X - 10, heroPos.Y, Color.DeepSkyBlue, "Evade: Spell");
                        }
                    }
                    else
                    {
                        Render.Text(heroPos.X - 10, heroPos.Y, Color.Gray, "Evade: OFF");
                    }
                }



            }
        }

        private void Render_OnPresent()
        {

            // fix just uncomment it all
            if (Evade.miscMenu["DrawEvadePosition"].As<MenuBool>().Enabled)
            {
                //Render.Circle.DrawCircle(myHero.Position.ExtendDir(dir, 500), 65, Color.Red, 10);

                /*foreach (var point in myHero.Path)
                {
                    Render.Circle.DrawCircle(point, 65, Color.Red, 10);
                }*/

                if (Evade.lastPosInfo != null)
                {
                    var pos = Evade.lastPosInfo.position; //Evade.lastEvadeCommand.targetPosition;
                    Render.Circle(new Vector3(pos.X, pos.Y, myHero.Position.Z), 65, 10, Color.Red);
                }
            }

            DrawEvadeStatus();

            if (Evade.drawMenu["DrawSkillShots"].As<MenuBool>().Value == false)
            {
                return;
            }

            foreach (KeyValuePair<int, Spell> entry in SpellDetector.drawSpells)
            {
                if (entry.Value == null )
                    continue;

                Spell spell = entry.Value;

                var dangerStr = spell.GetSpellDangerString();
                //// var spellDrawingConfig = Evade.menu[dangerStr + "Color"].As<Circle>();
                var spellDrawingWidth = Evade.drawMenu["DangerLevelDrawings"][dangerStr + "Width"].As<MenuSlider>().Value;
                var avoidRadius = Evade.bufferMenu["ExtraAvoidDistance"].As<MenuSlider>().Value;

                if (Evade.spellMenu[spell.info.charName + spell.info.spellName + "Settings"][spell.info.spellName + "DrawSpell"].As<MenuBool>().Enabled
                    /*&& spellDrawingConfig.Active*/)
                {

                    bool canEvade = !(Evade.lastPosInfo != null && Evade.lastPosInfo.undodgeableSpells.Contains(spell.spellID)) || !Evade.devModeOn;

                    if (spell.spellType == SpellType.Line)
                    {
                        Vector2 spellPos = spell.currentSpellPosition;
                        Vector2 spellEndPos = spell.GetSpellEndPosition();

                        DrawLineRectangle(spellPos, spellEndPos, (int)spell.radius,
                            spellDrawingWidth, !canEvade ? Color.Yellow : Color.White);

                        // fix
                        if (Evade.drawMenu["DrawSpellPos"].As<MenuBool>().Enabled)// && spell.spellObject != null)
                        {
                            Render.Circle(new Vector3(spellPos.X, spellPos.Y, spell.height), (int)spell.radius, (uint)spellDrawingWidth, !canEvade ? Color.Yellow : Color.White);
                        }

                    }
                    else if (spell.spellType == SpellType.Circular)
                    {
                        Render.Circle(new Vector3(spell.endPos.X, spell.endPos.Y, spell.height), (int)spell.radius, (uint)spellDrawingWidth, !canEvade ? Color.Yellow : Color.White);

                        if (spell.info.spellName == "VeigarEventHorizon")
                        {
                            Render.Circle(new Vector3(spell.endPos.X, spell.endPos.Y, spell.height), (int)spell.radius - 125, (uint)spellDrawingWidth, !canEvade ? Color.Yellow : Color.White);
                        }
                        else if (spell.info.spellName == "DariusCleave")
                        {
                            Render.Circle(new Vector3(spell.endPos.X, spell.endPos.Y, spell.height), (int)spell.radius - 220, (uint)spellDrawingWidth, !canEvade ? Color.Yellow : Color.White);
                        }
                    }
                    else if (spell.spellType == SpellType.Arc)
                    {
                        /*var spellRange = spell.startPos.Distance(spell.endPos);
                        var midPoint = spell.startPos + spell.Orientation * (spellRange / 2);

                        Render.Circle.DrawCircle(new Vector3(midPoint.X, midPoint.Y, myHero.Position.Z), (int)spell.radius, spellDrawingConfig.Color, spellDrawingWidth);
                        
                        Drawing.DrawLine(Drawing.WorldToScreen(spell.startPos.To3D()),
                                         Drawing.WorldToScreen(spell.endPos.To3D()), 
                                         spellDrawingWidth, spellDrawingConfig.Color);*/
                    }
                    else if (spell.spellType == SpellType.Cone)
                    {
                        DrawLineTriangle(spell.startPos, spell.endPos, (int)spell.radius, spellDrawingWidth, !canEvade ? Color.Yellow : Color.White);
                    }
                }
            }
        }
    }
}
