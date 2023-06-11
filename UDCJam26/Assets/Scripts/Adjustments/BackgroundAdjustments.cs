using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAdjustments : MonoBehaviour
{
    [SerializeField] private int MODE;

    void Start()
    {
        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Camera.main.aspect;

        switch (MODE)
        {
            case 0:
                // Set Position to screen bottom edge
                transform.position = new Vector2(transform.position.x, -halfHeight - 1);
                break;
            case 1:
                // Set Position to screen top edge
                transform.position = new Vector2(transform.position.x, halfHeight + 1);
                break;
            default:
                Debug.Log("Background MODE not implemented!");
                break;
        }
    }
}
