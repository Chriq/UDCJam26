using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatMovement : MonoBehaviour
{
    [SerializeField] private float vel;             // Initial velocity
    [SerializeField] private float accel;           // Fixed interval acceleration

    void Start()
    {
    }

    void FixedUpdate()
    {
        vel += accel;
        transform.Translate(Vector3.left * vel * Time.fixedDeltaTime);
        vel += accel;
    }
}

/*
    [SerializeField] private float dist;            // Distance of target
    [SerializeField] private float time;            // Time to reach target
    [SerializeField] private float accel;           // Incrementor to velocity
    [SerializeField] private float peak;            // Distance of target
        accel = 2 * (dist - vel * time) * Time.fixedDeltaTime / time / time / 2;
        peak = -vel * vel * Time.fixedDeltaTime / 4 / accel * 2;
*/