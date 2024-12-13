using UnityEngine;
using UnityEngine.InputSystem;

namespace ControllerModule.Controllers
{
    /// <summary>
    /// Class that manages the inputs of the player
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class InputMaster : UtilsModule.Singleton<InputMaster>
    {
        protected override bool DestroyOnLoad => true;

        #region Delegate

        public delegate void LookEvent(Vector2 direction);
        public delegate void MoveEvent(Vector2 direction);
        public delegate void FireEvent();

        #endregion

        #region Events

        public event LookEvent Look;
        public event MoveEvent Move;

        public event FireEvent OnFireStart;
        public event FireEvent OnFireEnd;

        #endregion

        #region Callback

        public void OnLook(InputValue value) => this.Look?.Invoke(value.Get<Vector2>());
        public void OnMove(InputValue value) => this.Move?.Invoke(value.Get<Vector2>());

        public void Unmount(InputAction.CallbackContext context)
        {
            if (context.started)
                ControllerManager.BackTo();
        }

        #endregion

        #region Operation

        public static InputMaster operator +(InputMaster input, Controller controller)
        {
            if (input == null)
                return null;

            // Subscribe all events
            input.Look += controller.OnLook;
            input.Move += controller.OnMove;

            input.OnFireStart += controller.OnFireStart;
            input.OnFireEnd += controller.OnFireEnd;
            
            return input;
        }

        public static InputMaster operator -(InputMaster input, Controller controller)
        {
            if (input == null)
                return null;

            // Unsubscribe all events
            input.Look -= controller.OnLook;
            input.Move -= controller.OnMove;

            input.OnFireStart -= controller.OnFireStart;
            input.OnFireEnd -= controller.OnFireEnd;
            
            return input;
        }

        #endregion

        #region Singleton

        private PlayerInput playerInput;

        /// <inheritdoc/>
        protected override void OnAwake() 
        {
            this.playerInput = this.GetComponent<PlayerInput>();
            var jumpAction = this.playerInput.actions["Jump"];

            var sprintAction = this.playerInput.actions["Sprint"];

            var fireAction = this.playerInput.actions["Fire"];
            fireAction.started += _ => this.OnFireStart?.Invoke();
            fireAction.canceled += _ => this.OnFireEnd?.Invoke();
        }

        #endregion
    }
}