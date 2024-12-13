using System;
using System.Collections.Generic;
using Dungeon.Generation;
using Enemies;
using UnityEngine;
using UtilsModule;
using Object = UnityEngine.Object;

namespace Dungeon.Drawers.Rooms
{
    public class TreasureRoomDrawer : RoomDrawer
    {
        private GameObject PotionPrefab;
        private readonly List<GameObject> treasuresSpawned;

        #region RoomDrawer

        /// <inheritdoc/>
        public override RoomType Type => RoomType.TREASURE;

        /// <inheritdoc/>
        protected override void OnDraw(Room room)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                for (int x = room.X; x < room.X + room.Width; x++)
                {
                    // If not a treasure, skip
                    if (!Level.Has(x, y, Tile.TREASURE))
                        continue;

                    var treasure = Object.Instantiate(PotionPrefab);
                    treasure.transform.position = new Vector3(x, -y, 0);

                    treasuresSpawned.Add(treasure);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnProcess(Room room)
        {
            Level.Add(room.X + room.Width / 2, room.Y + room.Height / 2, Tile.TREASURE);
        }

        #endregion

        #region Drawer

        public TreasureRoomDrawer(DungeonResult level, GameObject potionPrefab) : base(level)
        {
            PotionPrefab = potionPrefab;
            treasuresSpawned = new();
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            foreach (var enemy in treasuresSpawned)
            {
                // If already despawned, skip
                if (enemy == null)
                    continue;

                Object.Destroy(enemy);
            }

            treasuresSpawned.Clear();
        }

        #endregion
    }
}