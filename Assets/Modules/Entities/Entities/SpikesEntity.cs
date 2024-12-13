using System.Collections;
using Entities.Interfaces;
using UnityEngine;

namespace Entities
{
    public class SpikesEntity : GridEntity, ITurnable, IEventable<PlayerEntity>
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Sprite offSprite;

        [SerializeField]
        private Sprite onSprite;

        #endregion

        private int spikesTurns;

        #region IEventable

        /// <inheritdoc/>
        public void OnEntityLand(PlayerEntity entity) { /* Spikes can't move on another entity */ }

        /// <inheritdoc/>
        public void OnEntityLanded(PlayerEntity entity)
        {
            if (spikesTurns == 1)
                entity.TakeDamage(2);
            else if (spikesTurns < 0)
                spikesTurns = 3;
        }

        #endregion

        #region ITurnable

        public IEnumerator Think()
        {
            spikesTurns--;

            spriteRenderer.sprite = spikesTurns == 0 ? onSprite : offSprite;

            yield return null;
        }

        #endregion
    }
}