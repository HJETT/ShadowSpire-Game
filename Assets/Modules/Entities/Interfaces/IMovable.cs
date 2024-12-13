using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UtilsModule;

namespace Entities.Interfaces
{
    public interface IMovable
    {
        private const float MOVEMENT_DURATION_MS = 100f;

        public IEnumerator ApplyMovement(Movement movement)
        {
            // Must be a GridEntity
            if (this is not GridEntity gridEntity)
                yield break;

            Transform transform = gridEntity.transform;

            const float duration = MOVEMENT_DURATION_MS / 1000f;

            OnMoveStart(movement);

            // Make a note of where we are and where we are going.
            Vector2 startPosition = transform.position;
            Vector2 endPosition = GetNextPosition(movement);

            // If there is an object at the next position
            if (!CanMove(movement))
            {
                yield return new WaitForSeconds(duration);
            }
            else
            {
                // Smoothly move in the desired direction taking the required time.
                float elapsedTime = 0;
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    float percent = elapsedTime / duration;
                    transform.position = Vector2.Lerp(startPosition, endPosition, percent);
                    yield return null;
                }

                // Make sure we end up exactly where we want.
                transform.position = endPosition;
            }

            OnMoveEnd();
        }

        public bool CanMove(Movement movement)
        {
            Vector2Int gridPosition = GetNextPosition(movement);
            Vector3 endPosition = new(gridPosition.x, gridPosition.y);
            endPosition += new Vector3(1 / 2f, -1 / 2f, 0);
            return Physics2D.OverlapBox(endPosition, 0.9f * Vector2.one, 0, LayerMask.GetMask("Stoppable")) == null;
        }

        private Vector2Int GetNextPosition(Movement movement)
        {
            Vector2Int position = Vector2Int.zero;
            Vector2Int movePos = Vector2Int.zero;

            if (this is GridEntity gridEntity)
                position = gridEntity.Position;

            switch (movement)
            {
                case Movement.LEFT:
                    movePos = new(-1, 0);
                    break;
                case Movement.RIGHT:
                    movePos = new(1, 0);
                    break;
                case Movement.UP:
                    movePos = new(0, 1);
                    break;
                case Movement.DOWN:
                    movePos = new(0, -1);
                    break;
                default:
                    break;
            }

            position += movePos;

            var level = Managers.GameManager.Instance.Level;

            if (level == null)
            {
                Debug.LogError("No level has been initialized.");
                return position;
            }

            if (level.HasDoor(position.x, -position.y))
                position += movePos;

            return position;
        }

        /// <summary>
        /// Called before this object starts moving
        /// </summary>
        protected virtual void OnMoveStart(Movement movement) { }

        /// <summary>
        /// Called after this object moved
        /// </summary>
        protected virtual void OnMoveEnd() { }
    }
}