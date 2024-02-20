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
    public class DEWorld : DEMouseListener
    {
        private double updateTimeElipsed = 0.0f;

        private DECamera Camera;

        public bool isActive = false;

        public bool isContentLoaded = false;

        public DEBaseObject GrabbedWorldObject;

        public List<DETileMap> MapsObjects = new();

        public List<DEUIObject> InterfaceViews = new();

        public List<DEBaseObject> WorldObjects = new();

        private bool IgnoreListen = false;
        public bool isWorldObjectBeingGrabbed { get => GrabbedWorldObject == null; }
        public DECamera getCamera { get => Camera; }
        public void UseCamera() => Camera = new DECamera();
        public void ChangeActivity(DEWorld newActivity) => DE.Get().VM.SetActivity(newActivity);
        public void StartDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Camera != null)
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
                    Matrix.CreateTranslation(Camera.Rect.Location.ToVector2().X, Camera.Rect.Location.ToVector2().Y, 0));
            else
                spriteBatch.Begin();
            OnRender(gameTime, spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin();
            OnRenderUI(gameTime, spriteBatch);
            spriteBatch.End();
        }
        public virtual void OnRender(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            MapsObjects.ForEach((item) => item.OnRender(gameTime, SpriteBatch));
            WorldObjects.ForEach((item) => item.OnRender(gameTime, SpriteBatch));
        }
        public virtual void OnRenderUI(GameTime gameTime, SpriteBatch SpriteBatch) {
            var countObject = (MapsObjects.Count > 0) ? MapsObjects.First()?.Maps.Count : 0;
            InterfaceViews.ForEach((item) => item.OnRender(gameTime, SpriteBatch));
            SpriteBatch.DrawString(DE.Get().DefaultFont, $"{updateTimeElipsed}", new Vector2(0, 174), Color.Black);
            if (Camera != null)
                SpriteBatch.DrawString(DE.Get().DefaultFont, $"{Camera.Rect} {WorldObjects.Count} {countObject} {IgnoreListen} {GrabbedWorldObject == null}", new Vector2(0, 125), Color.Black);
        }
        public virtual void OnUpdate(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                GrabbedWorldObject = null;
            if (Camera != null)
                Camera.onUpdate(gameTime);

            updateTimeElipsed += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (updateTimeElipsed >= 32)
            {
                ShouldListenOnHover(gameTime);
                updateTimeElipsed = 0.0;
            } else
            {
                ShouldListen(gameTime);
            }
            if (updateTimeElipsed >= 16)
            {
                MapsObjects.ForEach((item) => item.OnUpdate(gameTime));
                WorldObjects.ForEach((item) => item.OnUpdate(gameTime));
            }
            InterfaceViews.ForEach((item) => item.OnUpdate(gameTime));
        }

        public virtual void OnLoadContent() => isActive = true;

        public override void SignalChanged()
        {
            //if (IgnoreListen) return;
            base.SignalChanged();
            IgnoreListen = true;
            SignalEachElement();
        }
        private async void SignalEachElement()
        {
            if (await SignalForInterfaces() == false)
            {
                ListenBehavior = DEMouseListenBehavior.FinishedListening;
                return;
            }
            if (await SignalForWorldObjects() == false)
            {
                ListenBehavior = DEMouseListenBehavior.FinishedListening;
                return;
            }
            if (await SignalForMaps() == false)
            {
                ListenBehavior = DEMouseListenBehavior.FinishedListening;
                return;
            }
            ListenBehavior = DEMouseListenBehavior.FinishedListening;
        }
        private Task<bool> SignalForInterfaces()
        {
            if (GrabbedWorldObject != null) return Task.FromResult(false);
            bool isRunning = true;
            InterfaceViews.ForEach(async (item) =>
            {
                if (!isRunning || item.isHidden) return;
                var result = await ExecuteItemBehavior(item);
                isRunning = !result;
            });
            return Task.FromResult(isRunning);
        }
        private Task<bool> SignalForWorldObjects()
        {
            if (GrabbedWorldObject != null) return Task.FromResult(false);
            bool isRunning = true;
            WorldObjects.ForEach(async (item) =>
            {
                if (item.isHidden || item.isOutFromVisibility || !isRunning) return;
                var result = await ExecuteItemBehavior(item);
                isRunning = !result;
            });
            return Task.FromResult(isRunning);
        }
        private Task<bool> SignalForMaps()
        {
            if (GrabbedWorldObject != null) return Task.FromResult(false);
            bool isRunning = true;
            MapsObjects.ForEach((parentItem) =>
            {
                if (!(parentItem.Behavior == DETileMapBehavior.BuildingFinished) || !isRunning) return;
                parentItem.Maps.ForEach(async (item) =>
                {
                    if (item.isHidden || item.isOutFromVisibility || !isRunning || !item.ListenToMouseInput) return;
                    var result = await ExecuteItemBehavior(item);
                    isRunning = !result;
                });
            });
            return Task.FromResult(isRunning);
        }
        public Task<bool> ExecuteItemBehavior(DEBaseObject item)
        {
            if (GrabbedWorldObject != null) return Task.FromResult(false);
            var InvokeFit = IsObjectHovered(item);
            if (!InvokeFit.Result) return Task.FromResult(false);
            InvokeMouseActionForObjects(item);
            return Task.FromResult(true);
        }
        public Task<bool> IsObjectHovered(DEBaseObject item)
        {
            if (!item.ListenToMouseHover) return Task.FromResult(false);
            if (DE.Get().VM.CurrentActivity.getCamera != null)
                return Task.FromResult(item.Transform.Contains(DMouseState.Position.ToVector2() - DE.Get().VM.CurrentActivity.getCamera.Rect.Location.ToVector2()));
            return Task.FromResult(item.Transform.Contains(DMouseState.Position));
        }
        public async void InvokeMouseActionForObjects(DEBaseObject item)
        {
            await Task.Run(() =>
            {
                switch (GetState)
                {
                    case DEMouseState.LeftClick:
                        item.OnLeftClickAction?.Invoke();
                        break;
                    case DEMouseState.LeftPress:
                        item.OnLeftPressAction?.Invoke();
                        break;
                    case DEMouseState.RightClick:
                        item.OnRightClickAction?.Invoke();
                        break;
                    case DEMouseState.RightPress:
                        item.OnRightPressAction?.Invoke();
                        break;
                    case DEMouseState.None:
                        item.OnHoverAction?.Invoke();
                        break;
                }
            });
        }
        public void selectObject(DEBaseObject _selectObject)
        {
            GrabbedWorldObject = _selectObject;
        }
    }
}
