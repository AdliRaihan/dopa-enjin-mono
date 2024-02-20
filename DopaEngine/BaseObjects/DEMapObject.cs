using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaEngine
{
    public class DEMapObject : DEBaseObject
    {
        public DEMapObject(string assetsName, Vector2 StartingPosition, Point Size)
        {
            SetTexture(assetsName);
            SetPosition(StartingPosition);
            SetSize(Size);
            isObjectUI = false;
        }
        public DEMapObject(string assetsName, float x, float y, int w, int h)
        {
            SetTexture(assetsName);
            SetPosition(new Vector2(x, y));
            SetSize(new Point(w, h));
            isObjectUI = false;
        }
    }
}
