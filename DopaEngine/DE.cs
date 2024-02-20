using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    public struct DEVariables
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public ContentManager Content;
        public DEWorld CurrentActivity;

        public void SetActivity(DEWorld _activity)
        {
            CurrentActivity = _activity;
            if (_graphics != null && _spriteBatch != null && Content != null)
                CurrentActivity.OnLoadContent();
        }
    }

    public class DE : Game
    {
        private FrameCounter _frameCounter = new FrameCounter();

        public DEVariables VM = new DEVariables();

        public SpriteFont DefaultFont;

        public static DE ActiveInstance;

        public static DEVariables VMinstance;

        public static float centerWindow
        {
            get => DE.Get().VM._graphics.PreferredBackBufferWidth / 2;
        }

        public static DE Get()
        {
            if (ActiveInstance == null)
                ActiveInstance = new DE();
            return ActiveInstance;
        }

        public DE()
        {
            VM._graphics = new GraphicsDeviceManager(this);
            VM._graphics.PreferredBackBufferWidth = 1024;
            VM._graphics.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            VM.Content = Content;
        }

        protected override void Initialize()
        {
            base.Initialize();
            IsFixedTimeStep = false;
            VM._graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void LoadContent()
        {
            VM._spriteBatch = new SpriteBatch(GraphicsDevice);
            DefaultFont = Content.Load<SpriteFont>("DefaultFont");
            if (VM.CurrentActivity != null)
                VM.CurrentActivity.OnLoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (VM.CurrentActivity != null)
                VM.CurrentActivity.OnUpdate(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            GraphicsDevice.Clear(Color.Black);
            if (VM.CurrentActivity != null)
            {
                VM.CurrentActivity.StartDraw(gameTime, VM._spriteBatch);
            }
            else
            {
                VM._spriteBatch.Begin();
                VM._spriteBatch.DrawString(DefaultFont, "No Activity Being Loaded!", Vector2.Zero, Color.Red);
                VM._spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public bool isVisible(Rectangle rect)
        {
            Rectangle rectBoundaries = new(0, 0, VM._graphics.PreferredBackBufferWidth, VM._graphics.PreferredBackBufferHeight);
            return !rectBoundaries.Contains(rect);
        }
    }
    public class FrameCounter
    {
        public long TotalFrames { get; private set; }
        public float TotalSeconds { get; private set; }
        public float AverageFramesPerSecond { get; private set; }
        public float CurrentFramesPerSecond { get; private set; }

        public const int MaximumSamples = 100;

        private Queue<float> _sampleBuffer = new();

        public void Update(float deltaTime)
        {
            CurrentFramesPerSecond = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MaximumSamples)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
        }
    }
}
