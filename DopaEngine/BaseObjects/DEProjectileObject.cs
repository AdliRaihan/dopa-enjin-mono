using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaEngine
{
    public enum DEProjectileObjectState { onFire, OutOfBounds, Nothing }
    public class DEProjectileObject : DEBaseObject
    {
        private Vector2 destinationProjectile;
        public List<Vector2> ProjectileLines = new();
        private DEProjectileObjectState state = DEProjectileObjectState.Nothing;
        private Vector2 savedDirection = new();
        public DEProjectileObject(string assetsName, Vector2 StartingPosition, Point Size)
        {
            SetTexture(assetsName);
            SetPosition(StartingPosition);
            SetSize(Size);
            ListenToMouseHover = ListenToMouseInput = true;
            isObjectUI = false;
        }
        public DEProjectileObject(string assetsName, float x, float y, int w, int h)
        {
            SetTexture(assetsName);
            SetPosition(new Vector2(x, y));
            SetSize(new Point(w, h));
            ListenToMouseHover = ListenToMouseInput = true;
            isObjectUI = false;
        }
        public DEProjectileObject(Texture2D Texture, float x, float y, int w, int h)
        {
            SetTexture(Texture);
            SetPosition(new Vector2(x, y));
            SetSize(new Point(w, h));
            ListenToMouseHover = ListenToMouseInput = true;
            isObjectUI = false;
        }

        public DEProjectileObject InitiateProjectile(Rectangle source, Vector2 _destinationProjectile)
        {
            ProjectileLines = new();
            List<Vector2> Popo = new();
            destinationProjectile = _destinationProjectile;
            SetPosition(source.Center.ToVector2());
            ProjectileLines = Popo;
            savedDirection = destinationProjectile - GetPosition;
            savedDirection.Normalize();
            return this;
        }

        public void StartFire() => state = DEProjectileObjectState.onFire;

        public override void OnRender(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            base.OnRender(gameTime, SpriteBatch);
        }

        public override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            if (state == DEProjectileObjectState.onFire)
            {
                if (isOutFromVisibility)
                {
                    state = DEProjectileObjectState.Nothing;
                    SetPosition(0, 0);
                    return;
                }
                var distance = Vector2.Distance(savedDirection, destinationProjectile);
                setXPosition += savedDirection.X * 15.0f;
                setYPosition += savedDirection.Y * 15.0f;
                ProjectileLines.Add(GetPosition);
            } else if (state == DEProjectileObjectState.OutOfBounds)
            {

            }
        }
    }
}
