using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private Transform Eyes;

    [Header("Cursor")]
    [SerializeField, Tooltip("Determines the sprite to use when an interaction is possible")]
    private Sprite interactCursor;

    [SerializeField, Tooltip("Determines the sprite to use when no interaction is available")]
    private Sprite normalCursor;

    [SerializeField, Tooltip("Image that represents the cursor")]
    private Image cursor;

    [SerializeField, Min(0), Tooltip("Determines how far the player can interact with things")]
    private float interactRange;

    /// <summary>
    /// Updates the cursor depending on the possible interactions
    /// </summary>
    public void UpdateCursor()
    {
        // If cursor invalid, skip
        if (this.cursor == null)
            return;

        // If eyes invalid, skip
        if (this.Eyes == null)
            return;

        if (IInteractable.CanInteract(this.Eyes.position, this.Eyes.forward, this.interactRange))
        {
            this.cursor.sprite = this.interactCursor;
            this.cursor.rectTransform.sizeDelta = new Vector2(100, 100);
            this.cursor.color = Color.white;
        }
        else
        {
            this.cursor.sprite = this.normalCursor;
            this.cursor.rectTransform.sizeDelta = new Vector2(10, 10);
            this.cursor.color = new Color(0, 0, 0, 0);
        }
    }

    public void Interact()
    {
        // If eyes invalid, skip
        if (this.Eyes == null)
            return;

        IInteractable.TryInteract(this.Eyes.position, this.Eyes.forward, this.interactRange);
    }

    private void OnDisable()
    {
        this.cursor.enabled = false;
    }
}
