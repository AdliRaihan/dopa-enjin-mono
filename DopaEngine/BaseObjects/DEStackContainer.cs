using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaEngine
{
    public enum DEStackContainerAxis { vertical, horizontal }
    public class DEStackContainer : DEUIObject
    {
        public DEStackContainer(string assetsName, Vector2 StartingPosition, Point Size) : base(assetsName, StartingPosition, Size) 
        { InitialSetup(); }
        public DEStackContainer(string assetsName, float x, float y, int w, int h) : base(assetsName, x, y, w, h) 
        { InitialSetup(); }
        public DEStackContainer(Texture2D Texture, float x, float y, int w, int h) : base(Texture, x, y, w, h) 
        { InitialSetup(); }

        public List<DEUIObject> Views = new();
        public float padding = 5.0f;
        public float spacing = 5.0f;
        public int fixedSizeWidth = 0;
        public int fixedSizeHeight = 0;
        public DEStackContainerAxis axis = DEStackContainerAxis.horizontal;
        private void InitialSetup()
        {
        }
        public void AddList(List<DEUIObject> list) => list.ForEach((item) => Views.Add(item));
        public override void OnRender(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            base.OnRender(gameTime, SpriteBatch);
            if (isHidden) return;
            Views.ForEach((item) => item.OnRender(gameTime, SpriteBatch));
        }
        public override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            float baseX = 0;
            float baseY = 0;
            int maximumHeight = 0;
            int maximumWidth = 0;
            Views.ForEach((item) =>
            {
                item.SetPosition(
                    GetPosition.X + baseX + padding,
                    GetPosition.Y + baseY + padding
                    );

                if (axis == DEStackContainerAxis.horizontal) baseX += item.Transform.Size.X + spacing;
                else baseY += item.Transform.Size.Y + spacing;

                if (baseX > DE.Get().VM._graphics.PreferredBackBufferWidth - (item.Transform.Size.X * 2))
                {
                    baseY += item.Transform.Size.Y + spacing;
                    baseX = 0;
                }

                item.OnUpdate(gameTime);

                if (DE.Get().VM.CurrentActivity.IsObjectHovered(item).Result)
                    DE.Get().VM.CurrentActivity.InvokeMouseActionForObjects(item);

                if (maximumWidth < item.Transform.Size.X)
                    maximumWidth = item.Transform.Size.X;
                if (maximumHeight < item.Transform.Size.Y)
                    maximumHeight = item.Transform.Size.Y;
            });

            if (axis == DEStackContainerAxis.horizontal) SetSize((int)(baseX + padding), (int)maximumHeight + (int)baseY + (int)(padding * 2));
            else SetSize(
                (fixedSizeWidth > 0) ? fixedSizeWidth : maximumWidth + (int)(padding * 2),
                (fixedSizeHeight > 0) ? fixedSizeHeight : (int)(baseY + (padding * 2)));

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                setXPosition += 5;
            if (Keyboard.GetState().IsKeyDown(Keys.E))
                setXPosition -= 5;
        }
    }
}
