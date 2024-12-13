using System;
using System.Collections.Generic;
using Entities;
using PathFinding.Graphs;

namespace Dungeon.Generation
{
    [Flags]
    public enum Tile
    {
        NONE = 0,
        GROUND = 1 << 1,
        COVERED_GROUND = 1 << 2,
        // 1 << 3
        // 1 << 4
        WALL = 1 << 5,
        // 1 << 6
        // 1 << 7
        // 1 << 8
        DOOR_CLOSED = 1 << 9,
        DOOR_OPENED = 1 << 10,
        // 1 << 11
        // 1 << 12
        ENTRANCE = 1 << 13,
        EXIT = 1 << 14,
        // 1 << 15
        // 1 << 16
        PLAYER = 1 << 17,
        ENEMY = 1 << 18,
        // 1 << 19
        // 1 << 20
        // 1 << 21
        // 1 << 22
        TREASURE = 1 << 23,
        SPIKES = 1 << 24
    }

    public class DungeonResult
    {
        #region Rooms

        /// <summary>
        /// All the final rooms in the dungeon
        /// </summary>
        public Room[] Rooms;

        /// <summary>
        /// Every rooms that is adjacent to a room
        /// </summary>
        public Dictionary<Room, HashSet<Room>> AdjacentRooms;

        /// <summary>
        /// Room that serves as the entrance of the dungeon
        /// </summary>
        public Room Entrance;

        /// <summary>
        /// Room that serves as the exit of the dungeon
        /// </summary>
        public Room Exit;

        #endregion

        #region Level

        /// <summary>
        /// Current level of the dungeon
        /// </summary>
        public int Index;

        /// <summary>
        /// The element used for random generation
        /// </summary>
        public Random Random;

        /// <summary>
        /// Width of the entire dungeon
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of the entire dungeon
        /// </summary>
        public int Height;

        #endregion

        #region Tiles

        /// <summary>
        /// Tiles for the dungeon
        /// </summary>
        public Tile[,] Grid;

        /// <summary>
        /// Graph of all the tiles for the dungeon
        /// </summary>
        public TileGraph TileGraph;

        #endregion

        public PlayerEntity Player;
    }
}