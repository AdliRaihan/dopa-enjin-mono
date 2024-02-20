using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaEngine
{
    public class DEUnitObject : DEBaseObject
    {
        public DEUnitObject(string assetsName, Vector2 StartingPosition, Point Size)
        {
            SetTexture(assetsName);
            SetPosition(StartingPosition);
            SetSize(Size);
            ListenToMouseHover = ListenToMouseInput = true;
            isObjectUI = false;
        }
        public DEUnitObject(string assetsName, float x, float y, int w, int h)
        {
            SetTexture(assetsName);
            SetPosition(new Vector2(x, y));
            SetSize(new Point(w, h));
            ListenToMouseHover = ListenToMouseInput = true;
            isObjectUI = false;
        }
        public DEUnitObject(Texture2D Texture, float x, float y, int w, int h)
        {
            SetTexture(Texture);
            SetPosition(new Vector2(x, y));
            SetSize(new Point(w, h));
            ListenToMouseHover = ListenToMouseInput = true;
            isObjectUI = false;
        }
    }
}
