using ControllerModule.Controllers;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController _characterController;

    [SerializeField]
    private Controller controller;

    [SerializeField]
    private float walkSpeed = 8;

    #region Movement

    public void ProcessMovement(Vector3 direction, float elapsed)
    {
        // Check for validity
        if (this._characterController == null)
            return;

        if (controller.Eyes == null)
            return;

        // Modify the direction
        Vector3 movement = (controller.Eyes.forward * direction.y) + (controller.Eyes.right * direction.x);
        movement.y = 0;
        movement.Normalize();

        // If not moving, skip
        if (direction.magnitude <= 0f)
            return;

        this._characterController.Move(elapsed * this.walkSpeed * movement);
    }

    #endregion

    #region Gravity

    private Vector3 gravityVelocity;
    private bool isGrounded;

    public void ProcessGravity(float elapsed)
    {
        // Check for validity
        if (this._characterController == null)
            return;

        if (this.isGrounded && this.gravityVelocity.y <= 0)
        {
            this.gravityVelocity.y = 0;
        }

        this.gravityVelocity += Physics.gravity * elapsed;
        this._characterController.Move(this.gravityVelocity * elapsed);

        // Update state
        this.isGrounded = this._characterController.isGrounded;
    }

    #endregion
}