using ControllerModule.Controllers;

namespace ControllerModule.Temp
{
    public class TempController1 : Controller
    {
        /// <inheritdoc/>
        protected override void OnSwitchIn()
        {
            SetCursorLock(true);
        }

        /// <inheritdoc/>
        protected override void OnSwitchOut()
        {
            SetCursorLock(false);
        }
    }
}