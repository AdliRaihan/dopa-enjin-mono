using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaEngine
{
    public class DECamera
    {
        private Vector2 Position = Vector2.Zero;
        private Point Size = Point.Zero;
        private int speedCamera = 5;
        private float _scaleFactor = 1f;
        public DECamera()
        {
            Size = new Point(DE.Get().VM._graphics.PreferredBackBufferWidth, DE.Get().VM._graphics.PreferredBackBufferHeight);
        }
        public float scaleFactor
        {
            get => _scaleFactor;
            set
            {
                _scaleFactor = value;
            }
        }
        public Rectangle Rect
        {
            get => new Rectangle(Position.ToPoint(), Size);
        }

        public bool isVisible(Rectangle rect)
        {
            // l.TilePosition.Offset(camera.Position.ToPoint());
            Rectangle _rect = rect;
            _rect.Offset(this.Rect.Location);
            var offsetDistance = 128;
            Rectangle visibleBoundary = new Rectangle(
                -offsetDistance,
                -offsetDistance,
                _rect.Width + DE.Get().VM._graphics.PreferredBackBufferWidth + offsetDistance,
                _rect.Height + DE.Get().VM._graphics.PreferredBackBufferHeight + offsetDistance
                );
            return visibleBoundary.Contains(_rect);
        }

        public void onUpdate(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                Position.Y += speedCamera;

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                Position.Y -= speedCamera;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                Position.X -= speedCamera;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                Position.X += speedCamera;

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                scaleFactor = 5f;

            if (Keyboard.GetState().IsKeyDown(Keys.C))
                scaleFactor = 1f;
        }
    }
}
