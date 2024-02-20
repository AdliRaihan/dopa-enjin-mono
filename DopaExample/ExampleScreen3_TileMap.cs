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
using System.Text.Json;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Input;

namespace DopaExample
{
    internal class ExampleScreen3_TileMap : DEWorld
    {
        private DEUnitObject CheckCollision;
        private DEUIObject StartGame;
        private DEUIObject ExampleTexturedObject;
        private DEUIObject ExampleTexturedObject2;
        private List<DEUIObject> Brush = new();
        private DEUIObject SelectedBrush;
        private DEStackContainer StackContainer;
        private DEStackContainer StackContainer2;
        private DEStackContainer StackContainer3;
        private ExampleTileMap TileMaps;
        private DETileMapSettings Settings;
        public override void OnLoadContent()
        {
            base.OnLoadContent();
            makeBrush();
            UseCamera();
            StackContainer = new DEStackContainer(DEDefaultTex.Get().blackTexture, 0, 0, 256, 64);
            StackContainer2 = new DEStackContainer(DEDefaultTex.Get().blackTexture, 0, 0, 256, 64);
            StackContainer3 = new DEStackContainer(DEDefaultTex.Get().blackTexture, 0, 0, 256, 64);
            CheckCollision = new("GoldCoin", Vector2.Zero, new Point(24, 24));
            StackContainer.axis = DEStackContainerAxis.vertical;
            StackContainer.fixedSizeWidth = DE.Get().VM._graphics.PreferredBackBufferWidth;

            ExampleTexturedObject = new DEUIObject("IslandSheets", 132, 132, 64, 64);
            ExampleTexturedObject.OnLeftClickAction = (() =>
            {
                GrabbedWorldObject.PasteTextureSpritedObject(ExampleTexturedObject);
            });


            ExampleTexturedObject.SpriteLocation = new(16 * 3, 16 * 3, 24, 24);
            ExampleTexturedObject2 = new DEUIObject("Button3x", 532, 522, 132, 32);
            ExampleTexturedObject2.OnLeftClickAction = PrintDraw;

            CheckCollision.readCollision = false;
            CheckCollision.ListenToMouseHover = CheckCollision.ListenToMouseInput = true;
            CheckCollision.OnLeftClickAction = (() =>
            {
                DE.Get().Exit();
            });

            StartGame = new DEUIObject("GoldCoin", 132, 132, 32, 32);
            StartGame.OnLeftClickAction = (() => DE.Get().Exit());
            instatiateTileMap();
            WorldObjects.AddMultiple(new List<DEBaseObject>() { CheckCollision });
            InterfaceViews.AddMultiple(new List<DEUIObject>() { StackContainer, ExampleTexturedObject2 });

            StackContainer.isHidden = false;

            StackContainer.AddList(new() { StackContainer2, StackContainer3 });
            StackContainer2.AddList(new() { StartGame });
            StackContainer3.AddList(Brush);
            Debug.WriteLine("Example Screen 3 Initiated!");
            isContentLoaded = true;
        }

        private void instatiateTileMap()
        {
            Settings = new();
            Settings.baseTexture = "BasicSquare";
            Settings.Position = Vector2.Zero;
            Settings.SquareSize = new(24, 24);
            Settings.WorldSize = new(64, 64);
            TileMaps = new(Settings);
            TileMaps.StartBuild(TileMapSuccessBuild);
        }
        public override void OnRender(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            if (!isContentLoaded) return;
            base.OnRender(gameTime, SpriteBatch);
        }

        public override void OnRenderUI(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            if (!isContentLoaded) return;
            base.OnRenderUI(gameTime, SpriteBatch);
            SpriteBatch.DrawString(DE.Get().DefaultFont, $"{CheckCollision.Transform} {gameTime.ElapsedGameTime}", new Vector2(0, 150), Color.Black);
        }
        public override void OnUpdate(GameTime gameTime)
        {
            if (!isContentLoaded) return;
            base.OnUpdate(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                CheckCollision.setXPosition -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                CheckCollision.setXPosition += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                CheckCollision.setYPosition -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                CheckCollision.setYPosition += 1;
        }

        private void TileMapSuccessBuild()
        {
            if (TileMaps == null) return;
            if (TileMaps.Behavior == DETileMapBehavior.Building) return;
            TileMaps.Maps.ForEach((item) =>
            {
                item.ListenToMouseInput = item.ListenToMouseHover = true;
                item.OnLeftClickAction = (() =>
                {
                    //CheckCollision.SetPosition(item.GetPosition);
                    //TileMaps.GrabRef(item);
                    //selectObject(item);
                    item.PasteTextureFromSprite(SelectedBrush.GetTexture, SelectedBrush.SpriteLocation.Location, SelectedBrush.SpriteLocation.Size);
                });
                item.OnLeftPressAction = (() =>
                {
                    item.PasteTextureFromSprite(SelectedBrush.GetTexture, SelectedBrush.SpriteLocation.Location, SelectedBrush.SpriteLocation.Size);
                });
                item.OnRightClickAction = (() => item.SetTexture("BasicSquare"));
                //item.OnHoverAction = (() =>
                //{
                //    TileMaps.RemoveMap(item);
                //});
            });
            MapsObjects.Add(TileMaps);
            WorldObjects.Add(TileMaps.BuyLand);
            LoadPreMap();
        }

        private void makeBrush()
        {
            var spriteSquareBase = 24;
            var maxSpriteLocation = 8;
            var iSprite = 0;
            var jSprite = 0;
            while (iSprite < maxSpriteLocation)
            {
                jSprite = 0;
                while (jSprite < maxSpriteLocation)
                {
                    var _b = new DEUIObject("IslandSheets", 0, 0, 16, 16);
                    _b.ListenToMouseInput = _b.ListenToMouseHover = true;
                    _b.SpriteLocation = new Rectangle(
                        spriteSquareBase * iSprite, 
                        spriteSquareBase * jSprite,
                        spriteSquareBase, spriteSquareBase
                        );
                    _b.OnLeftClickAction = (() =>
                    {
                        SelectedBrush = _b;
                    });
                    Brush.Add(_b);
                    jSprite += 1;
                }
                iSprite += 1;
            }

        }
        private void PrintDraw()
        {

            //var jsonContainerCoordinate = "";
            //var jsonContainerSpriteCoordinate = "";
            //var jsonBuilder = "";
            //TileMaps.Maps.ForEach((item) =>
            //{
            //    if (item.GetTextureName != "BasicSquare")
            //    {
            //        jsonContainerCoordinate = item.GetPosition.ToString();
            //        jsonContainerSpriteCoordinate = item.SpriteLocation.Location.ToString();

            //        jsonBuilder += "{" +
            //        $"Coodinate:{jsonContainerCoordinate}," +
            //        $"SpriteCoordinate:{jsonContainerSpriteCoordinate}" +
            //        "}";
            //    }
            //});
            //Debug.WriteLine($"{jsonBuilder}");
        }
        public void LoadPreMap()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"data/map/json1.json");
            var savedMaps = new SavedMaps();
            var data = File.ReadAllText(path);
            var jsonString = JsonSerializer.Deserialize<SavedMaps>(data);
            PreBuildSavedMaps(jsonString);
        }
        private void PreBuildSavedMaps(SavedMaps map) 
        {
            map.data.ForEach((item) =>
            {
                var foundObject = TileMaps.Maps.Find(
                    (pred) => pred.GetPosition == item.GetVectorCoordinate);

                if (foundObject != null)
                    foundObject.PasteTextureFromSprite(
                        ExampleTexturedObject.GetTexture, 
                        item.GetVectorSpriteCoordinate.ToPoint(),
                        new Point(24, 24));
            });
        }
    }

    class SavedMaps
    {
        public List<Data> data { get; set; }

        public class Data
        {
            public CoordinateData Coodinate { get; set; }
            public CoordinateData SpriteCoordinate { get; set; }
            public Vector2 GetVectorCoordinate { get => new(Coodinate.X, Coodinate.Y); }
            public Vector2 GetVectorSpriteCoordinate { get => new(SpriteCoordinate.X, SpriteCoordinate.Y); }
        }

        public class CoordinateData
        {
            public float X { get; set; }
            public float Y { get; set; }
        }
    }

    class ExampleTileMap : DETileMap
    {
        public DEUIObject BuyLand;
        private float BuyLandTexScale = 0.45f;

        private DEMapObject grabbedRefs;
        public ExampleTileMap(DETileMapSettings initialSettings) : base(initialSettings) {
            BuyLand = new("Button3x", Vector2.Zero, Point.Zero);
            BuyLand.SetSizeByTexture(0.45f);
            BuyLand.ObjectScalingValue = 0.0f;
            BuyLand.isObjectUI = false;
            BuyLand.OnLeftClickAction = (() =>
            {
                grabbedRefs = null;
                BuyLand.ObjectScalingValue = 0.0f;
            });
            BuyLand.isHidden = true;
        }

        public void GrabRef(DEMapObject _grab)
        {
            if (grabbedRefs != null) return;
            grabbedRefs = _grab;
        }

        public override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            BuyLand.isHidden = grabbedRefs == null;
            if (grabbedRefs != null)
            {
                BuyLand.SetPosition(grabbedRefs.GetPosition);
            }
        }

        public override void OnRender(GameTime gm, SpriteBatch sb)
        {
            base.OnRender(gm, sb);
            if (grabbedRefs != null)
            {
                sb.DrawString(DE.Get().DefaultFont, $"{BuyLand.ObjectScalingValue}", BuyLand.GetPosition, Color.Black);
            }
                // todo
        }


    }
}
