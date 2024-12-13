using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBattleMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;

    float movementX;
    float movementY;
    float[] boxLimit = { -0.75f, 0.75f, -0.68f, 0.68f };
    public BattleManager battleManager;
    
    void Update()
    {
        MovePlayer();
        LimitMovements();
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        movementX = direction.x;
        movementY = direction.y;
    }

    void MovePlayer()
    {
        Vector3 movement = new Vector3(movementX, movementY, 0) * movementSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    public void LimitMovements()
    {
        Vector3 currPosition = transform.localPosition ;
        currPosition.x = Mathf.Clamp(currPosition.x, boxLimit[0], boxLimit[1]);
        currPosition.y = Mathf.Clamp(currPosition.y, boxLimit[2], boxLimit[3]);
        transform.localPosition = currPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            battleManager.DamagePlayer(5);
            collision.gameObject.SetActive(false);
        }
    }
}
