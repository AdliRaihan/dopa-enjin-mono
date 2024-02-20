using DopaEngine;
using DopaEngine.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaExample
{
    internal class ExampleScreen2 : DEWorld
    {
        private DEUIObject StartGame;
        public override void OnLoadContent()
        {
            base.OnLoadContent();
            UseCamera();
            StartGame = new DEUIObject("GoldCoin", 32, 32, 16, 32);
            StartGame.isObjectUI = false;
            StartGame.SetTextureColor(Color.Red);
            StartGame.OnLeftClickAction = (() =>
            {
                ChangeActivity(new ExampleScreen3_TileMap());
            });
            Debug.WriteLine("Example Screen 2 Initiated!");
            InterfaceViews.AddMultiple(new() { StartGame });
        }
        public override void OnRender(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            base.OnRender(gameTime, SpriteBatch);
        }

        public override void OnRenderUI(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            base.OnRenderUI(gameTime, SpriteBatch);
        }
        public override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }
    }
}
