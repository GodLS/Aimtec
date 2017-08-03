﻿using System;
using Aimtec.SDK.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aimtec;
using Aimtec.SDK.Util.Cache;
using Aimtec.SDK;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;

//using SharpDX;

namespace zzzz
{
    public class HeroInfo
    {
        public Obj_AI_Hero hero;
        public Vector2 serverPos2D;
        public Vector2 serverPos2DExtra;
        public Vector2 serverPos2DPing;
        public Vector2 currentPosition;
        public bool HasPath;
        public float boundingRadius;
        public float moveSpeed;

        public HeroInfo(Obj_AI_Hero hero)
        {
            this.hero = hero;
            Game.OnUpdate += Game_OnGameUpdate;
        }

        private void Game_OnGameUpdate()
        {
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            try
            {
                // fix
                var extraDelayBuffer = Evade.bufferMenu["ExtraPingBuffer"].As<MenuSlider>().Value;
                serverPos2D = hero.ServerPosition.To2D(); //CalculatedPosition.GetPosition(hero, Game.Ping);
                serverPos2DExtra = EvadeUtils.GetGamePosition(hero, Game.Ping + extraDelayBuffer);
                serverPos2DPing = EvadeUtils.GetGamePosition(hero, Game.Ping);
                //CalculatedPosition.GetPosition(hero, Game.Ping + extraDelayBuffer);            
                currentPosition = hero.Position.To2D(); //CalculatedPosition.GetPosition(hero, 0); 
                boundingRadius = hero.BoundingRadius;
                moveSpeed = hero.MoveSpeed;
                HasPath = hero.HasPath;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }

    //public class MenuCache
    //{
    //    public Menu menu;
    //    public Dictionary<string, MenuComponent> cache = new Dictionary<string, MenuComponent>();

    //    public MenuCache(Menu menu)
    //    {
    //        this.menu = menu;

    //        AddMenuToCache(menu);
    //    }

    //    public void AddMenuToCache(Menu newMenu)
    //    {
    //        foreach (var item in ReturnAllItems(newMenu))
    //        {
    //            AddMenuComponentToCache(item);
    //        }
    //    }

    //    public void AddMenuComponentToCache(MenuComponent item)
    //    {
    //        if (item != null && !cache.ContainsKey(item.InternalName))
    //        {
    //            cache.Add(item.InternalName, item);
    //        }
    //    }

    //    //public static List<MenuItem> ReturnAllItems(Menu menu)
    //    //{
    //    //    List<MenuItem> menuList = new List<MenuItem>();

    //    //    menuList.AddRange(menu.Items);

    //    //    foreach (var submenu in menu.Children)
    //    //    {
    //    //        menuList.AddRange(ReturnAllItems(submenu));
    //    //    }

    //    //    return menuList;
    //    //}

    //    public static List<MenuComponent> ReturnAllItems(Menu menu)
    //    {
    //        List<MenuComponent> menuList = new List<MenuComponent>();

    //        menuList.AddRange(menu.OfType<MenuComponent>());

    //        foreach (var submenu in menu.)
    //        {
    //            menuList.AddRange(ReturnAllItems(submenu));
    //        }
    //        //if (menu != null)
    //        //{
    //        //    foreach (MenuComponent item in Evade.menu)
    //        //    {
    //        //        menuList.Add(item);
    //        //    }
    //        //    //foreach (MenuComponent item in menu)
    //        //    //{
    //        //    //    if (item != null)
    //        //    //    {
    //        //    //        menuList.Add(item);
    //        //    //    }
    //        //    //}
    //        //}
    //        //menuList.AddRange(menu.);

    //        //foreach (var submenu in menu.Children)
    //        //{
    //        //    menuList.AddRange(ReturnAllItems(submenu));
    //        //}

    //        return menuList;
    //        // return new List<MenuComponent>(0);
    //    }
    //}

    public static class ObjectCache
    {
        public static Dictionary<int, Obj_AI_Turret> turrets = new Dictionary<int, Obj_AI_Turret>();

        private static Obj_AI_Hero myHero => ObjectManager.GetLocalPlayer();

        public static HeroInfo myHeroCache = new HeroInfo(myHero);
        //public static MenuCache menuCache = new MenuCache(Evade.menu);

        public static float gamePing = 0;

        static ObjectCache()
        {
            InitializeCache();
            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static void Game_OnGameUpdate()
        {
            gamePing = Game.Ping;
        }

        private static void InitializeCache()
        {
            foreach (var obj in ObjectManager.Get<Obj_AI_Turret>())
            {
                if (!turrets.ContainsKey(obj.NetworkId))
                {
                    turrets.Add(obj.NetworkId, obj);
                }
            }
        }
    }
}
