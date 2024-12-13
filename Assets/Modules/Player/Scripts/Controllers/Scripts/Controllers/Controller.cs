using UnityEngine;

namespace ControllerModule.Controllers
{
    /// <summary>
    /// Class that provides methods to use other controller independently
    /// </summary>
    public abstract class Controller : MonoBehaviour
    {
        #region Look

        [Header("Look")]
        [SerializeField]
        private LookController lookController;

        /// <summary>
        /// Point from which the player sees the world
        /// </summary>
        public Transform Eyes => this.lookController?.cameraAnchor;
        
        private Vector3 camDirection;

        /// <summary>
        /// Called when the player requests a rotation
        /// </summary>
        /// <param name="direction">Direction of the rotation</param>
        public virtual void OnLook(Vector2 direction) => this.camDirection = direction;

        #endregion

        #region Movement

        /// <summary>
        /// Called when the player moves
        /// </summary>
        public virtual void OnMove(Vector2 dir) { }

        #endregion

        #region Switch

        [Header("Switch")]
        [SerializeField, Tooltip("Determines if, when this controller switches out, it disables itself")]
        private bool disableIfOut = true;

        /// <summary>
        /// Is this controller enabled or not?
        /// </summary>
        /// <remarks>
        /// This allows a controller to receive certain updates while being "disabled"
        /// </remarks>
        protected bool IsEnabled { get; private set; } = true;

        /// <summary>
        /// Starts using this controller
        /// </summary>
        public void SwitchIn()
        {
            // Subscribe all
            InputMaster.Instance += this;

            // Update enable states
            this.IsEnabled = true;
            this.enabled = true;

            // Call callback
            this.OnSwitchIn();
        }
        
        /// <summary>
        /// Called when this controller is started to being used
        /// </summary>
        protected virtual void OnSwitchIn() { }

        /// <summary>
        /// Stops using this controller
        /// </summary>
        public void SwitchOut()
        {
            // Unsubscribe all
            InputMaster.Instance -= this;

            // Update enable states
            this.IsEnabled = false;
            if (this.disableIfOut)
                this.enabled = false;

            // Resets the direction
            this.camDirection = Vector3.zero;

            // Call callback
            this.OnSwitchOut();
        }

        /// <summary>
        /// Called when this controller is no longer being used
        /// </summary>
        protected virtual void OnSwitchOut() { }

        #endregion

        #region Fire

        /// <summary>
        /// Called when the player presses the 'Fire' button
        /// </summary>
        public virtual void OnFireStart() { }

        /// <summary>
        /// Called when the player releases the 'Fire' button
        /// </summary>
        public virtual void OnFireEnd() {}

        #endregion

        #region MonoBehaviour

        /// <inheritdoc cref="Start" />
        public void Start() => this.OnStart();

        /// <summary>
        /// Called when this controller is being started
        /// </summary>
        protected virtual void OnStart() { }

        /// <inheritdoc cref="Update" />
        private void Update()
        {
            this.OnUpdate(Time.deltaTime);
            this.lookController?.UpdateRotation(this.camDirection, Time.deltaTime);
        }

        /// <summary>
        /// Called when this controller is being updated
        /// </summary>
        /// <param name="elapsed">Time passed since the last frame</param>
        protected virtual void OnUpdate(float elapsed) { }

        /// <inheritdoc cref="FixedUpdate" />
        private void FixedUpdate() => this.OnFixedUpdate(Time.fixedDeltaTime);

        /// <summary>
        /// Called when this controller is being updated
        /// </summary>
        /// <param name="elapsed">Time passed since the last call</param>
        protected virtual void OnFixedUpdate(float elapsed) { }

        /// <inheritdoc cref="OnDestroy" />
        private void OnDestroy()
        {
            // Switch out before destroying
            if (this.IsEnabled)
            {
                this.SwitchOut();
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Sets the cursor lock state 
        /// </summary>
        /// <param name="isLocked">Is the cursor locked or not?</param>
        protected static void SetCursorLock(bool isLocked)
        {
            if (isLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        #endregion
    }
}