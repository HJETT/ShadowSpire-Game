using System;
using System.Collections.Generic;
using Dungeon.Generation;
using Enemies;
using Entities;
using UnityEngine;
using UtilsModule;
using Object = UnityEngine.Object;

namespace Dungeon.Drawers.Rooms
{
    public class EnemyRoomDrawer : RoomDrawer
    {
        private readonly EnemySO[] EnemyPool;
        private GameObject EnemyPrefab;
        private readonly List<GameObject> enemiesSpawned;

        #region RoomDrawer

        /// <inheritdoc/>
        public override RoomType Type => RoomType.ENEMY;

        /// <inheritdoc/>
        protected override void OnDraw(Room room)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                for (int x = room.X; x < room.X + room.Width; x++)
                {
                    // If not an enemy, skip
                    if (!Level.Has(x, y, Tile.ENEMY))
                        continue;

                    var enemy = Object.Instantiate(EnemyPrefab);
                    enemy.transform.position = new Vector3(x, -y, 0);

                    if (enemy.TryGetComponent(out EnemyEntity entity))
                        entity.EnemyData = EnemyPool[Level.Random.Next(0, EnemyPool.Length)];

                    enemiesSpawned.Add(enemy);
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnProcess(Room room)
        {
            var validEnemyPredicate = new Func<int, int, bool>((x, y) =>
            {
                // If there is a door to the left, skip
                if (Level.HasDoor(x - 1, y))
                    return false;

                // If there is a door to the right, skip
                if (Level.HasDoor(x + 1, y))
                    return false;

                // If there is a door to the top, skip
                if (Level.HasDoor(x, y - 1))
                    return false;

                // If there is a door to the bottom, skip
                if (Level.HasDoor(x, y + 1))
                    return false;

                return true;
            });

            // Find valid positions
            var positions = GetValidPositions(room, validEnemyPredicate);

            // If no valid position, skip
            if (positions.Count == 0)
                return;

            int count = Mathf.CeilToInt(room.Width * room.Height / 8f * 0.6f);
            count = Mathf.Min(count, positions.Count);

            for (int i = 0; i < count; i++)
            {
                int rdmIndex = Level.Random.Next(0, positions.Count);
                var pos = positions[rdmIndex];

                Level.Add(pos.x, pos.y, Tile.ENEMY);

                positions.RemoveAt(rdmIndex);
            }
        }

        #endregion

        #region Drawer

        public EnemyRoomDrawer(DungeonResult level, GameObject enemyPrefab, EnemySO[] enemyPool) : base(level)
        {
            EnemyPool = enemyPool;
            EnemyPrefab = enemyPrefab;
            enemiesSpawned = new();
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            foreach (var enemy in enemiesSpawned)
            {
                // If already despawned, skip
                if (enemy == null)
                    continue;

                Object.Destroy(enemy);
            }

            enemiesSpawned.Clear();
        }

        #endregion
    }
}