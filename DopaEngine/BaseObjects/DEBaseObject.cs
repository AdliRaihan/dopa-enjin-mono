using DopaEngine.Extensions;
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
    public class DEBaseObject
    {
        private Rectangle _Transform = Rectangle.Empty;
        private Vector2 Position = Vector2.Zero;
        private Point Size = Point.Zero;
        private Texture2D Texture;
        private List<DEAnimation> Animation = new();
        public float ObjectScalingValue = 1.0f;
        public bool readCollision = false;
        public bool ListenToMouseInput = false;
        public bool ListenToMouseHover = false;
        public bool isHidden = false;
        public bool isOutFromVisibility = false;
        public bool isObjectUI = true;
        public bool isBlockingAllInteractions = false;
        public Rectangle SpriteLocation;
        public Action OnLeftClickAction;
        public Action OnLeftPressAction;
        public Action OnRightClickAction;
        public Action OnRightPressAction;
        public Action OnHoverAction;
        public Action OnDragAction;
        public Texture2D GetTexture { get => Texture; }
        public string GetTextureName { get => Texture.Name; }
        public Vector2 GetPosition { get => Position; }
        private Vector2 getScaleFromRect { 
            get {
                if (SpriteLocation != Rectangle.Empty)
                    return new Vector2((float)Size.X / (float)SpriteLocation.Size.X, (float)Size.Y / (float)SpriteLocation.Size.Y);
                return new Vector2((float)Size.X / (float)Texture.Width, (float)Size.Y / (float)Texture.Height);
            }
        }
        
        public float setXPosition { get => Position.X; set => SetPosition(value, Position.Y); }
        public float setYPosition { get => Position.Y; set => SetPosition(Position.X, value); }
        public Rectangle Transform { get => _Transform; }
        public void SetPosition(Vector2 value) => SetPosition(value.X, value.Y);
        public void SetSize(Point value) => SetSize(value.X, value.Y);
        public void SetSizeByTexture(float scale = 1.0f) => SetSize((int)(Texture.Width * scale), (int)(Texture.Height * scale));
        public void SetTexture(string assetsName) => Texture = DE.Get().VM.Content.Load<Texture2D>(assetsName);
        public void RemoveTexture() => Texture = null;
        public void SetTexture(Texture2D texture) => Texture = texture;
        public void SetTextureColor(Color _Color) { /* WIP */}
        public virtual void OnRender(GameTime gameTime, SpriteBatch SpriteBatch) {
            if (isOutFromVisibility || isHidden) return;
            SpriteBatch.Draw(
                Texture, 
                Position, 
                (SpriteLocation == Rectangle.Empty) ? null : SpriteLocation, 
                Color.White, 0f, 
                Vector2.Zero, 
                getScaleFromRect.Scale(ObjectScalingValue), SpriteEffects.None, 1f);
        }
        private void isVisibleToCamera()
        {
            if (isObjectUI || isHidden) return;
            DECamera _cam = DE.Get().VM.CurrentActivity.getCamera;
            if (_cam == null)
            {
                isOutFromVisibility = DE.Get().isVisible(Transform);
                return;
            }
            isOutFromVisibility = !_cam.isVisible(Transform);
        }

        public virtual void OnUpdate(GameTime gameTime) {
            if (isHidden) return;
            isVisibleToCamera();
            Animation.ForEach((item) => item.OnAnimateUpdate(gameTime));
        }
        public void SetPosition(float x, float y)
        {
            Position.X = x;
            Position.Y = y;
            _Transform.Location = Position.ToPoint();
        }
        public void SetSize(int width, int height)
        {
            Size.X = width;
            Size.Y = height;
            _Transform.Size = Size;
        }
        public void PasteTextureSpritedObject(DEBaseObject source)
        {
            this.Texture = source.Texture;
            SpriteLocation = source.SpriteLocation;
        }
        public void PasteTextureFromSprite(Texture2D texture, Point location, Point size)
        {
            this.Texture = texture;
            SpriteLocation = new Rectangle(location, size);
        }
        public void SetSpriteLocationVector(Vector2 Position)
        {
            var location = Position.ToPoint();
            var size = SpriteLocation.Size;
            SpriteLocation = new Rectangle(location, size);
        }
        public void AddAnimation(DEAnimation Anim) => Animation.Add(Anim);
        public void ResetAnimation() => Animation = new();
    }
}
