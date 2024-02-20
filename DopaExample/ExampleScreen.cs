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
    internal class ExampleScreen : DEWorld
    {
        private DEUIObject StartGame;
        public override void OnLoadContent()
        {
            base.OnLoadContent();
            StartGame = new DEUIObject("IslandSheets", Vector2.Zero, new Point(512, 512));
            StartGame.SpriteLocation = new(0, 0, 24, 24);
            StartGame.OnLeftClickAction = (() =>
            {
                //DEInstance.GetVM().SetActivity(new ExampleScreen2());
                ChangeActivity(new ExampleScreen2());
                Debug.WriteLine("Clicked!");
            });
            InterfaceViews.AddMultiple(new() { StartGame });
            Debug.WriteLine("Example Screen Initiated!");
        }
        public override void OnRender(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            base.OnRender(gameTime, SpriteBatch);
        }
        public override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }
    }
}
