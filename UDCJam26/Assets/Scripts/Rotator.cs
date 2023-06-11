using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private bool rotateRamp;
    [SerializeField] private float rotateRampSpeed;
    private float speed;

    void Start()
    {
        speed = rotateRamp ? 0 : rotateSpeed;
    }
    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * speed);
    }
    void FixedUpdate()
    {
        if (speed < rotateSpeed)
            speed = Mathf.Lerp(speed, rotateSpeed, rotateRampSpeed);
    }
}
