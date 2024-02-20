using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DopaEngine
{
    public class DETileMapEditor : DEWorld
    {

        public int spriteSquareBase = 24;
        public int maxSpriteLocation = 8;

        private DETileMapSettings setting = new DETileMapSettings();

        private DETileMap TileMapEditorBoard;

        private DESheetsInfo SheetsInfo = new();

        private DEStackContainer SheetEditorContainer;

        private DEUIObject SelectedBrush;

        private DEUIObject SaveMap;

        private DEFileManager FM = new();

        private List<DEUIObject> Brush = new();

        public override void OnLoadContent()
        {
            base.OnLoadContent();
            SetupBoard();
            SetupEditor();
            SetupBrush();
            SetupUI();
        }
        public override void OnRender(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            base.OnRender(gameTime, SpriteBatch);
        }
        public override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }
        private void SetupEditor()
        {
            SheetEditorContainer = new(DEDefaultTex.Get().clearTexture, 0, 0, 0, 0);
            InterfaceViews.Add(SheetEditorContainer);
        }
        private void SetupBoard()
        {
            setting.baseTexture = SheetsInfo.name;
            setting.SquareSize = new(24, 24);
            setting.WorldSize = new(24, 24);

            float width = DE.Get().VM._graphics.PreferredBackBufferWidth;
            float height = DE.Get().VM._graphics.PreferredBackBufferHeight;
            Vector2 center = new(
                (setting.WorldSize.X * setting.SquareSize.X) / 2,
                (setting.WorldSize.Y * setting.SquareSize.Y) / 2);
            setting.Position = new((width / 2) - center.X, (height / 2) - center.Y);
            TileMapEditorBoard = new(setting);
            TileMapEditorBoard.StartBuild(TetrisSquare_Success);
        }
        private void SetupBrush()
        {
            var iSprite = 0;
            var jSprite = 0;
            while (iSprite < maxSpriteLocation)
            {
                jSprite = 0;
                while (jSprite < maxSpriteLocation)
                {
                    var _b = new DEUIObject(SheetsInfo.name, 0, 0, 16, 16);
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
            SheetEditorContainer.AddList(Brush);
        }
        private void SetupUI()
        {
            SaveMap = new("Button3x", 0, SheetEditorContainer.Transform.Bottom + 150, 0, 0);
            SaveMap.SetSizeByTexture(0.5f);
            InterfaceViews.Add(SaveMap);
            SaveMap.isObjectUI = false;
            SaveMap.ListenToMouseHover = SaveMap.ListenToMouseInput = true;
            SaveMap.OnLeftClickAction = (() => {
                FM.Save(ConvertEditorToMapData(), "\\data\\map\\test.dedata");
            });
        }
        private void TetrisSquare_Success()
        {
            MapsObjects.Add(TileMapEditorBoard);
            TileMapEditorBoard.Maps.ForEach((map) =>
            {
                map.ListenToMouseHover = map.ListenToMouseInput = true;

                map.OnLeftClickAction  = map.OnLeftPressAction = (() =>
                {
                    if (SelectedBrush == null)
                        return;
                    map.SetTexture(SelectedBrush.GetTextureName);
                    map.SpriteLocation = SelectedBrush.SpriteLocation;
                });

                map.OnRightClickAction = (() =>
                {
                    map.RemoveTexture();
                });

            });
            BuildFromFile("\\data\\map\\test.dedata");
        }
        private string ConvertEditorToMapData()
        {
            string ContentBuild = "";
            TileMapEditorBoard.Maps.ForEach((item) =>
            {
                if (item.GetTexture != null)
                    ContentBuild += $"" +
                    $"{item.GetTextureName}|" +
                    $"{item.SpriteLocation == Rectangle.Empty}|" +
                    $"{item.SpriteLocation.X}|" +
                    $"{item.SpriteLocation.Y}|" +
                    $"{item.Transform.Size.X}|" +
                    $"{item.Transform.Size.Y}|" +
                    $"{item.GetPosition.X}|" +
                    $"{item.GetPosition.Y}\n";
            });
            return ContentBuild;
        }
        private void BuildFromFile(string path)
        {
            List<string> MapData = FM.Load(path).Result.Split("\n").ToList();
            MapData.ForEach((item) =>
            {
                List<string> DetailMapData = item.Split("|").ToList();
                if (DetailMapData.Count < 7) return;
                string xLocString = DetailMapData[6];
                string yLocString = DetailMapData[7];
                var filteredObj = TileMapEditorBoard.getMapByLoc(new Vector2(float.Parse(xLocString), float.Parse(yLocString)));
                if (filteredObj != null)
                {
                    filteredObj.SetTexture(DetailMapData[0]);
                    filteredObj.SpriteLocation = new(
                        int.Parse(DetailMapData[2]),
                        int.Parse(DetailMapData[3]),
                        int.Parse(DetailMapData[4]),
                        int.Parse(DetailMapData[5]));
                }
            });
        }
    }

    public class DESheetsInfo
    {
        public string name = "IslandSheets";
        public int sizeSheet = 24;
        public Rectangle GetSpriteLocation(int numberVertical = 0, int numberHorizontal = 0)
        {
            return new Rectangle(
                new(sizeSheet * numberVertical, sizeSheet * numberHorizontal),
                new(sizeSheet, sizeSheet)
                );
        }
    }

    public class DEFileManager
    {
        private bool isFileManagerIsInUse = false;
        public void Save(string Content, string _Path)
        {
            try
            {
                StreamWriter sw = new StreamWriter(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + _Path);
                sw.WriteLine(Content);
                sw.Close();
            }
            catch (Exception e)
            {
            }
            finally
            {
            }
        }
        public Task<string> Load(string _Path)
        {
            if (isFileManagerIsInUse) return Task.FromResult("");
            isFileManagerIsInUse = true;
            var ContentResult = "";
            try
            {
                StreamReader sr = new StreamReader(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + _Path);
                var line = sr.ReadLine();
                while (line != null)
                {
                    line = sr.ReadLine();
                    ContentResult += line + "\n";
                }
                sr.Close();
                isFileManagerIsInUse = false;
                return Task.FromResult(ContentResult);
            }
            catch (Exception e)
            {
                isFileManagerIsInUse = false;
                return Task.FromResult(ContentResult);
            }
            finally
            {
            }
        }
    }
}
