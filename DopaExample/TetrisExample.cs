using DopaEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopaExample
{
    public class TetrisExample : DEWorld
    {
        private DETileMap TetrisSquare;

        private DETileMapSettings setting = new DETileMapSettings();
        private IslandSheets MapSheets = new();

        private DEUnitObject Player;
        private DEProjectileObject Projectile;
        private DEAnimation ProjectileAnimation;
        private DEUnitObject LineObj;

        public override void OnLoadContent()
        {
            SetupTetrisTiles();
            SetupProjectile();
            SetupPlayer();
        }

        public override void OnRender(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            base.OnRender(gameTime, SpriteBatch);
            if (Projectile.isOutFromVisibility)
                return;
            Projectile.ProjectileLines.ForEach((item) =>
            {
                Rectangle RectDraw = new Rectangle((int)item.X, (int)item.Y, 16, 16);
                var Kanjut = TetrisSquare.getMapByLoc(RectDraw);
                if (Kanjut != null)
                    RectDraw = Kanjut.Transform;
                SpriteBatch.Draw(
                    LineObj.GetTexture,
                    RectDraw, 
                    Color.White);
            });
        }

        private void SetupPlayer()
        {
            Player = new("GoldCoin", new(0, 0), new(32, 32));
            LineObj = new("GoldCoin", new(0, 0), new(16, 16));
            WorldObjects.Add(Player);
        }

        private void SetupProjectile()
        {
            Projectile = new("FireRed16", new(156, 156), new(16, 16));
            Projectile.SpriteLocation = new(260, 115, 16, 16);
            WorldObjects.Add(Projectile);

            ProjectileAnimation = new(Projectile);
            ProjectileAnimation.actions = DEAnimationLoopAction.infinite;
            ProjectileAnimation.SetAnimationSprite(new()
            {
                new(260, 115),
                new(260 + 16, 115),
                new(260 + 32, 115),
                new(260 + 48, 115),
                new(260 + 64, 115)
            });
            Projectile.AddAnimation(ProjectileAnimation);
            Projectile.OnLeftClickAction = (() =>
            {
                ProjectileAnimation.StartAnimate();
            });
        }

        private void SetupTetrisTiles()
        {
            setting.baseTexture = MapSheets.name;
            setting.SquareSize = new(24, 24);
            setting.WorldSize = new(24, 24);

            float width = DE.Get().VM._graphics.PreferredBackBufferWidth;
            float height = DE.Get().VM._graphics.PreferredBackBufferHeight;
            Vector2 center = new(
                (setting.WorldSize.X * setting.SquareSize.X) / 2,
                (setting.WorldSize.Y * setting.SquareSize.Y) / 2);
            setting.Position = new((width / 2) - center.X, (height / 2) - center.Y);
            TetrisSquare = new(setting);
            TetrisSquare.StartBuild(TetrisSquare_Success);
        }
        private void TetrisSquare_Success()
        {
            MapsObjects.Add(TetrisSquare);
            TetrisSquare.Maps.ForEach((map) =>
            {
                map.ListenToMouseHover = map.ListenToMouseInput = true;
                map.OnLeftClickAction = (() =>
                {
                    Projectile.InitiateProjectile(Player.Transform,
                        map.Transform.Center.ToVector2() - new Vector2(map.Transform.Size.X / 4, map.Transform.Size.Y / 4)).StartFire();
                });

                map.OnRightClickAction = (() =>
                {
                    Player.SetPosition(map.GetPosition);
                });

                map.SpriteLocation = MapSheets.GetSpriteLocation(Random.Shared.Next(0, 3), Random.Shared.Next(0, 3));
            });
        }
        public void AreaSelectOn(DEMapObject item, int area)
        {
            var settings = TetrisSquare.settings;
            var maps = TetrisSquare.Maps;
            var areaSelectionPosition = item.GetPosition;
            var startAreaPosition = areaSelectionPosition - new Vector2(area * settings.SquareSize.X, area * settings.SquareSize.Y);
            for (int iTerrain = 0; iTerrain < area * 3; iTerrain++)
                for (int jTerrain = 0; jTerrain < area * 3; jTerrain++)
                {
                    var xValue = iTerrain * settings.SquareSize.X;
                    var yValue = jTerrain * settings.SquareSize.Y;
                    var itemByCoordinate = maps.Find((_pred) => {
                        var tResult = _pred.GetPosition == new Vector2(startAreaPosition.X + xValue, startAreaPosition.Y + yValue);
                        return tResult;
                    });
                    if (itemByCoordinate != null)
                        itemByCoordinate.SetTexture("GoldCoin");
                }
        }
    }

    public class IslandSheets
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
}
