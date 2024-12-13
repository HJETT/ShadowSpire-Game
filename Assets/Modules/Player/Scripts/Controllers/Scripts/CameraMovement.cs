using ExtensionsModule;
using UnityEngine;
using UtilsModule;

namespace ControllerModule.Controllers
{
    /// <summary>
    /// Moves the camera to the given object
    /// </summary>
    public class CameraMovement : Singleton<CameraMovement>
    {
        private const float LERP_THRESHOLD = 0.5f;

        [SerializeField, Tooltip("Determines how fast the camera moves")] 
        private float camSpeed = 1.0f;
        private Transform trackedObject;
        private bool isLerping = true;

        /// <summary>
        /// Updates the movement of the camera
        /// </summary>
        /// <param name="elapsed">Time passed since the last frame</param>
        private void UpdateMovement(float elapsed)
        {
            if (this.trackedObject == null)
                return;

            float duration = this.isLerping ? this.camSpeed * elapsed : 1;

            if (this.isLerping && Vector3.Distance(this.transform.position, this.trackedObject.position) < LERP_THRESHOLD)
                this.isLerping = false;

            this.transform.LerpToTarget(this.trackedObject, duration);
        }

        /// <summary>
        /// Updates the controller to follow
        /// </summary>
        public void SetController(Controller controller, bool teleportToTarget = true) 
        {
            this.trackedObject = controller.Eyes;
            this.isLerping = !teleportToTarget;
            this.transform.SetParent(this.trackedObject);

            this.UpdateMovement(0);
        }

        #region MonoBehaviour

        /// <inheritdoc cref="Update" />
        private void Update() => this.UpdateMovement(Time.deltaTime);

        #endregion

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion
    }
}