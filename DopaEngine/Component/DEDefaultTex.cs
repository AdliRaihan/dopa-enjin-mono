using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaEngine
{
    public class DEDefaultTex
    {
        public static DEDefaultTex Tex;
        public static DEDefaultTex Get()
        {
            if (Tex == null)
                Tex = new DEDefaultTex();
            return Tex;
        }

        public Texture2D blackTexture;
        public Texture2D whiteTexture;
        public Texture2D clearTexture;

        public DEDefaultTex()
        {
            blackTexture = new Texture2D(DE.Get().GraphicsDevice, 1, 1);
            blackTexture.SetData(new[] { Color.Black });
            whiteTexture = new Texture2D(DE.Get().GraphicsDevice, 1, 1);
            whiteTexture.SetData(new[] { Color.White });
            clearTexture = new Texture2D(DE.Get().GraphicsDevice, 1, 1);
        }
    }
}
