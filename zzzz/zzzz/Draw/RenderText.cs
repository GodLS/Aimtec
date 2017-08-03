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
    class RenderText : RenderObject
    {
        public Vector2 renderPosition = new Vector2(0, 0);
        public string text = "";

        public Color color = Color.White;

        public RenderText(string text, Vector2 renderPosition, float renderTime)
        {
            this.startTime = EvadeUtils.TickCount;
            this.endTime = this.startTime + renderTime;
            this.renderPosition = renderPosition;

            this.text = text;
        }

        public RenderText(string text, Vector2 renderPosition, float renderTime,
            Color color)
        {
            this.startTime = EvadeUtils.TickCount;
            this.endTime = this.startTime + renderTime;
            this.renderPosition = renderPosition;

            this.color = color;

            this.text = text;
        }

        override public void Draw()
        {
            if (!renderPosition.IsZero)
            {
                var textDimension = 10; // LUL Drawing.GetTextExtent
                Vector2 wardScreenPos;
                Aimtec.Render.WorldToScreen(renderPosition.To3D(), out wardScreenPos);

                Render.Text(wardScreenPos.X - textDimension / 2, wardScreenPos.Y, color, text);
            }
        }
    }
}
