using System;
using Dungeon.Generation;

namespace Dungeon.Drawers
{
    /// <summary>
    /// Class that processes and draws the given dungeon
    /// </summary>
    public abstract class Drawer
    {
        protected DungeonResult Level;

        public Drawer(DungeonResult level)
        {
            Level = level;
        }

        /// <summary>
        /// Processes the given rooms for the specific logic
        /// </summary>
        public abstract void Process(Room[] rooms);

        /// <summary>
        /// Draws the specific logic from the processed data
        /// </summary>
        public abstract void Draw(Room[] rooms);

        /// <summary>
        /// Clears the drawings
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Creates an empty grid with the given rooms
        /// </summary>
        protected bool[,] CreateEmpty2(Room[] rooms)
        {
            // Calculate size
            int minX = int.MaxValue;
            int maxX = int.MinValue;

            int minY = int.MaxValue;
            int maxY = int.MinValue;

            foreach (var room in rooms)
            {
                minX = Math.Min(minX, room.X);
                maxX = Math.Max(maxX, room.X + room.Width);

                minY = Math.Min(minY, room.Y);
                maxY = Math.Max(maxY, room.Y + room.Height);
            }

            return new bool[maxY - minY + 2, maxX - minX + 2];
        }

        public static Tile[,] CreateEmpty(Room[] rooms)
        {
            // Calculate size
            int minX = int.MaxValue;
            int maxX = int.MinValue;

            int minY = int.MaxValue;
            int maxY = int.MinValue;

            foreach (var room in rooms)
            {
                minX = Math.Min(minX, room.X);
                maxX = Math.Max(maxX, room.X + room.Width);

                minY = Math.Min(minY, room.Y);
                maxY = Math.Max(maxY, room.Y + room.Height);
            }

            return new Tile[maxY - minY + 1, maxX - minX + 1];
        }
    }
}