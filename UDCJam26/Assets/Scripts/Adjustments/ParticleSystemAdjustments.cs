using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemAdjustments : MonoBehaviour
{
    [SerializeField] private int MODE;

    void Start()
    {
        ParticleSystem particleSys = GetComponent<ParticleSystem>();

        ParticleSystem.MainModule particleSysMain = particleSys.main;
        ParticleSystem.ShapeModule particleSysShape = particleSys.shape;
        ParticleSystem.EmissionModule particleSysEmission = particleSys.emission;
        ParticleSystem.ForceOverLifetimeModule particleSysForce = particleSys.forceOverLifetime;
        ParticleSystem.MinMaxCurve particleSysForceY = particleSys.forceOverLifetime.y;

        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Camera.main.aspect;

        switch (MODE)
        {
            case 0:
                // Set radius to cover the entire screen
                particleSysShape.radius = halfWidth;

                // Set initial speed to peak at edge of screen
                particleSysMain.startSpeed = Mathf.Sqrt(-2 * particleSysForceY.constant * halfHeight);

                // Update emission rate
                particleSysEmission.rateOverTime = halfHeight * halfWidth / 2;
                break;
            case 1:
                // Set initial speed to scale with width
                particleSysMain.startSpeed = halfHeight;

                // Set acceleration to break before center
                particleSysForceY.constant = -halfHeight * halfHeight / 2 / (halfHeight - 1);
                break;
            case 2:
                // Set initial speed to scale with width
                particleSysMain.startSpeed = halfHeight;

                // Set acceleration to break before center
                particleSysForceY.constant = halfHeight * halfHeight / 2 / (halfHeight - 1);
                particleSysForce.y = particleSysForceY;
                break;
            default:
                Debug.Log("Particle System MODE not implemented!");
                break;
        }
    }
}
