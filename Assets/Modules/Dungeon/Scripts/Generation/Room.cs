using System;
using UnityEngine;

namespace Dungeon.Generation
{
    public enum RoomType
    {
        NORMAL,
        ENTRANCE, // Room that is the entrance
        EXIT, // Room that is the exit
        ENEMY, // Room that spawns monsters
        TREASURE, // Room that spawns a treasure
        SPIKES, // Room that spawns spikes
    }

    public class Room
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int Depth { get; set; }
        public RoomType Type { get; set; } = RoomType.NORMAL;

        public Room[] Children { get; private set; }

        public bool Split(System.Random rand, int minWidth, int minHeight)
        {
            var roomA = new Room();
            var roomB = new Room();

            bool hasSplitted = false;
            bool splitVertical = false;

            for (int i = 0; i < 10; i++)
            {
                splitVertical = rand.Next(0, 2) == 0;
                var percent = rand.NextDouble() * 0.4f + 0.3f; // 30-70%

                var newWidth = (int)Math.Floor(Width * percent);
                var newHeight = (int)Math.Floor(Height * percent);

                // If splitting vertically makes two big enough room
                if (splitVertical && newWidth >= minWidth && Width - newWidth >= minWidth)
                {
                    roomA.X = X;
                    roomA.Y = Y;
                    roomA.Width = newWidth;
                    roomA.Height = Height;

                    roomB.X = X + newWidth;
                    roomB.Y = Y;
                    roomB.Width = Width - newWidth;
                    roomB.Height = Height;

                    hasSplitted = true;
                    break;
                }
                else if (newHeight >= minHeight && Height - newHeight >= minHeight)
                {
                    roomA.X = X;
                    roomA.Y = Y;
                    roomA.Width = Width;
                    roomA.Height = newHeight;

                    roomB.X = X;
                    roomB.Y = Y + newHeight;
                    roomB.Width = Width;
                    roomB.Height = Height - newHeight;

                    splitVertical = false; // Vertical can fail
                    hasSplitted = true;
                    break;
                }
            }

            if (!hasSplitted)
                return false;

            Children = new Room[] { roomA, roomB };

            return true;
        }

        public bool IsAdjacent(Room other) => IsUnder(other) || IsBeside(other);

        public bool IsUnder(Room other)
        {
            var selfMax = new Vector2Int(X + Width - 1, Y + Height - 1);
            var otherMax = new Vector2Int(other.X + other.Width - 1, other.Y + other.Height - 1);

            return (other.Y - selfMax.y == 1) && // The rooms only are 1 tile apart 
                (otherMax.x > X) && (other.X < selfMax.x); // The rooms have a common X point
        }

        public bool IsBeside(Room other)
        {
            var selfMax = new Vector2Int(X + Width - 1, Y + Height - 1);
            var otherMax = new Vector2Int(other.X + other.Width - 1, other.Y + other.Height - 1);

            return (other.X - selfMax.x == 1) && // The rooms only are 1 tile apart
                (otherMax.y > Y) && (other.Y < selfMax.y); // The rooms have a common Y point
        }

        public override string ToString() => $"[{X};{Y} ({Width}x{Height})]";
    }
}