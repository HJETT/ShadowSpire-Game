using UnityEngine;

namespace ControllerModule.Controllers.Interfaces
{
    public interface IInteractable
    {
        public static readonly int LAYER = LayerMask.NameToLayer("Interactable");
        public static readonly int LAYER_MASK = 1 << LAYER;
        
        /// <summary>
        /// Called when something interacted with this object
        /// </summary>
        public void OnClick();

        /// <summary>
        /// Tries to find a target and interacts with it
        /// </summary>
        public static void TryInteract(Vector3 position, Vector3 direction, float maxDistance = float.MaxValue)
        {
            if (!CanInteract(position, direction, out IInteractable interactable, maxDistance))
                return;

            Interact(interactable);
        }

        /// <summary>
        /// Checks if the ray touches something to interact with
        /// </summary>
        /// <returns>Is there something to interact with?</returns>
        public static bool CanInteract(Vector3 position, Vector3 direction, out IInteractable interactable, float maxDistance = float.MaxValue)
        {
            if (Physics.Raycast(position, direction, out RaycastHit hit, maxDistance, LAYER_MASK))
                return hit.collider.gameObject.TryGetComponent(out interactable);
            
            interactable = null;
            return false;

        }

        /// <summary>
        /// Checks if the ray touches something to interact with
        /// </summary>
        /// <returns>Is there something to interact with?</returns>
        public static bool CanInteract(Vector3 position, Vector3 direction, float maxDistance = float.MaxValue)
            => CanInteract(position, direction, out _, maxDistance);

        /// <summary>
        /// Starts an interaction with the given target
        /// </summary>
        public static void Interact(IInteractable target) => target.OnClick();
    }
}