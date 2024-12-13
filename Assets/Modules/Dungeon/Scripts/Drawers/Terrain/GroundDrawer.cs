using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;
using UtilsModule;

namespace Dungeon.Drawers.Terrain
{
    public class GroundDrawer : Drawer
    {
        private readonly Tilemap groundMap;
        private readonly TileBase tile;

        #region Drawer

        public GroundDrawer(DungeonResult level, Tilemap groundMap, TileBase tile) : base(level)
        {
            this.groundMap = groundMap;
            this.tile = tile;
        }

        /// <inheritdoc/>
        public override void Process(Room[] rooms)
        {
            for (int y = 0; y < Level.Height; y++)
            {
                for (int x = 0; x < Level.Width; x++)
                {
                    if (Level.HasWall(x, y) || Level.HasObstacle(x, y))
                        continue;

                    Level.Add(x, y, Generation.Tile.GROUND);
                }
            }
        }

        /// <inheritdoc/>
        public override void Draw(Room[] rooms)
        {
            int height = Level.Grid.GetLength(0);
            int width = Level.Grid.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!Level.HasGround(x, y))
                        continue;

                    if (Level.Has(x, y, Generation.Tile.COVERED_GROUND))
                        continue;

                    groundMap.SetTile(new Vector3Int(x, -y, 0), tile);
                }
            }
        }

        /// <inheritdoc/>
        public override void Clear() => groundMap.ClearAllTiles();

        #endregion
    }
}