using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAdjustments : MonoBehaviour
{
    [SerializeField] private int MODE;

    // Start is called before the first frame update
    void Start()
    {
        switch (MODE)
        {
            case 0:
                ParticleSystem particleSys = GetComponent<ParticleSystem>();
                ParticleSystem.MainModule particleSysMain = particleSys.main;
                ParticleSystem.ShapeModule particleSysShape = particleSys.shape;
                ParticleSystem.EmissionModule particleSysEmission = particleSys.emission;
                ParticleSystem.ForceOverLifetimeModule particleSysForce = particleSys.forceOverLifetime;

                float halfHeight = Camera.main.orthographicSize;
                float halfWidth = halfHeight * Camera.main.aspect;

                // Set radius to cover the entire screen
                particleSysShape.radius = halfWidth;

                // Set initial speed to peak at edge of screen
                particleSysMain.startSpeed = Mathf.Sqrt(-2 * particleSysForce.y.constant * halfHeight);

                // Update emission rate
                particleSysEmission.rateOverTime = halfHeight * halfWidth / 2;
                break;
            default:
                Debug.Log("Background MODE not implemented!");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
