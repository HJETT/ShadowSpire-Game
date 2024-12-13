namespace Entities.Interfaces
{
    public interface IEventable
    {
        /// <summary>
        /// Called when another entity landed on this entity
        /// </summary>
        public void OnEntityLanded(GridEntity entity);

        /// <summary>
        /// Called when this entity landed on another entity
        /// </summary>
        public void OnEntityLand(GridEntity entity);
    }

    public interface IEventable<T> : IEventable where T : GridEntity
    {
        /// <inheritdoc/>
        void IEventable.OnEntityLand(GridEntity entity)
        {
            if (entity is not T typedEntity)
                return;

            OnEntityLand(typedEntity);
        }

        /// <inheritdoc/>
        void IEventable.OnEntityLanded(GridEntity entity)
        {
            if (entity is not T typedEntity)
                return;

            OnEntityLanded(typedEntity);
        }

        /// <summary>
        /// Called when another entity landed on this entity
        /// </summary>
        public void OnEntityLanded(T entity);

        /// <summary>
        /// Called when this entity landed on another entity
        /// </summary>
        public void OnEntityLand(T entity);
    }
}