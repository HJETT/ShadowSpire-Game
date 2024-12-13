using UnityEngine;

public class SpotLightRotation : MonoBehaviour
{
    [SerializeField] private Vector2 xRotationLimits = new Vector2(-45f, 45f);
    [SerializeField] private Vector2 yRotationLimits = new Vector2(-45f, 45f); 
    [SerializeField] private Vector2 zRotationLimits = new Vector2(-45f, 45f);

    [SerializeField] private float rotationSpeed = 1f; 
    private Quaternion targetRotation;

    void Start()
    {
        GenerateNewRotation();
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            GenerateNewRotation();
        }
    }

    private void GenerateNewRotation()
    {
        float randomX = Random.Range(xRotationLimits.x, xRotationLimits.y);
        float randomY = Random.Range(yRotationLimits.x, yRotationLimits.y);
        float randomZ = Random.Range(zRotationLimits.x, zRotationLimits.y);

        targetRotation = Quaternion.Euler(randomX, randomY, randomZ);
    }
}

