using UnityEngine;

public class InteractBlock : MonoBehaviour, IInteractable
{
    public void OnClick() 
    {
        this.transform.position += new Vector3(0, 0.1f, 0);
    }
}