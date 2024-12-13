using Entities;

namespace BattleEntity
{
    public class BattlePlayerEntity : BattleEntity
    {
        private PlayerEntity playerEntity;
        public BattlePlayerEntity(PlayerEntity playerEntity)
        {
            this.Health = playerEntity.Health;
            this.playerEntity = playerEntity;
            this.Type = BattleEntityType.NONE;
        }

        /// <inheritdoc/>
        protected override void OnHit(int damage)
        {
            playerEntity.TakeDamage(damage);
        }

        /// <inheritdoc/>
        protected override void OnDeath(int damage)
        {
            playerEntity.TakeDamage(damage);
        }

        public void Heal(int amount)
        {
            Health += amount;
            playerEntity.Heal(amount);
        }
    }
}
