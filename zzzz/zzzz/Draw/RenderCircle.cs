using System;
using Aimtec.SDK.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = System.Drawing.Color;

using Aimtec;
using Aimtec.SDK.Util.Cache;
using Aimtec.SDK;
//using SharpDX;

namespace zzzz.Draw
{
    class RenderCircle : RenderObject
    {
        public Vector2 renderPosition = new Vector2(0, 0);
        public SharpDX.Vector2 renderPositionDX = new SharpDX.Vector2(0, 0);

        public int radius = 65;
        public int width = 5;
        public Color color = Color.White;

        public RenderCircle(Vector2 renderPosition, float renderTime,
            int radius = 65, int width = 5)
        {
            this.startTime = EvadeUtils.TickCount;
            this.endTime = this.startTime + renderTime;
            this.renderPosition = renderPosition;

            this.radius = radius;
            this.width = width;
        }

        public RenderCircle(Vector2 renderPosition, float renderTime,
            Color color, int radius = 65, int width = 5)
        {
            this.startTime = EvadeUtils.TickCount;
            this.endTime = this.startTime + renderTime;
            this.renderPosition = renderPosition;

            this.color = color;

            this.radius = radius;
            this.width = width;
        }

        public RenderCircle(SharpDX.Vector2 renderPosition, float renderTime,
            Color color, int radius = 65, int width = 5)
        {
            this.startTime = EvadeUtils.TickCount;
            this.endTime = this.startTime + renderTime;
            this.renderPositionDX = renderPosition;

            this.color = color;

            this.radius = radius;
            this.width = width;
        }

        override public void Draw()
        {
            if (renderPosition.IsOnScreen())
            {
                Render.Circle(renderPosition.To3D(), radius, 50, color);
            }
        }
    }
}
