using Dungeon.Generation;
using Enemies;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace BattleEntity
{
    public class BattleEnemyEntity : BattleEntity
    {
        public EnemySO Enemy { get; private set; }
        public UnityEvent<int> Hit { get; private set; } = new();
        public UnityEvent<int> Death { get; private set; } = new();

        public BattleEnemyEntity(EnemySO enemy)
        {
            Enemy = enemy;
            Health = Mathf.RoundToInt(enemy.BaseHealth + enemy.BaseHealth * 0.5f * (GameManager.Instance.Level.Index - DungeonGenerator.ENEMY_ROOM_INDEX) * 0.5f);
            Attack = enemy.BaseAttack;
            Type = enemy.Type;
        }

        /// <inheritdoc/>
        protected override void OnHit(int damage) => Hit?.Invoke(damage);

        /// <inheritdoc/>
        protected override void OnDeath(int damage) => Death?.Invoke(damage);
    }
}
