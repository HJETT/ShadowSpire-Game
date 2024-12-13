using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToLive : MonoBehaviour
{
    [SerializeField] float lifetime = 10.0f;
    float initTime;

    // Start is called before the first frame update
    void OnEnable()
    {
        initTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > initTime + lifetime)
            Destroy(gameObject);
    }
}
