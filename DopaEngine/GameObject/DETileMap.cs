using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaEngine
{
    public struct DETileMapSettings
    {
        public string baseTexture;
        public Point SquareSize;
        public Point WorldSize;
        public Vector2 Position;
        public Rectangle Rect;
    }

    public enum DETileMapBehavior
    {
        Building, BuildingFinished, CurrentlyEditing
    }

    public class DETileMap
    {
        public bool isHidden = false;
        public DETileMapSettings settings;
        public List<DEMapObject> Maps = new();
        public DETileMapBehavior Behavior = DETileMapBehavior.BuildingFinished;
        public DETileMap(DETileMapSettings initialSettings) => settings = initialSettings;
        public async void StartBuild(Action WhenBuildSuccess = null)
        {
            Behavior = DETileMapBehavior.Building;
            await Task.Run(() =>
            {
                List<DEMapObject> _Objects = new();
                for (int iTerrain = 0; iTerrain < (settings.WorldSize.X); iTerrain++)
                    for (int jTerrain = 0; jTerrain < (settings.WorldSize.Y); jTerrain++)
                        _Objects.Add(new DEMapObject(
                            settings.baseTexture,
                            new Vector2(
                                settings.SquareSize.X * iTerrain,
                                settings.SquareSize.Y * jTerrain
                                ) + settings.Position,
                             new Point(
                                settings.SquareSize.X,
                                settings.SquareSize.Y
                                )
                            ));
                Maps = _Objects;
                Behavior = DETileMapBehavior.BuildingFinished;
                if (WhenBuildSuccess != null)
                    WhenBuildSuccess.Invoke();
            });
        }
        public DEMapObject getMapByLoc(Vector2 Loc)
        {
            return Maps.Find((item) => item.GetPosition == Loc);
        }
        public DEMapObject getMapByLoc(Rectangle Loc)
        {
            var locNormalize = new Rectangle(Loc.Location, new(128, 128));
            return Maps.Find((item) => locNormalize.Contains(item.Transform));
        }
        public async void Update(GameTime gametime)
        {
            await Task.Run(() =>
            {
                if (Behavior == DETileMapBehavior.CurrentlyEditing) return;
                Maps.ForEach((item) => item.OnUpdate(gametime));
            });
        }
        public void AreaSelectOn(DEMapObject item, int area)
        {
            var areaSelectionPosition = item.GetPosition;
            var startAreaPosition = areaSelectionPosition - new Vector2(area * settings.SquareSize.X, area * settings.SquareSize.Y);
            for (int iTerrain = 0; iTerrain < area * 3; iTerrain++)
                for (int jTerrain = 0; jTerrain < area * 3; jTerrain++)
                {
                    var xValue = iTerrain * settings.SquareSize.X;
                    var yValue = jTerrain * settings.SquareSize.Y;
                    var itemByCoordinate = Maps.Find((_pred) => {
                        var tResult = _pred.GetPosition == new Vector2(startAreaPosition.X + xValue, startAreaPosition.Y + yValue);
                        return tResult;
                    });
                    if (itemByCoordinate != null)
                        itemByCoordinate.SetTexture("GoldCoin");
                }
        }
        public async void RemoveMap(DEMapObject item)
        {
            Behavior = DETileMapBehavior.CurrentlyEditing;
            await Task.Run(() =>
            {
                List<DEMapObject> New_Objects = new();
                Maps.ForEach((itemForeach) =>
                {
                    if (itemForeach != item)
                        New_Objects.Add(itemForeach);
                });
                Maps = New_Objects;
                Behavior = DETileMapBehavior.BuildingFinished;
            });
        }
        public virtual void OnUpdate(GameTime gameTime) => Update(gameTime);
        public virtual void OnRender(GameTime gm, SpriteBatch sb) =>
            Maps.ForEach((map) =>
            {
                if (map.GetTexture != null)
                    map.OnRender(gm, sb);
            });
    }
}
