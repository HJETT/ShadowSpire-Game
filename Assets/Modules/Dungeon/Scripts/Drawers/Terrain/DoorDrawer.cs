using System;
using System.Collections.Generic;
using Dungeon.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;
using UtilsModule;

namespace Dungeon.Drawers.Terrain
{
    public class DoorDrawer : Drawer
    {
        private readonly Tilemap wallMap;
        private readonly TileBase doorTile;

        #region Drawer

        public DoorDrawer(DungeonResult level, Tilemap wallMap, TileBase doorTile) : base(level)
        {
            this.wallMap = wallMap;
            this.doorTile = doorTile;
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
                    if (!Level.HasDoor(x, y))
                        continue;

                    wallMap.SetTile(new Vector3Int(x, -y, 0), doorTile);
                }
            }
        }

        /// <inheritdoc/>
        public override void Process(Room[] rooms)
        {
            System.Random random = Level.Random;

            List<(Room, Room)> processedLinks = new();

            foreach (var (room, adjacents) in Level.AdjacentRooms)
            {
                foreach (var adjacent in adjacents)
                {
                    // If already processed, skip
                    if (processedLinks.Contains((adjacent, room)))
                        continue;

                    if (adjacent.X < room.X + room.Width && adjacent.Y < room.Y + room.Height)
                        continue;

                    processedLinks.Add((room, adjacent));

                    bool isVerticalWall = adjacent.X >= room.X + room.Width;

                    int min = isVerticalWall
                        ? Mathf.Max(room.Y, adjacent.Y)
                        : Mathf.Max(room.X, adjacent.X);

                    int max = isVerticalWall
                        ? Mathf.Min(room.Y + room.Height - 1, adjacent.Y + adjacent.Height - 1)
                        : Mathf.Min(room.X + room.Width - 1, adjacent.X + adjacent.Width - 1);

                    // if (max - min > 2 && Level.Random.NextDouble() < 0.333f)
                    // {
                    //     RemoveWall(room, adjacent);
                    //     continue;
                    // }

                    if (max - min >= 2)
                    {
                        min++;
                        max--;
                    }

                    if (isVerticalWall)
                        PlaceDoor(adjacent.X - 1, random.Next(min, max));
                    else
                        PlaceDoor(random.Next(min, max), adjacent.Y - 1);
                }
            }
        }

        /// <inheritdoc/>
        public override void Clear() => wallMap.ClearAllTiles();

        #endregion

        #region Door

        private void PlaceDoor(int x, int y) => Level.Add(x, y, Generation.Tile.DOOR_OPENED);
        private void RemoveWall(Room room, Room adjacent)
        {
            if (adjacent.X >= room.X + room.Width)
            {
                var minY = Mathf.Max(room.Y, adjacent.Y);
                var maxY = Mathf.Min(room.Y + room.Height - 1, adjacent.Y + adjacent.Height - 1);

                for (int y = minY; y <= maxY; y++)
                    Level.Remove(adjacent.X - 1, y, Generation.Tile.WALL);
            }
            else
            {
                var minX = Mathf.Max(room.X, adjacent.X);
                var maxX = Mathf.Min(room.X + room.Width - 1, adjacent.X + adjacent.Width - 1);

                for (int x = minX; x <= maxX; x++)
                    Level.Remove(x, adjacent.Y - 1, Generation.Tile.WALL);
            }
        }

        #endregion
    }
}