using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHandAnimation : MonoBehaviour
{
    bool isRotating = false;
    bool isMoving = false;
    float zRotationMax = 7;
    float zRotationMin = -7;
    float yPositionMaxOffset = 0.01f; 
    float yPositionMinOffset = -0.01f; 
    float movementSpeed = 3f;
    float initialYPosition;
    float rotationSpeed; 
    private void Start()
    {
        isRotating = Random.Range(0, 2) == 0;
        isMoving = Random.Range(0, 2) == 0;

        initialYPosition = transform.localPosition.y;
        rotationSpeed = Random.Range(2, 5);
    }
    void Update()
    {
        if (isRotating)
        {
            float zRotation = Mathf.Lerp(zRotationMin, zRotationMax, Mathf.PingPong(Time.time * rotationSpeed, 1));

            transform.rotation = Quaternion.Euler(0, 0, zRotation);
        }
        if (isMoving)
        {
            float yOffset = Mathf.Lerp(yPositionMinOffset, yPositionMaxOffset, Mathf.PingPong(Time.time * movementSpeed, 1));
            transform.localPosition = new Vector3(transform.localPosition.x, initialYPosition + yOffset, transform.localPosition.z);
        }
    }
}
