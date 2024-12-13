using ControllerModule.Controllers;
using UnityEngine;

namespace ControllerModule.Temp
{
    public class TempController2 : Controller
    {
        /// <inheritdoc/>
        protected override void OnSwitchIn()
        {
            SetCursorLock(true);
        }

        /// <inheritdoc/>
        protected override void OnSwitchOut()
        {
            this.Eyes.localRotation = Quaternion.identity;
            SetCursorLock(false);
        }
    }
}