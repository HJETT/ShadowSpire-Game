using Dungeon.Generation;

namespace UtilsModule
{
    public static class DungeonResultExtension
    {
        /// <summary>
        /// Sets the given tile at the given position
        /// </summary>
        public static void Set(this DungeonResult level, int x, int y, Tile tile) => level.Grid[y, x] = tile;

        /// <summary>
        /// Gets the tile at the given position
        /// </summary>
        public static Tile Get(this DungeonResult level, int x, int y) => level.Grid[y, x];

        /// <summary>
        /// Adds the given tile at the given position
        /// </summary>
        public static void Add(this DungeonResult level, int x, int y, Tile tile) => level.Set(x, y, level.Get(x, y) | tile);

        /// <summary>
        /// Removes the given tile at the given position
        /// </summary>
        public static void Remove(this DungeonResult level, int x, int y, Tile tile) => level.Set(x, y, level.Get(x, y) & ~tile);

        /// <summary>
        /// Checks if the tile at the given position has the given tile
        /// </summary>
        public static bool Has(this DungeonResult level, int x, int y, Tile tile) => (level.Get(x, y) & tile) != 0;

        #region Predicate

        public static bool HasWall(this DungeonResult level, int x, int y) => level.Has(x, y, Tile.WALL);
        public static bool HasGround(this DungeonResult level, int x, int y) => level.Has(x, y, Tile.GROUND | Tile.COVERED_GROUND);
        public static bool HasObstacle(this DungeonResult level, int x, int y) => level.Has(x, y, Tile.ENTRANCE | Tile.EXIT);
        public static bool IsBlocked(this DungeonResult level, int x, int y) => level.HasWall(x, y) || level.HasObstacle(x, y);
        public static bool HasDoor(this DungeonResult level, int x, int y) => level.Has(x, y, Tile.DOOR_CLOSED | Tile.DOOR_OPENED);

        #endregion
    }
}