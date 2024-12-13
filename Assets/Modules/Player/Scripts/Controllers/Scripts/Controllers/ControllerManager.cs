using System.Collections.Generic;
using System.Linq;

namespace ControllerModule.Controllers
{
    /// <summary>
    /// Class that manages the movement between multiple controllers
    /// </summary>
    public class ControllerManager : UtilsModule.Singleton<ControllerManager>
    {
        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion

        /// <summary>
        /// List of the controllers used
        /// </summary>
        private static readonly Stack<Controller> stack = new();

        /// <summary>
        /// Current controller being used
        /// </summary>
        private static Controller ActiveController => stack.Count > 0 ? stack.Peek() : null;

        /// <summary>
        /// Switches to the given controller
        /// </summary>
        /// <param name="controller">Controller to switch to</param>
        public static void SwitchTo(Controller controller)
        {
            // If exists, switch out current
            if (ActiveController != null)
                ActiveController.SwitchOut();

            // Cancel if given not found
            if (controller == null)
                return;

            // Switch in the given
            controller.SwitchIn();
            stack.Push(controller);

            // Set the controller to the given
            CameraMovement.Instance.SetController(controller);
        }

        /// <summary>
        /// Replaces the current controller with the given controller
        /// </summary>
        private static void ReplaceCurrent(Controller controller)
        {
            // Skip if current not found
            if (ActiveController == null)
                return;

            // Remove current
            Controller current = stack.Pop();
            current.SwitchOut();

            // Switch to given
            SwitchTo(controller);
        }

        /// <summary>
        /// Switches to the previous controller
        /// </summary>
        public static void BackTo()
        {
            if (stack.Count <= 1)
                return;

            Controller previous = stack.Skip(1).First();
            ReplaceCurrent(previous);
        }
    }
}