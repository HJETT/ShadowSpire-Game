using UnityEngine;

public class ToggleInteract : MonoBehaviour, IInteractable
{
    public MeshRenderer Renderer;
    public Material OffMaterial;
    public Material OnMaterial;
    private bool isOn;
    
    public void OnClick()
    {
        this.isOn = !this.isOn;
        this.Renderer.material = this.isOn ? this.OnMaterial : this.OffMaterial;
    }
}
