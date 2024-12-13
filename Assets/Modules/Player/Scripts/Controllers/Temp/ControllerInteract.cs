using ControllerModule.Controllers;
using UnityEngine;

namespace ControllerModule.Temp
{
    public class ControllerInteract : MonoBehaviour, IInteractable
    {
        public Controller Controller;
        
        public void OnClick() => ControllerManager.SwitchTo(this.Controller);
    }
}