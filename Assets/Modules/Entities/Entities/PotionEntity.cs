using Entities.Interfaces;

namespace Entities
{
    public class PotionEntity : GridEntity, IEventable<PlayerEntity>
    {
        /// <inheritdoc/>
        public void OnEntityLand(PlayerEntity entity) { /* Potion can't move on another entity */ }

        /// <inheritdoc/>
        public void OnEntityLanded(PlayerEntity entity)
        {
            entity.CollectPotion();
            Destroy(gameObject);
        }
    }
}