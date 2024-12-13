using System.Collections.Generic;
using Dungeon.Generation;
using Entities;
using UnityEngine;
using UtilsModule;

namespace Dungeon.Drawers.Terrain
{
    public class EntranceExitDrawer : Drawer
    {
        private readonly PlayerEntity player;

        #region Drawer

        public EntranceExitDrawer(DungeonResult level, EntranceEntity entrance, ExitEntity exit, PlayerEntity player) : base(level)
        {
            this.entrance = entrance;
            this.exit = exit;
            this.player = player;
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
                    if (Level.Has(x, y, Tile.ENTRANCE))
                        PlaceEntrance(x, y);

                    if (Level.Has(x, y, Tile.EXIT))
                        PlaceExit(x, y);

                    if (Level.Has(x, y, Tile.PLAYER))
                    {
                        player.transform.position = new Vector3(x, -y, 0);
                        player.FlipByMovement(Level.Has(x + 1, y, Tile.ENTRANCE) ? Movement.LEFT : Movement.RIGHT);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Process(Room[] rooms)
        {
            var entrancePosition = ProcessEntrance(Level.Entrance);
            Level.Add(entrancePosition.x, entrancePosition.y, Tile.ENTRANCE);

            bool isLeft = Level.HasWall(entrancePosition.x + 1, entrancePosition.y);
            Level.Add(entrancePosition.x + (isLeft ? -1 : 1), entrancePosition.y, Tile.PLAYER);

            var exitPosition = ProcessExit(Level.Exit);
            Level.Add(exitPosition.x, exitPosition.y, Tile.EXIT);
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            entrance.transform.position = Vector3.zero;
            exit.transform.position = Vector3.zero;
            player.transform.position = Vector3.zero;
        }

        #endregion

        #region Entrance

        private readonly EntranceEntity entrance;

        private void PlaceEntrance(int x, int y)
        {
            entrance.transform.position = new Vector3(x, -y, 0);
            entrance.FlipByMovement(Level.Has(x + 1, y, Tile.PLAYER) ? Movement.RIGHT : Movement.LEFT);
        }

        private Vector2Int ProcessEntrance(Room entrance)
        {
            var leftX = entrance.X;
            var rightX = entrance.X + entrance.Width - 1;

            var topY = entrance.Y;
            var bottomY = entrance.Y + entrance.Height - 1;

            // If can place top left, return
            if (CanPlaceEntrance(leftX, topY))
                return new Vector2Int(leftX, topY);

            // If can place top right, return
            if (CanPlaceEntrance(rightX, topY))
                return new Vector2Int(rightX, topY);

            // If can place bottom left, return
            if (CanPlaceEntrance(leftX, bottomY))
                return new Vector2Int(leftX, bottomY);

            // If can place bottom right, return
            if (CanPlaceEntrance(rightX, bottomY))
                return new Vector2Int(rightX, bottomY);

            var possiblePoints = new List<Vector2Int>();

            for (int y = topY + 1; y < bottomY; y++)
            {
                if (CanPlaceEntrance(leftX, y))
                    possiblePoints.Add(new Vector2Int(leftX, y));

                if (CanPlaceEntrance(rightX, y))
                    possiblePoints.Add(new Vector2Int(rightX, y));
            }

            if (possiblePoints.Count == 0)
            {
                Debug.LogWarning("Could not find a valid position to place the entrance.");
                return Vector2Int.zero;
            }

            return possiblePoints[Level.Random.Next(0, possiblePoints.Count)];
        }

        private bool CanPlaceEntrance(int x, int y)
        {
            // If there is an obstacle, invalid
            if (Level.HasObstacle(x, y))
                return false;

            // If has door on the left, invalid
            if (Level.HasDoor(x - 1, y))
                return false;

            // If has door on the top, invalid
            if (Level.HasDoor(x, y - 1))
                return false;

            // If has door on the bottom, invalid
            if (Level.HasDoor(x, y + 1))
                return false;

            // If there is an exit directly to the right, invalid
            if (!Level.HasWall(x + 1, y) && Level.Has(x + 2, y, Tile.EXIT))
                return false;

            // If there is an exit directly to the left, invalid
            if (!Level.HasWall(x - 1, y) && Level.Has(x - 2, y, Tile.EXIT))
                return false;

            return true;
        }

        public static Room FindEntrance(Room[] rooms)
        {
            // Find smallest room
            Room smallest = rooms[0];
            int smallestSize = smallest.Width * smallest.Height;

            for (int i = 1; i < rooms.Length; i++)
            {
                int size = rooms[i].Width * rooms[i].Height;

                if (smallestSize > size)
                {
                    smallest = rooms[i];
                    smallestSize = size;
                }
            }

            return smallest;
        }

        #endregion

        #region Exit

        private readonly ExitEntity exit;

        private void PlaceExit(int x, int y)
        {
            bool isLeft = Level.HasWall(x + 1, y);
            Movement direction = isLeft ? Movement.LEFT : Movement.RIGHT;

            exit.transform.position = new Vector3(x, -y, 0);
            exit.FlipByMovement(direction);
        }

        private Vector2Int ProcessExit(Room exit)
        {
            var leftX = exit.X;
            var rightX = exit.X + exit.Width - 1;

            var topY = exit.Y;
            var bottomY = exit.Y + exit.Height - 1;

            // If can place bottom right, return
            if (CanPlaceExit(rightX, bottomY))
                return new Vector2Int(rightX, bottomY);

            // If can place bottom left, return
            if (CanPlaceExit(leftX, bottomY))
                return new Vector2Int(leftX, bottomY);

            // If can place top right, return
            if (CanPlaceExit(rightX, topY))
                return new Vector2Int(rightX, topY);

            // If can place top left, return
            if (CanPlaceExit(leftX, topY))
                return new Vector2Int(leftX, topY);

            var possiblePoints = new List<Vector2Int>();

            for (int y = topY + 1; y < bottomY; y++)
            {
                if (CanPlaceExit(leftX, y))
                    possiblePoints.Add(new Vector2Int(leftX, y));

                if (CanPlaceExit(rightX, y))
                    possiblePoints.Add(new Vector2Int(rightX, y));
            }

            if (possiblePoints.Count == 0)
            {
                Debug.LogWarning("Could not find a valid position to place the exit.");
                return Vector2Int.zero;
            }

            return possiblePoints[Level.Random.Next(0, possiblePoints.Count)];
        }

        private bool CanPlaceExit(int x, int y)
        {
            // If there is an obstacle, invalid
            if (Level.HasObstacle(x, y))
                return false;

            // If has door on the left, invalid
            if (Level.HasDoor(x - 1, y))
                return false;

            // If has door on the top, invalid
            if (Level.HasDoor(x, y - 1))
                return false;

            // If has door on the bottom, invalid
            if (Level.HasDoor(x, y + 1))
                return false;

            // If there is an entrance directly to the right, invalid
            if (!Level.HasWall(x + 1, y) && Level.Has(x + 2, y, Tile.ENTRANCE))
                return false;

            // If there is an entrance directly to the right, invalid
            if (!Level.HasWall(x - 1, y) && Level.Has(x - 2, y, Tile.ENTRANCE))
                return false;

            return true;
        }

        public static Room FindExit(Room[] rooms, Room entrance)
        {
            Room exit = null;
            var entrancePosition = new Vector2Int(entrance.X, entrance.Y + (entrance.Height - 1) / 2);

            // Find furthest from entrance
            float distance = float.MinValue;

            for (int i = 0; i < rooms.Length; i++)
            {
                var newExit = new Vector2Int(rooms[i].X + (rooms[i].Width - 1) / 2, rooms[i].Y);
                float newDistance = Vector2.Distance(entrancePosition, newExit);

                if (newDistance > distance)
                {
                    distance = newDistance;
                    exit = rooms[i];
                }
            }

            return exit;
        }

        #endregion
    }
}