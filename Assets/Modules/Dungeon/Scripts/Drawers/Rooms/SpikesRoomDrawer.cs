using System;
using System.Collections.Generic;
using Dungeon.Generation;
using UnityEngine;
using UtilsModule;
using Object = UnityEngine.Object;

namespace Dungeon.Drawers.Rooms
{
    public class SpikesRoomDrawer : RoomDrawer
    {
        private GameObject SpikesPrefab;
        private readonly List<GameObject> spikesSpawned;

        #region RoomDrawer

        /// <inheritdoc/>
        public override RoomType Type => RoomType.SPIKES;

        /// <inheritdoc/>
        protected override void OnDraw(Room room)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                for (int x = room.X; x < room.X + room.Width; x++)
                {
                    // If not a treasure, skip
                    if (!Level.Has(x, y, Tile.SPIKES))
                        continue;

                    var treasure = Object.Instantiate(SpikesPrefab);
                    treasure.transform.position = new Vector3(x, -y, 0);

                    spikesSpawned.Add(treasure);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnProcess(Room room)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                for (int x = room.X; x < room.X + room.Width; x++)
                {
                    Level.Add(x, y, Tile.SPIKES);
                    Level.Add(x, y, Tile.COVERED_GROUND);
                }
            }
        }

        #endregion

        #region Drawer

        public SpikesRoomDrawer(DungeonResult level, GameObject spikesPrefab) : base(level)
        {
            SpikesPrefab = spikesPrefab;
            spikesSpawned = new();
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            foreach (var spike in spikesSpawned)
            {
                // If already despawned, skip
                if (spike == null)
                    continue;

                Object.Destroy(spike);
            }

            spikesSpawned.Clear();
        }

        #endregion
    }
}