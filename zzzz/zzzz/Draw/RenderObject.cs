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

//using SharpDX;

namespace zzzz.Draw
{
    abstract class RenderObject
    {
        public float endTime = 0;
        public float startTime = 0;

        abstract public void Draw();
    }

    class RenderObjects
    {
        private static List<RenderObject> objects = new List<RenderObject>();

        static RenderObjects()
        {
            Aimtec.Render.OnPresent += Render_OnPresent;//Render.OnPresent += Render_OnPresent;
        }

        private static void Render_OnPresent()
        {
            Render();
        }

        private static void Render()
        {
            foreach (RenderObject obj in objects)
            {
                if (obj.endTime - EvadeUtils.TickCount > 0)
                {
                    obj.Draw(); //weird after draw
                }
                else
                {
                    DelayAction.Add(1, () => objects.Remove(obj));
                }
            }
        }

        public static void Add(RenderObject obj)
        {
            objects.Add(obj);
        }
    }
}
