using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatMovement : MonoBehaviour
{
    private float vel;             // Initial velocity
    private float accel;           // Fixed interval acceleration

    void FixedUpdate()
    {
        vel += accel;
        transform.Translate(Vector3.left * vel * Time.fixedDeltaTime);
        vel += accel;
    }

    public void SetParameters(float velocity, float acceleration)
    {
        vel = velocity;
        accel = acceleration;
    }
}

/*
    [SerializeField] private float dist;            // Distance of target
    [SerializeField] private float time;            // Time to reach target
    [SerializeField] private float peak;            // Distance of target
    [SerializeField] private float vel;             // Initial velocity
    [SerializeField] private float accel;           // Fixed interval acceleration
        accel = 2 * (dist - vel * time) * Time.fixedDeltaTime / time / time / 2;
        peak = -vel * vel * Time.fixedDeltaTime / 4 / accel * 2;
*/