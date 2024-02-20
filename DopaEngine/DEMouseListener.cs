using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DopaEngine
{
    public enum DEMouseListenBehavior
    {
        Listening, FinishedListening, InputReceived
    }
    public enum DEMouseState
    {
        LeftClick, LeftPress, RightClick, RightPress, None
    }
    public class DEMouseListener
    {
        public bool isAnyMouseButtonBehaviorChanged
        {
            get => DMouseState.LeftButton == ButtonState.Pressed || DMouseState.RightButton == ButtonState.Pressed;
        }
        public Point MousePosition
        {
            get => DMouseState.Position;
        }

        public MouseState DMouseState { get => Mouse.GetState(); }
        private double elipsedPressedTime = 0.0;
        public DEMouseState GetState { get => EnumMouseState; }
        private DEMouseState EnumMouseState = DEMouseState.None;
        internal DEMouseListenBehavior ListenBehavior = DEMouseListenBehavior.FinishedListening;
        public void ShouldListen(GameTime gameTime)
        {
            Listen(gameTime);
        }
        public void ShouldListenOnHover(GameTime gameTime)
        {
            if (EnumMouseState == DEMouseState.None)
                SignalChanged();
        }
        private void Listen(GameTime gameTime)
        {
            if (DMouseState.LeftButton == ButtonState.Pressed || DMouseState.RightButton == ButtonState.Pressed)
            {
                ListenBehavior = DEMouseListenBehavior.InputReceived;
                elipsedPressedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                EnumMouseState = (DMouseState.LeftButton == ButtonState.Pressed) ? DEMouseState.LeftClick : DEMouseState.RightClick;

                if (elipsedPressedTime >= 300)
                {
                    EnumMouseState = (DMouseState.LeftButton == ButtonState.Pressed) 
                        ? DEMouseState.LeftPress 
                        : DEMouseState.RightPress;
                    SignalChanged();
                }
            }
            else if (elipsedPressedTime != 0.0)
            {
                if (EnumMouseState == DEMouseState.LeftPress || EnumMouseState == DEMouseState.RightPress)
                {
                    EnumMouseState = DEMouseState.None;
                } else
                {
                    SignalChanged();
                }
                Debug.WriteLine(elipsedPressedTime);
                ListenBehavior = DEMouseListenBehavior.FinishedListening;
                elipsedPressedTime = 0.0;
            } else
            {
                EnumMouseState = DEMouseState.None;
            }
        }
        public virtual void SignalChanged() { }
    }
}
