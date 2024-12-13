using ControllerModule.Controllers;
using UnityEngine;

public class PlayerController : Controller
{
    [Header("Movement")]
    [SerializeField]
    private PlayerMovement playerMovement;

    private Vector2 movementDirection;

    /// <inheritdoc/>
    protected override void OnStart()
    {
        ControllerManager.SwitchTo(this);
        CameraUtils.Camera2D.IsIn3D = true;
    }

    /// <inheritdoc/>
    protected override void OnSwitchIn() => SetCursorLock(true);

    /// <inheritdoc/>
    public override void OnMove(Vector2 dir) => this.movementDirection = dir;

    [Header("Interaction")]
    [SerializeField]
    private PlayerInteraction playerInteraction;

    /// <inheritdoc/>
    public override void OnFireStart() => this.playerInteraction.Interact();

    /// <inheritdoc/>
    protected override void OnUpdate(float elapsed)
    {
        this.playerMovement.ProcessMovement(this.movementDirection, elapsed);
        this.playerMovement.ProcessGravity(elapsed);
        this.playerInteraction.UpdateCursor();
    }

    private void OnDisable()
    {
        SwitchOut();
        playerInteraction.enabled = false;
    }

    private void OnEnable()
    {
        SwitchIn();
        playerInteraction.enabled = true;
    }
}